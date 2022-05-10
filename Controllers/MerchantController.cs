using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
//using Backend.Models;
using Backend.Services.Encryption;
using Backend.Services.WIMServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MerchantController : ControllerBase
    {
        const string INDICATE = "@@@@@";
        IEncryptionServices _enc;
        IWimServices _wim;

        public MerchantController(IEncryptionServices encServ, IWimServices wim)
        {
            _enc = encServ;
            _wim = wim;
        }

        private string EncBankingInfo (string Owner, string BankName, string indicate = INDICATE) {
            var BankInfo = _wim.GetBankingInfo(Owner);
            var encMerchantBankingInfo_Part1 =  _enc.EncryptData(
                BankName,
                BankInfo.Substring(0, 500),
                false
            );
            var encMerchantBankingInfo_Part2 =  _enc.EncryptData(
                BankName,
                BankInfo.Substring(500),
                false
            );
            return encMerchantBankingInfo_Part1 + indicate + encMerchantBankingInfo_Part2;
        }

        private string DecryptBankInfo (string data, string Owner, string BankName, string indicate = INDICATE){
            var encBankingInfo = data;
            var encBankingInfo_Part1 = data.Split(indicate)[0];
            var encBankingInfo_Part2 = data.Split(indicate)[1];
            var BankingInfo_Part1 = _enc.DecryptData(
                BankName,
                encBankingInfo_Part1,
                false
            );
            var BankingInfo_Part2 = _enc.DecryptData(
                BankName,
                encBankingInfo_Part2,
                false
            );
            return BankingInfo_Part1 + BankingInfo_Part2;
        }

        [HttpPost("SendInvoice")]
        public ActionResult<Object> PostInvoiceToMerchant(PaymentInfo PI)
        {
            Console.WriteLine(PI.Invoice);
            // Phải trả về những thông tin sau
            // (PI + BankingInfo(Merchant))Enc(PublicKey(CustomerBank))  +  (Invoice)Enc(PublicKey(Customer))
            // Thuat toan ma hoa co the la RSA_PKCS1
            // Kich thuoc key 2048
            // Output la 256
            var encPI = _enc.EncryptData(
                "client_bank",
                JsonSerializer.Serialize(PI),
                false
            );

            var encMerchantBankingInfo = this.EncBankingInfo("merchant", "client_bank");

            var encInvoice = _enc.EncryptData(
                "merchant",
                JsonSerializer.Serialize(
                    PI.Invoice
                ),
                true
            );
            return Ok(new
            {
                PI = encPI,
                MerchantBankingInfo = encMerchantBankingInfo,
                Invoice = encInvoice
            });
        }

    }
}