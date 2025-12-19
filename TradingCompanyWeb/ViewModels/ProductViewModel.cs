using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TradingCompany.DTO;

namespace TradingCompanyWeb.ViewModels
{
    public class ProductViewModel
    {
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Назва продукту обовʼязкова")]
        [StringLength(50, ErrorMessage = "Назва не може перевищувати 50 символів")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Категорія обовʼязкова")]
        public int? CategoryId { get; set; }

        [Required(ErrorMessage = "Постачальник обовʼязковий")]
        public int? SupplierId { get; set; }

        [Required(ErrorMessage = "Ціна обовʼязкова")]
        [Range(0.01, 1000000, ErrorMessage = "Ціна повинна бути більшою за 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Кількість обовʼязкова")]
        [Range(0, int.MaxValue, ErrorMessage = "Кількість не може бути відʼємною")]
        public int QuantityInStock { get; set; }

        public List<Category> Categories { get; set; } = new();
        public List<Supplier> Suppliers { get; set; } = new();
    }
}
