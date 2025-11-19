Below is your **cleaned, professional, GitHub-ready** version — no emojis, no icons, no decorations.
Everything is rewritten in a consistent engineering/documentation standard.

---

# Payment API

A comprehensive RESTful API for managing merchants, accounts, payments, and refunds, built with **Clean Architecture**, **CQRS**, **.NET 8**, and **Entity Framework Core**.

---

## Architecture

This project implements Clean Architecture with clear separation of concerns.

```
PaymentAPI/
│
├── PaymentAPI.Api/               # Presentation Layer (Controllers, Middleware)
├── PaymentAPI.Application/       # Application Layer (CQRS, Handlers, DTOs, Validation)
├── PaymentAPI.Domain/            # Domain Layer (Entities, Core Rules)
├── PaymentAPI.Infrastructure/    # Infrastructure Layer (EF Core, Repositories, Services)
└── PaymentAPI.Tests/             # Unit Tests (45 tests)
```

### Layer Responsibilities

#### Domain Layer (`PaymentAPI.Domain`)

* Core business entities (Merchant, Account, Transaction, User, PaymentMethod)
* Contains business rules and invariants
* No external dependencies
* Pure domain logic

#### Application Layer (`PaymentAPI.Application`)

* CQRS (Commands, Queries) using MediatR
* Command and query handlers
* DTOs and mapping
* FluentValidation validators
* Business orchestration
* Repository abstractions

#### Infrastructure Layer (`PaymentAPI.Infrastructure`)

* Entity Framework Core 9 with PostgreSQL
* Repository implementations
* JWT authentication service
* BCrypt password hashing
* Database configurations, migrations, and seeding

#### Presentation Layer (`PaymentAPI.Api`)

* REST API controllers
* JWT authentication
* Swagger/OpenAPI documentation
* Dependency injection configuration

---

## Features

### Authentication and Authorization

* User registration with BCrypt password hashing
* JWT token-based authentication
* Role-based authorization (Admin, User)
* Secure password storage (never stored in plain text)

### Merchant Management

* Create merchants with unique email validation
* Retrieve merchant details
* Merchant summary with accounts, balances, and transactions

### Account Management

* Create accounts for merchants
* Retrieve accounts for a specific merchant
* Balance tracking

### Transaction Management

* Process payments with unique reference numbers
* Process refunds with strict business rules
* Retrieve transaction history
* Automatic account balance updates
* Duplicate prevention for reference numbers

### Validation and Error Handling

* FluentValidation on all requests
* Consistent API response model
* Proper use of HTTP status codes
* Clear and descriptive error messages

---

## Technology Stack

### Backend Framework

* ASP.NET Core Web API (REST)
* .NET 8
* C# 12

### Database & ORM

* PostgreSQL
* Entity Framework Core 9
* Npgsql provider

### Architecture & Libraries

* MediatR (CQRS + Mediator Pattern)
* FluentValidation (Input Validation)
* Repository Pattern + Generic Repository
* Manual DTO mapping

### Security

* JWT Bearer Authentication
* BCrypt password hashing
* Role-based authorization

### Testing

* xUnit
* Moq (Mocking)
* Shouldly (Assertions)
* FluentValidation.TestHelper

### API Documentation

* Swagger / OpenAPI (Swashbuckle)

---

## Database Schema

### Merchant

* Id (PK)
* Name
* Email (Unique)
* Accounts (Navigation)

### Account

* Id (PK)
* HolderName
* Balance
* MerchantId (FK)
* Transactions (Navigation)

### Transaction

* Id (PK)
* Amount
* Type (Payment or Refund)
* Status
* ReferenceNumber (Unique)
* Date
* AccountId (FK)
* PaymentMethodId (FK)

### User

* Id (PK)
* Username (Unique)
* PasswordHash
* Role

### PaymentMethod

* Id (PK)
* Name

---

## API Endpoints

### Authentication

```
POST /api/auth/register
POST /api/auth/login
```

### Merchants

