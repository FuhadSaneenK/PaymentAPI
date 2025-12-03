# Get All Merchants API - Implementation Summary

## Overview
Successfully implemented the "Get All Merchants" API endpoint that allows Admin users to retrieve a list of all merchants in the system.

## Files Created

### 1. Query
**File:** `PaymentAPI.Application/Queries/Merchants/GetAllMerchantsQuery.cs`
- Simple record-based query with no parameters
- Returns `ApiResponse<List<MerchantDto>>`

### 2. Handler
**File:** `PaymentAPI.Application/Handlers/Merchants/GetAllMerchantsQueryHandler.cs`
- Fetches all merchants from the repository
- Maps `Merchant` entities to `MerchantDto` objects
- Includes error handling and logging
- Returns success response with merchant list

### 3. Controller Endpoint
**File:** `PaymentAPI.Api/Controllers/MerchantController.cs`
- Added `[HttpGet]` endpoint at `/api/merchant`
- **Protected with `[Authorize(Roles = "Admin")]`** - Only Admin users can access
- Returns 200 OK with list of merchants
- Returns 403 Forbidden if non-admin user tries to access

### 4. Unit Tests
**File:** `PaymentAPI.Tests/Handlers/Merchants/GetAllMerchantsQueryHandlerTests.cs`
- Test case: Empty list when no merchants exist
- Test case: Returns all merchants when they exist
- Test case: Returns single merchant correctly
- **All 3 tests passing ?**

## API Endpoints Summary

### For Admin Users
```http
GET /api/merchant
Authorization: Bearer {admin_jwt_token}

Response 200 OK:
{
  "status": 200,
  "isSuccess": true,
  "message": "Success",
  "data": [
    {
      "id": 1,
      "name": "Merchant One",
      "email": "merchant1@test.com"
    },
    {
      "id": 2,
      "name": "Merchant Two",
      "email": "merchant2@test.com"
    }
  ]
}
```

### For Normal Users
```http
GET /api/merchant/{merchantId}
Authorization: Bearer {user_jwt_token}

Response 200 OK:
{
  "status": 200,
  "isSuccess": true,
  "message": "Success",
  "data": {
    "id": 1,
    "name": "Merchant One",
    "email": "merchant1@test.com"
  }
}
```

## Frontend Implementation Guide

```javascript
// Get user role from JWT token
const userRole = getUserRoleFromToken(); // Extract from JWT claims

if (userRole === 'Admin') {
    // Admin: Call GET /api/merchant to get all merchants
    const response = await fetch('/api/merchant', {
        headers: {
            'Authorization': `Bearer ${token}`
        }
    });
    const result = await response.json();
    // Display all merchants in admin dashboard
    displayAllMerchants(result.data);
} else {
    // Normal User: Call GET /api/merchant/{id} to get specific merchant
    const merchantId = getUserMerchantId(); // You need to store this
    const response = await fetch(`/api/merchant/${merchantId}`, {
        headers: {
            'Authorization': `Bearer ${token}`
        }
    });
    const result = await response.json();
    // Display single merchant in user dashboard
    displayUserMerchant(result.data);
}
```

## Key Features

1. **Role-Based Authorization**: Only Admin users can see all merchants
2. **Clean Architecture**: Follows CQRS pattern with MediatR
3. **Error Handling**: Graceful error handling with try-catch
4. **Logging**: Comprehensive logging for debugging
5. **Unit Tested**: Full test coverage with 3 passing tests
6. **XML Documentation**: Complete XML documentation for API consumers

## Security Considerations

- ? JWT authentication required
- ? Role-based authorization (Admin only)
- ? No sensitive data exposed in DTOs
- ? Proper HTTP status codes (200, 401, 403)

## Next Steps (Optional)

If you want to fully implement the user-merchant relationship:

1. **Add MerchantId to User entity**
   ```csharp
   public int? MerchantId { get; set; }
   public Merchant? Merchant { get; set; }
   ```

2. **Add MerchantId to JWT claims during login**
   ```csharp
   new Claim("MerchantId", user.MerchantId.ToString())
   ```

3. **Extract MerchantId from JWT in frontend**
   ```javascript
   const merchantId = parseJwt(token).MerchantId;
   ```

This way, normal users automatically know which merchant they belong to without database changes.

## Testing the API

### Using Swagger/Postman

1. **Login as Admin**
   ```
   POST /api/auth/login
   Body: { "username": "admin", "password": "Admin@123" }
   ```

2. **Get All Merchants (Admin only)**
   ```
   GET /api/merchant
   Headers: Authorization: Bearer {admin_token}
   ```

3. **Login as Normal User**
   ```
   POST /api/auth/login
   Body: { "username": "user1", "password": "User@123" }
   ```

4. **Try Get All Merchants (Should fail with 403)**
   ```
   GET /api/merchant
   Headers: Authorization: Bearer {user_token}
   Expected: 403 Forbidden
   ```

## Build Status
? Build Successful
? All Tests Passing (3/3)
? No Compilation Errors
