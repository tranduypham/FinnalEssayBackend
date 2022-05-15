using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Models;

namespace Backend.DTO
{
    public class ReplyClientWTLSRequest_ServerHello
    {
        public CertificateInfo Cert { get; set; }
        public string ClientRand { get; set; }
        public string GatewayRand { get; set; }
    }
}