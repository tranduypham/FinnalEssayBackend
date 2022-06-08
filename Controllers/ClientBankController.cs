using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using Backend.DTO;
using Backend.Models;
using Backend.Repository;
using Backend.Services.Certificate;
using Backend.Services.Encryption;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;
//using Backend.Models;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientBankController : ControllerBase
    {
        ICertificateServices _cert;
        IEncryptionServices _enc;
        IBankingInfoRepo _bank;
        IHttpClientFactory _httpClient;
        public ClientBankController(ICertificateServices cert, IEncryptionServices enc, IBankingInfoRepo bank, IHttpClientFactory httpClient)
        {
            _cert = cert;
            _enc = enc;
            _bank = bank;
            _httpClient = httpClient;
        }

        JsonSerializerOptions options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = true
        };
        private static PaymentOrder PO = new PaymentOrder();

        [HttpPost("clien_bank_info")]
        public ActionResult<string> PostTModel(BankingInfoDto jsonClientBankInfo)
        {
            PO.Reset();
            // var temp = JsonSerializer.Deserialize<BankingInfoDto>(jsonClientBankInfo);
            var temp = jsonClientBankInfo;
            var info = temp.BankingInfo;
            var info_serialize = JsonSerializer.Serialize(info);
            var sign = temp.Signature;


            // invoice = invoice.Substring(1, invoice.Length - 2 ).ToString();
            Console.WriteLine("thong tin ngan hang {0}{1}", info, Environment.NewLine);
            var signature_valid = _enc.VerifySignature(
                "client_bank",
                info_serialize.Trim(),
                sign.Trim()
            );

            Console.WriteLine("Signature Verify: {0}", signature_valid == true ? "true" : "false");
            if (signature_valid)
            {
                PO.ClientBankProfile = info;
                return Ok(info);
            }
            return BadRequest();
        }

        [HttpPost("merchant_bank_info")]
        public ActionResult<string> PostMerchant(BankingInfoDto jsonClientBankInfo)
        {
            var temp = jsonClientBankInfo;
            var info = temp.BankingInfo;
            var info_serialize = JsonSerializer.Serialize(info);
            var sign = temp.Signature;


            // invoice = invoice.Substring(1, invoice.Length - 2 ).ToString();
            Console.WriteLine("thong tin ngan hang merchant {0}{1}", info_serialize, Environment.NewLine);
            var signature_valid = _enc.VerifySignature(
                "merchant_bank",
                info_serialize.Trim(),
                sign.Trim()
            );

            Console.WriteLine("Signature Verify: {0}", signature_valid == true ? "true" : "false");
            if (signature_valid)
            {
                PO.MerchantBankProfile = info;
                return Ok(info);
                // return "Get Merchant Banking Info, Client Bank Profile Recieve: " + info.ProfileNumber;
            }
            return BadRequest();
            // return "Get Merchant Banking Info, Invalid information";
        }

        private static string paymentInfoJSON = "";
        private static string date = "";
        [HttpPost("payment_request_info")]
        public ActionResult<string> PostPaymentRequest(EncryptDataDto encPaymentRequestByClientBank)
        {
            var pi = _enc.DecryptData(
                    "client_bank",
                    encPaymentRequestByClientBank.EncData,
                    false
                );

            var paymentInfo = JsonSerializer.Deserialize<VerifyOrder>(pi, options);
            Console.WriteLine("PI: {0}", pi);
            Console.WriteLine("OrderInfo: {0}", paymentInfo.Amount);
            PO.OrderInfo = paymentInfo.Amount;
            date = paymentInfo.RequestDate;
            
            var result = new VerifyOrder()
            {
                Payer = paymentInfo.Payer,
                Payee = paymentInfo.Payee,
                Amount = paymentInfo.Amount,
                RequestDate = paymentInfo.RequestDate
            };
            paymentInfoJSON = JsonSerializer.Serialize(result);
            return JsonSerializer.Serialize(result);
        }

        [HttpPost("verify_payment_request_info")]
        public ActionResult<string> PostVerifyPaymentRequest(SignedData_Verify data)
        {
            var signature = data.SignatureBase64.Trim();

            var signature_valid = _enc.VerifySignature(
                "merchant",
                paymentInfoJSON,
                signature
            );

            Console.WriteLine("Signature Verify: {0}", signature_valid == true ? "true" : "false");
            return Ok(signature_valid);
        }

        const string BANK_CODE = "1255070770448";
        [HttpPost("verify_bank_account")]
        public ActionResult<string> PostVerifyBankAccount(BankAccountDto clientAccount)
        {
            if (clientAccount.BankAccount == BANK_CODE)
            {
                return Ok(true);
            }
            return Ok(false);
        }
        [HttpPost("verify_client_signature")]
        public ActionResult<bool> PostVerifyClientSignature(SignedData_Verify data)
        {
            var result = _enc.VerifySignature("client", paymentInfoJSON, data.SignatureBase64);
            return result;
        }
        
        private string createPostRequest (string JsonData, string uri) {
            var httpClient = _httpClient.CreateClient();
            httpClient.BaseAddress = new Uri((Request.IsHttps ? "https://" : "http://") + Request.Host);
            var request = new StringContent(
                JsonData,
                Encoding.UTF8,
                Application.Json
            );
            return httpClient.PostAsync(uri, request).Result.Content.ReadAsStringAsync().Result;
        }
        
        [HttpPost("make_withdraw")]
        public ActionResult<bool> PostMakeDiposit(VerifyOrder payment)
        {
            var amount = payment.Amount;
            var payer = payment.Payer.AccountNumber;
            var payee = payment.Payee.AccountNumber;

            try {
                _bank.WithDraw(payer, amount);
                var makeMerchantWithDraw = createPostRequest(
                    JsonSerializer.Serialize(payment, options),
                    "\\api\\MerchantBank\\Make_Deposite"
                );
                if(makeMerchantWithDraw.ToLower() == "true"){
                    Console.WriteLine("Client Successful withdraw");
                    _bank.Save();
                    return Ok(true);
                }
                throw new Exception("Error While Transaction");
            }catch (Exception e) {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("account_state")]
        public ActionResult<string> PostTModel()
        {
            var bankAccount = _bank.GetBalance(PO.ClientBankProfile.ProfileNumber);
            return Ok(new {
                Payee = PO.MerchantBankProfile,
                Amount = PO.OrderInfo,
                Date = date,
                Client = bankAccount
            });
        }
    }
}