# Refund Approval Workflow Implementation

## Overview

The refund system has been updated to implement an **approval workflow** where refund requests must be approved or rejected by an admin user before the actual refund is processed.

---

## Key Changes

### Previous Behavior (Before)
- When a user requested a refund, it was **immediately processed**
- Account balance was **immediately updated**
- Refund transaction was **immediately created**
- **No admin oversight**

### New Behavior (After)
- When a user requests a refund, a **RefundRequest** is created with status "Pending"
- Account balance is **NOT updated** until admin approval
- Refund transaction is **NOT created** until admin approval
- **Admin must approve or reject** the request
- Full **audit trail** of who approved/rejected and when

---

## Architecture

### New Entities

#### 1. **RefundRequest** (`PaymentAPI.Domain/Entities/RefundRequest.cs`)
```csharp
public class RefundRequest
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public int AccountId { get; set; }
    public string OriginalPaymentReference { get; set; }
    public string Reason { get; set; }  // NEW: Required field
    public string Status { get; set; }  // "Pending", "Approved", "Rejected"
    public DateTime RequestDate { get; set; }
    public DateTime? ReviewDate { get; set; }
    public int? ReviewedByUserId { get; set; }
    public string? AdminComments { get; set; }
    public int? RefundTransactionId { get; set; }
    
    // Navigation properties
    public Account Account { get; set; }
    public User? ReviewedByUser { get; set; }
    public Transaction? RefundTransaction { get; set; }
}
```

### Database Changes

**New Table: RefundRequests**
- Stores all refund requests (pending, approved, rejected)
- Links to original payment via `OriginalPaymentReference`
- Links to admin user who reviewed via `ReviewedByUserId`
- Links to created transaction via `RefundTransactionId` (after approval)

**Migration Applied:**
- `AddRefundRequestEntity` migration created and applied
- Database schema updated successfully

---

## Workflow Diagram

```
???????????????????
? User Requests   ?
? Refund          ?
???????????????????
         ?
         ?
???????????????????????????????????
? MakeRefundCommand               ?
? - Validates original payment    ?
? - Creates RefundRequest         ?
? - Status: "Pending"             ?
? - Balance NOT updated           ?
???????????????????????????????????
         ?
         ?
???????????????????????????????????
? Admin Reviews Request           ?
? (GetPendingRefundRequestsQuery) ?
???????????????????????????????????
         ?
    ???????????
    ?         ?
    ?         ?
?????????? ??????????
?Approve ? ?Reject  ?
?????????? ??????????
    ?          ?
    ?          ?
????????????????????  ????????????????????
? Create Refund    ?  ? Mark Rejected    ?
? Transaction      ?  ? No Transaction   ?
? Update Balance   ?  ? No Balance Change?
? Status: Approved ?  ? Status: Rejected ?
????????????????????  ????????????????????
```

---

## API Endpoints

### For Users

#### 1. **Create Refund Request**
```http
POST /api/transaction/refund
Authorization: Bearer {token}

{
  "amount": 150.00,
  "accountId": 1,
  "referenceNo": "PAY-2024-12-001",
  "reason": "Product was defective"
}
```

**Response (201 Created):**
```json
{
  "status": 201,
  "data": {
    "id": 1,
    "amount": 150.00,
    "accountId": 1,
    "originalPaymentReference": "PAY-2024-12-001",
    "reason": "Product was defective",
    "status": "Pending",
    "requestDate": "2024-12-10T14:30:00Z",
    "reviewDate": null,
    "reviewedByUserId": null,
    "adminComments": null
  },
  "message": "Refund request submitted successfully and is pending admin approval"
}
```

### For Admins

#### 2. **Get Pending Refund Requests**
```http
GET /api/admin/refund-requests/pending
Authorization: Bearer {admin-token}
```

**Response (200 OK):**
```json
{
  "status": 200,
  "data": [
    {
      "id": 1,
      "amount": 150.00,
      "accountId": 1,
      "originalPaymentReference": "PAY-2024-12-001",
      "reason": "Product was defective",
      "status": "Pending",
      "requestDate": "2024-12-10T14:30:00Z"
    }
  ],
  "message": "Retrieved 1 pending refund requests"
}
```

#### 3. **Approve Refund Request**
```http
POST /api/admin/refund-requests/approve
Authorization: Bearer {admin-token}

{
  "refundRequestId": 1,
  "adminUserId": 5,
  "comments": "Approved due to verified product defect"
}
```

