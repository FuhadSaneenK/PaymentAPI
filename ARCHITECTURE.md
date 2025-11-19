# Payment API - Architecture Documentation

## ??? Clean Architecture Overview

This project implements **Clean Architecture** (also known as Onion Architecture) with clear separation of concerns and dependency inversion.

```
???????????????????????????????????????????????????
?            PaymentAPI.Api (UI Layer)            ?
?  Controllers, Middleware, Configuration, DI     ?
???????????????????????????????????????????????????
                    ? depends on ?
???????????????????????????????????????????????????
?      PaymentAPI.Application (Use Cases)         ?
?  Commands, Queries, Handlers, DTOs, Validators  ?
???????????????????????????????????????????????????
                    ? depends on ?
???????????????????????????????????????????????????
?       PaymentAPI.Domain (Entities)              ?
?      Business Entities, Domain Rules            ?
???????????????????????????????????????????????????
                    ? implements
???????????????????????????????????????????????????
?   PaymentAPI.Infrastructure (External Services) ?
?   Database, Repositories, JWT, BCrypt           ?
???????????????????????????????????????????????????
```

### Dependency Rules
- **Inner layers** define interfaces
- **Outer layers** implement interfaces
- Dependencies point **inward only**
- Domain has **no dependencies**

---

## ?? Layer Details

### 1. Domain Layer (`PaymentAPI.Domain`)

**Purpose:** Core business logic and entities

**Dependencies:** None (Pure .NET)

**Contents:**
```
Domain/
??? Entities/
    ??? Merchant.cs          # Merchant aggregate root
    ??? Account.cs           # Account entity
    ??? Transaction.cs       # Transaction entity
    ??? PaymentMethod.cs     # Payment method lookup
    ??? User.cs              # User for authentication
```

**Characteristics:**
- Pure C# classes (POCOs)
- No infrastructure concerns
- Business rules embedded in entities
- Navigation properties for EF Core

**Example Entity:**
```csharp
public class Account
{
    public int Id { get; set; }
    public string HolderName { get; set; }
    public decimal Balance { get; set; }
    public int MerchantId { get; set; }
    
    // Navigation properties
    public Merchant Merchant { get; set; }
    public List<Transaction> Transactions { get; set; } = new();
}
```

---

### 2. Application Layer (`PaymentAPI.Application`)

**Purpose:** Business logic orchestration and use cases

**Dependencies:** Domain layer only

**Contents:**
```
Application/
??? Commands/               # Write operations (CQRS)
?   ??? Auth/
?   ?   ??? LoginUserCommand.cs
?   ?   ??? RegisterUserCommand.cs
?   ??? Merchants/
?   ?   ??? CreateMerchantCommand.cs
?   ??? Accounts/
?   ?   ??? CreateAccountCommand.cs
?   ??? Transactions/
?       ??? MakePaymentCommand.cs
?       ??? MakeRefundCommand.cs
?
??? Queries/                # Read operations (CQRS)
?   ??? Merchants/
?   ?   ??? GetMerchantByIdQuery.cs
?   ?   ??? GetMerchantSummaryQuery.cs
?   ??? Accounts/
?   ?   ??? GetAccountsByMerchantIdQuery.cs
?   ??? Transactions/
?       ??? GetTransactionsByAccountIdQuery.cs
?
??? Handlers/              # MediatR request handlers
?   ??? Auth/
?   ??? Merchants/
?   ??? Accounts/
?   ??? Transactions/
?
??? DTOs/                  # Data Transfer Objects
?   ??? MerchantDto.cs
?   ??? MerchantSummaryDto.cs
?   ??? AccountDto.cs
?   ??? TransactionDto.cs
?
??? Validators/            # FluentValidation validators
?   ??? Merchants/
?   ?   ??? CreateMerchantValidator.cs
?   ??? Transactions/
?       ??? MakePaymentValidator.cs
?       ??? MakeRefundValidator.cs
?
??? Behaviors/             # MediatR pipeline behaviors
?   ??? ValidationBehavior.cs
?
??? Wrappers/              # Response wrappers
?   ??? ApiResponse.cs
?
??? Abstractions/          # Interface definitions
?   ??? Repositories/      # Repository interfaces
?   ?   ??? IGenericRepository.cs
?   ?   ??? IMerchantRepository.cs
?   ?   ??? IAccountRepository.cs
?   ?   ??? ITransactionRepository.cs
?   ?   ??? IPaymentMethodRepository.cs
?   ?   ??? IUserRepository.cs
?   ??? Services/          # Service interfaces
?       ??? IJwtService.cs
?       ??? IPasswordHasher.cs
?
??? Constants/
    ??? StatusCode.cs
```

**Key Patterns:**

