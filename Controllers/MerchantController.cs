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
using Backend.DTO;
using System.Text;
using System.Text.Encodings.Web;
using Backend.Repository;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MerchantController : ControllerBase
    {
        const string INDICATE = "@@@@@";
        IEncryptionServices _enc;
        IWimServices _wim;
        IBankingInfoRepo _bInfo;

        public MerchantController(IEncryptionServices encServ, IWimServices wim, IBankingInfoRepo bInfo)
        {
            _enc = encServ;
            _wim = wim;
            _bInfo = bInfo;
        }

        private string EncBankingInfo(string Owner, string BankName, string indicate = INDICATE)
        {
            var BankInfo = JsonSerializer.Serialize(_wim.GetBankingInfo(Owner));
            var encMerchantBankingInfo_Part1 = _enc.EncryptData(
                BankName,
                BankInfo.Substring(0, 500),
                false
            );
            var encMerchantBankingInfo_Part2 = _enc.EncryptData(
                BankName,
                BankInfo.Substring(500),
                false
            );
            return encMerchantBankingInfo_Part1 + indicate + encMerchantBankingInfo_Part2;
        }
        private string EncTooLongInfo(string data, string KeyName, string indicate = INDICATE)
        {
            var encMerchantTooLongInfo_Part1 = _enc.EncryptData(
                KeyName,
                data.Substring(0, 500),
                false
            );
            var encMerchantTooLongInfo_Part2 = _enc.EncryptData(
                KeyName,
                data.Substring(500),
                false
            );
            return encMerchantTooLongInfo_Part1 + indicate + encMerchantTooLongInfo_Part2;
        }

        private BankingInfoDto DecryptBankInfo(string data, string Owner, string BankName, string indicate = INDICATE)
        {
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
            var utf8_byte = Encoding.UTF8.GetBytes(BankingInfo_Part1 + BankingInfo_Part2);
            var utf8_string = Encoding.UTF8.GetString(utf8_byte);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            return JsonSerializer.Deserialize<BankingInfoDto>(utf8_string, options);
        }

        private string DecryptTooLongInfo(string data, string Owner, string KeyName, string indicate = INDICATE)
        {
            var encBankingInfo = data;
            var encBankingInfo_Part1 = data.Split(indicate)[0];
            var encBankingInfo_Part2 = data.Split(indicate)[1];
            var BankingInfo_Part1 = _enc.DecryptData(
                KeyName,
                encBankingInfo_Part1,
                false
            );
            var BankingInfo_Part2 = _enc.DecryptData(
                KeyName,
                encBankingInfo_Part2,
                false
            );
            var utf8_byte = Encoding.UTF8.GetBytes(BankingInfo_Part1 + BankingInfo_Part2);
            var utf8_string = Encoding.UTF8.GetString(utf8_byte);
            // var options = new JsonSerializerOptions
            // {
            //     PropertyNameCaseInsensitive = true,
            //     Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            //     WriteIndented = true
            // };
            return utf8_string;
        }

        
        static bool isLogin = false; 
        [HttpPost("VerifyUser")]
        public ActionResult<bool> PostTModel([FromBody] PinVerify pin)
        {
            if(!isLogin){
                if (pin.PIN == "123456"){
                    isLogin = true;
                }
            }
            return isLogin;
        }

        [HttpPost("Try_Decrypt_Banking_Info")]
        public ActionResult<BankingInfoDto> PostBankingInfoDto([FromHeader] string encBankInfo)
        {
            try
            {
                return Ok(DecryptBankInfo(encBankInfo, "merchant", "client_bank"));
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [HttpPost("Try_Decrypt_Too_Long_Info")]
        public ActionResult<string> PostTooLongInfoDto([FromHeader] string encTooLongInfo)
        {
            try
            {
                return Ok(DecryptTooLongInfo(encTooLongInfo, "merchant", "client_bank"));
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }


        [HttpPost("SendInvoice")]
        public ActionResult<Object> PostInvoiceToMerchant(PaymentInfo PI)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            };
            Console.WriteLine(PI.Invoice);
            // Phải trả về những thông tin sau
            // (PI + BankingInfo(Merchant))Enc(PublicKey(CustomerBank))  +  (Invoice)Enc(PublicKey(Customer))
            // Thuat toan ma hoa co the la RSA_PKCS1
            // Kich thuoc key 2048
            // Output la 256
            var merchant = _bInfo.Get("7467811997849");
            var client = _bInfo.Get("1255070770448");
            var result = new VerifyOrder()
            {
                Payer = new UserProofile()
                {
                    Name = client.Name,
                    AccountNumber = client.ProfileNumber
                },
                Payee = new UserProofile()
                {
                    Name =  merchant.Name,
                    AccountNumber =  merchant.ProfileNumber
                },
                Amount = PI.OrderInfo,
                RequestDate = DateTime.Now.ToShortDateString()
            };
            var temptemp = JsonSerializer.Serialize(result);
            var encPI = _enc.EncryptData(
                "client_bank",
                JsonSerializer.Serialize(result),
                false
            );

            var encMerchantBankingInfo = this.EncBankingInfo("merchant", "client_bank");

            var encInvoice = _enc.EncryptData(
                "client",
                JsonSerializer.Serialize(
                    PI.Invoice
                ),
                true
            );
            var tmp = PI.Invoice;
            var merchant_SignInvoice = _enc.SignData("merchant", PI.Invoice);
            var temp1 = JsonSerializer.Serialize(result);
            var temp2 = JsonSerializer.Serialize(result, options);

            var merchant_SignPaymentInfo = _enc.SignData("merchant", JsonSerializer.Serialize(result));
            var temp_merchant_SignPaymentInfo = this.EncTooLongInfo(merchant_SignPaymentInfo, "client_bank");
            return Ok(new
            {
                PI = encPI,
                Merchant_Sign_Invoice = merchant_SignInvoice,
                // Merchant_Sign_PaymentInfo_not_encrypt = merchant_SignPaymentInfo,
                Merchant_Sign_PaymentInfo = temp_merchant_SignPaymentInfo,
                MerchantBankingInfo = encMerchantBankingInfo,
                Invoice = encInvoice
            });
        }

    }
}