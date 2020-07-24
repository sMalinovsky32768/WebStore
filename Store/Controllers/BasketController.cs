using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Bson;
using Store.Models;

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
            var baskets = _context.Baskets.Where(b => b.UserID == uid);
            var basketsList = new List<Basket>();
            foreach (var item in baskets)
            {
                if (!item.IsPlaced)
                {
                    basketsList.Add(item);
                }
            }
            //return View(_context.Baskets.Where(b => b.UserID == uid && !b.IsPlaced));
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
                    if (!item.IsPlaced)
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
                basket.GoodCount--;
                _context.SaveChanges();
            }
            return RedirectToAction("Index", "Basket");
        }
    }
}
