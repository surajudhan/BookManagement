using BookManagement.Models;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookManagement.Business.Services.IServices
{
    public interface IOrderService
    {
        Task<OrderHeader> CreateOrderAsync(OrderHeader orderHeader);

        Task<OrderHeader?> GetOrderBiIdAsync(int id, bool includeUser = false, bool includeDetails = false);
        Task<IEnumerable<OrderHeader?>> GetAllOrdersAsync(string? userId = null, string? status = null, bool includeUser = false, bool includeDetails = false);

        Task UpdateOrderAsync(OrderHeader orderHeader);
        Task UpdateOrderStatusAsync(int orderId, string status, string? carrier = null, string? trackingNumber = null);
    }
}
