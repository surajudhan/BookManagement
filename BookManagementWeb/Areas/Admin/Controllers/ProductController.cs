using BookManagement.Business.Services.IServices;
using BookManagement.Models;
using BookManagement.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BookManagementWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {

        private readonly IProductServices _productService;
        private readonly ICategoryService _categoryService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IProductServices productService, ICategoryService categoryService,
           IWebHostEnvironment webHostEnvironment)
        {
            _productService = productService;
            _categoryService = categoryService;
            _webHostEnvironment = webHostEnvironment;

        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
           
            return View();
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            ProductVM productVm = new()
            {
                CategoryList = categories.Select(o => new SelectListItem
                {
                    Text = o.Name,
                    Value = o.Id.ToString()
                }),
                Product = new Product()
            };

            if (id == null || id == 0)
            {
                return View(productVm);
            }
            else
            {
                productVm.Product = await _productService.GetProductByIdAsync(id.Value);
                return View(productVm);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Upsert")]
        public async Task<IActionResult> UpsertPOST(ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;

                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine("images", "products");
                    string finalPath = Path.Combine(wwwRootPath, productPath);

                    if (!Directory.Exists(finalPath))
                    {
                        Directory.CreateDirectory(finalPath);
                    }
                    using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    productVM.Product.ImageUrl = Path.Combine(@"\", productPath, fileName).Replace("\\", "/");
                }

                if (productVM.Product.Id == 0)
                {
                    await _productService.UpdateProductAsync(productVM.Product);
                    TempData["success"] = "Product is created successfully";
                }
                else
                {
                    await _productService.UpdateProductAsync(productVM.Product);
                    TempData["success"] = "Product is updated successfully";
                }
                return RedirectToAction("Index");
            }
            else
            {
                var categories = await _categoryService.GetAllCategoriesAsync();
                productVM = new()
                {
                    CategoryList = categories.Select(o => new SelectListItem
                    {
                        Text = o.Name,
                        Value = o.Id.ToString()
                    }),
                };
                return View(productVM);
            }
        }




        #region API CALLS
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            return Json(new { data = await _productService.GetAllProductsAsync(true) });
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == 0)
            {
                return Json(new { success = false, message = "Invalid ID" });
            }

            var productToDeleted = await _productService.GetProductByIdAsync(id.Value);
            if (productToDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            // Delete image from the storage
            if (!string.IsNullOrEmpty(productToDeleted.ImageUrl))
            {
                var imgPath = Path.Combine(_webHostEnvironment.WebRootPath, productToDeleted.ImageUrl.TrimStart('\\'));
                if (System.IO.File.Exists(imgPath))
                {
                    System.IO.File.Delete(imgPath);
                }
            }

            await _productService.DeleteProductAsync(id);
            return Json(new { success = true, message = "Deleted successfully" });
        }


        #endregion
    }
}
