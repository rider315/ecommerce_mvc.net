using Microsoft.AspNetCore.Mvc;
using EcommerceApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace EcommerceApp.Controllers
{
    public class WishlistController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public WishlistController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize]
        public IActionResult Index()
        {
            var userId = _userManager.GetUserId(User);
            var wishlistItems = _context.Wishlists
                .Include(w => w.Product)
                .Where(w => w.UserId == userId)
                .ToList();
            var cartCount = _context.Carts
                .Where(c => c.UserId == userId)
                .Sum(c => c.Quantity);
            ViewData["CartCount"] = cartCount;
            return View(wishlistItems);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Remove(int id)
        {
            var userId = _userManager.GetUserId(User);
            var wishlistItem = _context.Wishlists
                .FirstOrDefault(w => w.Id == id && w.UserId == userId);
            if (wishlistItem != null)
            {
                _context.Wishlists.Remove(wishlistItem);
                _context.SaveChanges();
                TempData["Message"] = "Item removed from wishlist.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}