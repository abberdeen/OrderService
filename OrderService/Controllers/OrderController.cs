using Microsoft.AspNetCore.Mvc;
using OrderService.Models;
using OrderService.Services;
using OrderService.Services.Data;

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

        [HttpGet(Name = "GetOrderss")]
        public async Task<IEnumerable<OrderDto>> GetAsync()
        {
            return await _orderService.GetListAsync();
        }

        [HttpGet("{ordernum:int}", Name = "GetOrdersByOrdernum")] 
        public async Task<ActionResult<OrderDto>> GetByIdAsync(short ordernum)
        {
            if (ordernum <= 0)
            {
                return BadRequest();
            }

            var item = await _orderService.GetByIdAsync(ordernum);

            if (item == null)
            {
                return NotFound();
            }

            return item;
        }
    }
}