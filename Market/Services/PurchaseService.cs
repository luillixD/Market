using Market.Data.Repositories.Interfaces;
using Market.DTOs.Bill;
using Market.DTOs.Product;
using Market.DTOs.Purchase;
using Market.Middleware;
using Market.Models;
using Market.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Market.Services
{
    public class PurchaseService : IPurchaseService
    {
        private readonly IProductRepository _productRepository;
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly ILogger<PurchaseService> _logger;
        private readonly IConfiguration _configuration;

        public PurchaseService(IPurchaseRepository purchaseRepository, IProductRepository productRepository, IConfiguration configuration, ILogger<PurchaseService> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _purchaseRepository = purchaseRepository ?? throw new ArgumentNullException(nameof(_purchaseRepository));
        }

        public async Task<bool> NewPurchase(List<int> productsId, Purchase purchase)
        {
            decimal total = 0;
            decimal subTotal = 0;

            var products = IsExistProducts(productsId);

            foreach (var product in products)
            {
                var purchaseProduct = new PurchaseProducts
                {
                    ProductId = product.Id,
                    Purchase = purchase
                };
                subTotal += product.Price;
                purchase.PurchaseProducts.Add(purchaseProduct);
            }

            var impuesto = (double)subTotal * 0.13;

            total = subTotal + (decimal)impuesto;

            purchase.Total = Math.Round(total, 2);
            purchase.SubTotal = Math.Round(subTotal, 2);

            var result = await _purchaseRepository.AddAsync(purchase);
            return result;
        }

        private List<Product> IsExistProducts(List<int> productsId)
        {
            List<Product> products = new List<Product>();
            foreach (var productId in productsId)
            {
                Product product = _productRepository.GetById(productId).Result;
                if (product == null) throw new Exception("Product not exist");
                products.Add(product);
            }
            return products;
        }

        public Task<TotalDto> TotalAmount(List<int> productsId)
        {
            decimal total = 0;
            decimal subTotal = 0;

            var products = IsExistProducts(productsId);

            foreach (var product in products)
            {
                if (product == null) throw new Exception("Product not exist");
                subTotal += product.Price;
            }
            var impuesto = (double)subTotal * 0.13;

            total = subTotal + (decimal)impuesto;

            return Task<TotalDto>.FromResult(new TotalDto { Subtotal = subTotal, Total = total });
        }

        public async Task<PurchaseDto> GetById(int id)
        {
            var purchase = await _purchaseRepository.GetByIdAsync(id);
            var address = new DTOs.Address.AddressDto { AditionalData = purchase.Address.AditionalData, Latitud = purchase.Address.Latitud, Longitud = purchase.Address.Longitud };

            List<ProductDto> products = new List<ProductDto>();
            foreach (var purchaseProduct in purchase.PurchaseProducts)
            {
                products.Add(new ProductDto { Id = purchaseProduct.Product.Id, Name = purchaseProduct.Product.Name, Price = purchaseProduct.Product.Price, ImageUrl = purchaseProduct.Product.ImageUrl, Categoria = purchaseProduct.Product.Subcategory.Name });
            }

            return new PurchaseDto { Id = purchase.Id, UserName = purchase.User.FullName, AddressDto = address, DeliveryType = purchase.DeliveryType, Status = purchase.Status, Products = products, SubTotal = purchase.SubTotal, Total = purchase.Total };
        }
    }
}
