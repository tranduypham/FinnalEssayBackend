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
        public GatewayController(ICertificateServices cert, IWimServices wim, IEncryptionServices enc)
        {
            _cert = cert;
            _wim = wim;
            _enc = enc;
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

        // [HttpPost("Secure_Link_Communicatioin")]
        // public ActionResult<string> SecureCommunicate([FromHeader] Guid SessionID, [FromQuery] string cipherMess)
        // {
        //     try
        //     {
        //         var serverReply = this.Encrypt_Reply(SessionID, cipherMess,() => {
        //             return cipherMess;
        //         });
        //         return Ok(serverReply);
        //     }
        //     catch (Exception e)
        //     {
        //         Console.WriteLine(e.Message);
        //         return BadRequest();
        //     }
        // }

        private static PaymentOrder PO = new PaymentOrder();
        private static string paymentInfoJSON = "";

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
                            PO.Reset();
                            var serverReply = this.Encrypt_Reply(SessionID, cipherMess, (message) =>
                            {
                                var pi = _enc.DecryptData(
                                    "client_bank",
                                    message,
                                    false
                                );
                                paymentInfoJSON = pi;

                                var paymentInfo = JsonSerializer.Deserialize<PaymentInfo>(pi, options);
                                Console.WriteLine("PI: {0}", pi);
                                Console.WriteLine("OrderInfo: {0}", paymentInfo.OrderInfo);
                                PO.OrderInfo = paymentInfo.OrderInfo;
                                return "Get Payment info, total amount is " + paymentInfo.OrderInfo;
                            });
                            return Ok(serverReply);

                        }
                    case "merchantverify":
                        {
                            var serverReply = this.Encrypt_Reply(SessionID, cipherMess, (message) =>
                            {
                                
                                var temp = JsonSerializer.Deserialize<PaymentInfo>(paymentInfoJSON, options);

                                Console.WriteLine("chu ky {0}{1}", message.Trim(), Environment.NewLine);
                                var invoice = temp.Invoice;
                                // invoice = invoice.Substring(1, invoice.Length - 2 ).ToString();
                                Console.WriteLine(@"thong tin don hang {0}{1}", invoice, Environment.NewLine);
                                var signature_valid = _enc.VerifySignature(
                                    "merchant",
                                    invoice,
                                    message.Trim()
                                );

                                Console.WriteLine("Signature Verify: {0}", signature_valid == true ? "true" : "false");
                                return "Get merchant signature, signature validity: " + signature_valid;
                            });
                            return Ok(serverReply);

                        }
                    case "cbankinfo":
                        {
                            var serverReply = this.Encrypt_Reply(SessionID, cipherMess, (message) =>
                            {
                                
                                var temp = JsonSerializer.Deserialize<BankingInfoDto>(message, options);
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
                                if(signature_valid){
                                    PO.ClientBankProfileNumber = info.ProfileNumber;
                                    return "Get Client Banking Info, Client Bank Profile Recieve: " +  PO.ClientBankProfileNumber;
                                }
                                return "Get Client Banking Info, Invalid information";
                            });
                            return Ok(serverReply);

                        }
                    case "mbankinfo":
                        {
                            var serverReply = this.Encrypt_Reply(SessionID, cipherMess, (message) =>
                            {
                                var temp = JsonSerializer.Deserialize<BankingInfoDto>(message, options);
                                var info = temp.BankingInfo;
                                var info_serialize = JsonSerializer.Serialize(info);
                                var sign = temp.Signature;

                                
                                // invoice = invoice.Substring(1, invoice.Length - 2 ).ToString();
                                Console.WriteLine("thong tin ngan hang merchant {0}{1}", info, Environment.NewLine);
                                var signature_valid = _enc.VerifySignature(
                                    "merchant_bank",
                                    info_serialize.Trim(),
                                    sign.Trim()
                                );

                                Console.WriteLine("Signature Verify: {0}", signature_valid == true ? "true" : "false");
                                if(signature_valid){
                                    PO.MerchantBankProfileNumber = info.ProfileNumber;
                                    return "Get Client Banking Info, Client Bank Profile Recieve: " +  PO.MerchantBankProfileNumber;
                                }
                                return "Get Client Banking Info, Invalid information";
                            });
                            return Ok(serverReply);

                        }
                    default:
                        {
                            // foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(PO))
                            // {
                            //     string name = descriptor.Name;
                            //     object value = descriptor.GetValue(PO);
                            //     Console.WriteLine("{0} = {1}", name, value);
                            // }
                            throw new Exception("Process not yet declare");
                        }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return BadRequest();
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

    }
}