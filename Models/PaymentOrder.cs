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
        public string ClientBankProfileNumber { get; set; }
        public string MerchantBankProfileNumber { get; set; }
        public void Reset() {
            this.OrderInfo = 0;
            this.ClientVerifySignature = "";
            this.ClientBankProfileNumber = "";
            this.MerchantBankProfileNumber = "";
        }
    }
}