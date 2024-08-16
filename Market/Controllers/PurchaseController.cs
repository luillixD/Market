using Market.DTOs.Bill;
using Market.DTOs.Product;
using Market.Models;
using Market.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static Mysqlx.Crud.Order.Types;

namespace Market.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchaseController : Controller
    {
        private readonly IPurchaseService _service;
        private readonly ILogger<PurchaseController> _logger;
        public PurchaseController(IPurchaseService service, ILogger<PurchaseController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreatePurchaseDto categoryDto)
        {
            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var address = new Address
            {
                AditionalData = categoryDto.AditionalData,
                Latitud = categoryDto.Latitud,
                Longitud = categoryDto.Longitud
            };

            var purchase = new Purchase
            {
                SubTotal = 0,
                Total = 0,
                UserId = userId,
                Address = address,
                Status = Purchase.PurchaseStatus.Pending,
                DeliveryType = categoryDto.DeliveryType
            };

            var result = await _service.NewPurchase(categoryDto.ProductsIds, purchase);

            return Ok(result);
        }

        [HttpGet("/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetById(id);

            return Ok(result);
        }

        [HttpPost("/amount")]
        public async Task<IActionResult> Amount([FromBody] ProductsIdDto products)
        {
            var requets = products.ProductsId;
            if (requets.Count == 0)
            {
                return Ok(new TotalDto { Subtotal = 0, Total = 0 });
            }
            var result = _service.TotalAmount(requets);

            return Ok(result);
        }
    }
}
