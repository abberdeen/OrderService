using System.ComponentModel.DataAnnotations;

namespace OrderService.Models
{
    public class OrderStatus
    {
        [Key]
        public byte Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
