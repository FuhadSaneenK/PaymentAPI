using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PaymentAPI.Application.Commands.Transactions;

namespace PaymentAPI.Api.Controllers;

/// <summary>
/// Controller for processing payment transactions and refunds.
/// </summary>
/// <remarks>
/// Provides endpoints for payment processing and refund operations with strict business rule validation.
/// Requires JWT authentication. All transactions update account balances automatically.
/// </remarks>
[Authorize]
public class TransactionController : BaseApiController
{
    private readonly ILogger<TransactionController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TransactionController"/> class.
    /// </summary>
    /// <param name="mediator">The MediatR instance.</param>
    /// <param name="logger">The logger instance.</param>
    public TransactionController(IMediator mediator, ILogger<TransactionController> logger) : base(mediator)
    {
        _logger = logger;
    }

    /// <summary>
    /// Processes a payment transaction.
    /// </summary>
    /// <param name="command">The payment command containing amount, account ID, payment method ID, and reference number.</param>
    /// <returns>
    /// Returns 201 Created with transaction details if payment is processed successfully,
    /// or 400 Bad Request if validation fails or reference number already exists,
    /// or 404 Not Found if account or payment method doesn't exist.
    /// </returns>
    /// <response code="201">Payment processed successfully, account balance increased.</response>
    /// <response code="400">Validation failed or duplicate reference number.</response>
    /// <response code="404">Account or payment method not found.</response>
    /// <response code="401">Unauthorized - JWT token required.</response>
    /// <remarks>
    /// Business Rules:
    /// - Amount must be greater than zero
    /// - Reference number must be unique
    /// - Account and payment method must exist
    /// - Account balance increases by payment amount
    /// - Transaction type set to "Payment"
    /// - Transaction status set to "Completed"
    /// </remarks>
    /// <example>
    /// POST /api/transaction/payment
    /// {
    ///   "amount": 500.00,
    ///   "accountId": 1,
    ///   "paymentMethodId": 1,
    ///   "referenceNo": "REF-12345"
    /// }
    /// </example>
    [HttpPost("payment")]
    public async Task<IActionResult> MakePayment([FromBody] MakePaymentCommand command)
    {
        _logger.LogInformation("Payment request received - AccountId: {AccountId}, Amount: {Amount}, PaymentMethodId: {PaymentMethodId}, Reference: {ReferenceNo}",
            command.AccountId, command.Amount, command.PaymentMethodId, command.ReferenceNo);
        
        var response = await _mediator.Send(command);
        
        if (response.IsSuccess)
        {
            _logger.LogInformation("Payment processed successfully - AccountId: {AccountId}, Amount: {Amount}, Reference: {ReferenceNo}",
                command.AccountId, command.Amount, command.ReferenceNo);
        }
        else
        {
            _logger.LogWarning("Payment failed - AccountId: {AccountId}, Amount: {Amount}, Reason: {Message}",
                command.AccountId, command.Amount, response.Message);
        }
        
        return StatusCode(response.Status, response);
    }

    /// <summary>
    /// Creates a refund request that requires admin approval.
    /// </summary>
    /// <param name="command">The refund command containing amount, account ID, original payment reference number, and reason.</param>
    /// <returns>
    /// Returns 201 Created with refund request details if submitted successfully,
    /// or 400 Bad Request if validation fails or business rules are violated,
    /// or 404 Not Found if account or original payment doesn't exist.
    /// </returns>
    /// <response code="201">Refund request created successfully and is pending admin approval.</response>
    /// <response code="400">Validation failed or business rule violation.</response>
    /// <response code="404">Account or original payment not found.</response>
    /// <response code="401">Unauthorized - JWT token required.</response>
    /// <remarks>
    /// Business Rules:
    /// - Amount must be greater than zero
    /// - Original payment must exist with the provided reference number
    /// - Original transaction must be of type "Payment" (cannot refund a refund)
    /// - Refund amount cannot exceed original payment amount
    /// - Only one refund request allowed per payment reference
    /// - Reference number must belong to the same account
    /// - Reason is required (max 500 characters)
    /// - Refund request is created in "Pending" status
    /// - Admin approval is required before the actual refund is processed
    /// 
    /// Workflow:
    /// 1. User submits refund request
    /// 2. Request is validated and stored in "Pending" status
    /// 3. Admin reviews and approves/rejects the request
    /// 4. If approved, refund transaction is created and balance is updated
    /// 5. If rejected, no transaction is created
    /// </remarks>
    /// <example>
    /// POST /api/transaction/refund
    /// {
    ///   "amount": 200.00,
    ///   "accountId": 1,
    ///   "referenceNo": "REF-12345",
    ///   "reason": "Product was defective"
    /// }
    /// 
    /// This will create a refund request that requires admin approval.
    /// Once approved, a refund transaction with reference "REF-12345-REF" will be created.
    /// </example>
    [HttpPost("refund")]
    public async Task<IActionResult> MakeRefund([FromBody] MakeRefundCommand command)
    {
        _logger.LogInformation("Refund request received - AccountId: {AccountId}, Amount: {Amount}, Reference: {ReferenceNo}",
            command.AccountId, command.Amount, command.ReferenceNo);
        
        var response = await _mediator.Send(command);
        
        if (response.IsSuccess)
        {
            _logger.LogInformation("Refund request created successfully - AccountId: {AccountId}, Amount: {Amount}, Reference: {ReferenceNo}",
                command.AccountId, command.Amount, command.ReferenceNo);
        }
        else
        {
            _logger.LogWarning("Refund request failed - AccountId: {AccountId}, Amount: {Amount}, Reason: {Message}",
                command.AccountId, command.Amount, response.Message);
        }
        
        return StatusCode(response.Status, response);
    }
}
