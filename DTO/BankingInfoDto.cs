using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Models;

namespace Backend.DTO
{
    public class BankingInfoDto
    {
        public BankingInfo BankingInfo { get; set; }
        public string Signature { get; set; }
    }
}