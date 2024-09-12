using static Market.Models.Purchase;

namespace Market.DTOs.Purchase
{
    public class PurchaseResumeDto
    {
        public int Id { get; set; }
        public Delivery DeliveryType { get; set; }
        public PurchaseStatus Status { get; set; }
        public decimal Total { get; set; }
        public int TotalOfProducts { get; set; }
    }
}
