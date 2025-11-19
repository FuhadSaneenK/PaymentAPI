# Payment API

A comprehensive RESTful API for managing merchants, accounts, payments, and refunds built with **Clean Architecture** principles using **.NET 8** and **Entity Framework Core**.

---

## ??? Architecture

This project follows **Clean Architecture** with clear separation of concerns:

```
PaymentAPI/
??? PaymentAPI.Api/              # Presentation Layer (Controllers, Middleware)
??? PaymentAPI.Application/      # Application Layer (CQRS, DTOs, Validators)
??? PaymentAPI.Domain/          # Domain Layer (Entities, Business Rules)
??? PaymentAPI.Infrastructure/  # Infrastructure Layer (Database, Services)
??? PaymentAPI.Tests/           # Unit Tests (45 tests - 100% passing)
```

### Layer Responsibilities

**Domain Layer** (`PaymentAPI.Domain`)
- Core business entities (Merchant, Account, Transaction, User, PaymentMethod)
- No external dependencies
- Pure business logic

**Application Layer** (`PaymentAPI.Application`)
- CQRS pattern with MediatR
- Command/Query handlers
- DTOs and mapping
- FluentValidation validators
- Business orchestration
- Repository abstractions

**Infrastructure Layer** (`PaymentAPI.Infrastructure`)
- Entity Framework Core with PostgreSQL
- Repository implementations
- JWT authentication service
- BCrypt password hashing
- Database migrations and seeding

**Presentation Layer** (`PaymentAPI.Api`)
- REST API controllers
- JWT Bearer authentication
- Swagger/OpenAPI documentation
- Dependency injection configuration

---

## ?? Features

### Authentication & Authorization
- ? User registration with BCrypt password hashing
- ? JWT token-based authentication
- ? Role-based authorization (Admin, User)
- ? Secure password management

### Merchant Management
- ? Create merchants with unique email validation
- ? Retrieve merchant details by ID
- ? Get comprehensive merchant summary (accounts, balance, transactions)

### Account Management
- ? Create accounts linked to merchants
- ? Retrieve accounts by merchant ID
- ? Track account balances

### Transaction Management
- ? Process payments with reference number tracking
- ? Process refunds with validation rules
- ? Retrieve transaction history by account
- ? Automatic balance updates
- ? Duplicate reference number prevention

### Validation & Error Handling
- ? FluentValidation for input validation
- ? Consistent API response wrapper
- ? Proper HTTP status codes (200, 201, 400, 404)
- ? Descriptive error messages

---

## ??? Technology Stack

### Backend Framework
- **.NET 8** - Latest LTS version
- **ASP.NET Core Web API** - REST API framework
- **C# 12** - Modern language features

### Database & ORM
- **PostgreSQL** - Production database
- **Entity Framework Core 9.0** - ORM
- **Npgsql** - PostgreSQL driver

### Design Patterns & Libraries
- **MediatR** - CQRS implementation
- **FluentValidation** - Input validation
- **AutoMapper** (Manual mapping) - DTO mapping
- **Repository Pattern** - Data access abstraction
- **Generic Repository** - Reusable data operations

### Authentication & Security
- **JWT Bearer Tokens** - Stateless authentication
- **BCrypt.Net-Next** - Password hashing (bcrypt algorithm)
- **ASP.NET Core Identity** concepts

### Testing
- **xUnit** - Testing framework
- **Moq** - Mocking library
- **Shouldly** - Fluent assertions
- **FluentValidation.TestHelper** - Validator testing

### API Documentation
- **Swashbuckle (Swagger)** - OpenAPI/Swagger UI
- **Swagger UI** - Interactive API documentation

---

## ?? Database Schema

### Entities

**Merchant**
- Id (PK)
- Name
- Email (Unique)
- Accounts (Navigation)

**Account**
- Id (PK)
- HolderName
- Balance
- MerchantId (FK)
- Merchant (Navigation)
- Transactions (Navigation)

**Transaction**
- Id (PK)
- Amount
- Type (Payment/Refund)
- Status (Completed/Pending/Failed)
- ReferenceNumber (Unique)
- Date
- AccountId (FK)
- PaymentMethodId (FK)

**User**
- Id (PK)
- Username (Unique)
- PasswordHash
- Role (Admin/User)

**PaymentMethod**
- Id (PK)
- Name (Credit Card, Debit Card, etc.)

---

## ?? API Endpoints

### Authentication
```http
POST   /api/auth/register          # Register new user
POST   /api/auth/login             # Login and get JWT token
```

### Merchants
```http
POST   /api/merchants              # Create merchant
GET    /api/merchants/{id}         # Get merchant by ID
GET    /api/merchants/{id}/summary # Get merchant summary
```

### Accounts
```http
POST   /api/accounts               # Create account
GET    /api/accounts/merchant/{merchantId}  # Get accounts by merchant
```

### Transactions
```http
POST   /api/transactions/payment   # Process payment
POST   /api/transactions/refund    # Process refund
GET    /api/transactions/account/{accountId} # Get transactions
```

---

## ?? Configuration

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

## ?? Getting Started

### Prerequisites
- .NET 8 SDK
- PostgreSQL 14+
- Visual Studio 2022 or VS Code

