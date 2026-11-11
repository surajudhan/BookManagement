using BookManagement.Business.Services;
using BookManagement.Business.Services.IServices;
using BookManagement.Models;
using BookManagement.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookManagementWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.RoleAdmin)]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        [BindProperty]
        public OrderHeader OrderHeader { get; set; }


        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int orderId)
        {
            OrderHeader = await _orderService.GetOrderBiIdAsync(orderId, includeDetails: true, includeUser: true);
            return View(OrderHeader);
        }

        [HttpPost]
        [Authorize(Roles = SD.RoleAdmin + "," + SD.RoleEmployee)]
        public async Task<IActionResult> UpdateOrderDetails()
        {
            var orderHeaderFromDB = await _orderService.GetOrderBiIdAsync(OrderHeader.Id);
            orderHeaderFromDB.Name = OrderHeader.Name;
            orderHeaderFromDB.PhoneNumber = OrderHeader.PhoneNumber;
            orderHeaderFromDB.StreetAddress = OrderHeader.StreetAddress;
            orderHeaderFromDB.City = OrderHeader.City;
            orderHeaderFromDB.State = OrderHeader.State;
            orderHeaderFromDB.PostalCode = OrderHeader.PostalCode;
            if (!string.IsNullOrEmpty(OrderHeader.Carrier) && orderHeaderFromDB.OrderStatus == SD.StatusShipped)
            {
                orderHeaderFromDB.Carrier = OrderHeader.Carrier;
            }

            if (!string.IsNullOrEmpty(OrderHeader.TrackingNumber) && orderHeaderFromDB.OrderStatus == SD.StatusShipped)
            {
                orderHeaderFromDB.TrackingNumber = OrderHeader.TrackingNumber;
            }

            await _orderService.UpdateOrderAsync(orderHeaderFromDB);

            TempData["success"] = "Order Details Updated Successfully.";
            return RedirectToAction(nameof(Details), new { orderId = orderHeaderFromDB.Id });

        }

        [HttpPost]
        [Authorize(Roles = SD.RoleAdmin + "," + SD.RoleEmployee)]
        public async Task<IActionResult> UpdateOrderStatus(string status)
        {
            var order = await _orderService.GetOrderBiIdAsync(OrderHeader.Id);
            if (order == null)
            {
                TempData["error"] = "Order not found...";
                return RedirectToAction(nameof(Index));
            }

            string successmessage;

            switch (status)
            {
                case SD.StatusInProcess:
                    {
                        await _orderService.UpdateOrderStatusAsync(OrderHeader.Id, status);
                        successmessage = "Order processing started successfully.";
                    }
                    break;

                case SD.StatusCancelled:
                    {
                        await _orderService.UpdateOrderStatusAsync(OrderHeader.Id, status);
                        successmessage = "Order cancelled successfully.";
                    }
                    break;

                case SD.StatusRefunded:
                    {
                        await _orderService.UpdateOrderStatusAsync(OrderHeader.Id, status);
                        successmessage = "Order refunded successfully.";
                    }
                    break;


                case SD.StatusShipped:
                    {
                        if (string.IsNullOrEmpty(OrderHeader.Carrier) || string.IsNullOrEmpty(OrderHeader.TrackingNumber))
                        {
                            TempData["error"] = "Please provide both carrier and tracking number.";
                            return RedirectToAction(nameof(Details), new { orderId = OrderHeader.Id });
                        }

                        await _orderService.UpdateOrderStatusAsync(OrderHeader.Id, status, OrderHeader.Carrier, OrderHeader.TrackingNumber);
                        successmessage = "Order shipped successfully.";
                    }
                    break;

                default:
                    {
                        TempData["error"] = "Invalid status update.";
                        return RedirectToAction(nameof(Details), new { orderId = OrderHeader.Id });
                    }
            }

            TempData["success"] = successmessage;
            return RedirectToAction(nameof(Details), new { orderId=OrderHeader.Id});
        }

        #region API CALLS
        [AllowAnonymous]
        public async Task<IActionResult> GetAll(string status)
        {
            string? userID = null;
            if (!User.IsInRole(SD.RoleAdmin) && !User.IsInRole(SD.RoleEmployee))
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                userID = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userID))
                {
                    return Unauthorized();
                }
            }
            return Json(new { data = await _orderService.GetAllOrdersAsync(userId: userID, status: status, includeUser: true) });
        }
        #endregion
    }
}