```
POST /api/merchants
GET  /api/merchants/{id}
GET  /api/merchants/{id}/summary
```

### Accounts

```
POST /api/accounts
GET  /api/accounts/merchant/{merchantId}
```

### Transactions

```
POST /api/transactions/payment
POST /api/transactions/refund
GET  /api/transactions/account/{accountId}
```

---

## Configuration

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=PaymentDB;Username=postgres;Password=yourpassword"
  },
  "Jwt": {
    "Key": "your-super-secret-key-min-32-chars",
    "Issuer": "PaymentAPI",
    "Audience": "PaymentAPIUsers",
    "ExpiryMinutes": 60
  }
}
```

---

## Getting Started

### Prerequisites

* .NET 8 SDK
* PostgreSQL 14+
* Visual Studio 2022 / VS Code

### Setup Steps

1. Clone the repository:

   ```bash
   git clone https://github.com/FuhadSaneenK/PaymentAPI.git
   cd PaymentAPI
   ```

2. Update PostgreSQL connection string in `PaymentAPI.Api/appsettings.json`.

3. Run database migrations:

   ```bash
   dotnet ef database update --project PaymentAPI.Infrastructure --startup-project PaymentAPI.Api
   ```

4. Run the API:

   ```bash
   cd PaymentAPI.Api
   dotnet run
   ```

5. Access Swagger:

   ```
   https://localhost:<port>/swagger
   ```

---

## Testing

### Run all tests

```bash
dotnet test PaymentAPI.Tests/PaymentAPI.Tests.csproj
```

### Detailed output

```bash
dotnet test PaymentAPI.Tests --logger "console;verbosity=detailed"
```

### Test Summary

* Total Tests: 45
* Pass Rate: 100%
* Categories: Handler tests, Validator tests, Utility tests

---

## Project Structure

```
PaymentAPI/
│
├── PaymentAPI.Api/
│   ├── Controllers/
│   ├── Program.cs
│   └── appsettings.json
│
├── PaymentAPI.Application/
│   ├── Commands/
│   ├── Queries/
│   ├── Handlers/
│   ├── Validators/
│   ├── DTOs/
│   ├── Behaviors/
│   └── Wrappers/
│
├── PaymentAPI.Domain/
│   └── Entities/
│
├── PaymentAPI.Infrastructure/
│   ├── Persistence/
│   ├── Repositories/
│   └── Services/
│
└── PaymentAPI.Tests/
    ├── Handlers/
    ├── Validators/
    └── Mocks/
```

---

## Security Features

1. BCrypt hashing for passwords
2. JWT Authentication (stateless)
3. Role-based access control
4. FluentValidation for all requests
5. Unique constraints and business rule validation
6. Prevention of duplicate reference numbers
7. Refund validation rules

---

## Business Rules

### Merchant

* Email must be unique
* Name is required
* Email must be valid

### Account

* Must be linked to an existing merchant
* Holder name required
* Balance cannot be negative

### Payments

* Amount must be greater than zero
* Reference number must be unique
* Account must exist
* Balance increases

### Refunds

* Refund amount must be valid
* Cannot exceed original payment
* One refund per reference number
* Must belong to same account
* Balance decreases

---

## Contributing

1. Fork the repository
2. Create a new feature branch
3. Commit your changes
4. Push to GitHub
5. Create a Pull Request

---

## Author

**Fuhad Saneen K**
GitHub: [https://github.com/FuhadSaneenK](https://github.com/FuhadSaneenK)
Branch: `dev`

---

## Version History

### v1.0.0

* Initial release
* Full CRUD operations
* Authentication and authorization
* Payment and refund processing
* 45 unit tests (100% passing)
* PostgreSQL integration
* Full Clean Architecture structure

---

## Future Enhancements

* Integration tests
* Centralized logging (Serilog)
* API rate limiting
* Redis caching
* Email notifications
* Webhook support
* Reporting and analytics
* Multi-currency support
* Batch operations
* Audit tracking

---

## Support

For issues or questions, please open an issue on GitHub.


