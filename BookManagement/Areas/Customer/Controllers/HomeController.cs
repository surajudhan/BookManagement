using BookManagement.Business.Services.IServices;
using BookManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BookManagementWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IProductServices _productServices;
        private readonly IShoppingCartService _shoppingCartServices;
        public HomeController(IProductServices producServices, IShoppingCartService shoppingCartServices)
        {
            _productServices = producServices;
            _shoppingCartServices = shoppingCartServices;

        }
        public async Task<IActionResult> Index()
        {
            var products = await _productServices.GetAllProductsAsync(true);
            return View(products);
        }

        public async Task<IActionResult> Details(int productId)
        {
            var product = await _productServices.GetProductByIdAsync(productId, includeCategory: true);
            if (product == null)
            {
                return NotFound();
            }

            ShoppingCart cart = new()
            {
                Product = product,
                ProductId = productId,
                Count = 1
            };

            return View(cart);
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Details(ShoppingCart cart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            cart.ApplicationUserId = userId;
            await _shoppingCartServices.AddToCartAsync(cart);

            TempData["success"] = "Item added to cart";


            return RedirectToAction("Index");
        }
    }
}
