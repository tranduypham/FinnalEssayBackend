using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Models;

namespace Backend.Repository
{
    public interface IBankingInfoRepo
    {
        public BankingInfo? Get (string ProfileNumber);
    }
}