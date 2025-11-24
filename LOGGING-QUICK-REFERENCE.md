# Quick Reference: Using Logging in PaymentAPI

## ?? Quick Start

### Adding Logger to a New Handler

```csharp
using Microsoft.Extensions.Logging;

public class YourNewCommandHandler : IRequestHandler<YourCommand, ApiResponse<YourDto>>
{
    private readonly ILogger<YourNewCommandHandler> _logger;

    // 1. Inject ILogger<T> via constructor
    public YourNewCommandHandler(ILogger<YourNewCommandHandler> logger)
    {
        _logger = logger;
    }

    public async Task<ApiResponse<YourDto>> Handle(YourCommand request, CancellationToken cancellationToken)
    {
        // 2. Log at appropriate level
        _logger.LogInformation("Processing command - Property: {Value}", request.Property);

        try
        {
            // Your business logic here
            
            _logger.LogInformation("Command completed successfully");
            return ApiResponse<YourDto>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing command");
            throw;
        }
    }
}
```

## ?? Log Level Guidelines

### ? LogInformation
**When to use**: Normal business operations
```csharp
_logger.LogInformation("User registered - UserId: {UserId}, Username: {Username}", 
    user.Id, user.Username);

_logger.LogInformation("Payment processed - AccountId: {AccountId}, Amount: {Amount}", 
    accountId, amount);
```

### ?? LogWarning
**When to use**: Expected errors, validation failures, business rule violations
```csharp
_logger.LogWarning("Login failed: Invalid credentials for user {Username}", username);

_logger.LogWarning("Refund failed: Amount exceeds original payment - Reference: {RefNo}", refNo);
```

### ?? LogError
**When to use**: Unexpected errors, exceptions
```csharp
_logger.LogError(ex, "Error creating merchant - Name: {Name}, Email: {Email}", 
    name, email);

_logger.LogError(ex, "Database connection failed");
```

### ?? LogDebug
**When to use**: Detailed debugging information (not logged in production by default)
```csharp
_logger.LogDebug("Fetching merchant - MerchantId: {MerchantId}", id);

_logger.LogDebug("Query returned {Count} records", results.Count);
```

### ?? LogCritical
**When to use**: Critical failures requiring immediate attention
```csharp
_logger.LogCritical(ex, "Application cannot start - Database unavailable");
```

## ?? Structured Logging Best Practices

### ? DO: Use Named Parameters
```csharp
// ? GOOD - Structured with named parameters
_logger.LogInformation("Payment created - AccountId: {AccountId}, Amount: {Amount}, Reference: {ReferenceNo}",
    accountId, amount, referenceNo);
```

### ? DON'T: Use String Concatenation
```csharp
// ? BAD - String concatenation loses structure
_logger.LogInformation("Payment created for account " + accountId + " with amount " + amount);
```

### ? DO: Log Business Context
```csharp
// ? GOOD - Includes relevant business context
_logger.LogInformation("Refund completed - TransactionId: {TxnId}, PreviousBalance: {OldBalance}, NewBalance: {NewBalance}",
    txnId, oldBalance, newBalance);
```

### ? DON'T: Log Sensitive Data
```csharp
// ? NEVER LOG THESE
_logger.LogInformation("User password: {Password}", password); // SECURITY RISK!
_logger.LogInformation("Credit card: {CardNumber}", cardNumber); // ILLEGAL!
_logger.LogInformation("JWT Token: {Token}", token); // SECURITY BREACH!
```

## ?? Security Guidelines

### Safe to Log
- ? UserIds
- ? Usernames
- ? Transaction IDs
- ? Reference Numbers
- ? Amounts
- ? Timestamps
- ? Status codes

### NEVER Log
- ? Passwords (plain or hashed)
- ? Tokens (JWT, API keys)
- ? Credit card numbers
- ? SSN, Personal identification numbers
- ? Full email addresses (in some regulations)
- ? Complete request payloads (may contain sensitive data)

## ?? Common Patterns

### Pattern 1: Command/Query Entry
```csharp
public async Task<ApiResponse<TDto>> Handle(TCommand request, CancellationToken cancellationToken)
{
    _logger.LogInformation("Processing {CommandName} - Key: {Value}", 
        typeof(TCommand).Name, request.KeyProperty);
    
    // ... business logic
}
```

