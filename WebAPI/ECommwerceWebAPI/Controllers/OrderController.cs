using ECommerceBuinessDTO;
using ECommerceBusinessAbstractions;
using ECommerceDataAccessDTO;
using ECommerceInfrastructureAbstraction;
using ECommerceWebApiDto;
using ECommwerceWebAPI.Mapping;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApiAbstraction;

namespace ECommwerceWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private IOrderManager orderManager;
        private IEmailService emailService;

        public OrderController(IOrderManager orderManager,IEmailService emailService)
        {
            this.orderManager = orderManager;
            this.emailService = emailService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddOrder([FromBody] OrderRequestDto orderRequestDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

           
                string? customerEmail = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;

                OrderBusinessDTO orderBusinessDTO = orderRequestDto.ToBusinessDto();
                orderBusinessDTO.CustomerEmail = customerEmail;

                var result = await orderManager.CreateOrder(orderBusinessDTO);

                return Ok(result);
            
  
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetOrders()
        {
            var result = await orderManager.GetAllOrders();
            return Ok(result);
        }

        [HttpGet("data")]
        [Authorize(Policy = "UserPolicy")]
        public IActionResult GetSecureData()
        {
            return Ok("This is protected data.");
        }

    }
}
