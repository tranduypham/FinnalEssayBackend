using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Models;
using Backend.Repository.Entity;

namespace Backend.Repository
{
    public class BankingInfoRepo : IBankingInfoRepo
    {
        SWPPDbContext _dbContext;
        public BankingInfoRepo(SWPPDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Deposite(string AccountNumber, int Amount)
        {
            try{
                var account = _dbContext.BankAccounts
                                        .Where( x => x.ProfileNumber == AccountNumber )
                                        .FirstOrDefault();
                var balance = account.Balances;
                account.Balances = balance + Amount;
                // _dbContext.SaveChanges();
            }catch(Exception e){
                throw e;
            }
        }

        public BankingInfo? Get (string ProfileNumber) {
            return _dbContext.BankingInfos.Where( x => x.ProfileNumber == ProfileNumber )
                                            .FirstOrDefault();
        }

        public BankAccount GetBalance(string account)
        {
            return _dbContext.BankAccounts.Where(x => x.ProfileNumber == account).FirstOrDefault();
        }

        public void Save()
        {
            _dbContext.SaveChanges();
        }

        public void SaveTransaction(string Payer, string Payee, int Amount)
        {
            var trans = new Transaction(){
                Payee = Payee,
                Payer = Payer,
                Amount = Amount
            };
            _dbContext.Transactions.Add(trans);
        }

        public void WithDraw(string AccountNumber, int Amount)
        {
            try{
                var account = _dbContext.BankAccounts
                                        .Where( x => x.ProfileNumber == AccountNumber )
                                        .FirstOrDefault();
                var balance = account.Balances;
                if(balance - Amount >= 0){
                    account.Balances = balance - Amount;
                    // _dbContext.SaveChanges();
                }else{
                    throw new Exception("Not enough money in bank account");
                }
            }catch(Exception e){
                throw e;
            }
        }
    }
}