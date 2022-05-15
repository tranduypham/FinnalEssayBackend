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
        
    }
}