using System.ComponentModel.DataAnnotations;
using static Market.Models.Purchase;

namespace Market.DTOs.Bill
{
    public class CreatePurchaseDto
    {
        [StringLength(60, ErrorMessage = "Aditional data cannot exceed 60 characters")]
        public string AdditionalData { get; set; }
        public decimal Latitud { get; set; }
        public decimal Longitud { get; set; }
        [Required]
        public Delivery DeliveryType { get; set; }
        [Required]
        public List<int> ProductsIds { get; set; } 
    }
}
