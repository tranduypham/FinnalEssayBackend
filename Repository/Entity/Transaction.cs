using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Repository.Entity
{
    public class Transaction
    {
        public Guid TransactionID { get; set; }
        public string Payer { get; set; }
        public string Payee { get; set; }
        public int Amount { get; set; }
    }
}