using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Models
{
    public class BankingInfo
    {
        public Guid BankId { get; set; }
        public string? BankLocation { get; set; } = "HaNoi";
        public string ProfileNumber { get; set; }
    }
}