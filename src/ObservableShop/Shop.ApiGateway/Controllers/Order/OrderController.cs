using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shop.ApiGateway.Controllers.Order.CreateOrder;
using Shop.ApiGateway.Controllers.Order.CreateOrder.DataContracts;

namespace Shop.ApiGateway.Controllers.Order
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController(IMediator mediator) : Controller
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost]
        public async Task<ActionResult<OrderHandler.ResponseDto>> Post(OrderDto orderDto)
        {
            var response = await _mediator.Send(new OrderHandler.RequestDto(orderDto));
            return Ok(response);
        }
    }
}
