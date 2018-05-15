using Neo.Compiler.MSIL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Neo.Compiler
{
    class _DebugOutput
    {
        public static void DebugOutput(string outpath, NeoModule module, byte[] avm, MyJson.JsonNode_Object abi)
        {
            if (System.IO.Directory.Exists(outpath) == false)
            {
                System.IO.Directory.CreateDirectory(outpath);
            }
            string mapInfo = null;
            string srcfile = null;

            {//gen debug info
                Neo.Compiler.MyJson.JsonNode_Array arr = new Neo.Compiler.MyJson.JsonNode_Array();
                foreach (var m in module.mapMethods)
                {
                    Neo.Compiler.MyJson.JsonNode_Object item = new Neo.Compiler.MyJson.JsonNode_Object();
                    arr.Add(item);
                    item.SetDictValue("name", m.Value.displayName);
                    item.SetDictValue("addr", m.Value.funcaddr.ToString("X04"));
                    Neo.Compiler.MyJson.JsonNode_Array infos = new Neo.Compiler.MyJson.JsonNode_Array();
                    item.SetDictValue("map", infos);
                    foreach (var c in m.Value.body_Codes)
                    {
                        if (c.Value.debugcode != null)
                        {
                            var debugcode = c.Value.debugcode.ToLower();
                            if (debugcode.Contains(".cs"))
                            {
                                srcfile = debugcode;
                                infos.AddArrayValue(c.Value.addr.ToString("X04") + "-" + c.Value.debugline.ToString());
                            }
                        }
                    }
                }
                mapInfo = arr.ToString();
            }


            var hash = abi["hash"].AsString();
            var abistr = abi.ToString();
            var outfile = System.IO.Path.Combine(outpath, hash);
            System.IO.File.WriteAllBytes(outfile + ".avm", avm);
            System.IO.File.WriteAllText(outfile + ".map.json", mapInfo);
            System.IO.File.WriteAllText(outfile + ".abi.json", abistr);
            if (srcfile != null)
            {
                var targetfilename = outfile + ".cs";
                System.IO.File.Delete(targetfilename);
                System.IO.File.Copy(srcfile, targetfilename);
            }
            new DefLogger().Log("output to local succ");
            DebugOutputToService(hash,avm, abistr, mapInfo,System.IO.File.ReadAllText(srcfile));
        }

        public static void DebugOutputToService(string hash,byte[] avm, string abi,string map,string cs)
        {
            System.Net.WebClient wc = new WebClient();
            try
            {
                System.Collections.Specialized.NameValueCollection vs = new System.Collections.Specialized.NameValueCollection();
                vs["jsonrpc"] = "2.0";
                vs["id"] = "1";
                vs["method"] = "setcontractscript";
                MyJson.JsonNode_Array array = new MyJson.JsonNode_Array();

                StringBuilder sb = new StringBuilder();
                foreach (var d in avm)
                {
                    sb.Append(d.ToString("x02"));
                }

                array.Add(new MyJson.JsonNode_ValueString(hash));
                array.Add(new MyJson.JsonNode_ValueString(sb.ToString()));
                array.Add(new MyJson.JsonNode_ValueString(Uri.EscapeDataString(cs)));
                array.Add(new MyJson.JsonNode_ValueString(map));
                array.Add(new MyJson.JsonNode_ValueString(abi));
                vs["params"] = array.ToString();

                var ret = wc.UploadValues("https://apiaggr.nel.group/api/testnet", vs);
                new DefLogger().Log(System.Text.Encoding.UTF8.GetString(ret));
            }
            catch (Exception err)
            {
                new DefLogger().Log(err.ToString());
            }
        }

    }
}
