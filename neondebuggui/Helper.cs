using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
namespace client
{
    class ApiHelper
    {
        public static void uploadScript(string api, string path, string hash)
        {
            System.Net.WebClient wc = new MyWebClient();
            try
            {
                System.Collections.Specialized.NameValueCollection vs = new System.Collections.Specialized.NameValueCollection();
                vs["jsonrpc"] = "2.0";
                vs["id"] = "1";
                vs["method"] = "setcontractscript";
                MyJson.JsonNode_Array array = new MyJson.JsonNode_Array();
                MyJson.JsonNode_Object jsonmap = new MyJson.JsonNode_Object();
                jsonmap["hash"] = new MyJson.JsonNode_ValueString(hash);
                var avm = System.IO.File.ReadAllBytes(System.IO.Path.Combine(path, hash + ".avm"));
                var cs = System.IO.File.ReadAllText(System.IO.Path.Combine(path, hash + ".cs"));
                var map = System.IO.File.ReadAllText(System.IO.Path.Combine(path, hash + ".map.json"));
                var abi = System.IO.File.ReadAllText(System.IO.Path.Combine(path, hash + ".abi.json"));

                jsonmap["avm"] = new MyJson.JsonNode_ValueString(ThinNeo.Helper.Bytes2HexString(avm));
                jsonmap["cs"] = new MyJson.JsonNode_ValueString(Uri.EscapeDataString(cs));
                jsonmap["map"] = MyJson.Parse(map);
                jsonmap["abi"] = MyJson.Parse(abi);
                array.Add(new MyJson.JsonNode_ValueString(jsonmap.ToString()));
                vs["params"] = array.ToString();

                var ret = wc.UploadValues(api, vs);
                var txt = System.Text.Encoding.UTF8.GetString(ret);
                MessageBox.Show(txt);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }

        }
        public static MyJson.JsonNode_Object downloadScript(string api, string savepath, string scripthash)
        {
            System.Net.WebClient wc = new MyWebClient();
            try
            {
                var str = wc.DownloadString(api + "?jsonrpc=2.0&id=1&method=getcontractscript&params=[\"" + scripthash + "\"]");
                var json = MyJson.Parse(str).AsDict()["result"].AsList()[0].AsDict();
                if (json.ContainsKey("cs"))
                {
                    var srcResult = json["cs"].AsString();
                    srcResult = Uri.UnescapeDataString(srcResult);
                    var outfile = System.IO.Path.Combine(savepath, scripthash + ".cs");
                    System.IO.File.WriteAllText(outfile, srcResult);
                }
                if (json.ContainsKey("avm"))
                {
                    var avmResult = json["avm"].AsString();
                    var bts = ThinNeo.Helper.HexString2Bytes(avmResult);
                    var outfile = System.IO.Path.Combine(savepath, scripthash + ".avm");
                    System.IO.File.WriteAllBytes(outfile, bts);
                }
                if (json.ContainsKey("map"))
                {
                    var mapResult = json["map"].ToString();
                    var outfile = System.IO.Path.Combine(savepath, scripthash + ".map.json");
                    System.IO.File.WriteAllText(outfile, mapResult);
                }
                if (json.ContainsKey("abi"))
                {
                    var mapResult = json["abi"].ToString();
                    var outfile = System.IO.Path.Combine(savepath, scripthash + ".abi.json");
                    System.IO.File.WriteAllText(outfile, mapResult);
                }
                return json;
            }
            catch (Exception err)
            {
                return null;
            }
        }

        public static void downloadFullLog(string api, string path, string transid)
        {
            System.Net.WebClient wc = new MyWebClient();
            var str = wc.DownloadString(api + "?jsonrpc=2.0&id=1&method=getfulllog&params=[\"" + transid + "\"]");
            var json = MyJson.Parse(str).AsDict()["result"].AsList()[0].AsDict();
            var fulllog = json["fulllog7z"].ToString();
            var txid = json["txid"].ToString();
            System.IO.File.WriteAllText(System.IO.Path.Combine(path, txid + ".llvmhex.txt"),fulllog);
        }
    }
}
