using BookManagement.Business.Services.IServices;
using BookManagement.Models.ViewModels;
using BookManagement.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;
using System.Security.Claims;

namespace BookManagementWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IShoppingCartService _cartService;
        private readonly IApplicationUserServices _applicationUserServices;

        public CartController(IShoppingCartService cartService, IApplicationUserServices applicationUserServices, IOrderService orderService)
        {
            _orderService = orderService;
            _cartService = cartService;
            _applicationUserServices = applicationUserServices;
        }
        public async Task<IActionResult> Index()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimIdentity?.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (string.IsNullOrEmpty(userId))
            {
                return NotFound();
            }

            ShoppingCartVM shoppingCartVM = new()
            {
                ShoppingCartList = await _cartService.GetUserCartItemsAsync(userId),
                OrderHeader = new()
            };

            var user = await _applicationUserServices.GetUserByIdAsync(userId);

            shoppingCartVM.OrderHeader.ApplicationUser = user;
            shoppingCartVM.OrderHeader.Name = user.Name;
            shoppingCartVM.OrderHeader.PhoneNumber = user.PhoneNumber;
            shoppingCartVM.OrderHeader.StreetAddress = user.StreetAddress;
            shoppingCartVM.OrderHeader.City = user.City;
            shoppingCartVM.OrderHeader.State = user.State;
            shoppingCartVM.OrderHeader.PostalCode = user.PostalCode;

            foreach (var cart in shoppingCartVM.ShoppingCartList)
            {
                shoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            return View(shoppingCartVM);
        }
        [HttpPost]
        [ActionName("Index")]
        public async Task<IActionResult> IndexPOST(ShoppingCartVM shoppingCartVM)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimIdentity?.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (string.IsNullOrEmpty(userId))
            {
                return NotFound();
            }


            var cartItems = await _cartService.GetUserCartItemsAsync(userId);
            shoppingCartVM.ShoppingCartList = cartItems;
            shoppingCartVM.OrderHeader.OrderDate = DateTime.UtcNow;
            shoppingCartVM.OrderHeader.ApplicationUserId = userId;

            foreach (var cart in shoppingCartVM.ShoppingCartList)
            {
                shoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            shoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
            shoppingCartVM.OrderHeader.OrderDetails = shoppingCartVM.ShoppingCartList.Select(cart => new BookManagement.Models.OrderDetails
            {
                ProductId = cart.ProductId,
                Price = cart.Price,
                Count = cart.Count
            }).ToList();

            await _orderService.CreateOrderAsync(shoppingCartVM.OrderHeader);

            //create order
            return RedirectToAction("OrderConfirmation", new { id = shoppingCartVM.OrderHeader.Id });
        }

        public async Task<IActionResult> Plus(int cartId)
        {
            var cartItem = await _cartService.GetCartByIdAsync(cartId);
            if (cartItem != null)
            {
                if (cartItem.Count == 1000)
                {
                    //Do nothing
                }
                else
                {
                    cartItem.Count++;
                }
                await _cartService.UpdateCartAsync(cartItem);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Minus(int cartId)
        {
            var cartItem = await _cartService.GetCartByIdAsync(cartId);
            if (cartItem != null)
            {
                cartItem.Count--;
                await _cartService.UpdateCartAsync(cartItem);
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Remove(int cartId)
        {
            var cartItem = await _cartService.GetCartByIdAsync(cartId);
            if (cartItem != null)
            {
                cartItem.Count = 0;
                await _cartService.UpdateCartAsync(cartItem);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCart(int cartId, int count)
        {
            var cartItem = await _cartService.GetCartByIdAsync(cartId);
            if (cartItem == null)
            {
                return NotFound();
            }

            if (count <= 0)
            {
                cartItem.Count = 0;
            }
            else
            {
                if (count >= 1000)
                {
                    cartItem.Count = 1000;
                }
                else
                {
                    cartItem.Count = count;
                }
            }

            await _cartService.UpdateCartAsync(cartItem);
            return Ok(new { success = true });
        }

        public async Task<IActionResult> OrderConfirmation(int id)
        {

            return View(id);
        }
    }
}
