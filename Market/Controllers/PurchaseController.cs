using AutoMapper;
using Market.DTOs.Bill;
using Market.DTOs.Product;
using Market.DTOs.Purchase;
using Market.DTOs.Users;
using Market.Models;
using Market.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Utilities;
using System.Security.Claims;
using static Mysqlx.Crud.Order.Types;

namespace Market.Controllers
{
    [ApiController]
    public class PurchaseController : Controller
    {
        private readonly IPurchaseService _service;
        private readonly ILogger<PurchaseController> _logger;
        private readonly IMapper _mapper;
        public PurchaseController(IPurchaseService service, ILogger<PurchaseController> logger, IMapper mapper)
        {
            _service = service;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpPost("/api/[controller]")]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreatePurchaseDto createPurchaseDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (createPurchaseDto.ProductsIds.Count == 0) return BadRequest("Products are required");
            if (createPurchaseDto.DeliveryType == Purchase.Delivery.Express)
            {
                if (string.IsNullOrWhiteSpace(createPurchaseDto.AdditionalData)) return BadRequest("Additional Data of address is required");
            }
            var purchase = _mapper.Map<Purchase>(createPurchaseDto);
            purchase.UserId = userId;

            var result = await _service.NewPurchase(createPurchaseDto.ProductsIds, purchase);

            return Ok(result);
        }

        [HttpGet("/api/[controller]/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0) return BadRequest("Invalid purchase");
            var result = await _service.GetById(id);

            return Ok(result);
        }

        [HttpGet("/api/[controller]/own/{id}")]
        [Authorize]
        public async Task<IActionResult> GetByIdAndUser(int id)
        {
            if (id <= 0) return BadRequest("Invalid purchase");
            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _service.GetByIdAndUser(id, userId);

            return Ok(result);
        }

        [HttpPut("/api/[controller]/reject/{purchaseId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Reject(int purchaseId)
        {
            await _service.RejectPurchase(purchaseId);

            return Ok();
        }

        [HttpPut("/api/[controller]/accept/{purchaseId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Accept(int purchaseId)
        {
            await _service.AcceptPurchase(purchaseId);
            return Ok();
        }

        [HttpPut("/api/[controller]/done/{purchaseId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Done(int purchaseId)
        {
            await _service.DonePurchase(purchaseId);

            return Ok();
        }

        [HttpGet("/api/[controller]/resume")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<PurchaseResumeDto>>> GetAllPurchasesByPlayer([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] int userId = 0)
        {
            if(userId <= 0) return BadRequest("User id is required");
            var result = await _service.PurchaseResume(userId, page, pageSize);

            return Ok(result);
        }
        
        [HttpGet("/api/[controller]/resume/own")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllMyPurchases([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _service.PurchaseResume(userId, page, pageSize);
            return Ok(result);
        }

        [HttpPost("/api/[controller]/amount")]
        [Authorize]
        public async Task<IActionResult> Amount([FromBody] ProductsIdDto products)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

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
