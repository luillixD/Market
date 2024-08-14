using System.ComponentModel.DataAnnotations;

namespace Market.DTOs.Category
{
    public class CreateCategoryDto
    {
        [Required]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; }
    }
}
