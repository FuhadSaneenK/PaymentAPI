using Microsoft.EntityFrameworkCore;
using PaymentAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PaymentAPI.Infrastructure.Persistance
{
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

    }
}
