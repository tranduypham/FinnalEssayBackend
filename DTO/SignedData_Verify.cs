using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.DTO
{
    public class SignedData_Verify
    {
        public string KeyName { get; set; }
        public string DataRaw { get; set; }
        public string SignatureBase64 { get; set; }
    }
}