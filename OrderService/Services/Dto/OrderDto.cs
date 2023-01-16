using OrderService.Models;

namespace OrderService.Services.Dto
{
    public class OrderDto
    { 
        public short OrderId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public OrderStatusEnum OrderStatus { get; set; }
        public List<OrderItemDto> OrderItems { get; set; }
    }
}
