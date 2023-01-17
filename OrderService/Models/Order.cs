using System.ComponentModel.DataAnnotations;

namespace OrderService.Models
{
    public class Order
    {
        [Key]
        public short Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public OrderStatusEnum OrderStatusId { get; set; }

        public virtual OrderStatus OrderStatus { get; set; }
        public virtual List<OrderItem> OrderItems { get; set; } 
    }
}
