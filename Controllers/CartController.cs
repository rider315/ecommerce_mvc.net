using Microsoft.AspNetCore.Mvc;
using EcommerceApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace EcommerceApp.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CartController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize]
        public IActionResult Index()
        {
            var userId = _userManager.GetUserId(User);
            var cartItems = _context.Carts
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToList();
            var cartCount = cartItems.Sum(c => c.Quantity);
            ViewData["CartCount"] = cartCount;
            return View(cartItems);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateQuantity(int cartId, int quantity)
        {
            var cartItem = _context.Carts.Find(cartId);
            if (cartItem != null && User.Identity.Name == cartItem.UserId)
            {
                if (quantity <= 0)
                {
                    _context.Carts.Remove(cartItem);
                }
                else
                {
                    cartItem.Quantity = quantity;
                }
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}