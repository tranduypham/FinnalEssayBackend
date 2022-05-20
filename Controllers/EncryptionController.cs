using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.DTO;
using Backend.Helper;
using Backend.Models;
using Backend.Services.Encryption;
using Microsoft.AspNetCore.Mvc;
//using Backend.Models;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EncryptionController : ControllerBase
    {
        IEncryptionServices _encryptionServices;
        public EncryptionController(IEncryptionServices encryptionServices)
        {
            _encryptionServices = encryptionServices;
        }

        [HttpGet("")]
        public ActionResult<MyKeyPair> GetKeyPair([FromQuery] string KeyName)
        {
            var keyPair = _encryptionServices.GetKeys(KeyName);
            Console.WriteLine(keyPair);
            if (keyPair == null) return NotFound();
            return Ok(keyPair);
        }
        [HttpPost("Encrypt")]
        public ActionResult<EncryptedData> EncryptData(string KeyName, string data, bool? usePrivate)
        {
            EncryptedData result = new EncryptedData();
            if (usePrivate.HasValue && usePrivate == true)
            {
                var encData = _encryptionServices.EncryptData(KeyName, data, true);
                if (encData == null) return BadRequest();
                result.DataBase64 = encData;
                result.isPrivate = true;
                result.KeyName = KeyName;
            }
            else
            {
                var encData = _encryptionServices.EncryptData(KeyName, data, false);
                if (encData == null) return BadRequest();
                result.DataBase64 = encData;
                result.isPrivate = false;
                result.KeyName = KeyName;
            }
            return Ok(result);
        }
        [HttpPost("Decrypt")]
        public ActionResult<DecryptedData> DecryptData(string KeyName, string data, bool? usePrivate)
        {
            DecryptedData result = new DecryptedData();
            if (usePrivate.HasValue && usePrivate == true)
            {
                var dencData = _encryptionServices.DecryptData(KeyName, data, true);
                if (dencData == null) return BadRequest();
                result.DataBase64 = dencData;
                result.isPrivate = true;
                result.KeyName = KeyName;
            }
            else
            {
                var dencData = _encryptionServices.DecryptData(KeyName, data, false);
                if (dencData == null) return BadRequest();
                result.DataBase64 = dencData;
                result.isPrivate = false;
                result.KeyName = KeyName;
            }
            return Ok(result);
        }
        [HttpPost("SignData")]
        public ActionResult<SignedData> SignData(string KeyName, string data)
        {
            try
            {
                
                SignedData result = new SignedData();
                var signature = _encryptionServices.SignData(KeyName, data);
                result.DataRaw = data;
                result.DataBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(data));
                result.DigitalSignatureBase64 = signature;
                result.KeyName = KeyName;
                result.isPrivate = false;
                return Ok(result);
            }
            catch (Exception e)
            {
                return null;
            }
        }
        [HttpPost("Verify")]
        public ActionResult<bool> VerifyData(SignedData_Verify data)
        {
            try
            {
                bool result = false;
                result = _encryptionServices.VerifySignature(data.KeyName, data.DataRaw, data.SignatureBase64);
                return Ok(result);
            }
            catch (Exception e)
            {
                return false;
            }
        }
        [HttpPost("Hash-SHA256")]
        public ActionResult<string> HashData(string data)
        {
            try
            {
                return Ok(_encryptionServices.HashData(data));
            }
            catch (Exception e)
            {
                return null;
            }
        }


    }
}