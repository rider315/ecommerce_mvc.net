using Microsoft.AspNetCore.Mvc;
using EcommerceApp.Data;
using EcommerceApp.Models;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace EcommerceApp.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ProductsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index(string search, decimal? minPrice, decimal? maxPrice, string sort)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = _userManager.GetUserId(User);
                var cartCount = _context.Carts
                    .Where(c => c.UserId == userId)
                    .Sum(c => c.Quantity);
                ViewData["CartCount"] = cartCount;
            }

            var products = _context.Products.AsQueryable();

            // Search
            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(p => p.Name.Contains(search) || p.Description.Contains(search));
            }

            // Price Filter
            if (minPrice.HasValue)
            {
                products = products.Where(p => p.Price >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                products = products.Where(p => p.Price <= maxPrice.Value);
            }

            // Sort
            switch (sort)
            {
                case "priceAsc":
                    products = products.OrderBy(p => p.Price);
                    break;
                case "priceDesc":
                    products = products.OrderByDescending(p => p.Price);
                    break;
                default:
                    products = products.OrderBy(p => p.Name);
                    break;
            }

            // Map to ProductViewModel with ratings
            var productViewModels = products.Select(p => new ProductViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                AverageRating = _context.Ratings.Where(r => r.ProductId == p.Id).Average(r => (double?)r.Score) ?? 0,
                ReviewCount = _context.Ratings.Count(r => r.ProductId == p.Id)
            }).ToList();

            ViewBag.Search = search;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.Sort = sort;

            return View(productViewModels);
        }

        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Products.Add(product);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddToCart(int productId)
        {
            var userId = _userManager.GetUserId(User);
            var cartItem = _context.Carts.FirstOrDefault(c => c.UserId == userId && c.ProductId == productId);

            if (cartItem == null)
            {
                cartItem = new Cart
                {
                    UserId = userId,
                    ProductId = productId,
                    Quantity = 1
                };
                _context.Carts.Add(cartItem);
            }
            else
            {
                cartItem.Quantity++;
            }

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RateProduct(int productId, int score)
        {
            if (score < 1 || score > 5)
            {
                TempData["Error"] = "Rating must be between 1 and 5.";
                return RedirectToAction(nameof(Index));
            }

            var userId = _userManager.GetUserId(User);
            var rating = _context.Ratings.FirstOrDefault(r => r.UserId == userId && r.ProductId == productId);

            if (rating == null)
            {
                rating = new Rating
                {
                    UserId = userId,
                    ProductId = productId,
                    Score = score
                };
                _context.Ratings.Add(rating);
            }
            else
            {
                rating.Score = score;
            }

            _context.SaveChanges();
            TempData["Message"] = "Thank you for your rating!";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult QuickView(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            // Track recently viewed
            if (User.Identity.IsAuthenticated)
            {
                var recentlyViewed = HttpContext.Session.GetString("RecentlyViewed")?.Split(',').Select(int.Parse).ToList() ?? new List<int>();
                if (!recentlyViewed.Contains(id))
                {
                    recentlyViewed.Insert(0, id);
                    if (recentlyViewed.Count > 4)
                    {
                        recentlyViewed = recentlyViewed.Take(4).ToList();
                    }
                    HttpContext.Session.SetString("RecentlyViewed", string.Join(",", recentlyViewed));
                }
            }

            return PartialView("QuickView", product);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddToWishlist(int productId)
        {
            var userId = _userManager.GetUserId(User);
            var wishlistItem = _context.Wishlists.FirstOrDefault(w => w.UserId == userId && w.ProductId == productId);

            if (wishlistItem == null)
            {
                wishlistItem = new Wishlist
                {
                    UserId = userId,
                    ProductId = productId
                };
                _context.Wishlists.Add(wishlistItem);
                _context.SaveChanges();
                TempData["Message"] = "Added to wishlist!";
            }
            else
            {
                TempData["Error"] = "Item is already in your wishlist.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}