using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Models
{
    public class BankingInfo
    {
        public Guid BankId { get; set; }
        public string Name { get; set; } = "User";
        public string? BankLocation { get; set; } = "HaNoi";
        public string BankWebsite { get; set; } = "abc.com";
        public string ProfileNumber { get; set; }
    }
}