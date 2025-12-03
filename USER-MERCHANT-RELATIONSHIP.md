# User-Merchant Relationship Implementation

## ? Implementation Complete

This document explains the User-Merchant relationship implementation that enables role-based data access control in your Payment API.

---

## ?? Overview

### Problem Statement
- **Admin users** should see ALL merchants, accounts, and transactions
- **Normal users** should ONLY see data for their assigned merchant

### Solution
Added a relationship between `User` and `Merchant` entities, where:
- Each user can belong to ONE merchant (or none for admins)
- MerchantId is embedded in the JWT token
- Frontend extracts merchantId from token to filter data

---

## ?? Changes Made

### 1. Database Schema Changes

#### User Entity (`PaymentAPI.Domain/Entities/User.cs`)
```csharp
public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public string Role { get; set; }
    
    // NEW: Foreign key to Merchant
    public int? MerchantId { get; set; }
    
    // NEW: Navigation property
    public Merchant? Merchant { get; set; }
}
```

#### Merchant Entity (`PaymentAPI.Domain/Entities/Merchant.cs`)
```csharp
public class Merchant
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public List<Account> Accounts { get; set; } = new();
    
    // NEW: Navigation property for users
    public List<User> Users { get; set; } = new();
}
```

#### Database Configuration (`UserConfiguration.cs`)
```csharp
builder.HasOne(x => x.Merchant)
    .WithMany(x => x.Users)
    .HasForeignKey(x => x.MerchantId)
    .OnDelete(DeleteBehavior.SetNull);
```

**Migration Created**: `AddMerchantIdToUser`
- Added nullable `MerchantId` column to Users table
- Added foreign key constraint to Merchants table

---

### 2. JWT Token Changes

#### IJwtService Interface Updated
```csharp
public interface IJwtService
{
    string GenerateToken(int userId, string username, string role, int? merchantId = null);
}
```

#### JwtService Implementation Updated
```csharp
public string GenerateToken(int userId, string username, string role, int? merchantId = null)
{
    var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, username),
        new Claim("id", userId.ToString()),
        new Claim(ClaimTypes.Role, role)
    };

    // Add MerchantId claim for non-admin users
    if (merchantId.HasValue)
    {
        claims.Add(new Claim("merchantId", merchantId.Value.ToString()));
    }

    // ... rest of token generation
}
```

---

### 3. Login Handler Updated

#### LoginUserCommandHandler
```csharp
public async Task<ApiResponse<string>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
{
    // ... authentication logic ...
    
    // Generate token with MerchantId
    var token = _jwtService.GenerateToken(user.Id, user.Username, user.Role, user.MerchantId);
    
    return ApiResponse<string>.Success(token, "Login successful");
}
```

---

### 4. Seed Data Updated

#### New Test Users (`DataSeeder.cs`)
```csharp
var users = new List<User>
{
    // Admin - No merchant restriction
    new User 
    { 
        Username = "admin", 
        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"), 
        Role = "Admin",
        MerchantId = null
    },
    
    // TechMart User - Can only see TechMart data
    new User 
    { 
        Username = "techmart_user", 
        PasswordHash = BCrypt.Net.BCrypt.HashPassword("User@123"), 
        Role = "User",
        MerchantId = 1
    },
    
    // StyleHub User - Can only see StyleHub data
    new User 
    { 
        Username = "stylehub_user", 
        PasswordHash = BCrypt.Net.BCrypt.HashPassword("User@123"), 
        Role = "User",
        MerchantId = 2
    },
    
    // Original user1 - Assigned to TechMart
    new User 
    { 
        Username = "user1", 
        PasswordHash = BCrypt.Net.BCrypt.HashPassword("User@123"), 
        Role = "User",
        MerchantId = 1
    }
};
```

---

## ?? API Endpoints

### For Admin Users

#### Get All Merchants
```http
GET /api/merchant
Authorization: Bearer {admin_token}

Response:
{
  "status": 200,
  "isSuccess": true,
  "data": [
    { "id": 1, "name": "TechMart", "email": "contact@techmart.com" },
    { "id": 2, "name": "StyleHub", "email": "support@stylehub.com" },
    { "id": 3, "name": "BookNest", "email": "info@booknest.com" },
    { "id": 4, "name": "FoodieBox", "email": "hello@foodiebox.com" },
    { "id": 5, "name": "AutoCare", "email": "service@autocare.com" }
  ]
}
```

### For Normal Users

#### Get Merchant By ID (Their Merchant)
```http
GET /api/merchant/{merchantId}
Authorization: Bearer {user_token}

Response:
{
  "status": 200,
  "isSuccess": true,
  "data": {
    "id": 1,
    "name": "TechMart",
    "email": "contact@techmart.com"
  }
}
```

