# User-Merchant Relationship - Quick Reference

## ?? Quick Summary

Users are now linked to merchants. Admin sees all, normal users see only their merchant.

---

## ?? Test Users

| Username | Password | Role | MerchantId | Can See |
|----------|----------|------|------------|---------|
| `admin` | `Admin@123` | Admin | null | ALL merchants |
| `techmart_user` | `User@123` | User | 1 | TechMart only |
| `stylehub_user` | `User@123` | User | 2 | StyleHub only |
| `user1` | `User@123` | User | 1 | TechMart only |

---

## ?? API Endpoints

### Admin Only
```http
GET /api/merchant
Authorization: Bearer {admin_token}
```

### Everyone
```http
GET /api/merchant/{id}
Authorization: Bearer {token}
```

---

## ?? Frontend: Decode JWT

```javascript
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

// Usage
const token = localStorage.getItem('authToken');
const userInfo = parseJwt(token);

console.log(userInfo.id);         // User ID
console.log(userInfo.sub);        // Username
console.log(userInfo.role);       // "Admin" or "User"
console.log(userInfo.merchantId); // Merchant ID (undefined for admin)
```

---

## ?? Frontend: Load Data by Role

```javascript
async function loadDashboard() {
    const token = localStorage.getItem('authToken');
    const userInfo = parseJwt(token);
    
    const headers = {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
    };

    if (userInfo.role === 'Admin') {
        // Admin: Get ALL merchants
        const response = await fetch('/api/merchant', { headers });
        const result = await response.json();
        displayAllMerchants(result.data);
    } else {
        // User: Get ONLY their merchant
        const merchantId = userInfo.merchantId;
        const response = await fetch(`/api/merchant/${merchantId}`, { headers });
        const result = await response.json();
        displayUserMerchant(result.data);
    }
}
```

---

## ?? JWT Token Structure

### Admin Token
```json
{
  "sub": "admin",
  "id": "1",
  "role": "Admin"
  // No merchantId
}
```

### User Token
```json
{
  "sub": "techmart_user",
  "id": "2",
  "role": "User",
  "merchantId": "1"
}
```

---

## ?? Database Changes

### Users Table - NEW Column
```sql
ALTER TABLE "Users" ADD COLUMN "MerchantId" INTEGER NULL;
ALTER TABLE "Users" ADD FOREIGN KEY ("MerchantId") REFERENCES "Merchants"("Id");
```

---

## ? Implementation Checklist

- [x] User entity updated with MerchantId
- [x] Merchant entity updated with Users collection
- [x] EF Core relationship configured
- [x] JWT service updated to include merchantId
- [x] Login handler passes merchantId to JWT
- [x] Data seeder creates test users with merchantId
- [x] Migration created and applied
- [x] All tests passing (65/65)
- [ ] Frontend: Decode JWT token
- [ ] Frontend: Extract merchantId
- [ ] Frontend: Filter data by role

---

## ?? Test It

### 1. Login as Admin
```bash
POST /api/auth/login
{
  "username": "admin",
  "password": "Admin@123"
}
# Token will NOT have merchantId
```

### 2. Login as Normal User
```bash
POST /api/auth/login
{
  "username": "techmart_user",
  "password": "User@123"
}
# Token WILL have merchantId: 1
```

### 3. Admin Gets All Merchants
```bash
GET /api/merchant
Authorization: Bearer {admin_token}
# Returns all 5 merchants ?
```

### 4. User Gets All Merchants (Should Fail)
```bash
GET /api/merchant
Authorization: Bearer {user_token}
# Returns 403 Forbidden ?
```

### 5. User Gets Their Merchant
```bash
GET /api/merchant/1
Authorization: Bearer {user_token}
# Returns TechMart ?
```

---

## ?? Common Issues

### Issue: merchantId is undefined
**Fix**: User doesn't have MerchantId in database. Check seeded users.

### Issue: 403 when user calls /api/merchant
**Fix**: This is CORRECT! Users must use `/api/merchant/{id}` instead.

### Issue: Cannot decode token
**Fix**: Check token format. Should be: `header.payload.signature`

---

## ?? Full Documentation

See: [USER-MERCHANT-RELATIONSHIP.md](./USER-MERCHANT-RELATIONSHIP.md)

---

**Status**: ? Complete  
**Tests**: ? 65/65 Passing  
**Build**: ? Successful
