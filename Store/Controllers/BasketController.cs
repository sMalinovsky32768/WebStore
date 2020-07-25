using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Store.Models;
using System;
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

        [Authorize(Roles = "admin, user")]
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
            return View(_context.Baskets.Include(b => b.Good)
                .Include(b => b.Good.Producer).Include(b => b.Good.GoodType).AsEnumerable()
                .Where(b => b.UserID == uid && !_context.GetIsPlaced(b.ID)));
        }

        [Authorize(Roles = "admin, user")]
        public IActionResult Add(int userid, int goodid, int count)
        {
            if (_context.Baskets.AsEnumerable().FirstOrDefault(
                b => b.GoodID == goodid && b.UserID == userid && !_context.GetIsPlaced(b.ID))
                is Basket basket)
            {
                basket.GoodCount += count;
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
        public IActionResult PlaceOrder()
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
                        Date = DateTime.Now,
                    });
                }
            }
            _context.SaveChanges();
            return RedirectToAction("Index", "Basket");
        }
    }
}
