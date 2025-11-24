using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PaymentAPI.Application.Commands.Merchants;
using PaymentAPI.Application.Queries.Accounts;
using PaymentAPI.Application.Queries.Merchants;

namespace PaymentAPI.Api.Controllers;

/// <summary>
/// Controller for managing merchant operations.
/// </summary>
/// <remarks>
/// Provides endpoints for creating merchants, retrieving merchant details,
/// managing merchant accounts, and generating merchant summaries.
/// Requires JWT authentication.
/// </remarks>
[Authorize]
public class MerchantController : BaseApiController
{
    private readonly ILogger<MerchantController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="MerchantController"/> class.
    /// </summary>
    /// <param name="mediator">The MediatR instance.</param>
    /// <param name="logger">The logger instance.</param>
    public MerchantController(IMediator mediator, ILogger<MerchantController> logger) : base(mediator)
    {
        _logger = logger;
    }

    /// <summary>
    /// Creates a new merchant.
    /// </summary>
    /// <param name="command">The command containing merchant details (name and email).</param>
    /// <returns>
    /// Returns 201 Created with merchant details if successful,
    /// or 400 Bad Request if email already exists or validation fails.
    /// </returns>
    /// <response code="201">Merchant created successfully.</response>
    /// <response code="400">Email already exists or validation failed.</response>
    /// <response code="401">Unauthorized - JWT token required.</response>
    /// <example>
    /// POST /api/merchant
    /// {
    ///   "name": "TechMart",
    ///   "email": "contact@techmart.com"
    /// }
    /// </example>
    [HttpPost]
    public async Task<IActionResult> CreateMerchant([FromBody] CreateMerchantCommand command)
    {
        _logger.LogInformation("Create merchant request received - Name: {Name}, Email: {Email}", 
            command.Name, command.Email);
        
        var response = await _mediator.Send(command);
        
        if (response.IsSuccess)
        {
            _logger.LogInformation("Merchant created successfully - Name: {Name}", command.Name);
        }
        else
        {
            _logger.LogWarning("Failed to create merchant - Name: {Name}, Reason: {Message}", 
                command.Name, response.Message);
        }
        
        return StatusCode(response.Status, response);
    }

    /// <summary>
    /// Retrieves merchant details by ID.
    /// </summary>
    /// <param name="id">The unique identifier of the merchant.</param>
    /// <returns>
    /// Returns 200 OK with merchant details if found,
    /// or 404 Not Found if merchant doesn't exist.
    /// </returns>
    /// <response code="200">Merchant found and returned.</response>
    /// <response code="404">Merchant not found.</response>
    /// <response code="401">Unauthorized - JWT token required.</response>
    /// <example>
    /// GET /api/merchant/1
    /// </example>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetMerchantById(int id)
    {
        _logger.LogInformation("Get merchant by ID request - MerchantId: {MerchantId}", id);
        
        var query = new GetMerchantByIdQuery(id);
        var response = await _mediator.Send(query);

        return StatusCode(response.Status, response);
    }

    /// <summary>
    /// Retrieves paginated accounts associated with a specific merchant with optional search.
    /// </summary>
    /// <param name="id">The unique identifier of the merchant.</param>
    /// <param name="pageNumber">The page number (default: 1).</param>
    /// <param name="pageSize">The number of items per page (default: 20).</param>
    /// <param name="search">Optional search term to filter by holder name.</param>
    /// <example>
    /// GET /api/merchant/1/accounts?pageNumber=1&pageSize=20
    /// GET /api/merchant/1/accounts?pageNumber=2&pageSize=50&search=John
    /// </example>
    [HttpGet("{id}/accounts")]
    public async Task<IActionResult> GetAccountsByMerchantId(
        int id,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null)
    {
        _logger.LogInformation("Get paginated accounts request - MerchantId: {MerchantId}, Page: {Page}/{Size}, Search: {Search}",
            id, pageNumber, pageSize, search);
        
        var query = new GetAccountsByMerchantIdQuery(id)
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SearchTerm = search
        };
        
        var response = await _mediator.Send(query);

        return StatusCode(response.Status, response);
    }

    /// <summary>
    /// Retrieves a comprehensive summary of merchant operations.
    /// </summary>
    /// <param name="id">The unique identifier of the merchant.</param>
    /// <returns>
    /// Returns 200 OK with merchant summary including total balance, 
    /// total accounts, total transactions, payments count, and refunds count.
    /// Returns 404 Not Found if merchant doesn't exist.
    /// </returns>
    /// <response code="200">Summary generated successfully.</response>
    /// <response code="404">Merchant not found.</response>
    /// <response code="401">Unauthorized - JWT token required.</response>
    /// <remarks>
    /// The summary includes:
    /// - Total balance across all accounts
    /// - Total number of account holders
    /// - Total transaction count
    /// - Total payments
    /// - Total refunds
    /// </remarks>
    /// <example>
    /// GET /api/merchant/1/summary
    /// </example>
    [HttpGet("{id}/summary")]
    public async Task<IActionResult> GetMerchantSummary(int id)
    {
        _logger.LogInformation("Get merchant summary request - MerchantId: {MerchantId}", id);
        
        var query = new GetMerchantSummaryQuery(id);
        var response = await _mediator.Send(query);
        
        return StatusCode(response.Status, response);
    }
}
