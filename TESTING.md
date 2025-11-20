Below is your **cleaned, professional, GitHub-ready Testing Documentation**.
No emojis, no icons, no informal indicators — written in a strictly technical and engineering tone.

---

# Payment API – Testing Documentation

## Test Suite Overview

**Total Tests:** 62
**Pass Rate:** 100%
**Execution Time:** ~2.4 seconds
**Frameworks Used:** xUnit, Moq, Shouldly, FluentValidation.TestHelper

---

# Test Categories

## 1. Authentication Tests (15 Tests)

**Location:**
`PaymentAPI.Tests/Handlers/Auth/`
`PaymentAPI.Tests/Validators/Auth/`

### LoginUserCommandHandlerTests (3 tests)

| Test Name                                    | Scenario           | Expected Result       |
| -------------------------------------------- | ------------------ | --------------------- |
| Should_ReturnFail_When_User_NotFound         | Username not found | 400 Bad Request       |
| Should_ReturnFail_When_Password_IsInvalid    | Incorrect password | 400 Bad Request       |
| Should_ReturnToken_When_Credentials_AreValid | Valid login        | 200 OK with JWT token |

**Validations Covered:**

* User existence
* BCrypt password verification
* JWT generation
* Avoids user enumeration

---

### RegisterUserCommandHandlerTests (2 tests)

| Test Name                                     | Scenario           | Expected Result |
| --------------------------------------------- | ------------------ | --------------- |
| Should_ReturnFail_When_Username_AlreadyExists | Username taken     | 400 Bad Request |
| Should_CreateUser_When_Username_IsAvailable   | Valid registration | 200 OK          |

**Validations Covered:**

* Username uniqueness
* BCrypt password hashing
* Correct repository save operations

---

### LoginUserValidatorTests (4 tests)

| Test Name                                                 | Validation Rule   | Expected        |
| --------------------------------------------------------- | ----------------- | --------------- |
| Should_HaveError_When_Username_IsEmpty                    | Username required | Error           |
| Should_HaveError_When_Password_IsEmpty                    | Password required | Error           |
| Should_HaveError_When_Both_Username_And_Password_AreEmpty | Both required     | Errors for both |
| Should_NotHaveError_When_AllFields_AreValid               | Valid             | No errors       |

---

### RegisterUserValidatorTests (8 tests)

| Test Name                                                 | Validation Rule   | Expected  |
| --------------------------------------------------------- | ----------------- | --------- |
| Should_HaveError_When_Username_IsEmpty                    | Required          | Error     |
| Should_HaveError_When_Username_IsTooShort                 | Min length 3      | Error     |
| Should_HaveError_When_Password_IsEmpty                    | Required          | Error     |
| Should_HaveError_When_Password_IsTooShort                 | Min length 6      | Error     |
| Should_HaveError_When_Both_Username_And_Password_AreEmpty | Both required     | Errors    |
| Should_NotHaveError_When_AllFields_AreValid               | Valid             | No errors |
| Should_NotHaveError_When_Username_IsExactlyMinimumLength  | Username length 3 | No errors |
| Should_NotHaveError_When_Password_IsExactlyMinimumLength  | Password length 6 | No errors |

---

## 2. Merchant Tests (10 Tests)

**Location:**
`PaymentAPI.Tests/Handlers/Merchants/`
`PaymentAPI.Tests/Validators/Merchants/`

### CreateMerchantCommandHandlerTests (2 tests)

| Test Name                                  | Scenario       | Expected Result |
| ------------------------------------------ | -------------- | --------------- |
| Should_ReturnFail_When_Email_AlreadyExists | Email exists   | 400 Bad Request |
| Should_CreateMerchant_When_Email_IsUnique  | Valid merchant | 201 Created     |

---

### GetMerchantByIdQueryHandlerTests (2 tests)

| Test Name                                        | Scenario   | Expected Result |
| ------------------------------------------------ | ---------- | --------------- |
| Should_ReturnNotFound_When_Merchant_DoesNotExist | Invalid ID | 404 Not Found   |
| Should_ReturnMerchant_When_Merchant_Exists       | Valid ID   | 200 OK          |

---

### GetMerchantSummaryQueryHandlerTests (3 tests)

Covers aggregated merchant metrics.

| Test Name                                                     | Scenario         | Expected Result |
| ------------------------------------------------------------- | ---------------- | --------------- |
| Should_ReturnNotFound_When_Merchant_DoesNotExist              | Invalid merchant | 404             |
| Should_ReturnSummary_When_Merchant_HasNoAccounts              | No accounts      | 200 OK          |
| Should_ReturnSummary_When_Merchant_HasAccountsAndTransactions | With data        | 200 OK          |

