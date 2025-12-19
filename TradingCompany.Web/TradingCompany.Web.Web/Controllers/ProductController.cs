using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using TradingCompany.BLL.Interfaces;
using TradingCompany.DAL.Interfaces;
using TradingCompany.DTO;
using TradingCompany.Web.Models;

namespace TradingCompany.Web.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IProductManager _productManager;
        private readonly IProductDAL _productDal;
        private readonly ICategoryDAL _categoryDal;
        private readonly ISupplierDAL _supplierDal;

        public ProductController(
            IProductManager productManager,
            IProductDAL productDal,
            ICategoryDAL categoryDal,
            ISupplierDAL supplierDal)
        {
            _productManager = productManager;
            _productDal = productDal;
            _categoryDal = categoryDal;
            _supplierDal = supplierDal;
        }

        // GET: /Product
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index(string searchTerm = "", string sortOrder = "")
        {
            var products = _productManager.GetProducts(searchTerm, sortOrder);

            var viewModels = products.Select(p => new ProductViewModel
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Price = p.Price,
                QuantityInStock = p.QuantityInStock,
                CategoryId = p.CategoryId,
                SupplierId = p.SupplierId,
                CategoryName = p.Category?.ToString() ?? "Немає",
                SupplierName = p.Supplier?.ToString() ?? "Немає"
            }).ToList();

            ViewBag.CurrentSearch = searchTerm;
            ViewBag.CurrentSort = sortOrder;

            // Сортування для View
            ViewBag.NameSortParam = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.PriceSortParam = sortOrder == "Price_Asc" ? "Price_Desc" : "Price_Asc";
            ViewBag.QuantitySortParam = sortOrder == "Quantity_Asc" ? "Quantity_Desc" : "Quantity_Asc";

            return View(viewModels);
        }

        // GET: /Product/Details/5
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Details(int id)
        {
            var product = _productManager.GetProductById(id);

            if (product == null)
            {
                return NotFound();
            }

            var viewModel = new ProductViewModel
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Price = product.Price,
                QuantityInStock = product.QuantityInStock,
                CategoryId = product.CategoryId,
                SupplierId = product.SupplierId,
                CategoryName = product.Category?.ToString() ?? "Немає",
                SupplierName = product.Supplier?.ToString() ?? "Немає"
            };

            return View(viewModel);
        }

        // GET: /Product/Create
        [HttpGet]
        [Authorize(Roles = "Менеджер")]
        public IActionResult Create()
        {
            // Отримуємо списки для dropdown
            var categories = _categoryDal.GetAll() ?? new List<Category>();
            var suppliers = _supplierDal.GetAll() ?? new List<Supplier>();

            ViewBag.Categories = new SelectList(categories, "CategoryId", "CategoryName");
            ViewBag.Suppliers = new SelectList(suppliers, "SupplierId", "Brand");

            return View();
        }

        // POST: /Product/Create
        [HttpPost]
        [Authorize(Roles = "Менеджер")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Конвертуємо ViewModel у DTO
                    var productDto = new Product
                    {
                        Name = model.Name,
                        Price = model.Price,
                        QuantityInStock = model.QuantityInStock,
                        CategoryId = model.CategoryId,
                        SupplierId = model.SupplierId
                    };

                    // Викликаємо метод Create з DAL
                    var createdProduct = _productDal.Create(productDto);

                    if (createdProduct != null)
                    {
                        TempData["SuccessMessage"] = "Продукт успішно створено!";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("", "Помилка при створенні продукту.");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Помилка: {ex.Message}");
                }
            }

            // Якщо щось пішло не так - повертаємо форму з даними
            var categories = _categoryDal.GetAll() ?? new List<Category>();
            var suppliers = _supplierDal.GetAll() ?? new List<Supplier>();

            ViewBag.Categories = new SelectList(categories, "CategoryId", "CategoryName");
            ViewBag.Suppliers = new SelectList(suppliers, "SupplierId", "Brand");

            return View(model);
        }

        // GET: /Product/Edit/5
        [HttpGet]
        [Authorize(Roles = "Менеджер")]
        public IActionResult Edit(int id)
        {
            var product = _productManager.GetProductById(id);

            if (product == null)
            {
                return NotFound();
            }

            var viewModel = new ProductEditViewModel
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Price = product.Price,
                QuantityInStock = product.QuantityInStock,
                CategoryId = product.CategoryId,
                SupplierId = product.SupplierId
            };

            // Отримуємо списки для dropdown
            var categories = _categoryDal.GetAll() ?? new List<Category>();
            var suppliers = _supplierDal.GetAll() ?? new List<Supplier>();

            ViewBag.Categories = new SelectList(categories, "CategoryId", "CategoryName", product.CategoryId);
            ViewBag.Suppliers = new SelectList(suppliers, "SupplierId", "Brand", product.SupplierId);

            return View(viewModel);
        }

        // POST: /Product/Edit/5
        [HttpPost]
        [Authorize(Roles = "Менеджер")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, ProductEditViewModel model)
        {
            if (id != model.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var productDto = new Product
                    {
                        ProductId = model.ProductId,
                        Name = model.Name,
                        Price = model.Price,
                        QuantityInStock = model.QuantityInStock,
                        CategoryId = model.CategoryId,
                        SupplierId = model.SupplierId
                    };

                    // Викликаємо метод Update з DAL
                    var updatedProduct = _productDal.Update(productDto);

                    if (updatedProduct != null)
                    {
                        TempData["SuccessMessage"] = "Продукт успішно оновлено!";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("", "Помилка при оновленні продукту.");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Помилка: {ex.Message}");
                }
            }

            // Якщо щось пішло не так - повертаємо форму з даними
            var categories = _categoryDal.GetAll() ?? new List<Category>();
            var suppliers = _supplierDal.GetAll() ?? new List<Supplier>();

            ViewBag.Categories = new SelectList(categories, "CategoryId", "CategoryName", model.CategoryId);
            ViewBag.Suppliers = new SelectList(suppliers, "SupplierId", "Brand", model.SupplierId);

            return View(model);
        }

        // GET: /Product/Delete/5
        [HttpGet]
        [Authorize(Roles = "Менеджер")]
        public IActionResult Delete(int id)
        {
            var product = _productManager.GetProductById(id);

            if (product == null)
            {
                return NotFound();
            }

            var viewModel = new ProductViewModel
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Price = product.Price,
                QuantityInStock = product.QuantityInStock,
                CategoryName = product.Category?.ToString() ?? "Немає",
                SupplierName = product.Supplier?.ToString() ?? "Немає"
            };

            return View(viewModel);
        }

        // POST: /Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Менеджер")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                var result = _productDal.Delete(id);

                if (result)
                {
                    TempData["SuccessMessage"] = "Продукт успішно видалено!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Не вдалося видалити продукт. Можливо, він не існує.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Помилка при видаленні: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /Product/BySupplier/5
        [HttpGet]
        [AllowAnonymous]
        public IActionResult BySupplier(int supplierId)
        {
            var products = _productManager.GetProductsBySupplier(supplierId);

            var viewModels = products.Select(p => new ProductViewModel
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Price = p.Price,
                QuantityInStock = p.QuantityInStock,
                CategoryId = p.CategoryId,
                SupplierId = p.SupplierId,
                CategoryName = p.Category?.ToString() ?? "Немає",
                SupplierName = p.Supplier?.ToString() ?? "Немає"
            }).ToList();

            ViewBag.SupplierId = supplierId;

            if (products.Any())
            {
                ViewBag.SupplierName = products.First().Supplier?.ToString() ?? "Постачальник";
            }

            return View(viewModels);
        }
    }
}