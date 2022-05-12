using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Models;

namespace Backend.Services.Certificate
{
    public interface ICertificateServices
    {
        public CertificateInfo ReadCert (string RawCert); 
        public CertificateInfo ReadCert_KeyName (string KeyName); 
        public bool Verify (string RawCert); 
        public bool Verify_KeyName(string KeyName);
    }
}