
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PaymentAPI.Application.Commands.Transactions;

namespace PaymentAPI.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly IMediator _mediator;
        public TransactionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // POST: /api/transaction/payment
        [HttpPost("payment")]
        public async Task<IActionResult> MakePayment([FromBody] MakePaymentCommand command)
        {
            var response = await _mediator.Send(command);
            return StatusCode(response.Status, response);
        }

        // POST: /api/transaction/refund
        [HttpPost("refund")]
        public async Task<IActionResult> MakeRefund([FromBody] MakeRefundCommand command)
        {
            var response = await _mediator.Send(command);
            return StatusCode(response.Status, response);
        }
    }
}