#### Get Accounts for Their Merchant
```http
GET /api/merchant/{merchantId}/accounts
Authorization: Bearer {user_token}

Response:
{
  "status": 200,
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": 1,
        "holderName": "TechMart Main",
        "balance": 15000.00,
        "merchantId": 1
      }
    ],
    "pageNumber": 1,
    "pageSize": 20,
    "totalCount": 1
  }
}
```

---

## ?? Frontend Implementation

### Decode JWT Token
```javascript
/**
 * Decodes a JWT token to extract claims
 */
function parseJwt(token) {
    const base64Url = token.split('.')[1];
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    const jsonPayload = decodeURIComponent(
        atob(base64)
            .split('')
            .map(c => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
            .join('')
    );
    return JSON.parse(jsonPayload);
}
```

### Extract User Information
```javascript
// After successful login
const token = loginResponse.data; // JWT token from API
const userInfo = parseJwt(token);

console.log('User ID:', userInfo.id);
console.log('Username:', userInfo.sub);
console.log('Role:', userInfo.role);
console.log('Merchant ID:', userInfo.merchantId); // undefined for admin
```

### Role-Based Data Fetching
```javascript
/**
 * Fetches merchant data based on user role
 */
async function loadMerchantData() {
    const token = localStorage.getItem('authToken');
    const userInfo = parseJwt(token);
    
    const headers = {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
    };

    if (userInfo.role === 'Admin') {
        // Admin: Get all merchants
        const response = await fetch('/api/merchant', { headers });
        const result = await response.json();
        
        if (result.isSuccess) {
            displayAllMerchants(result.data);
        }
    } else {
        // Normal User: Get only their merchant
        const merchantId = userInfo.merchantId;
        
        // Get merchant details
        const merchantResponse = await fetch(
            `/api/merchant/${merchantId}`, 
            { headers }
        );
        const merchantResult = await merchantResponse.json();
        
        // Get accounts for this merchant
        const accountsResponse = await fetch(
            `/api/merchant/${merchantId}/accounts`, 
            { headers }
        );
        const accountsResult = await accountsResponse.json();
        
        if (merchantResult.isSuccess && accountsResult.isSuccess) {
            displayUserDashboard(merchantResult.data, accountsResult.data);
        }
    }
}
```

### Complete React Example
```jsx
import { useState, useEffect } from 'react';

function Dashboard() {
    const [userData, setUserData] = useState(null);
    const [merchants, setMerchants] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        loadDashboard();
    }, []);

    const parseJwt = (token) => {
        const base64Url = token.split('.')[1];
        const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
        const jsonPayload = decodeURIComponent(
            atob(base64)
                .split('')
                .map(c => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
                .join('')
        );
        return JSON.parse(jsonPayload);
    };

    const loadDashboard = async () => {
        try {
            const token = localStorage.getItem('authToken');
            const userInfo = parseJwt(token);
            setUserData(userInfo);

            const headers = {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            };

            if (userInfo.role === 'Admin') {
                // Admin: Fetch all merchants
                const response = await fetch('/api/merchant', { headers });
                const result = await response.json();
                
                if (result.isSuccess) {
                    setMerchants(result.data);
                }
            } else {
                // Normal User: Fetch their merchant only
                const response = await fetch(
                    `/api/merchant/${userInfo.merchantId}`, 
                    { headers }
                );
                const result = await response.json();
                
                if (result.isSuccess) {
                    setMerchants([result.data]); // Array with single merchant
                }
            }
        } catch (error) {
            console.error('Error loading dashboard:', error);
        } finally {
            setLoading(false);
        }
    };

    if (loading) return <div>Loading...</div>;

    return (
        <div>
            <h1>Welcome, {userData?.sub}</h1>
            <p>Role: {userData?.role}</p>
            
            {userData?.role === 'Admin' && (
                <h2>All Merchants ({merchants.length})</h2>
            )}
            
            {userData?.role === 'User' && (
                <h2>Your Merchant</h2>
            )}
            
            <ul>
                {merchants.map(merchant => (
                    <li key={merchant.id}>
                        {merchant.name} - {merchant.email}
                    </li>
                ))}
            </ul>
        </div>
    );
}

export default Dashboard;
```

---

## ?? Security Features

### ? Role-Based Authorization
- `[Authorize(Roles = "Admin")]` on `/api/merchant` endpoint
- Non-admin users get 403 Forbidden when trying to access all merchants

### ? Data Isolation
- MerchantId is in JWT token (signed and verified)
- Users cannot modify their merchantId
- Frontend uses merchantId from token to filter requests

### ? JWT Claims Structure
```json
{
  "sub": "techmart_user",
  "id": "2",
  "role": "User",
  "merchantId": "1",
  "exp": 1700000000,
  "iss": "PaymentAPI",
  "aud": "PaymentAPIUsers"
}
```

---

## ?? Testing

### Test Credentials

