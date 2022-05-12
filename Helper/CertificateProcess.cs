using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Backend.KeyPair.Client;
using Backend.KeyPair.Gateway;
using Backend.KeyPair.Merchant;
using Backend.KeyPair.Root;

namespace Backend.Helper
{
    public class CertificateProcess
    {
        public static X509Certificate ReadCertKeyName (string KeyName) {
            var temp = KeyName.ToLower();
            switch(temp) {
                case "client":
                {
                    byte[] bytes = ClientKeyPair.CertReader();
                    X509Certificate cert1 = new X509Certificate(bytes);
                    return cert1;
                }
                case "merchant":
                {
                    byte[] bytes = MerchantKeyPair.CertReader();
                    X509Certificate cert2 = new X509Certificate(bytes);
                    return cert2;
                }
                case "gateway" :
                {
                    byte[] bytes = GatewayKeyPair.CertReader();
                    X509Certificate cert3 = new X509Certificate(bytes);
                    return cert3;
                }
                case "root" :
                {
                    byte[] bytes = RootKeyPair.CertReader();
                    X509Certificate cert4 = new X509Certificate(bytes);
                    return cert4;
                }
                default :
                    return null;
            }
        }
        public static X509Certificate ReadCert(string RawCert)
        {
            var cert_bytes = Encoding.ASCII.GetBytes(RawCert);
            X509Certificate cert = new X509Certificate(cert_bytes);
            // return new {
            //     Issuer = cert.Issuer,
            //     ExpiredDate = cert.GetExpirationDateString(),
            //     Name = cert.GetName(),
            //     PublicKeyBase64 = Convert.ToBase64String(cert.GetPublicKey()),
            //     Format = cert.GetFormat()
            // };
            return cert;
        }

        public static bool VerifyCert(string RawCert)
        {
            var cert = ReadCert(RawCert);
            var issuer = cert.Issuer.Split(",");
            var CN = (from ele in issuer
                      where ele.Contains("CN")
                      select ele.Split("=")[1]).First();
            Console.WriteLine(CN);
            if(CN == "DuyRootCA") return true;
            return false;
        }
    }
}