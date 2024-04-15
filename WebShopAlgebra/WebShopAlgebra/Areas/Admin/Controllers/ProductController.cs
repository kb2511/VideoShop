using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebShopData.Data;
using WebShopData.Interfaces;
using WebShopData.Services;
using WebShopModels;
using WebShopModels.ViewModels;

namespace WebShopAlgebra.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IProductService productService, ICategoryService categoryService, IWebHostEnvironment webHostEnvironment)
        {
            _productService = productService;
            _categoryService = categoryService;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Admin/Product
        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAll();

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

            productVM.Product = await _productService.Get(p => p.Id == id);
            return View(productVM);
            
        }

        // POST: Admin/Product/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images/product");

                    if(!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                        //delete old image
                        var oldImagePath = Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('/'));

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    productVM.Product.ImageUrl = @"images/product/" + fileName;
                }

                if (productVM.Product.Id == 0)
                {
                    await _productService.Create(productVM.Product);
                    TempData["Success"] = "Product created successfully!";
                }
                else
                {
                    await _productService.Update(productVM.Product);
                    TempData["Success"] = "Product updated successfully!";
                }
                
                return RedirectToAction(nameof(Index));
            }

            var allCategories = await _categoryService.GetAll();
            productVM.CategoryList = allCategories.Select(c => 
            new SelectListItem()
            {
                Text = c.Name,
                Value = c.Id.ToString()
            });
            return View(productVM);
        }

        // GET: Admin/Product/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _productService.Get(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Admin/Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _productService.Get(p => p.Id == id);
            if (product != null)
            {
                await _productService.Delete(product);
            }

            TempData["Success"] = "Product deleted successfully!";
            return RedirectToAction(nameof(Index));
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
    }
}
