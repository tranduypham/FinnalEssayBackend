using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Repository.Entity
{
    public class SessionKeys
    {
        public Guid SessionID { get; set; }
        public string ClientWriteKeyBase64 { get; set; }
        public string ServerWriteKeyBase64 { get; set; }
        public string ClientWriteMacKeyBase64 { get; set; }
        public string ServerWriteMacKeyBase64 { get; set; }
    }
}