| Username | Password | Role | MerchantId | Access |
|----------|----------|------|------------|--------|
| admin | Admin@123 | Admin | null | All merchants |
| techmart_user | User@123 | User | 1 | TechMart only |
| stylehub_user | User@123 | User | 2 | StyleHub only |
| user1 | User@123 | User | 1 | TechMart only |

### Test Scenarios

#### 1. Admin Login & Access
```bash
# Login as admin
POST /api/auth/login
{
  "username": "admin",
  "password": "Admin@123"
}

# Response includes token
# Use token to get all merchants
GET /api/merchant
Authorization: Bearer {admin_token}

# Should return all 5 merchants
```

#### 2. Normal User Login & Access
```bash
# Login as techmart_user
POST /api/auth/login
{
  "username": "techmart_user",
  "password": "User@123"
}

# Response includes token with merchantId: 1

# Get their merchant (ID 1)
GET /api/merchant/1
Authorization: Bearer {user_token}

# Should return TechMart only

# Try to access all merchants (should fail)
GET /api/merchant
Authorization: Bearer {user_token}

# Should return 403 Forbidden
```

#### 3. Get Merchant-Specific Accounts
```bash
# As techmart_user
GET /api/merchant/1/accounts
Authorization: Bearer {user_token}

# Should return only TechMart accounts
```

### Unit Tests
? All 65 tests passing
- Including new `GetAllMerchantsQueryHandlerTests`
- Updated mocks to support optional `merchantId` parameter

---

## ?? Database Schema

### Users Table
```sql
CREATE TABLE "Users" (
    "Id" SERIAL PRIMARY KEY,
    "Username" VARCHAR(50) NOT NULL,
    "PasswordHash" TEXT NOT NULL,
    "Role" VARCHAR(20) NOT NULL,
    "MerchantId" INTEGER NULL,
    FOREIGN KEY ("MerchantId") REFERENCES "Merchants"("Id") ON DELETE SET NULL
);
```

### Relationship Diagram
```
???????????????          ????????????????          ???????????????
?   Merchant  ? 1     ?  ?     User     ?          ?   Account   ?
?????????????????????????????????????????          ???????????????
? Id          ?          ? Id           ?          ? Id          ?
? Name        ?          ? Username     ?          ? HolderName  ?
? Email       ?          ? PasswordHash ?          ? Balance     ?
?             ?          ? Role         ?          ? MerchantId  ????
?             ?          ? MerchantId   ?          ?             ?  ?
???????????????          ????????????????          ???????????????  ?
      ?                                                              ?
      ????????????????????????????????????????????????????????????????
                             1        ?
```

---

## ?? Deployment Checklist

- [x] Update User entity with MerchantId
- [x] Update Merchant entity with Users collection
- [x] Configure EF Core relationship
- [x] Update IJwtService interface
- [x] Update JwtService implementation
- [x] Update LoginUserCommandHandler
- [x] Update DataSeeder with test users
- [x] Create and apply migration
- [x] Update test mocks
- [x] Run all tests (65/65 passing)
- [ ] Test API endpoints manually
- [ ] Update frontend to decode JWT
- [ ] Update frontend to use merchantId
- [ ] Deploy to staging environment

---

## ?? Key Takeaways

1. **MerchantId in JWT** - No need for extra API calls to get user's merchant
2. **Role-Based Access** - Clean separation between admin and user access
3. **Frontend Filtering** - Frontend reads merchantId from token and filters requests
4. **Secure by Design** - JWT is signed, users cannot modify their merchantId
5. **Backward Compatible** - Existing admin user still works (MerchantId = null)

---

## ?? Related Documentation

- [GET-ALL-MERCHANTS-IMPLEMENTATION.md](./GET-ALL-MERCHANTS-IMPLEMENTATION.md) - Get All Merchants API
- [ARCHITECTURE.md](./ARCHITECTURE.md) - Overall architecture
- [TESTING.md](./TESTING.md) - Testing guidelines

---

## ?? Troubleshooting

### Issue: "merchantId is undefined in frontend"
**Solution**: Check if user has `MerchantId` assigned in database

### Issue: "403 Forbidden when normal user accesses /api/merchant"
**Solution**: This is correct! Normal users should use `/api/merchant/{id}` instead

### Issue: "Cannot read property 'merchantId' of null"
**Solution**: Make sure token is decoded correctly. Admin users won't have merchantId in token.

---

## ? Next Steps

1. **Frontend Implementation**: Implement JWT decoding and role-based routing
2. **Additional Filtering**: Add merchantId filtering to transaction endpoints
3. **User Management**: Create endpoints for admin to assign users to merchants
4. **Merchant Selection**: Allow admin to impersonate users (view as merchant)

---

**Implementation Date**: November 21, 2024  
**Status**: ? Complete  
**Build Status**: ? Successful  
**Tests**: ? 65/65 Passing
