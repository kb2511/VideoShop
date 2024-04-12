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

namespace WebShopAlgebra.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: Admin/Product
        public async Task<IActionResult> Index()
        {
            var result = await _productService.GetAll();
            return View(result);
        }

        // GET: Admin/Product/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Product/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Duration,YearOfRelease,ListPrice,Price,PriceMoreThen3,PriceMoreThen10")] Product product)
        {
            if (ModelState.IsValid)
            {
                await _productService.Create(product);
                TempData["Success"] = "Product created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Admin/Product/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _productService.Get(c => c.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Admin/Product/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Duration,YearOfRelease,ListPrice,Price,PriceMoreThen3,PriceMoreThen10,CategoryId")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _productService.Update(product);
                    TempData["Success"] = "Product updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    bool productExists = await ProductExists(product.Id);
                    if (!productExists)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
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
