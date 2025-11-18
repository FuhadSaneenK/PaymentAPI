using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PaymentAPI.Application.Commands.Merchants;
using PaymentAPI.Application.Queries.Accounts;
using PaymentAPI.Application.Queries.Merchants;

namespace PaymentAPI.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MerchantController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MerchantController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // POST: api/merchant
        [HttpPost]
        public async Task<IActionResult> CreateMerchant([FromBody] CreateMerchantCommand command)
        {
            var response = await _mediator.Send(command);
            return StatusCode(response.Status, response);
        }

        // GET: api/merchant/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMerchantById(int id)
        {
            var query = new GetMerchantByIdQuery(id);
            var response = await _mediator.Send(query);

            return StatusCode(response.Status, response);
        }

        // GET: api/merchant/{id}/accounts
        [HttpGet("{id}/accounts")]
        public async Task<IActionResult> GetAccountsByMerchantId(int id)
        {
            var query = new GetAccountsByMerchantIdQuery(id);
            var response = await _mediator.Send(query);

            return StatusCode(response.Status, response);
        }
    }
}
