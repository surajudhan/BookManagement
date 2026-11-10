using BookManagement.Business.Services.IServices;
using BookManagement.DataAccess.Data;
using BookManagement.Models;
using BookManagement.Utility;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookManagement.Business.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        public OrderService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<OrderHeader> CreateOrderAsync(OrderHeader orderHeader)
        {
            await _context.OrderHeaders.AddAsync(orderHeader);
            await _context.SaveChangesAsync();
            return orderHeader;
        }

        public async Task<IEnumerable<OrderHeader?>> GetAllOrdersAsync(string? userId = null, string? status = null, bool includeUser = false, bool includeDetails = false)
        {
            var query = _context.OrderHeaders.AsQueryable();

            if (includeUser)
            {
                query = query.Include(u => u.ApplicationUser);
            }

            if (includeDetails)
            {
                query = query.Include(u => u.OrderDetails).ThenInclude(p => p.Product);
            }

            if (!string.IsNullOrEmpty(userId))
            {
                query = query.Where(u => u.ApplicationUserId == userId);
            }

            if (!string.IsNullOrEmpty(status))
            {
                if (status.ToLower() != "all")
                {
                    query = query.Where(o => o.OrderStatus.ToLower() == status.ToLower());
                }
            }

            return await query.ToListAsync();

        }
        public async Task<OrderHeader?> GetOrderBiIdAsync(int id, bool includeUser = false, bool includeDetails = false)
        {
            var query = _context.OrderHeaders.AsQueryable();

            if (includeUser)
            {
                query = query.Include(u => u.ApplicationUser);
            }

            if (includeDetails)
            {
                query = query.Include(u => u.OrderDetails).ThenInclude(p => p.Product);
            }

            return await query.FirstOrDefaultAsync(u => u.Id == id);


        }

        public async Task UpdateOrderAsync(OrderHeader orderHeader)
        {
            _context.OrderHeaders.Update(orderHeader);
            await _context.SaveChangesAsync();
        }

        async Task IOrderService.UpdateOrderStatusAsync(int orderId, string orderStatus, string? carrier, string? trackingNumber)
        {
            var order = await _context.OrderHeaders.FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null)
            {
                throw new Exception($"Order {orderId} is not valid!");
            }

            order.OrderStatus = orderStatus;

            if (orderStatus == SD.StatusShipped)
            {
                order.ShippingDate = DateTime.UtcNow;
            }

            if (!string.IsNullOrEmpty(carrier) && !string.IsNullOrEmpty(trackingNumber))
            {
                order.Carrier = carrier;
                order.TrackingNumber = trackingNumber;
            }

            _context.OrderHeaders.Update(order);
            await _context.SaveChangesAsync();
        }
    }
}
