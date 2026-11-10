using BookManagement.DataAccess.Data;
using BookManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using BookManagement.Business.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using BookManagement.Utility;

namespace BookManagementWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.RoleAdmin)]
    public class CategoryController : Controller
    {
        private readonly ICategoryService categoryService;
        public CategoryController(ICategoryService _categoryService)
        {
            categoryService = _categoryService;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var cetegories = await categoryService.GetAllCategoriesAsync();
            return View("Index", cetegories);
        }

        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Create")]
        public async Task<IActionResult> CreatePOST(Category category)
        {
            if (!string.IsNullOrEmpty(category.Name) && !await categoryService.IsCategoryNameUniqueAsync(category.Name, category.Id))
            {
                ModelState.AddModelError("", "Name already exist!");
            }

            if (ModelState.IsValid)
            {
                await categoryService.CreateNewCategoryAsync(category);
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index");
            }

            return View();
        }


        public async Task<IActionResult> Update(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var category = await categoryService.GetCategoryByIdAsync(id.Value);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Update")]
        public async Task<IActionResult> UpdatePOST(Category category)
        {
            if (!string.IsNullOrEmpty(category.Name) && !await categoryService.IsCategoryNameUniqueAsync(category.Name, category.Id))
            {
                ModelState.AddModelError("Name", "Category already exist!");
            }

            if (ModelState.IsValid)
            {
                await categoryService.UpdateCategoryAsync(category);
                TempData["success"] = "Category updated successfully";
                return RedirectToAction("Index");
            }

            return View("Update");
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var category = await categoryService.GetCategoryByIdAsync(id.Value);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeletePOST(Category category)
        {
            if (await categoryService.GetCategoryByIdAsync(category.Id) == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await categoryService.DeleteCategoryAsync(category.Id);
                TempData["success"] = "Category deleted successfully";
                return RedirectToAction("Index");
            }

            return View("Delete");
        }
    }
}
