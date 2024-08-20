using static Market.Models.Purchase;

namespace Market.Models
{
    public class Address
    {
        public int Id { get; set; }
        public string AdditionalData { get; set; }
        public decimal Latitud { get; set; }
        public decimal Longitud { get; set; }
        public Purchase Purchase { get; set; }

    }
}
