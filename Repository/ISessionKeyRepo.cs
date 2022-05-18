using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Repository.Entity;

namespace Backend.Repository
{
    public interface ISessionKeyRepo
    {
        public void AddSessionKeys (SessionKeys sk);
        public SessionKeys getKey ();
        public SessionKeys getKey (string SessionID);
    }
}