using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using OrderService.Exceptions;
using OrderService.Services.Dto;
using System.Net;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        private readonly OrdersService _orderService;

        public OrderController(OrdersService goodService, ILogger<OrderController> logger)
        {
            _logger = logger;
            _orderService = goodService;
        }

        [HttpGet(Name = "GetOrders")]
        [ProducesResponseType(typeof(IEnumerable<OrderDto>), (int)HttpStatusCode.OK)]
        public async Task<IEnumerable<OrderDto>> GetAsync()
        {
            return await _orderService.GetListAsync();
        }

        [HttpGet("{forDate:DateTime}", Name = "GetOrdersFordate")]
        [ProducesResponseType(typeof(IEnumerable<OrderDto>), (int)HttpStatusCode.OK)]
        public async Task<IEnumerable<OrderDto>> GetForDateAsync(DateTime forDate)
        {
            return await _orderService.GetListAsync(forDate);
        }

        [HttpGet("{ordernum:int}", Name = "GetOrdersByOrdernum")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(OrderDto), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<OrderDto>> GetByIdAsync(short ordernum)
        {
            if (ordernum <= 0)
            {
                return BadRequest();
            }

            var item = await _orderService.GetByIdAsync(ordernum);

            if (item != null)
            {
                return item;
            }

            return NotFound();
        }

        [HttpPost(Name = "CreateOrder")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(OrderDto), (int)HttpStatusCode.Created)]
        public async Task<ActionResult<OrderDto>> CreateAsync(OrderCreateDto order)
        {
            try
            {
                var newOrder = await _orderService.CreateAsync(order);
                if (newOrder != null)
                {
                    var url = Url.RouteUrl("GetOrdersByOrdernum", new { ordernum = newOrder.OrderId });
                    return Created(url, newOrder);
                }
            }
            catch (OrderCreateException ex)
            {
                return BadRequest(ex.Message);
            }

            return StatusCode(500);
        }

        [HttpPut("{ordernum:int}", Name = "UpdateOrder")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(OrderDto), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<OrderDto>> UpdateAsync(short ordernum, OrderDto order)
        {
            try
            {
                var updatedOrder = await _orderService.UpdateAsync(ordernum, order);
                if (updatedOrder != null)
                {
                    return updatedOrder;
                }
            }
            catch (OrderUpdateException ex)
            {
                return BadRequest(ex.Message);
            }

            return StatusCode(500);
        }

        [HttpPatch("{ordernum:int}")]
        public async Task<ActionResult<OrderDto>> PatchAsync(short ordernum, [FromBody] JsonPatchDocument<OrderDto> orderPatch)
        {
            if (orderPatch == null)
            {
                return BadRequest();
            }

            var order = await _orderService.GetByIdAsync(ordernum);

            if (order == null)
            {
                return NotFound();
            }

            orderPatch.ApplyTo(order);

            try
            {
                var updatedOrder = await _orderService.UpdateAsync(ordernum, order);

                return Ok(updatedOrder);
            }
            catch (OrderUpdateException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{ordernum:int}", Name = "DeleteOrder")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<OrderDto>> DeleteAsync(short ordernum)
        {
            try
            {
                await _orderService.DeleteAsync(ordernum);
                return Ok();
            }
            catch (OrderNotFoundException)
            {
                return NotFound();
            }
            catch (OrderUpdateException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}