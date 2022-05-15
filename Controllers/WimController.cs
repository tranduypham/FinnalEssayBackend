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
            return _wim.genSessionKey(Master_Secret, RandString);
        }
        
    }
}