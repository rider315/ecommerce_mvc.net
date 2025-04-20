using Microsoft.AspNetCore.Mvc;
using EcommerceApp.Data;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using EcommerceApp.Models;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;

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

            // Get recently viewed products from session
            var recentlyViewedIds = HttpContext.Session.GetString("RecentlyViewed")?.Split(',').Select(int.Parse).ToList() ?? new List<int>();
            var recentlyViewedProducts = _context.Products
                .Where(p => recentlyViewedIds.Contains(p.Id))
                .OrderBy(p => p.Name) // Added to fix EF warning
                .Take(4)
                .ToList();

            var model = new HomeViewModel
            {
                FeaturedProducts = _context.Products.Take(4).ToList(),
                Categories = new List<Category>
                {
                    new Category { Name = "Laptops", ImageUrl = "https://images.unsplash.com/photo-1496181133206-80ce9b88a853?w=600&auto=format&fit=crop&q=60", Link = "/Products" },
                    new Category { Name = "Smartphones", ImageUrl = "https://images.unsplash.com/photo-1598965402089-897ce52e8355?w=600&auto=format&fit=crop&q=60", Link = "/Products" },
                    new Category { Name = "Accessories", ImageUrl = "https://plus.unsplash.com/premium_photo-1679513691474-73102089c117?w=600&auto=format&fit=crop&q=60", Link = "/Products" }
                },
                Promotions = new List<Promotion>
                {
                    new Promotion { Title = "20% Off Electronics", Description = "Shop now and save big!", ImageUrl = "https://images.unsplash.com/photo-1557825835-70d97c73318b?w=600&auto=format&fit=crop&q=60", Link = "/Products" },
                    new Promotion { Title = "Free Shipping", Description = "On orders over $50!", ImageUrl = "https://images.unsplash.com/photo-1583394838336-acd977736f90?w=600&auto=format&fit=crop&q=60", Link = "/Products" },
                    new Promotion { Title = "New Arrivals", Description = "Check out the latest gadgets!", ImageUrl = "https://images.unsplash.com/photo-1600585154340-be6161a56a0c?w=600&auto=format&fit=crop&q=60", Link = "/Products" }
                }
            };

            ViewBag.RecentlyViewedProducts = recentlyViewedProducts;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Subscribe(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                TempData["Error"] = "Email is required.";
            }
            else if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                TempData["Error"] = "Invalid email format.";
            }
            else if (_context.Newsletters.Any(n => n.Email == email))
            {
                TempData["Error"] = "Email is already subscribed.";
            }
            else
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