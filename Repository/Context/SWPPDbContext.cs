using Backend.Repository;
using Backend.Repository.Entity;
using Microsoft.EntityFrameworkCore;

namespace Backend.Models
{
    public class SWPPDbContext : DbContext
    {
        public SWPPDbContext() { }
        public SWPPDbContext(DbContextOptions<SWPPDbContext> options) : base(options) { }

        // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        // {
        //     optionsBuilder.UseSqlServer("Server=DESKTOP-C00IDIB;Initial Catalog=SWPPStimulation;Integrated Security=True");
        // }

        public DbSet<BankingInfo> BankingInfos { get; set; }
        public DbSet<SessionKeys> SessionKeys { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BankingInfo>().HasKey(x => x.BankId);

            modelBuilder.Entity<BankAccount>().HasKey(x => x.ProfileNumber);
            modelBuilder.Entity<BankAccount>()
                        .HasOne<BankingInfo>()
                        .WithOne()
                        .HasForeignKey<BankingInfo>(x => x.ProfileNumber);

            modelBuilder.Entity<SessionKeys>().HasKey( x => x.SessionID );

            modelBuilder.Entity<Transaction>().HasKey( x => x.TransactionID );

            SWPPSeed.Seed(modelBuilder);
        }
    }
}