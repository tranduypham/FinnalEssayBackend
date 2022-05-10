using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.KeyPair.MerchantBank
{
    public class MerchantBankKeyPair
    {
        public static string PrivatePem (){
            var str = File.ReadAllText("./KeyPair/MerchantBank/private.pem");
            return str;
        }
    }
}