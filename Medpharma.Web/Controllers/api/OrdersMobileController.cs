using Medpharma.Mobile.Models;
using Medpharma.Web.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Schema;
using System.Linq;

namespace Medpharma.Web.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersMobileController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;

        public OrdersMobileController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [HttpGet]
        [Route("getUserOrders")]
        public IActionResult GetUserOrders(AuthIdentity model)
        {
            var order = _orderRepository.GetAll().Include("Items.Product").Where(o => o.Customer.Email == model.Email);

            return Ok(order);
        }

        [HttpGet]
        [Route("getUserOrderDetails/{id}")]
        public IActionResult GetUserOrderDetails(int? id)
        {
            var app = _orderRepository.GetAll()
                 .Include(o => o.Items)
                 .Where(o => o.Id == id.Value);

            return Ok(app);
        }
    }
}
