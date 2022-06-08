using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;
using Backend.DTO;
using Backend.Models;
using Backend.Services.Certificate;
using Backend.Services.Encryption;
using Backend.Services.WIMServices;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;
//using Backend.Models;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GatewayController : ControllerBase
    {
        ICertificateServices _cert;
        IWimServices _wim;
        IEncryptionServices _enc;
        IHttpClientFactory _httpClient;
        public GatewayController(ICertificateServices cert, IWimServices wim, IEncryptionServices enc, IHttpClientFactory httpClient)
        {
            _cert = cert;
            _wim = wim;
            _enc = enc;
            _httpClient = httpClient;
        }

        [HttpGet("Test")]
        public async Task<ActionResult<string>> GetTModel(string method, string uri, string data)
        {
            return await createRequest(method, uri, data);
        }



        [HttpGet("RequestWTLS")]
        public ActionResult<ReplyClientWTLSRequest_ServerHello> ReplyClient_ServerHello(string ClientRand)
        {
            var cert = _cert.ReadCert_KeyName("gateway");
            var gateRand = _wim.RandNumString(20);
            var randNum = ClientRand;
            return Ok(new ReplyClientWTLSRequest_ServerHello()
            {
                Cert = cert,
                ClientRand = randNum,
                GatewayRand = gateRand
            });
        }

        [HttpPost("RecieveClientCert")]
        public ActionResult<string> Recieve_Cert_From_Client([FromQuery] string RawCert)
        {
            var verifyClientCert = _cert.Verify(RawCert);
            return Ok(verifyClientCert);
        }


        [HttpPost("VerifyGateCert")]
        public ActionResult<string> Verify_Cert_From_Client([FromQuery] string RawCert)
        {
            var verifyClientCert = _cert.Verify(RawCert);
            return Ok(verifyClientCert);
        }

        const string END_HANDSHAKE_MESS = "CREATE_KEYS_SUCCESSFULY_SECURE_LINK_HAS_BEEN_ESTABLISHED";
        [HttpPost("End_Handshake_message")]
        public ActionResult<string> Recieve_ClientEndHandshake([FromQuery] string cipherMess, [FromHeader] string SessionID, [FromQuery] string plainMess)
        {
            var messFromClient = this.SymDecrypt(cipherMess, new AESDto { KeyName = "client", SessionID = Guid.Parse(SessionID) });
            Console.WriteLine("{0}{0}{0}{0}Client : {1}{0}", Environment.NewLine, messFromClient);
            Console.WriteLine("ACKNOWNLEDGE CLIENT DONE HANDSHAKE");
            if (messFromClient != null && messFromClient == END_HANDSHAKE_MESS)
            {
                Console.WriteLine("SUCCESSFUL READ CLIENT ENCRYPT MESSAGE ");
                Console.WriteLine("PROCESSING SENDING ENCRYPT MESSAGE TO CLIENT ...");

                var endMess = "GATE_HANDSHAKE_END";
                var endHandShake = "CREATE_KEYS_SUCCESSFULY_SECURE_LINK_HAS_BEEN_ESTABLISHED";
                var Enc_endHandShake = this.SymEncrypt(endHandShake, new AESDto
                {
                    KeyName = "gateway",
                    SessionID = Guid.Parse(SessionID)
                });
                Console.WriteLine(" ENDING HANDSHAKE PHASE ");
                Console.WriteLine(" -------------------------------------------------------- ");
                return Ok(new
                {
                    Mess = endMess,
                    Enc_Mess = Enc_endHandShake
                });
            }
            Console.WriteLine("CAN NOT READ CLIENT ENCRYPT MESSAGE ");
            return BadRequest(new { Mess = "SecureLink can not be established, Try again later" });
            // Console.Write("Gateway: ");
            // var reply = Console.ReadLine().ToString();
            // Console.WriteLine();
            // var Enc_reply = this.SymEncrypt(reply, new AESDto{KeyName = "gateway"});
            // return Ok(Enc_reply);
        }


        private static PaymentOrder PO = new PaymentOrder();
        private static string paymentInfoJSON = "";

        const string BANK_CODE = "1255070770448";
        [HttpPost("Verify_client")]
        public ActionResult<bool> VerifyClientAccount(BankAccountDto clientAccount)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    WriteIndented = true
                };
                var result = createRequest(
                    "Post",
                    "\\api\\ClientBank\\verify_bank_account",
                    JsonSerializer.Serialize(clientAccount, options)
                ).Result;
                if (result == "true")
                {

                    return Ok(true);
                }
                throw new Exception("Wrong validate");
            }
            catch (Exception e)
            {
                return BadRequest(false);
            }
        }

        [HttpPost("Verify_client_signature")]
        public ActionResult<bool> VerifyClientSignature([FromQuery] string signature)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    WriteIndented = true
                };
                var send = new SignedData_Verify
                {
                    KeyName = "client",
                    DataRaw = "",
                    SignatureBase64 = signature
                };
                var result = createRequest(
                    "post",
                    "\\api\\ClientBank\\verify_client_signature",
                    JsonSerializer.Serialize(send, options)
                ).Result;
                if (result == "true")
                {
                    var deposit = createRequest(
                        "Post",
                        "\\api\\ClientBank\\make_withdraw",
                        paymentInfoJSON
                    ).Result;
                    // Console.WriteLine(deposit);
                    // Console.WriteLine((Request.IsHttps ? "https://" : "http://") + Request.Host);
                    if (deposit == "true"){
                        return Ok(true);
                    }
                }
                throw new Exception("Wrong validate");
            }
            catch (Exception e)
            {
                return BadRequest(false);
            }
        }


        static string orederToVerify = "";
        [HttpPost("Secure_Link_Communicatioin")]
        public ActionResult<string> SecureCommunicate([FromHeader] Guid SessionID, [FromQuery] string cipherMess, [FromHeader] string Process)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            };
            try
            {
                switch (Process.ToLower())
                {
                    case "paymentinfo":
                        {
                            var serverReply = this.Encrypt_Reply(SessionID, cipherMess, (message) =>
                            {
                                try
                                {
                                    var sendData = new EncryptDataDto
                                    {
                                        EncData = message
                                    };
                                    // var temp = message.Replace("\\", "");
                                    var result = createRequest(
                                        "POST",
                                        "\\api\\ClientBank\\payment_request_info",
                                        JsonSerializer.Serialize(sendData, options)
                                    ).Result;
                                    paymentInfoJSON = result;
                                    var info = JsonSerializer.Deserialize<VerifyOrder>(result, options);
                                    PO.OrderInfo = info.Amount;
                                    return result;
                                }
                                catch (Exception e)
                                {
                                    throw new Exception(e.Message);
                                    
                                }
                            });
                            return Ok(serverReply);

                        }
                    case "merchantverify":
                        {

                            var serverReply = this.Encrypt_Reply(SessionID, cipherMess, (message) =>
                            {
                                try
                                {
                                    var sendData = new SignedData_Verify
                                    {
                                        KeyName = "",
                                        DataRaw = paymentInfoJSON,
                                        SignatureBase64 = message
                                    };
                                    // var temp = message.Replace("\\", "");
                                    var result = createRequest(
                                        "POST",
                                        "\\api\\ClientBank\\verify_payment_request_info",
                                        JsonSerializer.Serialize(sendData, options)
                                    ).Result;

                                    return "Get merchant signature, signature validity: " + result;
                                }
                                catch (Exception e)
                                {
                                    throw new Exception(e.Message);
                                }
                            });
                            return Ok(serverReply);

                        }
                    case "cbankinfo":
                        {
                            PO.Reset();
                            var serverReply = this.Encrypt_Reply(SessionID, cipherMess, (message) =>
                            {
                                try
                                {
                                    // var temp = message.Replace("\\", "");
                                    var result = createRequest("POST", "\\api\\ClientBank\\clien_bank_info", message).Result;
                                    var info = JsonSerializer.Deserialize<BankingInfo>(result, options);
                                    PO.ClientBankProfile = info;
                                    return "Get Client Banking Info, Client Bank Profile Recieve: " + info.ProfileNumber;
                                }
                                catch (Exception e)
                                {
                                    return "Get Client Banking Info, Invalid information" + e.Message;
                                }
                            });
                            return Ok(serverReply);

                        }
                    case "mbankinfo":
                        {
                            var serverReply = this.Encrypt_Reply(SessionID, cipherMess, (message) =>
                            {
                                try
                                {
                                    // var temp = message.Replace("\\", "");
                                    var result = createRequest("POST", "\\api\\ClientBank\\merchant_bank_info", message).Result;
                                    var info = JsonSerializer.Deserialize<BankingInfo>(result, options);
                                    PO.MerchantBankProfile = info;
                                    return "Get Client Banking Info, Client Bank Profile Recieve: " + info.ProfileNumber;
                                }
                                catch (Exception e)
                                {
                                    return "Get Client Banking Info, Invalid information" + e.Message;
                                }
                                
                            });
                            return Ok(serverReply);

                        }
                    default:
                        {

                            throw new Exception("Process not yet declare");
                        }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return BadRequest(e.Message);
            }
        }

        private async Task<string> createRequest(string method, string uri, string data)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            };
            using var httpClient = _httpClient.CreateClient();
            // httpClient.BaseAddress = new Uri(HttpContext.Request.Host.ToString());
            // BankingInfoDto temp = JsonSerializer.Deserialize<BankingInfoDto>(data, options) ?? throw new Exception("Sai doan nay");
            httpClient.BaseAddress = new Uri("https://localhost:7109");
            var requestData = new StringContent(
                data,
                Encoding.UTF8,
                Application.Json
            );
            switch (method.ToLower())
            {
                case "get":
                    {
                        return await httpClient.GetAsync(uri).Result.Content.ReadAsStringAsync();
                    }
                case "post":
                    {
                        var postResult = await httpClient.PostAsync(uri, requestData).Result.Content.ReadAsStringAsync();
                        return postResult;
                    }
                default:
                    {
                        throw new Exception("Request to Bank Fail");
                    }
            }
        }
        private string Encrypt_Reply(Guid SessionID, string client_cipher_mess, Func<string, string>? clientReplyMethod)
        {
            var messFromClient = this.SymDecrypt(client_cipher_mess, new AESDto { KeyName = "client", SessionID = SessionID });
            Console.WriteLine("{0}{0}{0}{0}Client : {1}{0}", Environment.NewLine, messFromClient);
            Console.Write("Server : ");
            var serverMess = clientReplyMethod(messFromClient);
            // var serverMess = messFromClient;
            var Enc_serverMess = this.SymEncrypt(serverMess, new AESDto
            {
                KeyName = "gateway",
                SessionID = SessionID
            });
            return Enc_serverMess;
        }


        private string SymEncrypt(string plaintext, AESDto aesParam)
        {
            try
            {
                var pass64 = aesParam.PasswordBase64;
                var authPass64 = aesParam.AuthPasswordBase64;
                var sessionID = aesParam.SessionID;
                var keyName = aesParam.KeyName;
                byte[] cipher = null;
                if (pass64 != null && authPass64 != null)
                {
                    cipher = _wim.SymetricEncryption(plaintext, pass64, authPass64);
                }
                else if (sessionID.HasValue && keyName != null)
                {
                    cipher = _wim.SymetricEncryption(plaintext, Guid.Parse(sessionID.ToString()), keyName);
                }
                else if (!sessionID.HasValue && keyName != null)
                {
                    cipher = _wim.SymetricEncryption(plaintext, keyName);
                }
                return Convert.ToBase64String(cipher);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private string SymDecrypt(string cipher, AESDto aesParam)
        {
            try
            {
                if (cipher == null) throw new Exception("Missing parameter cipher");
                var cipher_byte = Convert.FromBase64String(cipher);
                // var plainText = _wim.SymetricDecryption(cipher_byte, KeyName);
                // return plainText;
                var pass64 = aesParam.PasswordBase64;
                var authPass64 = aesParam.AuthPasswordBase64;
                var sessionID = aesParam.SessionID;
                var keyName = aesParam.KeyName;
                string plainText = "";
                if (pass64 != null && authPass64 != null)
                {
                    plainText = _wim.SymetricDecryption(cipher_byte, pass64, authPass64);
                }
                else if (sessionID.HasValue && keyName != null)
                {
                    plainText = _wim.SymetricDecryption(cipher_byte, Guid.Parse(sessionID.ToString()), keyName);
                }
                else if (!sessionID.HasValue && keyName != null)
                {
                    plainText = _wim.SymetricDecryption(cipher_byte, keyName);
                }
                return plainText;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        [HttpGet("bank_statement")]
        public ActionResult<IEnumerable<string>> GetStatement()
        {
            var result = this.createRequest(
                "post",
                "\\api\\ClientBank\\account_state",
                ""
            ).Result;
            return Ok(result);
        }
        
    }
}