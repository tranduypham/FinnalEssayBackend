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
    }
}