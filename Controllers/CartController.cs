using DigitalMagazineStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalMagazineStore.Controllers
{
    [Authorize] 
    public class CartController : Controller
    {
        private readonly ICartRepository _cartRepo;

        public CartController(ICartRepository cartRepo)
        {
            _cartRepo = cartRepo;
        }

        [HttpGet]
        public async Task<IActionResult> AddItem(int magazineId, int qty = 1, int redirect = 0)
        {
            try
            {
                var cartCount = await _cartRepo.AddItem(magazineId, qty);

                if (redirect == 0)
                    return Ok(cartCount);

                return RedirectToAction(nameof(GetUserCart));
            }
            catch (UnauthorizedAccessException)
            {
                // Redirect to login if not authenticated
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpGet]
        public async Task<IActionResult> RemoveItem(int magazineId)
        {
            try
            {
                await _cartRepo.RemoveItem(magazineId);
                return RedirectToAction(nameof(GetUserCart));
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUserCart()
        {
            try
            {
                var cart = await _cartRepo.GetUserCart();
                return View(cart);
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCartItemCount()
        {
            try
            {
                int cartItem = await _cartRepo.GetCartItemCount();
                return Ok(cartItem);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        [HttpGet]
        public IActionResult Checkout()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(CheckoutModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                bool isCheckedOut = await _cartRepo.DoCheckout(model);

                if (!isCheckedOut)
                    return RedirectToAction(nameof(OrderFailure));

                return RedirectToAction(nameof(OrderSuccess));
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public IActionResult OrderSuccess()
        {
            return View();
        }

        public IActionResult OrderFailure()
        {
            return View();
        }
    }
}
