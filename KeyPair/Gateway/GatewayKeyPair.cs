using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.KeyPair.Gateway
{
    public class GatewayKeyPair
    {
        public static string PrivatePem (){
            var str = File.ReadAllText("KeyPair/Gateway/private.pem");
            return str;
        }
        public static byte[] CertReader (){
            var bytes = File.ReadAllBytes("./KeyPair/Gateway/gateway.cer");
            return bytes;
        }
    }
}