### Pattern 2: Validation Failure
```csharp
if (account == null)
{
    _logger.LogWarning("{EntityName} not found - {EntityId}: {Id}", 
        "Account", nameof(accountId), accountId);
    return ApiResponse<TDto>.NotFound("Account not found");
}
```

### Pattern 3: Success with Metrics
```csharp
_logger.LogInformation("{Operation} completed successfully - {Metric1}: {Value1}, {Metric2}: {Value2}",
    "Payment", "TransactionId", txn.Id, "NewBalance", account.Balance);
```

### Pattern 4: Error Handling
```csharp
try
{
    // business logic
}
catch (Exception ex)
{
    _logger.LogError(ex, "Error in {Operation} - Context: {Context}", 
        "MakePayment", $"AccountId: {accountId}, Amount: {amount}");
    throw; // or return error response
}
```

## ?? Log Output Templates

### Console (Development)
```
[12:34:56 INF] PaymentAPI.Application.Handlers.Auth.LoginUserCommandHandler - Login attempt for username: admin
```

### File (Production)
```
2025-01-21 12:34:56.123 [INF] PaymentAPI.Application.Handlers.Auth.LoginUserCommandHandler - Login attempt for username: admin {SourceContext}
```

## ?? Configuration Quick Reference

### Change Log Level (appsettings.json)
```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Debug",  // Change to: Trace, Debug, Information, Warning, Error, Critical
      "Override": {
        "Microsoft": "Warning",
        "YourNamespace": "Information"
      }
    }
  }
}
```

### Add New Sink
```json
{
  "Serilog": {
    "WriteTo": [
      { "Name": "Console" },
      { "Name": "File", "Args": { "path": "logs/app-.log" } },
      { "Name": "Seq", "Args": { "serverUrl": "http://localhost:5341" } }
    ]
  }
}
```

## ?? Testing with Logger Mock

```csharp
using PaymentAPI.Tests.Mocks;

[Fact]
public async Task Should_DoSomething()
{
    // Arrange
    var logger = LoggerMock.Create<YourHandler>();
    var handler = new YourHandler(dependencies, logger);
    
    // Act
    var result = await handler.Handle(command, CancellationToken.None);
    
    // Assert
    result.Should().BeSuccessful();
}
```

## ?? Where to Find Logs

### Development
- **Console**: Visual Studio Output Window
- **Files**: `ProjectRoot/logs/paymentapi-YYYYMMDD.log`

### Production
- **Files**: Configure path in appsettings.Production.json
- **Centralized**: Use Seq, ELK, or Application Insights

## ?? Common Scenarios

### Scenario 1: New Feature Development
```csharp
// Start of operation
_logger.LogInformation("Starting {Feature} - Input: {Input}", "NewFeature", input);

// Validation
if (!IsValid(input))
{
    _logger.LogWarning("{Feature} validation failed - Reason: {Reason}", "NewFeature", reason);
    return Error();
}

// Success
_logger.LogInformation("{Feature} completed - Result: {Result}", "NewFeature", result);
```

### Scenario 2: Debugging Production Issue
1. Check logs for error patterns
2. Increase log level if needed (Information ? Debug)
3. Restart application
4. Reproduce issue
5. Analyze detailed logs
6. Fix and restore log level

### Scenario 3: Performance Investigation
```csharp
var sw = Stopwatch.StartNew();
var result = await SlowOperation();
sw.Stop();

_logger.LogInformation("Operation completed in {ElapsedMs}ms", sw.ElapsedMilliseconds);

if (sw.ElapsedMilliseconds > 1000)
{
    _logger.LogWarning("Slow operation detected - {ElapsedMs}ms", sw.ElapsedMilliseconds);
}
```

## ?? Troubleshooting

### Logs Not Appearing?
1. Check `appsettings.json` configuration
2. Verify Serilog packages are installed
3. Ensure `builder.Host.UseSerilog()` is called in Program.cs
4. Check log level settings

### Too Many Logs?
```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Warning"  // Reduce to Warning or Error
    }
  }
}
```

### Need More Detail?
```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Debug"  // Increase to Debug or Trace
    }
  }
}
```

---

## ?? Additional Resources

- [Serilog Documentation](https://serilog.net/)
- [Structured Logging Best Practices](https://github.com/serilog/serilog/wiki/Structured-Data)
- [ASP.NET Core Logging](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/logging/)

---

**Remember**: Good logging is like good documentation - it helps future you understand what's happening! ??
