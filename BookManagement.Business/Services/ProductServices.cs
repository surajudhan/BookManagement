using BookManagement.Business.Services.IServices;
using BookManagement.DataAccess.Data;
using BookManagement.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookManagement.Business.Services
{
    public class ProductServices : IProductServices
    {
        private readonly ApplicationDbContext _context;

        public ProductServices(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync(bool includeCategory = false)
        {
            if (includeCategory)
            {
                return await _context.Products.Include(x => x.Category).AsNoTracking().ToListAsync();
            }
            else
            {
                return await _context.Products.AsNoTracking().ToListAsync();
            }
        }

        public async Task<Product?> GetProductByIdAsync(int id, bool includeCategory = false)
        {
            if (includeCategory)
            {
                return await _context.Products.Include(u => u.Category).FirstOrDefaultAsync(x => x.Id == id);
            }
            else
            {
                return await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
            }
        }
        public async Task<Product> CreateProductAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task DeleteProductAsync(int? id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateProductAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> IsProductNameUnique(string name, int? id)
        {
            if (id.HasValue)
            {
                return (!await _context.Products.AnyAsync(x => x.Title.ToLower() == name.ToLower() && x.Id != id));
            }
            else
            {
                return !await _context.Products.AnyAsync(x => x.Title.ToLower() == name.ToLower());
            }
        }


    }
}
