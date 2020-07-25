using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Store.Models;
using System.Linq;
using System.Security.Claims;

namespace Store.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationContext _context;

        public OrdersController(ApplicationContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "admin, user")]
        public IActionResult Index()
        {
            string email = User.FindFirst(x => x.Type == ClaimsIdentity.DefaultNameClaimType).Value;
            int uid = _context.Users.FirstOrDefault(u => u.Email == email).ID;
            ViewData["BasketString"] = _context.BasketString(uid);
            return View(_context.Orders.Include(o => o.Basket).Include(o => o.DeliveryMethod)
                .Include(o => o.Basket.Good).Include(o => o.Basket.Good.Producer)
                .Include(o => o.Basket.Good.GoodType).Where(o => o.Basket.UserID == uid));
        }
    }
}
