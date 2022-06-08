using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Models
{
    public class PaymentOrder
    {
        public int OrderInfo { get; set; }
        public string ClientVerifySignature { get; set; }
        public BankingInfo ClientBankProfile { get; set; }
        public BankingInfo MerchantBankProfile { get; set; }
        public void Reset() {
            this.OrderInfo = 0;
            this.ClientVerifySignature = "";
            this.ClientBankProfile = new BankingInfo();
            this.MerchantBankProfile = new BankingInfo();
        }
    }
}