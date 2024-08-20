using static Market.Models.Purchase;
using Market.DTOs.Product;
using Market.DTOs.Address;

namespace Market.DTOs.Purchase
{
    public class PurchaseDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public AddressDto AddressDto { get; set; }
        public string DeliveryType { get; set; }
        public Delivery DeliveryTypeCode { get; set; }
        public string Status { get; set; }
        public PurchaseStatus StatusCode { get; set; }
        public List<ProductDto> Products { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
    }
}
