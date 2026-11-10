using BookManagement.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookManagement.Business.Services.IServices
{
    public interface IProductServices
    {
        Task<IEnumerable<Product>> GetAllProductsAsync(bool includeCategory = false);
        Task<Product?> GetProductByIdAsync(int id, bool includeCategory = false);
        Task<Product> CreateProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(int? id);
        Task<bool> IsProductNameUnique(string name, int? id);
    }
}
