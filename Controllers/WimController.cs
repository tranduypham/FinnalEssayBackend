using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.DTO;
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
        public ActionResult<string> GetRandNumber()
        {
            return null;
        }
        
        
        [HttpGet("Pre_Master_secret")]
        public ActionResult<string> GetPreMasterSecret()
        {
            return null;
        }
        
    }
}