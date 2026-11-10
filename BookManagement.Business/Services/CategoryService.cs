using BookManagement.Business.Services.IServices;
using BookManagement.Models;
using System;
using System.Collections.Generic;
using System.Text;
using BookManagement.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace BookManagement.Business.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;
        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories.AsNoTracking().ToListAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            return await _context.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<Category> CreateNewCategoryAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if (category == null)
            {
                throw new KeyNotFoundException($"Category {id} not found!");
            }
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

        }

        public async Task UpdateCategoryAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsCategoryNameUniqueAsync(string name, int? categoryId = null)
        {
            if (categoryId.HasValue)
            {
                return !await _context.Categories.AnyAsync(x => x.Name.ToLower() == name.ToLower() && x.Id != categoryId.Value);
            }
            else
            {
                return !await _context.Categories.AnyAsync(x => x.Name.ToLower() == name.ToLower());
            }
        }
    }
}