**CQRS (Command Query Responsibility Segregation)**
```csharp
// Command - Modifies state
public class CreateMerchantCommand : IRequest<ApiResponse<MerchantDto>>
{
    public string Name { get; set; }
    public string Email { get; set; }
}

// Query - Reads state
public class GetMerchantByIdQuery : IRequest<ApiResponse<MerchantDto>>
{
    public int Id { get; set; }
}
```

**Handler Pattern**
```csharp
public class CreateMerchantCommandHandler 
    : IRequestHandler<CreateMerchantCommand, ApiResponse<MerchantDto>>
{
    private readonly IMerchantRepository _merchantRepository;
    
    public async Task<ApiResponse<MerchantDto>> Handle(
        CreateMerchantCommand request, 
        CancellationToken cancellationToken)
    {
        // Business logic here
    }
}
```

**Validation Pipeline Behavior**
```csharp
public class ValidationBehavior<TRequest, TResponse> 
    : IPipelineBehavior<TRequest, TResponse>
{
    // Automatically validates all requests before handlers
    // Throws ValidationException if validation fails
}
```

---

### 3. Infrastructure Layer (`PaymentAPI.Infrastructure`)

**Purpose:** External services and data persistence

**Dependencies:** Application, Domain

**Contents:**
```
Infrastructure/
??? Persistance/
?   ??? PaymentDbContext.cs      # EF Core DbContext
?   ??? DataSeeder.cs            # Initial data seeding
?   ?
?   ??? Configurations/          # EF Core entity configs
?   ?   ??? MerchantConfiguration.cs
?   ?   ??? AccountConfiguration.cs
?   ?   ??? TransactionConfiguration.cs
?   ?   ??? PaymentMethodConfiguration.cs
?   ?   ??? UserConfiguration.cs
?   ?
?   ??? Migrations/              # EF Core migrations
?       ??? 20251114090432_InitialCreate.cs
?       ??? 20251119052244_AddUserTable.cs
?
??? Repositories/                # Repository implementations
?   ??? GenericRepository.cs     # Base repository
?   ??? MerchantRepository.cs
?   ??? AccountRepository.cs
?   ??? TransactionRepository.cs
?   ??? PaymentMethodRepository.cs
?   ??? UserRepository.cs
?
??? Services/                    # Service implementations
    ??? JwtService.cs            # JWT token generation
    ??? PasswordHasher.cs        # BCrypt hashing
```

**Key Implementations:**

**Generic Repository Pattern**
```csharp
public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly PaymentDbContext _context;
    protected readonly DbSet<T> _dbSet;
    
    public async Task<T?> GetByIdAsync(int id, CancellationToken ct)
        => await _dbSet.FindAsync(new object[] { id }, ct);
    
    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken ct)
        => await _dbSet.ToListAsync(ct);
    
    public async Task AddAsync(T entity, CancellationToken ct)
        => await _dbSet.AddAsync(entity, ct);
    
    public async Task<int> SaveChangesAsync(CancellationToken ct)
        => await _context.SaveChangesAsync(ct);
}
```

**Specific Repository**
```csharp
public class MerchantRepository : GenericRepository<Merchant>, IMerchantRepository
{
    public async Task<Merchant?> GetByEmailAsync(string email, CancellationToken ct)
        => await _dbSet.FirstOrDefaultAsync(m => m.Email == email, ct);
}
```

**Entity Configuration (Fluent API)**
```csharp
public class MerchantConfiguration : IEntityTypeConfiguration<Merchant>
{
    public void Configure(EntityTypeBuilder<Merchant> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Name).IsRequired().HasMaxLength(200);
        builder.Property(m => m.Email).IsRequired().HasMaxLength(200);
        builder.HasIndex(m => m.Email).IsUnique();
        
        // Relationship
        builder.HasMany(m => m.Accounts)
               .WithOne(a => a.Merchant)
               .HasForeignKey(a => a.MerchantId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
```

---

### 4. Presentation Layer (`PaymentAPI.Api`)

**Purpose:** API endpoints and configuration

**Dependencies:** Application, Infrastructure

**Contents:**
```
Api/
??? Controllers/
?   ??? AuthController.cs        # POST /api/auth/register, /login
?   ??? MerchantController.cs    # CRUD for merchants
?   ??? AccountController.cs     # CRUD for accounts
?   ??? TransactionController.cs # Payment & refund operations
?
??? Program.cs                   # Application entry point & DI
??? appsettings.json            # Configuration
??? appsettings.Development.json
```

**Controller Pattern**
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
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetMerchant(int id)
    {
        var result = await _mediator.Send(new GetMerchantByIdQuery(id));
        return StatusCode(result.Status, result);
    }
}
```

**Dependency Injection Setup**
```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<PaymentDbContext>(options =>
    options.UseNpgsql(connectionString));