### Installation

1. **Clone the repository**
```bash
git clone https://github.com/FuhadSaneenK/PaymentAPI.git
cd PaymentAPI
```

2. **Update database connection string**
   - Edit `appsettings.json` in `PaymentAPI.Api`
   - Set your PostgreSQL credentials

3. **Run database migrations**
```bash
dotnet ef database update --project PaymentAPI.Infrastructure --startup-project PaymentAPI.Api
```

4. **Run the application**
```bash
cd PaymentAPI.Api
dotnet run
```

5. **Access Swagger UI**
   - Open browser: `https://localhost:7001/swagger`

---

## ?? Testing

### Run All Tests
```bash
dotnet test PaymentAPI.Tests/PaymentAPI.Tests.csproj
```

### Run with Detailed Output
```bash
dotnet test PaymentAPI.Tests/PaymentAPI.Tests.csproj --logger "console;verbosity=detailed"
```

### Test Coverage
- **Total Tests**: 45
- **Pass Rate**: 100%
- **Categories**: Handlers (27), Validators (15), Integration (3)

---

## ?? Project Structure

```
PaymentAPI/
?
??? PaymentAPI.Api/
?   ??? Controllers/
?   ?   ??? AuthController.cs
?   ?   ??? MerchantController.cs
?   ?   ??? AccountController.cs
?   ?   ??? TransactionController.cs
?   ??? Program.cs
?   ??? appsettings.json
?
??? PaymentAPI.Application/
?   ??? Commands/
?   ?   ??? Auth/
?   ?   ??? Merchants/
?   ?   ??? Accounts/
?   ?   ??? Transactions/
?   ??? Queries/
?   ?   ??? Merchants/
?   ?   ??? Accounts/
?   ?   ??? Transactions/
?   ??? Handlers/
?   ??? DTOs/
?   ??? Validators/
?   ??? Behaviors/
?   ??? Wrappers/
?
??? PaymentAPI.Domain/
?   ??? Entities/
?       ??? Merchant.cs
?       ??? Account.cs
?       ??? Transaction.cs
?       ??? User.cs
?       ??? PaymentMethod.cs
?
??? PaymentAPI.Infrastructure/
?   ??? Persistance/
?   ?   ??? PaymentDbContext.cs
?   ?   ??? Configurations/
?   ?   ??? Migrations/
?   ?   ??? DataSeeder.cs
?   ??? Repositories/
?   ??? Services/
?
??? PaymentAPI.Tests/
    ??? Handlers/
    ??? Validators/
    ??? Mocks/
```

---

## ?? Security Features

1. **Password Security**
   - BCrypt hashing (salt + hash)
   - Never stores plain text passwords

2. **JWT Authentication**
   - Token-based stateless authentication
   - Configurable expiry
   - Role-based claims

3. **Input Validation**
   - FluentValidation on all inputs
   - Email format validation
   - Amount and ID validation

4. **Business Rules**
   - Unique email validation
   - Unique reference number validation
   - Refund amount limits
   - Account ownership verification

---

## ?? Design Patterns Used

1. **CQRS** (Command Query Responsibility Segregation)
   - Commands for write operations
   - Queries for read operations

2. **Repository Pattern**
   - Abstraction over data access
   - Generic repository for common operations

3. **Mediator Pattern**
   - MediatR for decoupling
   - Pipeline behaviors for cross-cutting concerns

4. **Dependency Injection**
   - Constructor injection
   - Interface-based dependencies

5. **Unit of Work**
   - SaveChanges through repository
   - Transaction management

---

## ?? Business Rules

### Merchant Rules
- ? Email must be unique
- ? Name is required
- ? Valid email format

### Account Rules
- ? Must belong to existing merchant
- ? Holder name required
- ? Initial balance ? 0

### Payment Rules
- ? Amount must be > 0
- ? Account must exist
- ? Payment method must exist
- ? Reference number must be unique
- ? Balance increases on payment

### Refund Rules
- ? Amount must be > 0
- ? Original payment must exist
- ? Refund amount ? original payment amount
- ? One refund per reference number
- ? Refund to same account as payment
- ? Balance decreases on refund

---

## ?? Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

---

## ?? License

This project is licensed under the MIT License.

---

## ?? Author

**Fuhad Saneen K**
- GitHub: [@FuhadSaneenK](https://github.com/FuhadSaneenK)
- Branch: `dev`

---

## ?? Version History

### v1.0.0 (Current)
- ? Initial release
- ? Complete CRUD operations
- ? JWT authentication
- ? Payment & refund processing
- ? 45 unit tests (100% passing)
- ? Clean Architecture implementation
- ? PostgreSQL integration

---

## ?? Future Enhancements

- [ ] Add integration tests
- [ ] Implement logging (Serilog)
- [ ] Add API rate limiting
- [ ] Implement caching (Redis)
- [ ] Add email notifications
- [ ] Implement webhook support
- [ ] Add transaction reports/analytics
- [ ] Multi-currency support
- [ ] Batch payment processing
- [ ] Audit trail implementation

---

## ?? Support

For issues, questions, or contributions, please open an issue on GitHub or contact the maintainer.

---

**Built with ?? using Clean Architecture and .NET 8**
