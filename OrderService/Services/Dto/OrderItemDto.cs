namespace OrderService.Services.Dto
{
    public class OrderItemDto
    {
        public int RowId { get; set; }
        public sbyte ProductId { get; set; }
        public byte Count { get; set; }
    }
}
