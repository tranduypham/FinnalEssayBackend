using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.DTO;
using Backend.Models;

namespace Backend.Services.WIMServices
{
    public interface IWimServices
    {
        public BankingInfoDto? GetBankingInfo (string KeyName);
        public string RandNumString (int length);
        public string genPreMasterSecret();
        public string genMasterSecret(string Pre_Master_Secret, string RandString);
        public SessionKey genSessionKey (string Master_Secret_64, string RandString);
        public byte[] SymetricEncryption (string plaintText, string passwordBase64, string authPasswordBase64);
        public byte[] SymetricEncryption (string plaintText, Guid SessionID, string KeyName);
        public byte[] SymetricEncryption (string plaintText, string KeyName);
        public string SymetricDecryption (byte[] cipherText, string passwordBase64, string authPasswordBase64);
        public string SymetricDecryption (byte[] cipherText, Guid SessionID, string KeyName);
        public string SymetricDecryption (byte[] cipherText, string KeyName);
    }
}