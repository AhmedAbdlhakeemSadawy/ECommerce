using AutoMapper;
using ECommerceBuinessDTO;
using ECommerceBusinessAbstractions;
using ECommerceDataAccessDTO;
using ECommerceInfrastructureAbstraction;
using ECommerceWebApiDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommwerceWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private IOrderManager orderManager;
        private IMapper mapper;
        private IEmailService emailService;

        public OrderController(IOrderManager orderManager,IMapper mapper,IEmailService emailService)
        {
            this.orderManager = orderManager;
            this.mapper = mapper;
            this.emailService = emailService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddOrder([FromBody] OrderRequestDto orderRequestDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                 await emailService.SendEmailAsync("");
                OrderBusinessDTO orderBusinessDTO = mapper.Map<OrderBusinessDTO>(orderRequestDto);

                var result = await orderManager.CreateOrder(orderBusinessDTO);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return  StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("data")]
        [Authorize(Policy = "UserPolicy")]
        public IActionResult GetSecureData()
        {
            return Ok("This is protected data.");
        }

    }
}
