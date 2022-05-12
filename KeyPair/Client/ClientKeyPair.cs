using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.KeyPair.Client
{
    public class ClientKeyPair
    {
        public static string PrivatePem (){
            var str = File.ReadAllText("./KeyPair/Client/private.pem");
            return str;
        }
        
        public static byte[] CertReader (){
            var bytes = File.ReadAllBytes("./KeyPair/Client/client.cer");
            return bytes;
        }

    }
}