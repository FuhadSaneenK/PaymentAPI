Understood — here is the **clean, fully professional, GitHub-ready** version of your documentation.
No emojis, no icons, no decoration — strictly engineering-grade formatting.

---

# Payment API – Architecture Documentation

## Clean Architecture Overview

This project follows Clean Architecture (Onion Architecture) with strict separation of concerns and dependency rules.

```
┌──────────────────────────────────────────────────────────────┐
│                    PaymentAPI.Api (Presentation Layer)        │
│ Controllers • Middleware • Configuration • DI                 │
└──────────────────────────────────────────────────────────────┘
                        depends on
┌──────────────────────────────────────────────────────────────┐
│               PaymentAPI.Application (Use Cases)              │
│ Commands • Queries • Handlers • DTOs • Validators             │
└──────────────────────────────────────────────────────────────┘
                        depends on
┌──────────────────────────────────────────────────────────────┐
│                  PaymentAPI.Domain (Entities)                 │
│ Core business entities and rules                              │
└──────────────────────────────────────────────────────────────┘
                        implemented by
┌──────────────────────────────────────────────────────────────┐
│        PaymentAPI.Infrastructure (External Implementations)   │
│ Database • Repositories • Auth • Services                     │
└──────────────────────────────────────────────────────────────┘
```

### Dependency Rules

* Inner layers define abstractions; outer layers implement them.
* All dependencies point inward.
* Domain layer has no dependencies.
* Application layer depends only on Domain.
* Infrastructure and API depend on Application and Domain.

---

# Layer Details

## 1. Domain Layer (`PaymentAPI.Domain`)

### Purpose

Contains core business entities and domain rules.

### Contents

```
Domain/
  Entities/
    Merchant.cs
    Account.cs
    Transaction.cs
    PaymentMethod.cs
    User.cs
```

### Characteristics

* Pure POCO classes
* No external libraries
* Contains invariants and rules
* Entity relationships modeled via navigation properties

### Example

```csharp
public class Account
{
    public int Id { get; set; }
    public string HolderName { get; set; }
    public decimal Balance { get; set; }
    public int MerchantId { get; set; }

    public Merchant Merchant { get; set; }
    public List<Transaction> Transactions { get; set; } = new();
}
```

---

## 2. Application Layer (`PaymentAPI.Application`)

### Purpose

Implements use cases using CQRS, MediatR, validation, and mapping. Defines interfaces for infrastructure.

### Contents

```
Application/
  Commands/
  Queries/
  Handlers/
  Validators/
  DTOs/
  Behaviors/
  Wrappers/
  Abstractions/
  Constants/
```

### Key Patterns

#### CQRS

Commands modify state; queries retrieve state.

```csharp
public class CreateMerchantCommand : IRequest<ApiResponse<MerchantDto>>
{
    public string Name { get; set; }
}
```

#### Handler Example

```csharp
public class CreateMerchantCommandHandler 
    : IRequestHandler<CreateMerchantCommand, ApiResponse<MerchantDto>>
{
    private readonly IMerchantRepository _merchantRepository;

    public async Task<ApiResponse<MerchantDto>> Handle(
        CreateMerchantCommand request, 
        CancellationToken cancellationToken)
    {
        // business logic
    }
}
```

#### Validation Behavior

Centralized validation using FluentValidation and MediatR pipeline behaviors.

```csharp
public class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
{
    // validates before executing handlers
}
```

---

## 3. Infrastructure Layer (`PaymentAPI.Infrastructure`)

### Purpose

Implements database access, repository logic, authentication, hashing, and external systems.

### Contents

```
Infrastructure/
  Persistence/
    PaymentDbContext.cs
    Configurations/
    Migrations/
  Repositories/
  Services/
```

### Generic Repository Example

```csharp
public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly PaymentDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public async Task<T?> GetByIdAsync(int id, CancellationToken ct)
        => await _dbSet.FindAsync(new object[] { id }, ct);
}
```

### Specific Repository Example

```csharp
public class MerchantRepository : GenericRepository<Merchant>, IMerchantRepository
{
    public async Task<Merchant?> GetByEmailAsync(string email, CancellationToken ct)
        => await _dbSet.FirstOrDefaultAsync(m => m.Email == email, ct);
}
```

---

## 4. Presentation Layer (`PaymentAPI.Api`)

### Purpose

Exposes HTTP endpoints, configures dependency injection, and handles API-level concerns.

### Contents

```
Api/
  Controllers/
  Program.cs
  appsettings.json
```

### Controller Example

```csharp
[ApiController]
[Route("api/[controller]")]
public class MerchantController : ControllerBase
{
    private readonly IMediator _mediator;

    [HttpPost]
    public async Task<IActionResult> CreateMerchant(CreateMerchantCommand command)
    {
        var result = await _mediator.Send(command);
        return StatusCode(result.Status, result);
    }
}
```

### DI Configuration (Program.cs)

```csharp
builder.Services.AddScoped<IMerchantRepository, MerchantRepository>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateMerchantCommand).Assembly));
```

---

# Request Lifecycle Example: Create Merchant

1. Client sends POST request
2. Controller receives request and sends command to MediatR
3. ValidationBehavior validates command
4. Handler executes use case
5. Repository interacts with EF Core
6. Changes saved to PostgreSQL
7. Response returned via ApiResponse wrapper

---

# Design Patterns Used

| Pattern              | Purpose                                           |
| -------------------- | ------------------------------------------------- |
| CQRS                 | Separation of read/write logic                    |
| Mediator (MediatR)   | Decouples controllers from handlers               |
| Repository Pattern   | Abstracts EF Core operations                      |
| Unit of Work         | DbContext manages transactions                    |
| DTO Pattern          | Decouples API contract from entities              |
| Pipeline Behaviors   | Centralized validation and cross-cutting concerns |
| Dependency Injection | Testability and loose coupling                    |

---

# Database Design

### Entities and Relationships

```
Merchant (1) ─── (*) Account (1) ─── (*) Transaction
PaymentMethod (1) ─── (*) Transaction
User (Authentication)
```

### Indexes

* Merchant.Email (unique)
* Transaction.ReferenceNumber (unique)
* User.Username (unique)
* Foreign key indexes for joins

---

# Security Architecture

### Authentication

* BCrypt hashing for passwords
* JWT tokens for authentication
* 60-minute expiration
* Claims include UserId and Role

### Authorization

* Role-based authorization using `[Authorize(Roles = "...")]`

### Additional Security Measures

* HTTPS enforced
* FluentValidation for all requests
* EF Core parameterized queries (prevents SQL injection)

---

# Performance Considerations

* All database access is asynchronous
* Proper indexing on frequently queried columns
* EF Core optimizations (eager loading where needed)
* Query filtering executed at the database level
* Avoiding N+1 queries

---

# Scalability Considerations

### Supported

* Horizontal scaling (stateless API)
* Database replicas (compatible with CQRS)
* Configurable interfaces for future caching layers

### Potential Enhancements

* Redis caching
* Event sourcing
* Read/write DB splitting
* Microservice decomposition

---

# Metadata

**Architect:** Fuhad Saneen K
**Architecture Style:** Clean Architecture + CQRS
**Last Updated:** December 2025


