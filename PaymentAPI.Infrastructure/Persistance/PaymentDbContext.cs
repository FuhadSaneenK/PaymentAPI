using Microsoft.EntityFrameworkCore;
using PaymentAPI.Domain.Entities;
using System.Reflection;

namespace PaymentAPI.Infrastructure.Persistance;

public class PaymentDbContext : DbContext
{
    public PaymentDbContext(DbContextOptions<PaymentDbContext> options)
: base(options)
    {
    }
    // DbSets
    public DbSet<Merchant> Merchants { get; set; }
    public DbSet<Account> Accounts { get; set; }    
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<PaymentMethod>PaymentMethods { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<RefundRequest> RefundRequests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

}
