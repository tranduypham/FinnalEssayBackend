using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.DTO;
using Backend.Services.Certificate;
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
        public GatewayController(ICertificateServices cert, IWimServices wim)
        {
            _cert = cert;
            _wim = wim;
        }

        [HttpGet("RequestWTLS")]
        public ActionResult<ReplyClientWTLSRequest_ServerHello> ReplyClient_ServerHello(string ClientRand)
        {
            var cert = _cert.ReadCert_KeyName("gateway");
            var gateRand = _wim.RandNumString(20);
            var randNum = ClientRand;
            return Ok(new ReplyClientWTLSRequest_ServerHello(){
                Cert = cert,
                ClientRand = randNum,
                GatewayRand = gateRand
            });
        }

        [HttpPost("RecieveClientCert")]
        public ActionResult<string> Recieve_Cert_From_Client([FromQuery]string RawCert)
        {
            var verifyClientCert = _cert.Verify(RawCert);
            return Ok(verifyClientCert);
        }
        

        [HttpPost("VerifyGateCert")]
        public ActionResult<string> Verify_Cert_From_Client([FromQuery]string RawCert)
        {
            var verifyClientCert = _cert.Verify(RawCert);
            return Ok(verifyClientCert);
        }
        
        const string END_HANDSHAKE_MESS = "CREATE_KEYS_SUCCESSFULY_SECURE_LINK_HAS_BEEN_ESTABLISHED";
        [HttpPost("End_Handshake_message")]
        public ActionResult<string> Recieve_ClientEndHandshake([FromQuery]string cipherMess, [FromHeader]string SessionID, [FromQuery] string plainMess)
        {
            var messFromClient = this.SymDecrypt(cipherMess, new AESDto{KeyName = "client", SessionID = Guid.Parse(SessionID)});
            Console.WriteLine("{0}{0}{0}{0}Client : {1}{0}", Environment.NewLine, messFromClient);
            if(messFromClient != null && messFromClient == END_HANDSHAKE_MESS) {
                var endMess = "GATE_HANDSHAKE_END";
                var endHandShake = "CREATE_KEYS_SUCCESSFULY_SECURE_LINK_HAS_BEEN_ESTABLISHED";
                var Enc_endHandShake = this.SymEncrypt(endHandShake, new AESDto{
                    KeyName = "gateway",
                    SessionID = Guid.Parse(SessionID)
                });
                return Ok(new {
                    Mess = endMess,
                    Enc_Mess = Enc_endHandShake
                });
            }
            return BadRequest(new {Mess = "SecureLink can not be established, Try again later"});
            // Console.Write("Gateway: ");
            // var reply = Console.ReadLine().ToString();
            // Console.WriteLine();
            // var Enc_reply = this.SymEncrypt(reply, new AESDto{KeyName = "gateway"});
            // return Ok(Enc_reply);
        }

        private string SymEncrypt (string plaintext, AESDto aesParam)
        {
            try{
                var pass64 = aesParam.PasswordBase64;
                var authPass64 = aesParam.AuthPasswordBase64;
                var sessionID = aesParam.SessionID;
                var keyName = aesParam.KeyName;
                byte[] cipher = null;
                if(pass64 != null && authPass64 != null){
                    cipher = _wim.SymetricEncryption(plaintext, pass64, authPass64);
                }
                else if(sessionID.HasValue && keyName != null){
                    cipher = _wim.SymetricEncryption(plaintext, Guid.Parse( sessionID.ToString() ), keyName);
                }else if(!sessionID.HasValue && keyName!= null ){
                    cipher = _wim.SymetricEncryption(plaintext, keyName);
                }
                return Convert.ToBase64String(cipher);
            }catch (Exception e) {
                return null;
            }
        }

        private string SymDecrypt (string cipher, AESDto aesParam)
        {
            try{
                if (cipher == null) throw new Exception("Missing parameter cipher");
                var cipher_byte = Convert.FromBase64String(cipher);
                // var plainText = _wim.SymetricDecryption(cipher_byte, KeyName);
                // return plainText;
                var pass64 = aesParam.PasswordBase64;
                var authPass64 = aesParam.AuthPasswordBase64;
                var sessionID = aesParam.SessionID;
                var keyName = aesParam.KeyName;
                string plainText = "";
                if(pass64 != null && authPass64 != null){
                    plainText = _wim.SymetricDecryption(cipher_byte, pass64, authPass64);
                }
                else if(sessionID.HasValue && keyName != null){
                    plainText = _wim.SymetricDecryption(cipher_byte, Guid.Parse( sessionID.ToString() ), keyName);
                }else if(!sessionID.HasValue && keyName!= null ){
                    plainText = _wim.SymetricDecryption(cipher_byte, keyName);
                }
                return plainText;
            }catch (Exception e) {
                return null;
            }
        }
        
    }
}