**Response (200 OK):**
```json
{
  "status": 200,
  "data": {
    "id": 10,
    "amount": 150.00,
    "type": "Refund",
    "status": "Completed",
    "referenceNo": "PAY-2024-12-001-REF",
    "accountId": 1,
    "paymentMethodId": 1,
    "date": "2024-12-10T15:00:00Z"
  },
  "message": "Refund approved and processed successfully"
}
```

#### 4. **Reject Refund Request**
```http
POST /api/admin/refund-requests/reject
Authorization: Bearer {admin-token}

{
  "refundRequestId": 1,
  "adminUserId": 5,
  "reason": "Insufficient documentation provided"
}
```

**Response (200 OK):**
```json
{
  "status": 200,
  "data": null,
  "message": "Refund request rejected successfully"
}
```

---

## Validation Rules

### MakeRefundCommand (User Request)
- ? Amount must be > 0
- ? AccountId must be valid
- ? ReferenceNo is required
- ? **Reason is required** (max 500 characters)
- ? Original payment must exist
- ? Original transaction must be of type "Payment"
- ? Refund amount cannot exceed original payment
- ? No completed refund can already exist
- ? No pending refund request can already exist
- ? Reference must belong to the same account

### ApproveRefundCommand (Admin)
- ? RefundRequestId must be valid
- ? AdminUserId must be valid
- ? Comments (optional, max 1000 characters)
- ? Request must be in "Pending" status

### RejectRefundCommand (Admin)
- ? RefundRequestId must be valid
- ? AdminUserId must be valid
- ? Reason is required (max 1000 characters)
- ? Request must be in "Pending" status

---

## Key Classes & Files

### Domain Layer
- `PaymentAPI.Domain/Entities/RefundRequest.cs` - Entity definition

### Infrastructure Layer
- `PaymentAPI.Infrastructure/Persistance/Configurations/RefundRequestConfiguration.cs` - EF Core configuration
- `PaymentAPI.Infrastructure/Repositories/RefundRequestRepository.cs` - Repository implementation
- `PaymentAPI.Infrastructure/Persistance/PaymentDbContext.cs` - Updated with RefundRequests DbSet

### Application Layer

**Commands:**
- `PaymentAPI.Application/Commands/Transactions/MakeRefundCommand.cs` - User refund request
- `PaymentAPI.Application/Commands/Admin/ApproveRefundCommand.cs` - Admin approval
- `PaymentAPI.Application/Commands/Admin/RejectRefundCommand.cs` - Admin rejection

**Queries:**
- `PaymentAPI.Application/Queries/Admin/GetPendingRefundRequestsQuery.cs` - Get pending requests

**Handlers:**
- `PaymentAPI.Application/Handlers/Transactions/MakeRefundCommandHandler.cs` - Creates refund request
- `PaymentAPI.Application/Handlers/Admin/ApproveRefundCommandHandler.cs` - Processes approval
- `PaymentAPI.Application/Handlers/Admin/RejectRefundCommandHandler.cs` - Processes rejection
- `PaymentAPI.Application/Handlers/Admin/GetPendingRefundRequestsQueryHandler.cs` - Retrieves pending requests

**Validators:**
- `PaymentAPI.Application/Validators/Transactions/MakeRefundValidator.cs` - Updated with Reason validation
- `PaymentAPI.Application/Validators/Admin/ApproveRefundValidator.cs` - Approval validation
- `PaymentAPI.Application/Validators/Admin/RejectRefundValidator.cs` - Rejection validation

**DTOs:**
- `PaymentAPI.Application/DTOs/RefundRequestDto.cs` - Data transfer object

**Repositories:**
- `PaymentAPI.Application/Abstractions/Repositories/IRefundRequestRepository.cs` - Interface
- `PaymentAPI.Infrastructure/Repositories/RefundRequestRepository.cs` - Implementation

### API Layer
- `PaymentAPI.Api/Controllers/TransactionController.cs` - Updated refund endpoint documentation
- `PaymentAPI.Api/Controllers/AdminController.cs` - **NEW** Admin endpoints
- `PaymentAPI.Api/Program.cs` - Updated with RefundRequestRepository registration

### Tests
- `PaymentAPI.Tests/Handlers/Transactions/MakeRefundCommandHandlerTests.cs` - Updated for new workflow
- `PaymentAPI.Tests/Validators/Transactions/MakeRefundValidatorTests.cs` - Updated with Reason tests

---

## Security

### Authorization
- **User endpoints** (`/api/transaction/refund`) - Requires JWT authentication
- **Admin endpoints** (`/api/admin/*`) - Requires JWT authentication **AND** `"Admin"` role

