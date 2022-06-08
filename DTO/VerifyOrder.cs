using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.DTO
{
    public class VerifyOrder
    {
        public string InvoiceNumber { get; } = "1127625485";
        public UserProofile Payer { get; set; }
        public UserProofile Payee { get; set; }
        public int Amount { get; set; }
        public string RequestDate { get; set; }
    }
}