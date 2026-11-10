using BookManagement.Business.Services.IServices;
using BookManagement.DataAccess.Data;
using BookManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookManagement.Business.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly ApplicationDbContext _context;
        public ShoppingCartService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ShoppingCart>> GetUserCartItemsAsync(string userId)
        {
            return await _context.ShoppingCarts
                .Include(p => p.Product)
                .Where(o => o.ApplicationUserId == userId)
                .ToListAsync();
        }

        public async Task<ShoppingCart?> GetCartByIdAsync(int cartId)
        {
            return await _context.ShoppingCarts
                .Include(p => p.Product)
                .FirstOrDefaultAsync(o => o.Id == cartId);
        }

        public async Task<int> GetCartCountAsync(string userId)
        {
            return await _context.ShoppingCarts
                .Where(o => o.ApplicationUserId == userId)
                .SumAsync(u => u.Count);
        }


        public async Task ClearCartAsync(string userId)
        {
            var cartItems = await _context.ShoppingCarts
                .Include(p => p.Product)
                .Where(o => o.ApplicationUserId == userId)
                .ToListAsync();

            if (cartItems.Any())
            {
                _context.ShoppingCarts.RemoveRange(cartItems);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ShoppingCart> AddToCartAsync(ShoppingCart cart)
        {
            var existingCart = await _context.ShoppingCarts
                 .Include(p => p.Product)
                 .FirstOrDefaultAsync(o => o.ApplicationUserId == cart.ApplicationUserId && o.ProductId == cart.ProductId);

            if (existingCart != null)
            {
                existingCart.Count += cart.Count;
                await _context.SaveChangesAsync();
                return existingCart;
            }
            else
            {
                _context.ShoppingCarts.Add(cart);
                await _context.SaveChangesAsync();
                return cart;
            }
        }

        public async Task UpdateCartAsync(ShoppingCart cart)
        {
            if (cart.Count <= 0)
            {
                _context.ShoppingCarts.Remove(cart);
            }
            else
            {
                _context.ShoppingCarts.Update(cart);
            }

            await _context.SaveChangesAsync();

        }

    }
}
