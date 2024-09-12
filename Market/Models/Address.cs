using static Market.Models.Purchase;

namespace Market.Models
{
    public class Address
    {
        public int Id { get; set; }
        public string AdditionalData { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public Purchase Purchase { get; set; }

    }
}
