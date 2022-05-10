using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.KeyPair.ClientBank
{
    public class ClientBankKeyPair
    {
        public static string PrivatePem (){
            var str = File.ReadAllText("KeyPair/ClientBank/private.pem");
            return str;
        }
    }
}