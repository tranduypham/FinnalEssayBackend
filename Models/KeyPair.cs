using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Models
{
    public class MyKeyPair
    {
        public string KeyName { get; set; }
        public string PublicBase64 { get; set; }
        public string PrivateBase64 { get; set; }
    }
}