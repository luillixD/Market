using Market.Validation;
using System.ComponentModel.DataAnnotations;

namespace Market.DTOs.Product
{
    public class UpdateProductDto
    {
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero")]
        public decimal? Price { get; set; }

        [AllowedExtensions(new[] { ".jpg", ".jpeg" })]
        [MaxFileSize(100 * 1024)] // 100 KB
        public IFormFile ImageFile { get; set; }

        public int? SubcategoryId { get; set; }

        public bool IsDeleted { get; set; }
    }

}
