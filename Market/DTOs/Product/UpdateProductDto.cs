namespace Market.DTOs.Product
{
    public class UpdateProductDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal? Price { get; set; }
        public IFormFile ImageFile { get; set; }
        public int? SubcategoryId { get; set; }
    }

}
