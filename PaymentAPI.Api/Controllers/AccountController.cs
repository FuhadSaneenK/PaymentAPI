using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PaymentAPI.Application.Commands.Accounts;
using PaymentAPI.Application.Queries.Transactions;

namespace PaymentAPI.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // POST: api/account
        [HttpPost]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountCommand command)
        {
            var response = await _mediator.Send(command);
            return StatusCode(response.Status, response);
        }

        // GET: api/account/{id}/transactions
        [HttpGet("{id}/transactions")]
        public async Task<IActionResult> GetTransactionsByAccountId(int id)
        {
            var query = new GetTransactionsByAccountIdQuery(id);
            var response = await _mediator.Send(query);
            return StatusCode(response.Status, response);
        }
    }
}
