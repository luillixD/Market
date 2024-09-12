using static Mysqlx.Crud.Order.Types;

namespace Market.Models
{
    public class Purchase
    {
        public int Id { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
        public Delivery DeliveryType { get; set; }
        public PurchaseStatus Status { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int AddressId { get; set; }
        public Address Address { get; set; }
        public ICollection<PurchaseProducts> PurchaseProducts { get; set; } = new List<PurchaseProducts>();

        public enum Delivery
        {
            Local = 1,
            Express = 2
        }

        public enum PurchaseStatus
        {
            Pending = 1,
            Done = 2,
            Reject = 3,
            Accept = 4
        }
    }
}
