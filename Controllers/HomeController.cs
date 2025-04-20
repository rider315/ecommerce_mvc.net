using Microsoft.AspNetCore.Mvc;
using EcommerceApp.Data;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using EcommerceApp.Models;
using System.Collections.Generic;

namespace EcommerceApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = _userManager.GetUserId(User);
                var cartCount = _context.Carts
                    .Where(c => c.UserId == userId)
                    .Sum(c => c.Quantity);
                ViewData["CartCount"] = cartCount;
            }

            var model = new HomeViewModel
            {
                FeaturedProducts = _context.Products.Take(4).ToList(),
                Categories = new List<Category>
                {
                    new Category { Name = "Laptops", ImageUrl = "https://images.unsplash.com/photo-1496181133206-80ce9b88a853?w=600&auto=format&fit=crop&q=60", Link = "/Products" },
                    new Category { Name = "Smartphones", ImageUrl = "https://images.unsplash.com/photo-1598965402089-897ce52e8355?w=600&auto=format&fit=crop&q=60", Link = "/Products" },
                    new Category { Name = "Accessories", ImageUrl = "https://plus.unsplash.com/premium_photo-1679513691474-73102089c117?w=600&auto=format&fit=crop&q=60", Link = "/Products" }
                }
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Subscribe(string email)
        {
            if (!string.IsNullOrEmpty(email))
            {
                var subscription = new Newsletter { Email = email };
                _context.Newsletters.Add(subscription);
                _context.SaveChanges();
                TempData["Message"] = "Thank you for subscribing!";
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}