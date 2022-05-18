using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Models
{
    public class SessionKey
    {
        public string SessionID { get; set; }
        public string ClientWriteKey64 { get; set; }
        public string ServerWriteKey64 { get; set; }
        public string ClientWriteMacKey64 { get; set; }
        public string ServerWriteMacKey64 { get; set; }
    }
}