using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using TradingCompany.BLL.Interfaces;
using TradingCompany.DAL.Interfaces;
using TradingCompany.DTO;
using TradingCompanyWeb.ViewModels;

namespace TradingCompanyWeb.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly IProductManager _productManager;
        private readonly IProductDAL _productDal;
        private readonly ICategoryDAL _categoryDal;
        private readonly ISupplierDAL _supplierDal;
        private readonly IMapper _mapper;

        public ProductsController(
            IProductManager productManager,
            IProductDAL productDal,
            ICategoryDAL categoryDal,
            ISupplierDAL supplierDal,
            IMapper mapper)
        {
            _productManager = productManager;
            _productDal = productDal;
            _categoryDal = categoryDal;
            _supplierDal = supplierDal;
            _mapper = mapper;
        }

        // GET: Products
        public IActionResult Index(string searchTerm, string sortOrder)
        {
            var products = _productManager.GetProducts(searchTerm, sortOrder);
            ViewBag.SearchTerm = searchTerm;
            return View(products);
        }

        // GET: Products/Create
        [Authorize(Policy = "WarehouseManager")]
        public IActionResult Create()
        {
            var vm = new ProductViewModel
            {
                Categories = _categoryDal.GetAll() ?? new List<Category>(),
                Suppliers = _supplierDal.GetAll() ?? new List<Supplier>()
            };
            return View(vm);
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "WarehouseManager")]
        public IActionResult Create(ProductViewModel vm)
        {
            // Перевірка обов'язкових полів
            if (!vm.CategoryId.HasValue)
                ModelState.AddModelError("CategoryId", "Категорія обовʼязкова");

            if (!vm.SupplierId.HasValue)
                ModelState.AddModelError("SupplierId", "Постачальник обовʼязковий");

            if (!ModelState.IsValid)
            {
                // Заново підставляємо списки для SelectList
                vm.Categories = _categoryDal.GetAll() ?? new List<Category>();
                vm.Suppliers = _supplierDal.GetAll() ?? new List<Supplier>();
                return View(vm);
            }

            try
            {
                var product = new Product
                {
                    Name = vm.Name,
                    Price = vm.Price,
                    QuantityInStock = vm.QuantityInStock,
                    CategoryId = vm.CategoryId.Value,
                    SupplierId = vm.SupplierId.Value
                };

                var created = _productDal.Create(product);

                if (created == null)
                {
                    TempData["ErrorMessage"] = "Не вдалося створити продукт";
                    vm.Categories = _categoryDal.GetAll() ?? new List<Category>();
                    vm.Suppliers = _supplierDal.GetAll() ?? new List<Supplier>();
                    return View(vm);
                }

                TempData["SuccessMessage"] = $"Продукт '{created.Name}' успішно створено";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Помилка при створенні продукту");
                vm.Categories = _categoryDal.GetAll() ?? new List<Category>();
                vm.Suppliers = _supplierDal.GetAll() ?? new List<Supplier>();
                ModelState.AddModelError("", "Сталася помилка при створенні продукту");
                return View(vm);
            }
        }

        // GET: Products/Edit/5
        [Authorize(Policy = "WarehouseManager")]
        public IActionResult Edit(int id)
        {
            var product = _productDal.GetById(id);
            if (product == null) return NotFound();

            var vm = _mapper.Map<ProductViewModel>(product);
            vm.Categories = _categoryDal.GetAll();
            vm.Suppliers = _supplierDal.GetAll();

            return View(vm);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "WarehouseManager")]
        public IActionResult Edit(int id, ProductViewModel vm)
        {
            if (!vm.CategoryId.HasValue || !vm.SupplierId.HasValue)
                ModelState.AddModelError("", "Категорія та постачальник обовʼязкові");

            if (!ModelState.IsValid)
            {
                vm.Categories = _categoryDal.GetAll();
                vm.Suppliers = _supplierDal.GetAll();
                return View(vm);
            }

            var product = _productDal.GetById(id);
            if (product == null) return NotFound();

            product.Name = vm.Name;
            product.Price = vm.Price;
            product.QuantityInStock = vm.QuantityInStock;
            product.CategoryId = vm.CategoryId.Value;
            product.SupplierId = vm.SupplierId.Value;

            _productDal.Update(product);
            return RedirectToAction(nameof(Index));
        }
    }
}
