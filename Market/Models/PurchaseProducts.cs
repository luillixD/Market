using Market.DTOs.Product;
using static Market.Models.Purchase;

namespace Market.Models
{
    public class PurchaseProducts
    {
        public int PurchaseId { get; set; }
        public Purchase Purchase { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
