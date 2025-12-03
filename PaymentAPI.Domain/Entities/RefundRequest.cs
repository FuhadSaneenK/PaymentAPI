namespace PaymentAPI.Domain.Entities;

/// <summary>
/// Represents a refund request that requires admin approval.
/// </summary>
/// <remarks>
/// Refund requests go through an approval workflow where they remain in "Pending" status
/// until an admin user approves or rejects them. Only approved requests result in actual refund transactions.
/// </remarks>
public class RefundRequest
{
    /// <summary>
    /// Gets or sets the unique identifier for the refund request.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the refund amount requested.
    /// </summary>
    /// <remarks>
    /// Must not exceed the original payment amount.
    /// </remarks>
    public decimal Amount { get; set; }

    /// <summary>
    /// Gets or sets the account ID requesting the refund.
    /// </summary>
    public int AccountId { get; set; }

    /// <summary>
    /// Gets or sets the reference number of the original payment transaction.
    /// </summary>
    /// <remarks>
    /// Used to locate and validate the original payment that is being refunded.
    /// </remarks>
    public string OriginalPaymentReference { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the reason for the refund request.
    /// </summary>
    public string Reason { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the status of the refund request.
    /// </summary>
    /// <remarks>
    /// Possible values: "Pending", "Approved", "Rejected". Default is "Pending".
    /// </remarks>
    public string Status { get; set; } = "Pending";

    /// <summary>
    /// Gets or sets the date when the refund request was created.
    /// </summary>
    public DateTime RequestDate { get; set; }

    /// <summary>
    /// Gets or sets the date when the refund request was reviewed by admin.
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

    /// <summary>
    /// Gets or sets the ID of the refund transaction created after approval.
    /// </summary>
    /// <remarks>
    /// Null until the request is approved. Links to the actual Transaction entity.
    /// </remarks>
    public int? RefundTransactionId { get; set; }

    // Navigation properties
    
    /// <summary>
    /// Gets or sets the account associated with this refund request.
    /// </summary>
    /// <remarks>
    /// Navigation property.
    /// </remarks>
    public Account Account { get; set; } = null!;
    
    /// <summary>
    /// Gets or sets the admin user who reviewed the request.
    /// </summary>
    /// <remarks>
    /// Navigation property. Null until reviewed.
    /// </remarks>
    public User? ReviewedByUser { get; set; }
    
    /// <summary>
    /// Gets or sets the refund transaction created after approval.
    /// </summary>
    /// <remarks>
    /// Navigation property. Null until approved.
    /// </remarks>
    public Transaction? RefundTransaction { get; set; }
}
