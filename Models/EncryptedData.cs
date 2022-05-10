using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Models
{
    public class EncryptedData
    {
        public string DataBase64 { get; set; }
        public string KeyName { get; set; }
        public bool isPrivate { get; set; }
    }
}