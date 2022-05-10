using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Backend.Helper;
using Backend.Models;

namespace Backend.Services.Encryption
{
    public class EncryptionServices : IEncryptionServices
    {
        RSA rsa;
        HashAlgorithmName _hashAlName = HashAlgorithmName.SHA256;
        RSASignaturePadding _rsaSignPadd = RSASignaturePadding.Pkcs1;
        public MyKeyPair GetKeys(string KeyName)
        {
            try
            {
                string rsaPem = KeyProcess.readKey(KeyName);
                rsa = RSA.Create();
                {
                    rsa.ImportFromPem(rsaPem);
                    var result = new MyKeyPair();
                    result.KeyName = KeyName;
                    result.PublicBase64 = Convert.ToBase64String(rsa.ExportRSAPublicKey());
                    result.PrivateBase64 = Convert.ToBase64String(rsa.ExportRSAPrivateKey());
                    return (result);
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public string EncryptData(string KeyName, string data, bool isPrivate = false)
        {
            try
            {
                this.GetKeys(KeyName);
                byte[] result;
                byte[] byteData = Encoding.ASCII.GetBytes(data);
                if (isPrivate)
                {
                    result = KeyProcess.PrivareEncryption_RSA(rsa, byteData);
                }
                else
                {
                    result = rsa.Encrypt(byteData, RSAEncryptionPadding.Pkcs1);
                }
                return Convert.ToBase64String(result);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public string DecryptData(string KeyName, string enctyptedData, bool isPrivate)
        {
            try
            {
                this.GetKeys(KeyName);
                byte[] result;
                byte[] byteData = Convert.FromBase64String(enctyptedData);
                if (isPrivate)
                {
                    result = KeyProcess.PublicDecryption_RSA(rsa, byteData);
                }
                else
                {
                    result = rsa.Decrypt(byteData, RSAEncryptionPadding.Pkcs1);
                }
                return Encoding.ASCII.GetString(result);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public string SignData(string KeyName, string data)
        {
            try
            {
                this.GetKeys(KeyName);
                byte[] result;
                byte[] byteData = Encoding.ASCII.GetBytes(data);
                result = rsa.SignData(byteData, 0, byteData.Length, _hashAlName, _rsaSignPadd);
                return Convert.ToBase64String(result);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public bool VerifySignature(string KeyName, string data, string signature)
        {
            try
            {
                this.GetKeys(KeyName);
                bool result;
                byte[] byteData = Encoding.ASCII.GetBytes(data);
                byte[] byteSignature = Convert.FromBase64String(signature);
                result = rsa.VerifyData(byteData, byteSignature, _hashAlName, RSASignaturePadding.Pkcs1);
                return result;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public string HashData(string data)
        {
            var hasher = SHA256.Create();
            byte[] byteData = Encoding.ASCII.GetBytes(data);
            var mess_hash = hasher.ComputeHash(byteData);

            return Convert.ToBase64String(mess_hash);
        }
    }
}