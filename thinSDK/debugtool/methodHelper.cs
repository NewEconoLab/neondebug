using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinNeo.Debug
{
    public class methodHelper
    {
        public static methodHelper Ins 
        {
            get
            {
                if (ins == null)
                    ins = new methodHelper();
                return ins;
            }
        }
        private static methodHelper ins;

        private Dictionary<uint, string> dic = new Dictionary<uint, string>();

        private string[] methods = new string[] {
            "System.Enumerator.Create",
            "System.Enumerator.Next",
            "System.Enumerator.Value",
            "System.Enumerator.Concat",
            "System.Json.Serialize",
            "System.Json.Deserialize",
            "System.Runtime.Platform",
            "System.Runtime.GetTrigger",
            "System.Runtime.GetTime",
            "System.Runtime.GetScriptContainer",
            "System.Runtime.GetExecutingScriptHash",
            "System.Runtime.GetCallingScriptHash",
            "System.Runtime.GetEntryScriptHash",
            "System.Runtime.CheckWitness",
            "System.Runtime.GetInvocationCounter",
            "System.Runtime.Log",
            "System.Runtime.Notify",
            "System.Runtime.GetNotifications",
            "System.Storage.GetContext",
            "System.Storage.GetReadOnlyContext",
            "System.Storage.AsReadOnly",
            "System.Storage.Get",
            "System.Storage.Find",
            "System.Storage.Put",
            "System.Storage.PutEx",
            "System.Storage.Delete",
            "System.Contract.Create",
            "System.Contract.Update",
            "System.Contract.Destroy",
            "System.Contract.Call",
            "System.Contract.CallEx",
            "System.Contract.IsStandard",
            "System.Iterator.Create",
            "System.Iterator.Key",
            "System.Iterator.Keys",
            "System.Iterator.Values",
            "System.Iterator.Concat",
            "System.Blockchain.GetHeight",
            "System.Blockchain.GetBlock",
            "System.Blockchain.GetTransaction",
            "System.Blockchain.GetTransactionHeight",
            "System.Blockchain.GetTransactionFromBlock",
            "System.Blockchain.GetContract",
            "System.Binary.Serialize",
            "System.Binary.Deserialize"
        };

        public methodHelper()
        {
            for (var i = 0; i < methods.Length; i++)
            {
                string method = methods[i];
                uint api = BitConverter.ToUInt32(ThinNeo.Helper.Sha256(System.Text.Encoding.ASCII.GetBytes(method)), 0);
                dic.Add(api, methods[i]);
            }
        }

        public string GetMethodName(uint _api)
        {
            if (dic.ContainsKey(_api))
                return dic[_api];
            return "UNKONW";
        }
    }
}