```csharp
[Authorize]  // Any authenticated user
public class TransactionController : BaseApiController

[Authorize(Roles = "Admin")]  // Admin role required
public class AdminController : BaseApiController
```

---

## Benefits of This Implementation

1. **Fraud Prevention** - Admins can review refunds before processing
2. **Quality Control** - Ensures only valid refunds are processed
3. **Audit Trail** - Complete history of who requested, who approved/rejected, and when
4. **Accountability** - Links decisions to specific admin users
5. **Flexibility** - Admins can add comments explaining their decision
6. **Data Integrity** - Account balance only changes after approval
7. **Compliance** - Meets regulatory requirements for financial transactions

---

## Testing

All tests have been updated to work with the new workflow:

### Unit Tests Updated:
- ? `MakeRefundCommandHandlerTests` - Tests refund request creation
- ? `MakeRefundValidatorTests` - Tests validation including new Reason field

### Test Coverage:
- ? Account not found
- ? Original payment not found
- ? Original transaction is not a payment
- ? Refund amount exceeds original amount
- ? Refund already processed
- ? Pending refund request already exists
- ? Reference belongs to different account
- ? Successful refund request creation
- ? All validation rules

---

## Database Migration

```bash
# Migration created
dotnet ef migrations add AddRefundRequestEntity --project PaymentAPI.Infrastructure --startup-project PaymentAPI.Api

# Migration applied
dotnet ef database update --project PaymentAPI.Infrastructure --startup-project PaymentAPI.Api
```

**Migration Creates:**
- `RefundRequests` table with all columns
- Foreign key relationships to `Accounts`, `Users`, and `Transactions`
- Indexes on `Status`, `AccountId`, and `OriginalPaymentReference`

---

## Example Usage Scenario

### Scenario: Customer requests refund for defective product

1. **Customer submits refund request:**
   ```http
   POST /api/transaction/refund
   {
     "amount": 299.99,
     "accountId": 42,
     "referenceNo": "PAY-2024-12-155",
     "reason": "Product arrived damaged"
   }
   ```
   - ? RefundRequest created with Status = "Pending"
   - ? Customer receives confirmation
   - ? Account balance NOT changed yet

2. **Admin reviews pending requests:**
   ```http
   GET /api/admin/refund-requests/pending
   ```
   - Sees request with ID 15

3. **Admin investigates and approves:**
   ```http
   POST /api/admin/refund-requests/approve
   {
     "refundRequestId": 15,
     "adminUserId": 3,
     "comments": "Verified with shipping company. Approved."
   }
   ```
   - ? Refund transaction created (PAY-2024-12-155-REF)
   - ? Account balance decreased by $299.99
   - ? RefundRequest.Status = "Approved"
   - ? RefundRequest.ReviewDate = now
   - ? RefundRequest.ReviewedByUserId = 3
   - ? RefundRequest.RefundTransactionId = {new transaction ID}

---

## Next Steps / Future Enhancements

Potential improvements for the future:

1. **Email Notifications**
   - Notify customer when refund request is created
   - Notify customer when approved/rejected
   - Notify admins when new requests arrive

2. **Partial Refunds**
   - Allow multiple partial refunds up to original amount
   - Track total refunded amount per payment

3. **Refund Reasons Dropdown**
   - Predefined list of common refund reasons
   - Better analytics and reporting

4. **Admin Dashboard**
   - Statistics on pending/approved/rejected refunds
   - Average processing time
   - Most common refund reasons

5. **Customer Refund History**
   - Endpoint for customers to view their refund requests
   - Filter by status (Pending/Approved/Rejected)

6. **Automated Rules**
   - Auto-approve refunds under certain amount
   - Auto-approve for trusted customers
   - Flag suspicious patterns

---

## Troubleshooting

### Common Issues

**Issue:** "A pending refund request already exists for this reference number"
- **Solution:** Check if there's already a pending request. Admin must approve/reject it first.

**Issue:** "Refund request has already been approved"
- **Solution:** The request was already processed. Cannot modify approved/rejected requests.

**Issue:** 403 Forbidden on admin endpoints
- **Solution:** User must have "Admin" role in their JWT token.

---

## Summary

The refund approval workflow provides a robust, secure, and auditable process for handling refunds. It ensures that all refunds are reviewed by authorized personnel before being processed, maintaining financial integrity while providing flexibility for legitimate refund requests.

**Status:** ? **Fully Implemented and Tested**
