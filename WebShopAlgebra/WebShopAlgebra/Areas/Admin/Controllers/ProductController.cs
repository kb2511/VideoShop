using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebShopData.Data;
using WebShopData.Interfaces;
using WebShopData.Services;
using WebShopModels;
using WebShopModels.Utility;
using WebShopModels.ViewModels;

namespace WebShopAlgebra.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Roles.Admin)]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IProductImageService _productImageService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IProductService productService, 
                                 ICategoryService categoryService, 
                                 IProductImageService productImageService, 
                                 IWebHostEnvironment webHostEnvironment)
        {
            _productService = productService;
            _categoryService = categoryService;
            _productImageService = productImageService;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Admin/Product
        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAll(includeProperties: new string[] { "Category", "ProductImages" });

            return View(products);
        }

        // GET: Admin/Product/Create
        public async Task<IActionResult> Upsert(int? id)
        {
            var allCategories = await _categoryService.GetAll();

            ProductVM productVM = new ProductVM()
            {
                CategoryList = allCategories.Select(c =>
                                new SelectListItem()
                                {
                                    Text = c.Name,
                                    Value = c.Id.ToString()
                                }),
                Product = new Product()
            };

            //for create
            if (id == null || id == 0)
            {
                return View(productVM);
            }
            //else update

            productVM.Product = await _productService.Get(p => p.Id == id, includeProperties: new string[] { "Category", "ProductImages" });
            return View(productVM);
            
        }

        // POST: Admin/Product/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(ProductVM productVM, List<IFormFile>? files)
        {

            if (ModelState.IsValid)
            {
                if (productVM.Product.Id == 0)
                {
                    await _productService.Create(productVM.Product);
                    TempData["Success"] = "Product created successfully";
                }
                else
                {
                    await _productService.Update(productVM.Product);
                    TempData["Success"] = "Product updated successfully";
                }

                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (files != null)
                {

                    foreach (IFormFile file in files)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string productPath = @"images/products/product-" + productVM.Product.Id;
                        string finalPath = Path.Combine(wwwRootPath, productPath);

                        if (!Directory.Exists(finalPath))
                        {
                            Directory.CreateDirectory(finalPath);
                        }                            

                        using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                        ProductImage productImage = new ProductImage()
                        {
                            ImageUrl = @"/" + productPath + @"/" + fileName,
                            ProductId = productVM.Product.Id,
                        };

                        if (productVM.Product.ProductImages == null)
                        {
                            productVM.Product.ProductImages = new List<ProductImage>();
                        }
                          
                        productVM.Product.ProductImages.Add(productImage);

                    }

                    await _productService.Update(productVM.Product);

                }

                return RedirectToAction("Index");
            }
            else
            {
                var categories = await _productService.GetAll();    
                productVM.CategoryList = categories.Select(u => new SelectListItem
                {
                    Text = u.Title,
                    Value = u.Id.ToString()
                });

                return View(productVM);
            }
        }


        private async Task<bool> ProductExists(int id)
        {
            var product = await _productService.Get(p => p.Id == id);

            if (product != null)
            {
                return true;
            }

            return false;
        }

        public async Task<IActionResult> DeleteImage(int imageId)
        {
            var imageToBeDeleted = await _productImageService.Get(u => u.Id == imageId);
            int productId = imageToBeDeleted.ProductId;
            if (imageToBeDeleted != null)
            {
                if (!string.IsNullOrEmpty(imageToBeDeleted.ImageUrl))
                {
                    var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, imageToBeDeleted.ImageUrl.TrimStart('/'));

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                await _productImageService.Delete(imageToBeDeleted);

                TempData["success"] = "Deleted successfully";
            }

            return RedirectToAction(nameof(Upsert), new { id = productId });
        }


        #region API CALLS

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAll(includeProperties: new string[] {"Category"});
            return Json(new { data = products.ToList() });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int? id)
        {
            var productToBeDeleted = await _productService.Get(p => p.Id == id);
            if (productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            string productPath = @"images/products/product-" + id;
            string finalPath = Path.Combine(_webHostEnvironment.WebRootPath, productPath);

            if (Directory.Exists(finalPath))
            {
                string[] filePaths = Directory.GetFiles(finalPath);
                foreach (string filePath in filePaths)
                {
                    System.IO.File.Delete(filePath);
                }

                Directory.Delete(finalPath);
            }

            await _productService.Delete(productToBeDeleted);

            return Json(new { success = true, message = "Delete successful" });
        }
        #endregion
    }
}
