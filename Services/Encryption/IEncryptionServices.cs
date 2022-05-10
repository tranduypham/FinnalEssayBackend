using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Models;

namespace Backend.Services.Encryption
{
    public interface IEncryptionServices
    {
        public MyKeyPair GetKeys(string KeyName);
        public string EncryptData(string KeyName, string data, bool isPrivate);
        public string DecryptData(string KeyName, string data, bool isPrivate);
        public string SignData(string KeyName, string data); //Signed by private key
        public string HashData(string data); //Signed by private key
        public bool VerifySignature(string KeyName, string data, string signature);
    }
}