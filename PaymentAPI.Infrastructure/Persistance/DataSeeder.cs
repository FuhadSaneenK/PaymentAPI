using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PaymentAPI.Domain.Entities;

namespace PaymentAPI.Infrastructure.Persistance;

public class DataSeeder
{
    public static async Task SeedAsync(PaymentDbContext context, ILogger? logger = null)
    {
        try
        {
            logger?.LogInformation("Starting database seeding...");

            // Apply migrations if pending
            await context.Database.MigrateAsync();
            logger?.LogInformation("Database migrations applied successfully");

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
                logger?.LogInformation("Seeded {Count} merchants", merchants.Count);
            }
            else
            {
                logger?.LogDebug("Merchants already exist, skipping seed");
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
                logger?.LogInformation("Seeded {Count} accounts", accounts.Count);
            }
            else
            {
                logger?.LogDebug("Accounts already exist, skipping seed");
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
                logger?.LogInformation("Seeded {Count} payment methods", methods.Count);
            }
            else
            {
                logger?.LogDebug("Payment methods already exist, skipping seed");
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
                logger?.LogInformation("Seeded {Count} transactions", transactions.Count);
            }
            else
            {
                logger?.LogDebug("Transactions already exist, skipping seed");
            }

            // -------- USERS --------
            if (!context.Users.Any())
            {
                // No users exist - seed new users
                var users = new List<User>
                {
                    // Admin user (no MerchantId - can see all merchants)
                    new User 
                    { 
                        Username = "admin", 
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"), 
                        Role = "Admin",
                        MerchantId = null  // Admin has no merchant restriction
                    },
                    
                    // Normal user for TechMart (MerchantId = 1)
                    new User 
                    { 
                        Username = "techmart_user", 
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("User@123"), 
                        Role = "User",
                        MerchantId = 1  // Can only see TechMart accounts
                    },
                    
                    // Normal user for StyleHub (MerchantId = 2)
                    new User 
                    { 
                        Username = "stylehub_user", 
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("User@123"), 
                        Role = "User",
                        MerchantId = 2  // Can only see StyleHub accounts
                    },
                    
                    // Keep existing user1 for backward compatibility (assign to TechMart)
                    new User 
                    { 
                        Username = "user1", 
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("User@123"), 
                        Role = "User",
                        MerchantId = 1  // Assigned to TechMart
                    }
                };

                await context.Users.AddRangeAsync(users);
                await context.SaveChangesAsync();
                logger?.LogInformation("Seeded {Count} users", users.Count);
            }
            else
            {
                // Users already exist - check if we need to update MerchantId
                logger?.LogDebug("Users already exist, checking if MerchantId needs to be updated...");
                
                var existingUsers = await context.Users.ToListAsync();
                bool updated = false;

                foreach (var user in existingUsers)
                {
                    // Only update if MerchantId is null and user is not Admin
                    if (user.MerchantId == null && user.Role != "Admin")
                    {
                        // Assign MerchantId based on username or default to TechMart
                        if (user.Username == "user1" || user.Username == "techmart_user")
                        {
                            user.MerchantId = 1; // TechMart
                            updated = true;
                            logger?.LogInformation("Updated user '{Username}' with MerchantId = 1 (TechMart)", user.Username);
                        }
                        else if (user.Username == "stylehub_user")
                        {
                            user.MerchantId = 2; // StyleHub
                            updated = true;
                            logger?.LogInformation("Updated user '{Username}' with MerchantId = 2 (StyleHub)", user.Username);
                        }
                        else if (user.Username == "booknest_user")
                        {
                            user.MerchantId = 3; // BookNest
                            updated = true;
                            logger?.LogInformation("Updated user '{Username}' with MerchantId = 3 (BookNest)", user.Username);
                        }
                        else
                        {
                            // Default: Assign to TechMart for any other non-admin users
                            user.MerchantId = 1; // TechMart
                            updated = true;
                            logger?.LogInformation("Updated user '{Username}' with MerchantId = 1 (TechMart - Default)", user.Username);
                        }
                    }
                    else if (user.Role == "Admin" && user.MerchantId != null)
                    {
                        // Ensure Admin users don't have a MerchantId
                        user.MerchantId = null;
                        updated = true;
                        logger?.LogInformation("Removed MerchantId from Admin user '{Username}'", user.Username);
                    }
                }

                if (updated)
                {
                    await context.SaveChangesAsync();
                    logger?.LogInformation("✅ Updated existing users with MerchantId assignments");
                }
                else
                {
                    logger?.LogDebug("No users needed MerchantId update");
                }
                
                // Check if we need to add new test users
                var usernamesInDb = existingUsers.Select(u => u.Username).ToList();
                var newUsers = new List<User>();

                if (!usernamesInDb.Contains("techmart_user"))
                {
                    newUsers.Add(new User 
                    { 
                        Username = "techmart_user", 
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("User@123"), 
                        Role = "User",
                        MerchantId = 1
                    });
                }

                if (!usernamesInDb.Contains("stylehub_user"))
                {
                    newUsers.Add(new User 
                    { 
                        Username = "stylehub_user", 
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("User@123"), 
                        Role = "User",
                        MerchantId = 2
                    });
                }

                if (newUsers.Any())
                {
                    await context.Users.AddRangeAsync(newUsers);
                    await context.SaveChangesAsync();
                    logger?.LogInformation("Added {Count} new test users", newUsers.Count);
                }
            }

            logger?.LogInformation("Database seeding completed successfully");
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Error occurred during database seeding");
            throw;
        }
    }
}
