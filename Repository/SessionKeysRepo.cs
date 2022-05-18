using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Models;
using Backend.Repository.Entity;

namespace Backend.Repository
{
    public class SessionKeysRepo : ISessionKeyRepo
    {
        SWPPDbContext _dbContext;
        public SessionKeysRepo(SWPPDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void AddSessionKeys(SessionKeys sk)
        {
            _dbContext.SessionKeys.Add(sk);
            _dbContext.SaveChanges();
        }

        public SessionKeys getKey()
        {
            return _dbContext.SessionKeys.OrderByDescending(x => x.SessionID ).First();
        }

        public SessionKeys getKey(string SessionID)
        {
            return _dbContext.SessionKeys.Find(Guid.Parse(SessionID));
        }
    }
}