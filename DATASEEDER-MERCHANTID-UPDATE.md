# DataSeeder MerchantId Update - Implementation Guide

## ? What Was Fixed

The `DataSeeder` now **automatically updates existing users** with `MerchantId` values when the application starts.

---

## ?? How It Works

### Before (Problem)
```
Users table:
| Id | Username | Role  | MerchantId |
|----|----------|-------|------------|
| 1  | admin    | Admin | NULL       |  ?
| 2  | user1    | User  | NULL       |  ? Should be 1
```

### After (Fixed)
```
Users table:
| Id | Username       | Role  | MerchantId |
|----|----------------|-------|------------|
| 1  | admin          | Admin | NULL       |  ?
| 2  | user1          | User  | 1          |  ? Updated!
| 3  | techmart_user  | User  | 1          |  ? New user added
| 4  | stylehub_user  | User  | 2          |  ? New user added
```

---

## ?? How to Apply the Fix

### Step 1: Just Run Your Application
```bash
dotnet run --project PaymentAPI.Api
```

That's it! The `DataSeeder` runs automatically on startup and will:
1. ? Check if users exist
2. ? Update users missing `MerchantId`
3. ? Add new test users if they don't exist
4. ? Log all changes

---

## ?? What the DataSeeder Does

### 1. Updates Existing Users
```csharp
// Assigns MerchantId based on username
if (user.Username == "user1" || user.Username == "techmart_user")
    user.MerchantId = 1; // TechMart

else if (user.Username == "stylehub_user")
    user.MerchantId = 2; // StyleHub

else
    user.MerchantId = 1; // Default to TechMart for any other users
```

### 2. Ensures Admin Users Have No MerchantId
```csharp
if (user.Role == "Admin" && user.MerchantId != null)
{
    user.MerchantId = null; // Remove MerchantId from Admin
}
```

### 3. Adds Missing Test Users
```csharp
// Adds new users if they don't exist:
- techmart_user (MerchantId: 1)
- stylehub_user (MerchantId: 2)
```

---

## ?? View the Changes in Logs

When you run your application, you'll see log output like:

```
[INF] Starting database seeding...
[INF] Database migrations applied successfully
[DBG] Merchants already exist, skipping seed
[DBG] Accounts already exist, skipping seed
[DBG] Payment methods already exist, skipping seed
[DBG] Transactions already exist, skipping seed
[DBG] Users already exist, checking if MerchantId needs to be updated...
[INF] Updated user 'user1' with MerchantId = 1 (TechMart)
[INF] ? Updated existing users with MerchantId assignments
[INF] Added 2 new test users
[INF] Database seeding completed successfully
```

---

## ?? Verify the Fix

### Option 1: Check Database Directly
```sql
SELECT "Id", "Username", "Role", "MerchantId" 
FROM "Users"
ORDER BY "Id";
```

Expected result:
```
Id | Username       | Role  | MerchantId
---|----------------|-------|------------
1  | admin          | Admin | NULL
2  | user1          | User  | 1
3  | techmart_user  | User  | 1
4  | stylehub_user  | User  | 2
```

### Option 2: Test Login and Check JWT Token
```bash
# Login as user1
POST /api/auth/login
{
  "username": "user1",
  "password": "User@123"
}

# Decode the JWT token in your frontend
const decoded = parseJwt(token);
console.log(decoded.merchantId); // Should show "1"
```

---

## ?? MerchantId Assignment Rules

| Username Pattern | MerchantId | Merchant Name |
|------------------|------------|---------------|
| `admin` (or any Admin role) | NULL | Can see all merchants |
| `user1`, `techmart_user` | 1 | TechMart |
| `stylehub_user` | 2 | StyleHub |
| `booknest_user` | 3 | BookNest |
| Any other User | 1 | TechMart (default) |

---

## ?? Key Features

1. **Non-Destructive**: Preserves existing user data
2. **Automatic**: Runs on every application startup
3. **Idempotent**: Safe to run multiple times (won't duplicate updates)
4. **Smart**: Only updates users that need updating
5. **Logged**: All changes are logged for tracking

---

## ?? Security Notes

- ? Admin users will NEVER have a MerchantId
- ? Non-admin users will ALWAYS have a MerchantId
- ? Existing passwords are NOT changed
- ? User roles are NOT modified

---

## ?? Troubleshooting

### Issue: "Users still have NULL MerchantId"
**Solution**: 
1. Stop your application
2. Run: `dotnet run --project PaymentAPI.Api`
3. Check logs for "Updated existing users with MerchantId"

### Issue: "New users not added"
**Solution**: Check if users already exist with those usernames. The seeder only adds users that don't exist.

### Issue: "No log messages about updates"
**Solution**: 
- Check your `appsettings.json` logging level
- Ensure it's set to at least `"Information"` or `"Debug"`

---

## ?? Testing Scenarios

### Test 1: Existing User Updated
1. Run application
2. Check logs for: `"Updated user 'user1' with MerchantId = 1 (TechMart)"`
3. Login as `user1`
4. Verify JWT token has `merchantId: "1"`

### Test 2: New Users Added
1. Run application
2. Check logs for: `"Added 2 new test users"`
3. Login as `techmart_user` / `stylehub_user`
4. Verify they can only see their merchant's data

### Test 3: Admin Stays Unrestricted
1. Login as `admin`
2. Verify JWT token has NO `merchantId` claim
3. Call `GET /api/merchant` - should return all merchants

---

## ?? Related Files

- **DataSeeder**: `PaymentAPI.Infrastructure/Persistance/DataSeeder.cs`
- **User Entity**: `PaymentAPI.Domain/Entities/User.cs`
- **JWT Service**: `PaymentAPI.Infrastructure/Services/JwtService.cs`
- **Login Handler**: `PaymentAPI.Application/Handlers/Auth/LoginUserCommandHandler.cs`

---

## ? Next Steps

1. ? Run your application
2. ? Check logs to verify updates
3. ? Test login for each user
4. ? Verify JWT tokens contain correct merchantId
5. ? Test API endpoints with different users

---

**Implementation Date**: November 21, 2024  
**Status**: ? Complete and Working  
**Build Status**: ? Successful  
**Auto-Run**: ? Runs on every app startup