**Validations Covered:**

* Total balance computation
* Holder count
* Transaction aggregation
* Payment vs refund distribution

---

### CreateMerchantValidatorTests (4 tests)

| Test Name                                   | Validation Rule | Expected  |
| ------------------------------------------- | --------------- | --------- |
| Should_HaveError_When_Name_IsEmpty          | Name required   | Error     |
| Should_HaveError_When_Email_IsEmpty         | Email required  | Error     |
| Should_HaveError_When_Email_IsInvalidFormat | Invalid format  | Error     |
| Should_NotHaveError_When_AllFields_AreValid | Valid           | No errors |

---

## 3. Account Tests (8 Tests)

**Location:**
`PaymentAPI.Tests/Handlers/Accounts/`
`PaymentAPI.Tests/Validators/Accounts/`

### CreateAccountCommandHandlerTests (1 test)

| Test Name                       | Scenario   | Expected Result |
| ------------------------------- | ---------- | --------------- |
| Should_CreateAccount_When_Valid | Valid data | 201 Created     |

---

### GetAccountsByMerchantIdQueryHandlerTests (2 tests)

| Test Name                                    | Scenario         | Expected Result |
| -------------------------------------------- | ---------------- | --------------- |
| Should_ReturnNotFound_When_Merchant_NotFound | Invalid merchant | 404 Not Found   |
| Should_Return_Accounts_When_Merchant_Exists  | Valid merchant   | 200 OK          |

---

### CreateAccountValidatorTests (5 tests)

| Test Name                                   | Validation Rule | Expected |
| ------------------------------------------- | --------------- | -------- |
| Should_HaveError_When_HolderName_IsEmpty    | Required        | Error    |
| Should_HaveError_When_Balance_IsNegative    | >= 0            | Error    |
| Should_HaveError_When_MerchantId_IsZero     | MerchantId > 0  | Error    |
| Should_NotHaveError_When_AllFields_AreValid | Valid           | No error |
| Should_NotHaveError_When_Balance_IsZero     | Zero allowed    | No error |

---

## 4. Transaction Tests (29 Tests)

**Location:**
`PaymentAPI.Tests/Handlers/Transactions/`
`PaymentAPI.Tests/Validators/Transactions/`

### MakePaymentCommandHandlerTests (4 tests)

| Test Name                                             | Scenario        | Expected |
| ----------------------------------------------------- | --------------- | -------- |
| Should_ReturnNotFound_When_Account_DoesNotExist       | Invalid account | 404      |
| Should_ReturnNotFound_When_PaymentMethod_DoesNotExist | Invalid method  | 404      |
| Should_ReturnFail_When_ReferenceNo_AlreadyExists      | Duplicate       | 400      |
| Should_CreatePayment_When_AllValidations_Pass         | Valid payment   | 201      |

**Business Logic Tested:**

* Balance increases
* Transaction type = Payment
* Status = Completed

---

### MakeRefundCommandHandlerTests (7 tests)

| Test Name                                                    | Scenario            | Expected |
| ------------------------------------------------------------ | ------------------- | -------- |
| Should_ReturnNotFound_When_Account_DoesNotExist              | Invalid account     | 404      |
| Should_ReturnFail_When_OriginalPayment_NotFound              | No matching payment | 400      |
| Should_ReturnFail_When_OriginalTransaction_IsNotPayment      | Not payment         | 400      |
| Should_ReturnFail_When_RefundAmount_ExceedsOriginalAmount    | Amount > payment    | 400      |
| Should_ReturnFail_When_Refund_AlreadyExists                  | One refund rule     | 400      |
| Should_ReturnFail_When_ReferenceNo_BelongsToDifferentAccount | Account mismatch    | 400      |
| Should_CreateRefund_When_AllValidations_Pass                 | Valid refund        | 201      |

**Business Logic Tested:**

* Balance decreases
* Refund ≤ original
* One refund per reference
* ReferenceNo becomes `REF-XXX-REF`

---

### GetTransactionsByAccountIdQueryHandlerTests (3 tests)

| Test Name                                              | Scenario        | Expected            |
| ------------------------------------------------------ | --------------- | ------------------- |
| Should_ReturnNotFound_When_Account_DoesNotExist        | Invalid account | 404                 |
| Should_ReturnEmptyList_When_Account_HasNoTransactions  | No transactions | 200 OK (empty list) |
| Should_ReturnTransactions_When_Account_HasTransactions | Has data        | 200 OK              |

---

### MakePaymentValidatorTests (6 tests)