// MediatR (CQRS)
builder.Services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssembly(typeof(CreateMerchantCommand).Assembly));

// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<CreateMerchantValidator>();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// Repositories
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IMerchantRepository, MerchantRepository>();
// ... more repositories

// Services
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

// Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => { /* JWT config */ });
```

---

## ?? Request Flow

### Example: Create Merchant Flow

```
1. HTTP Request
   POST /api/merchants
   Body: { "name": "ABC Corp", "email": "abc@test.com" }
   
   ?

2. Controller (Presentation Layer)
   MerchantController.CreateMerchant(CreateMerchantCommand)
   ? Sends command to MediatR
   
   ?

3. MediatR Pipeline
   ? ValidationBehavior (validates command)
   ? If valid, continues; else throws ValidationException
   
   ?

4. Handler (Application Layer)
   CreateMerchantCommandHandler.Handle()
   ? Checks if email exists (via repository)
   ? Creates Merchant entity
   ? Saves to database (via repository)
   ? Maps entity to DTO
   ? Returns ApiResponse<MerchantDto>
   
   ?

5. Repository (Infrastructure Layer)
   MerchantRepository.GetByEmailAsync()
   MerchantRepository.AddAsync()
   MerchantRepository.SaveChangesAsync()
   ? EF Core translates to SQL
   ? PostgreSQL executes queries
   
   ?

6. HTTP Response
   Status: 201 Created
   Body: {
     "status": 201,
     "data": { "id": 1, "name": "ABC Corp", "email": "abc@test.com" },
     "message": "Merchant created successfully"
   }
```

---

## ?? Design Patterns Used

### 1. CQRS (Command Query Responsibility Segregation)
**Purpose:** Separate read and write operations

**Benefits:**
- Optimized queries for reads
- Cleaner command validation
- Scalability (separate read/write databases possible)

**Implementation:**
- Commands: `CreateMerchantCommand`, `MakePaymentCommand`
- Queries: `GetMerchantByIdQuery`, `GetMerchantSummaryQuery`

---

### 2. Mediator Pattern (MediatR)
**Purpose:** Decouple controllers from handlers

**Benefits:**
- Controllers don't know about handlers
- Pipeline behaviors (validation, logging, etc.)
- Single responsibility

**Implementation:**
```csharp
// Controller sends request
var result = await _mediator.Send(new CreateMerchantCommand());

// MediatR routes to handler
public class CreateMerchantCommandHandler 
    : IRequestHandler<CreateMerchantCommand, ApiResponse<MerchantDto>>
```

---

### 3. Repository Pattern
**Purpose:** Abstract data access logic

**Benefits:**
- Testable (easy to mock)
- Swappable data sources
- Centralized query logic

**Implementation:**
```csharp
// Generic base
public interface IGenericRepository<T>
{
    Task<T?> GetByIdAsync(int id, CancellationToken ct);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken ct);
    Task AddAsync(T entity, CancellationToken ct);
    Task<int> SaveChangesAsync(CancellationToken ct);
}

// Specific extension
public interface IMerchantRepository : IGenericRepository<Merchant>
{
    Task<Merchant?> GetByEmailAsync(string email, CancellationToken ct);
}
```

---

### 4. Dependency Injection
**Purpose:** Invert dependencies, improve testability

**Benefits:**
- Loose coupling
- Easy testing (mock dependencies)
- Centralized configuration

**Implementation:**
```csharp
// Interface in Application layer
public interface IJwtService
{
    string GenerateToken(int userId, string username, string role);
}

// Implementation in Infrastructure layer
public class JwtService : IJwtService { ... }

// Registration in API layer
builder.Services.AddScoped<IJwtService, JwtService>();

// Usage in Application layer
public class LoginUserCommandHandler
{
    private readonly IJwtService _jwtService;
    
    public LoginUserCommandHandler(IJwtService jwtService)
    {
        _jwtService = jwtService;
    }
}
```

---

### 5. DTO (Data Transfer Object) Pattern
**Purpose:** Control data shape, decouple entities from API

**Benefits:**
- API stability (entity changes don't affect API)
- Security (hide sensitive fields)
- Optimized data transfer

**Implementation:**
```csharp
// Entity (Domain layer)
public class Merchant
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public List<Account> Accounts { get; set; } // Navigation
}

