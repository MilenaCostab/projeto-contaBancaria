using Microsoft.EntityFrameworkCore;

namespace ContaBancaria
{
    public class BancoDbContext : DbContext
    {
        public BancoDbContext(DbContextOptions<BancoDbContext> options) : base(options) { }

        public DbSet<Conta> Contas { get; set; }
        public DbSet<ContaCorrente> ContasCorrentes { get; set; }
        public DbSet<ContaPoupanca> ContasPoupancas { get; set; }
        public DbSet<ContaMista> ContasMistas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Conta>().HasKey(c => c.Cpf); 
        }
    }
}