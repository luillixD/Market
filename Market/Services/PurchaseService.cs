using AutoMapper;
using Market.Data.Repositories.Interfaces;
using Market.DTOs.Bill;
using Market.DTOs.Product;
using Market.DTOs.Purchase;
using Market.DTOs.Users;
using Market.Middleware;
using Market.Migrations;
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
        private readonly IMapper _mapper;

        public PurchaseService(IPurchaseRepository purchaseRepository, IProductRepository productRepository, IConfiguration configuration, ILogger<PurchaseService> logger, IMapper mapper)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _purchaseRepository = purchaseRepository ?? throw new ArgumentNullException(nameof(_purchaseRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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

            if(purchase.DeliveryType == Purchase.Delivery.Express)
            {
                if(string.IsNullOrWhiteSpace(purchase.Address.AdditionalData)) throw new Exception("Additional Data of address is required");
            }
            else if(purchase.DeliveryType == Purchase.Delivery.Local)
            {
                purchase.Address.AdditionalData = "N/A";
                purchase.Address.Latitud = 0;
                purchase.Address.Longitud = 0;
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
            if (purchase == null) throw new Exception("Purchase not found");

            var products = await GetProducts(purchase.PurchaseProducts.ToList());

            PurchaseDto purchaseReponse = _mapper.Map<PurchaseDto>(purchase);
            if (purchase.DeliveryType == Purchase.Delivery.Express)
            {
                var address = new DTOs.Address.AddressDto { AditionalData = purchase.Address.AdditionalData, Latitud = purchase.Address.Latitud, Longitud = purchase.Address.Longitud };
                purchaseReponse.AddressDto = address;
            }

            purchaseReponse.Status = await StatusType(purchase.Status);
            purchaseReponse.DeliveryType = await DeliveryType(purchase.DeliveryType);

            purchaseReponse.DeliveryTypeCode = purchase.DeliveryType;
            purchaseReponse.StatusCode = purchase.Status;

            purchaseReponse.Products = products;

            return purchaseReponse;
        }

        public async Task<PurchaseDto> GetByIdAndUser(int id, int userId)
        {
            var purchase = await _purchaseRepository.GetByIdAsync(id);
            if (purchase == null) throw new Exception("Purchase not found");
            if (purchase.UserId != userId) throw new Exception("Purchase not found");

            var products = await GetProducts(purchase.PurchaseProducts.ToList());

            PurchaseDto purchaseReponse = _mapper.Map<PurchaseDto>(purchase);
            if (purchase.DeliveryType == Purchase.Delivery.Express)
            {
                var address = new DTOs.Address.AddressDto { AditionalData = purchase.Address.AdditionalData, Latitud = purchase.Address.Latitud, Longitud = purchase.Address.Longitud };
                purchaseReponse.AddressDto = address;
            }

            purchaseReponse.Status = await StatusType(purchase.Status);
            purchaseReponse.DeliveryType = await DeliveryType(purchase.DeliveryType);

            purchaseReponse.DeliveryTypeCode = purchase.DeliveryType;
            purchaseReponse.StatusCode = purchase.Status;

            purchaseReponse.Products = products;

            return purchaseReponse;
        }

        private async Task<string> DeliveryType (Purchase.Delivery delivery)
        {
            if (delivery == Purchase.Delivery.Express) return "Express";
            return "Local";
        }

        private async Task<string> StatusType(Purchase.PurchaseStatus status)
        {
            if (status == Purchase.PurchaseStatus.Pending) return "Pending";
            else if (status == Purchase.PurchaseStatus.Accept) return "Accept";
            else if (status == Purchase.PurchaseStatus.Done) return "Done";
            else if (status == Purchase.PurchaseStatus.Reject) return "Reject";
            return "Pending";
        }

        private async Task<List<ProductDto>> GetProducts(List<PurchaseProducts> purchaseProducts)
        {
            List<ProductDto> products = new List<ProductDto>();
            foreach (var purchaseProduct in purchaseProducts)
            {
                products.Add(new ProductDto { Id = purchaseProduct.Product.Id, Name = purchaseProduct.Product.Name, Price = purchaseProduct.Product.Price, ImageUrl = purchaseProduct.Product.ImageUrl, Categoria = purchaseProduct.Product.Subcategory.Name });
            }
            return products;
        }

        public async Task<bool> RejectPurchase(int id)
        {
            var purchase = await _purchaseRepository.GetByIdAsync(id);
            if (purchase == null) throw new Exception("Purchase not found");
            purchase.Status = Purchase.PurchaseStatus.Reject;
            var result = await _purchaseRepository.UpdateAsync(purchase);
            if (!result) throw new Exception("Error updating purchase");
            return true;
        }

        public async Task<bool> AcceptPurchase(int id)
        {
            var purchase = await _purchaseRepository.GetByIdAsync(id);
            if (purchase == null) throw new Exception("Purchase not found");
            purchase.Status = Purchase.PurchaseStatus.Accept;
            var result = await _purchaseRepository.UpdateAsync(purchase);
            if (!result) throw new Exception("Error updating purchase");
            return true;
        }

        public async Task<bool> DonePurchase(int id)
        {
            var purchase = await _purchaseRepository.GetByIdAsync(id);
            if (purchase == null) throw new Exception("Purchase not found");
            purchase.Status = Purchase.PurchaseStatus.Done;
            var result = await _purchaseRepository.UpdateAsync(purchase);
            if (!result) throw new Exception("Error updating purchase");
            return true;
        }

        public async Task<List<PurchaseResumeDto>> PurchaseResume(int userId, int page, int pageSize)
        {
            var result = await _purchaseRepository.GetPurchasesResumeAsync(userId, page, pageSize);
            List<PurchaseResumeDto> purchases = new List<PurchaseResumeDto>();
            foreach (var purchase in result)
            {
                purchases.Add(new PurchaseResumeDto { Id = purchase.Id, Status = purchase.Status, Total = purchase.Total, TotalOfProducts = purchase.PurchaseProducts.Count });
            }
            return purchases;
        }
    }
}
