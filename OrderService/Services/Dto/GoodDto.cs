using System.ComponentModel.DataAnnotations;

namespace OrderService.Services.Dto
{
    public class GoodDto
    { 
        public sbyte Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
