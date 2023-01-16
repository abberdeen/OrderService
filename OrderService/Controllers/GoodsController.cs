using Microsoft.AspNetCore.Mvc;
using OrderService.Services; 
using OrderService.Services.Dto;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GoodsController : ControllerBase
    {

        private readonly ILogger<GoodsController> _logger;
        private readonly GoodService _goodService;

        public GoodsController(GoodService goodService, ILogger<GoodsController> logger)
        {
            _logger = logger;
            _goodService = goodService;
        }

        [HttpGet(Name = "GetGoods")]
        public async Task<IEnumerable<GoodDto>> GetAsync()
        {
            return await _goodService.GetListAsync();
        }

        [HttpGet("{article:int}", Name = "GetGoodByArticle")] 
        public async Task<ActionResult<GoodDto>> GetByIdAsync(sbyte article)
        {
            if (article <= 0)
            {
                return BadRequest();
            }

            var item = await _goodService.GetByIdAsync(article);

            if (item == null)
            {
                return NotFound();
            }

            return item;
        }
    }
}