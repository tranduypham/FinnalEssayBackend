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
    }
}