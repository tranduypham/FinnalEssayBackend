using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Models
{
    public class SignedData : EncryptedData
    {
        public string DigitalSignatureBase64 { get; set; }
        public string DataRaw { get; set; }
    }
}