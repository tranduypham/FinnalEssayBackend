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
    }
}