// DTO (Application layer)
public class MerchantDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    // No Accounts - controlled exposure
}
```

---

### 6. Pipeline Behavior Pattern
**Purpose:** Cross-cutting concerns (validation, logging, etc.)

**Benefits:**
- DRY (Don't Repeat Yourself)
- Consistent behavior across all requests
- Easy to add new behaviors

**Implementation:**
```csharp
public class ValidationBehavior<TRequest, TResponse> 
    : IPipelineBehavior<TRequest, TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    
    public async Task<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken ct)
    {
        // Validate before handler
        var context = new ValidationContext<TRequest>(request);
        var failures = await Task.WhenAll(_validators
            .Select(v => v.ValidateAsync(context, ct)));
        
        if (failures.Any(f => !f.IsValid))
            throw new ValidationException(failures.SelectMany(f => f.Errors));
        
        // Continue to handler
        return await next();
    }
}
```

---

### 7. Unit of Work Pattern
**Purpose:** Manage database transactions

**Implementation:**
```csharp
// DbContext acts as Unit of Work
public class PaymentDbContext : DbContext
{
    public DbSet<Merchant> Merchants { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    
    // SaveChangesAsync commits transaction
    public override async Task<int> SaveChangesAsync(CancellationToken ct)
    {
        // All changes committed in single transaction
        return await base.SaveChangesAsync(ct);
    }
}
```

---

## ?? Database Design

### Entity Relationships

```
????????????????
?   Merchant   ?
????????????????
? Id (PK)      ?
? Name         ?
? Email (UK)   ?
????????????????
       ? 1
       ? has many
       ? *
????????????????
?   Account    ?
????????????????
? Id (PK)      ?
? HolderName   ?
? Balance      ?
? MerchantId(FK)?
????????????????
       ? 1
       ? has many
       ? *
???????????????????      ??????????????????
?   Transaction   ?      ? PaymentMethod  ?
???????????????????      ??????????????????
? Id (PK)         ?      ? Id (PK)        ?
? Amount          ? *  1 ? Name           ?
? Type            ???????? Description    ?
? Status          ?      ??????????????????
? ReferenceNo(UK) ?
? Date            ?
? AccountId (FK)  ?
? PaymentMethodId ?
???????????????????

????????????????
?     User     ?
????????????????
? Id (PK)      ?
? Username(UK) ?
? PasswordHash ?
? Role         ?
????????????????
```

### Indexes
- `Merchant.Email` - Unique index
- `Transaction.ReferenceNumber` - Unique index
- `User.Username` - Unique index
- `Account.MerchantId` - Foreign key index
- `Transaction.AccountId` - Foreign key index

---

## ?? Security Architecture

### Authentication Flow
```
1. User Registration
   ? Password hashed with BCrypt (10 rounds)
   ? Stored in database
   
2. User Login
   ? Password verified with BCrypt
   ? JWT token generated (60 min expiry)
   ? Token returned to client
   
3. API Request
   ? Client sends JWT in Authorization header
   ? JWT middleware validates token
   ? Claims extracted (UserId, Role)
   ? Request proceeds if valid
```

### Security Layers
1. **Password Security**: BCrypt with salt
2. **Transport Security**: HTTPS
3. **Authentication**: JWT tokens
4. **Authorization**: Role-based ([Authorize(Roles = "Admin")])
5. **Input Validation**: FluentValidation
6. **SQL Injection**: EF Core parameterized queries

---

## ?? Best Practices Implemented

### 1. Single Responsibility Principle (SRP)
- Each handler does one thing
- Repositories handle only data access
- Services handle specific concerns

### 2. Open/Closed Principle (OCP)
- Generic repository extensible
- Pipeline behaviors can be added without modifying existing code

### 3. Liskov Substitution Principle (LSP)
- Repository implementations are substitutable
- Mock repositories in tests

### 4. Interface Segregation Principle (ISP)
- Specific repository interfaces (IMerchantRepository, etc.)
- Not one huge IRepository

### 5. Dependency Inversion Principle (DIP)
- High-level modules depend on abstractions
- Application layer defines interfaces
- Infrastructure layer implements them

---

## ?? Performance Considerations

### 1. Async/Await
- All database operations are async
- Non-blocking I/O

### 2. EF Core Optimizations
- Eager loading where needed (`Include()`)
- AsNoTracking for read-only queries (future)
- Compiled queries for hot paths (future)

### 3. Database Indexing
- Unique indexes on frequently queried fields
- Foreign key indexes for joins

### 4. Query Optimization
- Filter in database (LINQ translates to SQL WHERE)
- Avoid N+1 queries (Include navigations)

---

## ?? Scalability Considerations

### Current Architecture Supports:
- ? Horizontal scaling (stateless API)
- ? Read replicas (CQRS ready)
- ? Caching layer (interface-based)
- ? Message queues (MediatR can publish events)

### Future Enhancements:
- [ ] Redis caching
- [ ] Event sourcing
- [ ] Read/Write database separation
- [ ] Microservices decomposition

---

**Architecture Designed By:** Fuhad Saneen K  
**Architecture Style:** Clean Architecture + CQRS  
**Last Updated:** December 2024
