using AutoMapper;
using ECommerceBuinessDTO;
using ECommerceBusinessAbstractions;
using ECommerceDataAccessDTO;
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

        public OrderController(IOrderManager orderManager,IMapper mapper)
        {
            this.orderManager = orderManager;
            this.mapper = mapper;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddOrder([FromBody] OrderRequestDto orderRequestDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                OrderBusinessDTO orderBusinessDTO = mapper.Map<OrderBusinessDTO>(orderRequestDto);

                var result = await orderManager.CreateOrder(orderBusinessDTO);
                // return CreatedAtAction(nameof(GetOrder), new { id = result.OrderId }, result);

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

        [HttpGet("tesr_data")]
        public IActionResult GetSecureDataWithoutAuthenticate()
        {
            return Ok("This is protected data.");
        }
    }
}
