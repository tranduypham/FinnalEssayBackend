using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.DTO;
using Backend.Models;
using Backend.Services.Encryption;
using Backend.Services.WIMServices;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WimController : ControllerBase
    {
        IEncryptionServices _enc;
        IWimServices _wim;

        public WimController(IEncryptionServices encServ, IWimServices wim)
        {
            _enc = encServ;
            _wim = wim;
        }

        const string PIN_CODE = "123456";
        const string BANK_CODE = "1255070770448";

        [HttpPost("GetClientBankInfo")]
        public ActionResult<string> GetClientBankInfo([FromBody]PinVerify PIN, string bankAccount)
        {
            if(PIN.PIN.Equals(PIN_CODE)){
                if(bankAccount.Equals(BANK_CODE)){
                    return Ok(new {clientEncBankInfoBase64 = _wim.GetBankingInfo("client")});
                }
                return BadRequest(new {mess = "Bank account not found"});
            }
            return BadRequest(new {mess = "Wrong PIN code"});
        }

        [HttpPost("Pin_Verify")]
        public ActionResult<string> VerifyPin([FromBody]PinVerify PIN)
        {
            if(PIN.PIN.Equals(PIN_CODE)){
                return Ok(PIN.PIN);
            }
            return BadRequest(new {mess = "Wrong PIN code"});
        }
        
        [HttpGet("Gen_Random_Num")]
        public ActionResult<string> GetRandNumber(int length)
        {
            return _wim.RandNumString(length);
        }
        
        [HttpGet("Pre_Master_secret")]
        public ActionResult<string> GetPreMasterSecret()
        {
            return _wim.genPreMasterSecret();
        }
        
        [HttpGet("Master_secret")]
        public ActionResult<string> GetMasterSecret(string Pre_Master_Secret, string RandString)
        {
            return _wim.genMasterSecret(Pre_Master_Secret, RandString);
        }
        
        
        [HttpGet("Session_Key")]
        public ActionResult<SessionKey> GetSessionKey(string Master_Secret, string RandString)
        {
            var result = _wim.genSessionKey(Master_Secret, RandString);
            return result;
        }
        
        [HttpPost("SymEncrypt")]
        public ActionResult<string> AESEncryption([FromQuery]string plaintext, AESDto aesParam)
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
                return Ok(Convert.ToBase64String(cipher));
            }catch (Exception e) {
                return BadRequest(e.Message);
            }
        }
        
        [HttpPost("SymDecrypt")]
        public ActionResult<string> AESDecryption([FromQuery]string cipher, AESDto aesParam)
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
                return Ok(plainText);
            }catch (Exception e) {
                return BadRequest(e.Message);
            }
        }
        
    }
}