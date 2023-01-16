using System.ComponentModel.DataAnnotations;

namespace OrderService.Models
{
    public class OrderItem
    {
        [Key]
        public int RowId { get; set; }  
        public short OrderId { get; set; }
        public sbyte ProductId { get; set; }
        public byte Count { get; set; }

        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
    }
}
