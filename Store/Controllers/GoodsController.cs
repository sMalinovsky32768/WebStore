using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Store.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using Microsoft.AspNetCore.Authorization;

namespace Store.Controllers
{
    public class GoodsController : Controller
    {
        private readonly ApplicationContext _context;

        public GoodsController(ApplicationContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "admin, user")]
        public async Task<IActionResult> Index()
        {
            ViewData["Role"] = User.FindFirst(x => x.Type == ClaimsIdentity.DefaultRoleClaimType).Value;
            string email = User.FindFirst(x => x.Type == ClaimsIdentity.DefaultNameClaimType).Value;
            int uid = _context.Users.FirstOrDefault(u => u.Email == email).ID;
            ViewData["UserID"] = uid;
            ViewData["BasketString"] = _context.BasketString(uid);
            return View(await _context.Goods.Include(g => g.GoodType).Include(g => g.Producer).ToListAsync());
        }

        [Authorize(Roles = "admin")]
        public IActionResult Import(IFormFile importFile)
        {
            if (importFile != null)
            {
                try
                {
                    XElement element = XElement.Load(importFile.OpenReadStream());
                    var collection = element.Descendants("Good").Select(
                        item => new Good
                        {
                            Name = (string)item.Attribute("Name"),
                            Articul = (string)item.Attribute("Articul"),
                            Value = CBConverter.Convert((decimal)item.Attribute("Value"),
                            (string)item.Attribute("Currency") ?? "RUB", "RUB"),
                            Producer = new Producer
                            {
                                Name = (string)item.Element("Producer").Attribute("Name"),
                                Code = (string)item.Element("Producer").Attribute("Code"),
                            },
                            GoodType = new GoodType
                            {
                                Name = (string)item.Element("GoodType").Attribute("Name"),
                                Code = (string)item.Element("GoodType").Attribute("Code"),
                            },
                        });
                    foreach(var item in collection)
                    {
                        var producer = _context.Producers.FirstOrDefault(
                            p => p.Name == item.Producer.Name && p.Code == item.Producer.Code);
                        var goodType = _context.Producers.FirstOrDefault(
                            t => t.Name == item.GoodType.Name && t.Code == item.GoodType.Code);
                        if (producer is null)
                        {
                            _context.Producers.Add(item.Producer);
                            _context.SaveChanges();
                        }
                        if (goodType is null)
                        {
                            _context.GoodTypes.Add(item.GoodType);
                            _context.SaveChanges();
                        }
                        item.ProducerID = _context.Producers.FirstOrDefault(
                            p => p.Name == item.Producer.Name && p.Code == item.Producer.Code).ID;
                        item.GoodTypeID = _context.GoodTypes.FirstOrDefault(
                            t => t.Name == item.GoodType.Name && t.Code == item.GoodType.Code).ID;
                        _context.Goods.Add(item);
                        _context.SaveChanges();
                    }
                }
                catch
                {
                    return StatusCode(505);
                }
            }
            return RedirectToAction("Index", "Goods");
        }
    }
}
