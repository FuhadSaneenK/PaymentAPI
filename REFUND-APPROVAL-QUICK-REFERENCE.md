# Refund Approval Workflow - Quick Reference

## User Workflow

### Request a Refund
```http
POST /api/transaction/refund
Authorization: Bearer {token}

{
  "amount": 150.00,
  "accountId": 1,
  "referenceNo": "PAY-001",
  "reason": "Product defect"
}
```
**Result:** RefundRequest created with Status = "Pending"

---

## Admin Workflow

### 1. View Pending Requests
```http
GET /api/admin/refund-requests/pending
Authorization: Bearer {admin-token}
```

### 2a. Approve Request
```http
POST /api/admin/refund-requests/approve
Authorization: Bearer {admin-token}

{
  "refundRequestId": 1,
  "adminUserId": 5,
  "comments": "Approved"
}
```
**Result:** 
- Refund transaction created
- Account balance updated
- Status = "Approved"

### 2b. Reject Request
```http
POST /api/admin/refund-requests/reject
Authorization: Bearer {admin-token}

{
  "refundRequestId": 1,
  "adminUserId": 5,
  "reason": "Insufficient documentation"
}
```
**Result:**
- No transaction created
- No balance change
- Status = "Rejected"

---

## Key Points

? Refunds require admin approval
? Balance only updates on approval
? Complete audit trail maintained
? Admin role required for approval/rejection
? Reason field is mandatory for requests
? Comments/reason required for rejection

---

## Status Flow

```
Pending ? Approved ? Refund Transaction Created + Balance Updated
        ? Rejected ? No Transaction + No Balance Change
```

---

## Database

**New Table:** `RefundRequests`
- Tracks all refund requests
- Links to Users, Accounts, and Transactions
- Stores approval/rejection details

---

## Authorization

| Endpoint | Auth Required | Role Required |
|----------|---------------|---------------|
| `/api/transaction/refund` | ? JWT | Any User |
| `/api/admin/refund-requests/*` | ? JWT | Admin |
