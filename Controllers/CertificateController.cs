using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.DTO;
using Backend.Models;
using Backend.Services.Certificate;
using Microsoft.AspNetCore.Mvc;
//using Backend.Models;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CertificateController : ControllerBase
    {
        ICertificateServices _cert;
        public CertificateController(ICertificateServices cert)
        {
            _cert = cert;
        }

        [HttpPost("")]
        public ActionResult<CertificateInfo> GetCert(CertDataInputDto certDto)
        {
            try{
                if(certDto.RawDataBase64 != null && certDto.RawDataBase64 != ""){
                    return Ok( _cert.ReadCert(certDto.RawDataBase64) );
                    // return Ok( "raw" );
                }
                
                if(certDto.KeyName != null && certDto.KeyName != ""){
                    return Ok( _cert.ReadCert_KeyName(certDto.KeyName));
                    // return Ok( "key" );
                }
            } catch (Exception e) {

            }
            return BadRequest();
        }

        [HttpPost("Verify")]
        public ActionResult<CertificateInfo> VerifyCert(CertDataInputDto certDto)
        {
            try{
                if(certDto.RawDataBase64 != null && certDto.RawDataBase64 != ""){
                    return Ok( _cert.Verify(certDto.RawDataBase64) );
                    // return Ok( "raw" );
                }
                
                if(certDto.KeyName != null && certDto.KeyName != ""){
                    return Ok( _cert.Verify_KeyName(certDto.KeyName));
                    // return Ok( "key" );
                }
            } catch (Exception e) {

            }
            return Ok(false);
        }
        
    }
}