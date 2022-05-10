using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Models;

namespace Backend.Repository
{
    public class BankingInfoRepo : IBankingInfoRepo
    {
        SWPPDbContext _dbContext;
        public BankingInfoRepo(SWPPDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public BankingInfo? Get (string ProfileNumber) {
            return _dbContext.BankingInfos.Where( x => x.ProfileNumber == ProfileNumber )
                                            .FirstOrDefault();
        }
    }
}