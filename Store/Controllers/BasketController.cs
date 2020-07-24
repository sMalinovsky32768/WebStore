using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Store.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Store.Controllers
{
    public class BasketController : Controller
    {
        private readonly ApplicationContext _context;

        public BasketController(ApplicationContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewData["Role"] = User.FindFirst(x => x.Type == ClaimsIdentity.DefaultRoleClaimType).Value;
            string email = User.FindFirst(x => x.Type == ClaimsIdentity.DefaultNameClaimType).Value;
            int uid = _context.Users.FirstOrDefault(u => u.Email == email).ID;
            ViewData["UserID"] = uid;
            ViewData["BasketString"] = _context.BasketString(uid);
            var deliveryMethods = _context.DeliveryMethods.ToList();
            var selectListItems = new List<SelectListItem>();
            foreach(var item in deliveryMethods)
            {
                selectListItems.Add(new SelectListItem
                {
                    Value = item.ID.ToString(),
                    Text = item.Name,
                });
            }
            ViewData["DeviveryMethods"] = selectListItems;
            var baskets = _context.Baskets.Include(b => b.Good.GoodType).Include(b => b.Good.Producer).Where(b => b.UserID == uid);
            var basketsList = new List<Basket>();
            foreach (var item in baskets)
            {
                if (!_context.GetIsPlaced(item.ID))
                {
                    basketsList.Add(item);
                }
            }
            return View(basketsList);
        }

        [Authorize(Roles = "admin, user")]
        public IActionResult Add(int userid, int goodid, int count)
        {
            var baskets = _context.Baskets.Where(
                b => b.GoodID == goodid && b.UserID == userid);
            if (baskets.Count() > 0)
            {
                foreach (var item in baskets)
                {
                    if (!_context.GetIsPlaced(item.ID))
                    {
                        item.GoodCount += count;
                        break;
                    }
                }
            }
            else
            {
                _context.Baskets.Add(new Basket
                {
                    UserID = userid,
                    GoodID = goodid,
                    GoodCount = count,
                });
            }
            _context.SaveChanges();
            return RedirectToAction("Index", "Goods");
        }

        public IActionResult PlusCount(int id)
        {
            if (_context.Baskets.FirstOrDefault(b => b.ID == id) is Basket basket)
            {
                basket.GoodCount++;
                _context.SaveChanges();
            }
            return RedirectToAction("Index", "Basket");
        }

        public IActionResult MinusCount(int id)
        {
            if (_context.Baskets.FirstOrDefault(b => b.ID == id) is Basket basket)
            {
                if (basket.GoodCount > 1)
                {
                    basket.GoodCount--;
                    _context.SaveChanges();
                }
                else return Delete(id);
            }
            return RedirectToAction("Index", "Basket");
        }

        public IActionResult Delete(int id)
        {
            if (_context.Baskets.FirstOrDefault(b => b.ID == id) is Basket basket)
            {
                _context.Baskets.Remove(basket);
                _context.SaveChanges();
            }
            return RedirectToAction("Index", "Basket");
        }

        [HttpPost]
        public IActionResult PlaceOrder()//[Bind("isSelected,ID,Good,GoodID,GoodCount,User,UserID")] IEnumerable<Basket> baskets
        {
            string email = User.FindFirst(x => x.Type == ClaimsIdentity.DefaultNameClaimType).Value;
            int uid = _context.Users.FirstOrDefault(u => u.Email == email).ID;
            int methodId = int.Parse(Request.Form["deliveryMethod"]);
            var method = _context.DeliveryMethods.FirstOrDefault(d => d.ID == methodId);
            foreach (var item in Request.Form.Keys)
            {
                if (int.TryParse(item, out int id))
                {
                    var basket = _context.Baskets.FirstOrDefault(b => b.ID == id);
                    _context.Orders.Add(new Order
                    {
                        BasketID = id,
                        Basket = basket,
                        DeliveryMethod = method,
                        DeliveryMethodID = methodId,
                    });
                }
            }
            _context.SaveChanges();
            return RedirectToAction("Index", "Basket");
        }
    }
}
