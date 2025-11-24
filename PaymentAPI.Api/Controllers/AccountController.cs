using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PaymentAPI.Application.Commands.Accounts;
using PaymentAPI.Application.Queries.Transactions;

namespace PaymentAPI.Api.Controllers;

/// <summary>
/// Controller for managing account operations.
/// </summary>
[Authorize]
public class AccountController : BaseApiController
{
    private readonly ILogger<AccountController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AccountController"/> class.
    /// </summary>
    /// <param name="mediator">The MediatR instance.</param>
    /// <param name="logger">The logger instance.</param>
    public AccountController(IMediator mediator, ILogger<AccountController> logger) : base(mediator)
    {
        _logger = logger;
    }

    /// <summary>
    /// Creates a new account for a merchant.
    /// </summary>
    /// <param name="command">The command containing account details (holder name, initial balance, merchant ID).</param>
    /// <returns>
    /// Returns 201 Created with account details if successful,
    /// or 400 Bad Request if validation fails or merchant doesn't exist.
    /// </returns>
    /// <response code="201">Account created successfully.</response>
    /// <response code="400">Validation failed or merchant not found.</response>
    /// <response code="401">Unauthorized - JWT token required.</response>
    /// <example>
    /// POST /api/account
    /// {
    ///   "holderName": "John Doe",
    ///   "balance": 1000.00,
    ///   "merchantId": 1
    /// }
    /// </example>
    [HttpPost]
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccountCommand command)
    {
        _logger.LogInformation("Create account request received - HolderName: {HolderName}, MerchantId: {MerchantId}, Balance: {Balance}",
            command.HolderName, command.MerchantId, command.Balance);
        
        var response = await _mediator.Send(command);
        
        if (response.IsSuccess)
        {
            _logger.LogInformation("Account created successfully - HolderName: {HolderName}, MerchantId: {MerchantId}",
                command.HolderName, command.MerchantId);
        }
        else
        {
            _logger.LogWarning("Failed to create account - HolderName: {HolderName}, Reason: {Message}",
                command.HolderName, response.Message);
        }
        
        return StatusCode(response.Status, response);
    }

    /// <summary>
    /// Retrieves paginated transactions for a specific account with optional filters.
    /// </summary>
    /// <param name="id">The unique identifier of the account.</param>
    /// <param name="pageNumber">The page number (default: 1).</param>
    /// <param name="pageSize">The number of items per page (default: 20).</param>
    /// <param name="startDate">Optional start date filter.</param>
    /// <param name="endDate">Optional end date filter.</param>
    /// <param name="type">Optional transaction type filter ("Payment" or "Refund").</param>
    /// <param name="status">Optional status filter ("Completed", "Pending", "Failed").</param>
    /// <example>
    /// GET /api/account/1/transactions?pageNumber=1&pageSize=20
    /// GET /api/account/1/transactions?pageNumber=1&pageSize=20&type=Payment&status=Completed
    /// GET /api/account/1/transactions?startDate=2024-01-01&endDate=2024-12-31
    /// </example>
    [HttpGet("{id}/transactions")]
    public async Task<IActionResult> GetTransactionsByAccountId(
        int id,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] string? type = null,
        [FromQuery] string? status = null)
    {
        _logger.LogInformation("Get paginated transactions request - AccountId: {AccountId}, Page: {Page}/{Size}",
            id, pageNumber, pageSize);
        
        var query = new GetTransactionsByAccountIdQuery(id)
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            StartDate = startDate,
            EndDate = endDate,
            Type = type,
            Status = status
        };
        
        var response = await _mediator.Send(query);
        
        return StatusCode(response.Status, response);
    }
}
