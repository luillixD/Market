using System.ComponentModel.DataAnnotations;

namespace Market.DTOs.Subcategory
{
    public class UpdateSubcategoryDto
    {
        [Required]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }
}
