using Market.DTOs.Subcategory;

namespace Market.DTOs.Category
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<SubcategoryDto> Subcategories { get; set; }
    }
}
