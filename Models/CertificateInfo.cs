using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Models
{
    public class CertificateInfo
    {
        public string? Issuer { get; set; }
        public string? EXP { get; set; }
        public string? Name { get; set; }
        public string? PublicKeyBase64 { get; set; }
        public string? Format { get; set; }
        public string? RawCertDataBase64 { get; set; }
    }
}