using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repository
{
    public class SWPPSeed
    {
        public static void Seed (ModelBuilder modelBuiler) {
            modelBuiler.Entity<BankingInfo>()
                    .HasData(
                        new BankingInfo{
                            BankId = new Guid("40822906-84f0-4dae-b8f0-696fce457db8"),
                            BankLocation = "Dong Da, Ha Noi, Ngan Hang co phan quan doi MB Bank, Merchant",
                            ProfileNumber = "7467811997849"
                        },
                        new BankingInfo{
                            BankId = new Guid("dbe552e4-37de-4ffb-b920-ab7caa9ebe0d"),
                            BankLocation = "Dong Da, Ha Noi, Ngan Hang co phan quan doi MB Bank, Client",
                            ProfileNumber = "1255070770448"
                        }
                    );
        }
    }
}
