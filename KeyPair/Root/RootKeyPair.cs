using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.KeyPair.Root
{
    public class RootKeyPair
    {
        // public static string PrivatePem (){
        //     var str = File.ReadAllText("./KeyPair/Root/root.key");
        //     return str;
        // }
        public static byte[] CertReader (){
            var bytes = File.ReadAllBytes("./KeyPair/Root/root.cer");
            return bytes;
        }
    }
}