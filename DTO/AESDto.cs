using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.DTO
{
    public class AESDto
    {
        public string? PasswordBase64 { get; set; }
        public string? AuthPasswordBase64 { get; set; }
        public string? KeyName { get; set; }
        public Guid? SessionID { get; set; }
    }
}