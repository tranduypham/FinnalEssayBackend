using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.KeyPair.Merchant
{
    public class MerchantKeyPair
    {
        public static string PrivatePem (){
            var str = File.ReadAllText("./KeyPair/Merchant/private.pem");
            return str;
        }
        public static byte[] CertReader (){
            var bytes = File.ReadAllBytes("./KeyPair/Merchant/merchant.cer");
            return bytes;
        }
    }
}