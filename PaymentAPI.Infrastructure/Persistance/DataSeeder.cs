using Microsoft.EntityFrameworkCore;
using PaymentAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentAPI.Infrastructure.Persistance
{
    public class DataSeeder
    {
        public static async Task SeedAsync(PaymentDbContext context)
        {
            // Apply migrations if pending
            await context.Database.MigrateAsync();

            // -------- MERCHANTS --------
            if (!context.Merchants.Any())
            {
                var merchants = new List<Merchant>
                {
                    new Merchant { Name = "TechMart", Email = "contact@techmart.com" },
                    new Merchant { Name = "StyleHub", Email = "support@stylehub.com" },
                    new Merchant { Name = "BookNest", Email = "info@booknest.com" },
                    new Merchant { Name = "FoodieBox", Email = "hello@foodiebox.com" },
                    new Merchant { Name = "AutoCare", Email = "service@autocare.com" }
                };
                await context.Merchants.AddRangeAsync(merchants);
                await context.SaveChangesAsync();
            }

            // -------- ACCOUNTS --------
            if (!context.Accounts.Any())
            {
                var accounts = new List<Account>
                {
                    new Account { HolderName = "TechMart Main", Balance = 15000m, MerchantId = 1 },
                    new Account { HolderName = "StyleHub Wallet", Balance = 8000m, MerchantId = 2 },
                    new Account { HolderName = "BookNest Central", Balance = 6000m, MerchantId = 3 },
                    new Account { HolderName = "FoodieBox HQ", Balance = 12000m, MerchantId = 4 },
                    new Account { HolderName = "AutoCare Finance", Balance = 9500m, MerchantId = 5 }
                };
                await context.Accounts.AddRangeAsync(accounts);
                await context.SaveChangesAsync();
            }

            // -------- PAYMENT METHODS --------
            if (!context.PaymentMethods.Any())
            {
                var methods = new List<PaymentMethod>
                {
                    new PaymentMethod { MethodName = "Credit Card", Provider = "Visa" },
                    new PaymentMethod { MethodName = "Debit Card", Provider = "MasterCard" },
                    new PaymentMethod { MethodName = "UPI", Provider = "Google Pay" },
                    new PaymentMethod { MethodName = "NetBanking", Provider = "ICICI Bank" },
                    new PaymentMethod { MethodName = "Wallet", Provider = "Paytm" }
                };
                await context.PaymentMethods.AddRangeAsync(methods);
                await context.SaveChangesAsync();
            }

            // -------- TRANSACTIONS (10 total) --------
            if (!context.Transactions.Any())
            {
                var transactions = new List<Transaction>
                {
                    new Transaction { Amount = 250.75m, Type = "Payment", Status = "Completed", ReferenceNumber = "TXN001", AccountId = 1, PaymentMethodId = 1 },
                    new Transaction { Amount = 120.00m, Type = "Refund", Status = "Completed", ReferenceNumber = "TXN002", AccountId = 2, PaymentMethodId = 2 },
                    new Transaction { Amount = 560.50m, Type = "Payment", Status = "Pending", ReferenceNumber = "TXN003", AccountId = 3, PaymentMethodId = 3 },
                    new Transaction { Amount = 300.00m, Type = "Payment", Status = "Completed", ReferenceNumber = "TXN004", AccountId = 4, PaymentMethodId = 4 },
                    new Transaction { Amount = 100.00m, Type = "Refund", Status = "Completed", ReferenceNumber = "TXN005", AccountId = 5, PaymentMethodId = 5 },
                    new Transaction { Amount = 999.99m, Type = "Payment", Status = "Completed", ReferenceNumber = "TXN006", AccountId = 1, PaymentMethodId = 3 },
                    new Transaction { Amount = 245.25m, Type = "Refund", Status = "Pending", ReferenceNumber = "TXN007", AccountId = 2, PaymentMethodId = 1 },
                    new Transaction { Amount = 780.00m, Type = "Payment", Status = "Completed", ReferenceNumber = "TXN008", AccountId = 3, PaymentMethodId = 2 },
                    new Transaction { Amount = 110.10m, Type = "Payment", Status = "Completed", ReferenceNumber = "TXN009", AccountId = 4, PaymentMethodId = 4 },
                    new Transaction { Amount = 420.42m, Type = "Refund", Status = "Completed", ReferenceNumber = "TXN010", AccountId = 5, PaymentMethodId = 5 }
                };

                await context.Transactions.AddRangeAsync(transactions);
                await context.SaveChangesAsync();
            }

            // -------- USERS --------
            if (!context.Users.Any())
            {
                var users = new List<User>
    {
        new User { Username = "admin", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"), Role = "Admin" },
        new User { Username = "user1", PasswordHash = BCrypt.Net.BCrypt.HashPassword("User@123"), Role = "User" }
    };

                await context.Users.AddRangeAsync(users);
                await context.SaveChangesAsync();
            }



        }


    }
}
