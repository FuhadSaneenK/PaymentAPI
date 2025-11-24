using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace PaymentAPI.Api.Controllers;

/// <summary>
/// Base API controller providing common functionality for all controllers.
/// </summary>
/// <remarks>
/// This abstract controller provides MediatR integration and common API configurations.
/// All API controllers should inherit from this base class.
/// </remarks>
[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    /// <summary>
    /// MediatR instance for sending commands and queries.
    /// </summary>
    protected readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseApiController"/> class.
    /// </summary>
    /// <param name="mediator">The MediatR instance for CQRS operations.</param>
    protected BaseApiController(IMediator mediator)
    {
        _mediator = mediator;
    }
}
