using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Models;
using Backend.Repository.Entity;

namespace Backend.Repository
{
    public interface IBankingInfoRepo
    {
        public BankingInfo? Get (string ProfileNumber);
        public void WithDraw (string AccountNumber, int Amount);
        public void Deposite (string AccountNumber, int Amount);
        public void SaveTransaction (string Payer, string Payee, int Amount);
        public BankAccount GetBalance (string account);
        public void Save ();
    }
}