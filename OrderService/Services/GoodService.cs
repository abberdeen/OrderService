using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Services.Dto;

namespace OrderService.Services
{
    public class GoodService
    {
        private readonly OrdersContext _ordersContext;
        public GoodService(OrdersContext ordersContext)
        {
            _ordersContext = ordersContext;
        }

        public async Task<List<GoodDto>> GetListAsync()
        {
            return await _ordersContext.Products
                .Select(x => new GoodDto()
                {
                    Id = x.Id,
                    Price = x.Price,
                    Title = x.Title
                })
                .ToListAsync();
        }

        public async Task<GoodDto?> GetByIdAsync(sbyte id)
        {
            var product = await _ordersContext.Products.FirstOrDefaultAsync(x => x.Id == id);

            if (product == null)
            {
                return null;
            }

            return new GoodDto()
            {
                Id = product.Id,
                Price = product.Price,
                Title = product.Title
            };
        }
    }
}
