using System.ComponentModel.DataAnnotations;

namespace Market.DTOs.Product
{
    public class ProductsIdDto
    {
        [Required]
        public List<int> ProductsId { get; set; }
    }
}
