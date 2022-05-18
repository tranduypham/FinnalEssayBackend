using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Repository.Entity
{
    public class BankAccount
    {
        public string ProfileNumber { get; set; }
        public int Balances { get; set; }
        public string Name { get; set; }
    }
}