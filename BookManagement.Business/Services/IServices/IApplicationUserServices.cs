using BookManagement.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookManagement.Business.Services.IServices
{
    public interface IApplicationUserServices
    {
        Task<ApplicationUser?> GetUserByIdAsync(string id);
    }
}