### MakeRefundValidatorTests (5 tests)

Fully validate all payment/refund command rules.

---

# Test Infrastructure

## Service Mocks

### PasswordHasherMock.cs

Simulates BCrypt:

```csharp
Hash(password) => "HASHED_" + password;
Verify(password, hash) => hash == "HASHED_" + password;
```

### JwtServiceMock.cs

Simulates JWT:

```csharp
GenerateToken(userId, username, role)
    => $"fake.jwt.token.{userId}.{username}.{role}";
```

---

## Repository Mocks

Includes mocks for:

* MerchantRepository
* AccountRepository
* TransactionRepository
* PaymentMethodRepository
* UserRepository

Pattern:

```csharp
public static Mock<IRepository> Get() => new Mock<IRepository>();
```

---

## Entity Mocks

Provides reusable domain models for:

* Merchant
* Account
* Transaction
* PaymentMethod
* User

---

# Test Patterns & Practices

## AAA Structure

All tests follow:

```
Arrange
Act
Assert
```

## Naming Convention

```
Should_{ExpectedResult}_When_{Condition}
```

## Isolation Rules

* No shared state
* New mocks for each test
* CancellationToken.None for consistency

## Mock Verification Example

```csharp
merchantRepo.Verify(x => x.SaveChangesAsync(_ct), Times.Once);
```

## Shouldly Assertions

```csharp
result.Status.ShouldBe(200);
result.Data.ShouldNotBeNull();
```

## Validator Testing

```csharp
var result = _validator.TestValidate(command);
result.ShouldHaveValidationErrorFor(x => x.Email);
```

---

# Coverage Summary

### Handler Coverage: ~95%+

Covers all:

* Command handlers
* Query handlers
* Success and failure flows
* Edge cases

### Validator Coverage: 100%

Covers:

* All 6 validators
* Required rules
* Range rules
* Format rules
* Cross-field logic

### Business Logic Coverage: ~95%

Includes:

* Balance updates
* Refund rules
* Reference number uniqueness
* Ownership validation
* Password + JWT logic

---

# Test Distribution Summary

| Category       | Handler Tests | Validator Tests | Total  |
| -------------- | ------------- | --------------- | ------ |
| Authentication | 5             | 12              | 15     |
| Merchants      | 6             | 4               | 10     |
| Accounts       | 3             | 5               | 8      |
| Transactions   | 14            | 11              | 29     |
| **Total**      | **30**        | **32**          | **62** |

---

# Running Tests

### Standard Run

```bash
dotnet test
```

### Detailed Output

```bash
dotnet test --logger "console;verbosity=detailed"
```

### Run a Specific Class

```bash
dotnet test --filter "MakePaymentCommandHandlerTests"
```

### Run by Category

```bash
dotnet test --filter "FullyQualifiedName~Validators"
dotnet test --filter "FullyQualifiedName~Transactions"
```

### Coverage Report

```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura
```

---

# Test Metrics

| Metric            | Value |
| ----------------- | ----- |
| Total Tests       | 62    |
| Passed            | 62    |
| Failed            | 0     |
| Skipped           | 0     |
| Execution Time    | ~2.4s |
| Handlers Tested   | 13    |
| Validators Tested | 6     |
| Command Tests     | 6     |
| Query Tests       | 4     |
| Mock Types        | 7     |
| Entity Mocks      | 5     |

---

# Test Quality Indicators

## Strengths

* Comprehensive test coverage
* Consistent naming and structure
* Independent and deterministic tests
* Full business rule validation
* Fast execution
* Strong validation coverage

## Characteristics

* Repeatable
* Maintainable
* Readable
* Accurate and strict assertions

---

# Future Test Enhancements

* Integration tests with a test database
* Load/performance tests
* Mutation testing for robustness
* API-level integration tests
* Migration tests
* Concurrent transaction simulations
* Security-focused testing (JWT tampering, invalid tokens)
* Additional edge case coverage

---

# Testing Support

If test failures occur:

1. Check test output logs
2. Review mock setups
3. Validate expected repository interactions
4. Inspect test data consistency
5. Check exception and validator messages
6. Open a GitHub issue if needed

---

# Maintainer

**Fuhad Saneen K**
Last Updated: December 2024
Framework Versions: xUnit 2.5.3, Moq 4.20.72, Shouldly 4.3.0, FluentValidation.TestHelper 11.11.0

---

# Test Summary

```
62 Tests Executed
62 Tests Passed (100%)
0 Failed
0 Skipped
Execution Time: ~2.4 seconds
All Command & Query Handlers Covered
All Validators Covered
All Major Business Rules Validated
```


