using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.DTO
{
    public class CertDataInputDto
    {
        public string? KeyName { get; set; }
        public string? RawDataBase64 { get; set; }
    }
}