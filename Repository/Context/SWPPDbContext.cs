using Backend.Repository;
using Microsoft.EntityFrameworkCore;

namespace Backend.Models {
    public class SWPPDbContext : DbContext {
        public SWPPDbContext() { }
        public SWPPDbContext(DbContextOptions<SWPPDbContext> options) : base (options) { }

        // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        // {
        //     optionsBuilder.UseSqlServer("Server=DESKTOP-C00IDIB;Initial Catalog=SWPPStimulation;Integrated Security=True");
        // }

        public DbSet<BankingInfo> BankingInfos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<BankingInfo>().HasKey( x => x.BankId );
            SWPPSeed.Seed(modelBuilder);
        }
    }
}