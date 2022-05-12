using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Helper;
using Backend.Models;

namespace Backend.Services.Certificate
{
    public class CertificateServices : ICertificateServices
    {
        public CertificateInfo ReadCert(string RawCert)
        {
            var cert = CertificateProcess.ReadCert(RawCert);
            return new CertificateInfo(){
                Issuer = cert.Issuer,
                EXP = cert.GetExpirationDateString(),
                Name = cert.GetName(),
                PublicKeyBase64 = Convert.ToBase64String(cert.GetPublicKey()),
                Format = cert.GetFormat(),
                RawCertDataBase64 = Convert.ToBase64String(cert.GetRawCertData())
            };
        }

        public CertificateInfo ReadCert_KeyName(string KeyName)
        {
            var cert = CertificateProcess.ReadCertKeyName(KeyName);
            return new CertificateInfo(){
                Issuer = cert.Issuer,
                EXP = cert.GetExpirationDateString(),
                Name = cert.GetName(),
                PublicKeyBase64 = Convert.ToBase64String(cert.GetPublicKey()),
                Format = cert.GetFormat(),
                RawCertDataBase64 = Convert.ToBase64String(cert.GetRawCertData())
            };
        }

        public bool Verify(string RawCert)
        {
            return CertificateProcess.VerifyCert(RawCert);
        }

        public bool Verify_KeyName(string KeyName)
        {
            var cert = CertificateProcess.ReadCertKeyName(KeyName);
            var cert_raw_data = Convert.ToBase64String(cert.GetRawCertData());
            return CertificateProcess.VerifyCert(cert_raw_data);
        }
    }
}