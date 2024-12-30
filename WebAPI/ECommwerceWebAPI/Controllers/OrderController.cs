using ECommerceBusinessAbstractions;
using Microsoft.AspNetCore.Mvc;

namespace ECommwerceWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : Controller
    {
        IOrderManager orderManager;
        public OrderController(IOrderManager orderManager)
        {
            this.orderManager = orderManager;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
