namespace Market.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public bool IsDeleted { get; set; } = false;
        public int SubcategoryId { get; set; }
        public Subcategory Subcategory { get; set; }
    }
}
