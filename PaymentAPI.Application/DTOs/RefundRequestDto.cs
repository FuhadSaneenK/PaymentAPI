namespace PaymentAPI.Application.DTOs;

/// <summary>
/// Data Transfer Object for RefundRequest.
/// </summary>
/// <remarks>
/// Used for transferring refund request data between layers without exposing domain entities.
/// Includes approval workflow status and admin review information.
/// </remarks>
/// <example>
/// <code>
/// {
///   "id": 1,
///   "amount": 150.00,
///   "accountId": 1,
///   "originalPaymentReference": "PAY-2024-12-001",
///   "reason": "Product defect",
///   "status": "Pending",
///   "requestDate": "2024-12-10T14:30:00Z",
///   "reviewDate": null,
///   "reviewedByUserId": null,
///   "adminComments": null
/// }
/// </code>
/// </example>
public class RefundRequestDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the refund request.
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the refund amount requested.
    /// </summary>
    public decimal Amount { get; set; }
    
    /// <summary>
    /// Gets or sets the account ID requesting the refund.
    /// </summary>
    public int AccountId { get; set; }
    
    /// <summary>
    /// Gets or sets the reference number of the original payment transaction.
    /// </summary>
    public string OriginalPaymentReference { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the reason for the refund request.
    /// </summary>
    public string Reason { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the status of the refund request.
    /// </summary>
    /// <remarks>
    /// Possible values: "Pending", "Approved", "Rejected".
    /// </remarks>
    public string Status { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the date when the refund request was created.
    /// </summary>
    public DateTime RequestDate { get; set; }
    
    /// <summary>
    /// Gets or sets the date when the refund request was reviewed.
    /// </summary>
    public DateTime? ReviewDate { get; set; }
    
    /// <summary>
    /// Gets or sets the ID of the admin user who reviewed the request.
    /// </summary>
    public int? ReviewedByUserId { get; set; }
    
    /// <summary>
    /// Gets or sets admin comments for approval/rejection.
    /// </summary>
    public string? AdminComments { get; set; }
}
