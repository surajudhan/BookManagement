using BookManagement.Business.Services.IServices;
using BookManagement.DataAccess.Data;
using BookManagement.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookManagement.Business.Services
{
    public class ApplicationUserService : IApplicationUserServices
    {
        private readonly ApplicationDbContext _context;

        public ApplicationUserService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<ApplicationUser?> GetUserByIdAsync(string id)
        {
            return await _context.ApplicationUsers.FirstOrDefaultAsync(o => o.Id == id);
        }
    }
}
