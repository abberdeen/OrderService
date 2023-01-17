using OrderService.Models;

namespace OrderService.Services.Dto
{
    public class OrderCreateDto
    {
        public string FullName { get; set; } = string.Empty;
        public OrderStatusEnum OrderStatus { get; set; }
        public List<OrderCreateItemDto> OrderItems { get; set; }
    }
}