using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.DTO;
using Backend.Repository;
using Microsoft.AspNetCore.Mvc;
//using Backend.Models;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MerchantBankController : ControllerBase
    {
        IBankingInfoRepo _bank;
        public MerchantBankController(IBankingInfoRepo bank)
        {
            _bank = bank;
        }

        [HttpPost("Make_Deposite")]
        public ActionResult<bool> PostDeposite(VerifyOrder payment)
        {
            var amount = payment.Amount;
            var payer = payment.Payer.AccountNumber;
            var payee = payment.Payee.AccountNumber;
            try
            {
                _bank.Deposite(payee, amount);
                _bank.SaveTransaction(payer, payee, amount);
                _bank.Save();
                Console.WriteLine("Merchant Successful deposite");
                return Ok(true);
            }
            catch (Exception e)
            {
                return BadRequest(false);
            }
        }

    }
}