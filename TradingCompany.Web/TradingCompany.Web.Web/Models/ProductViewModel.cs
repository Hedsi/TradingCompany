using System.ComponentModel.DataAnnotations;

namespace TradingCompany.Web.Models
{
    public class ProductViewModel
    {
        public int ProductId { get; set; }

        [Display(Name = "Назва")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Ціна")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Price { get; set; }

        [Display(Name = "Кількість")]
        public int QuantityInStock { get; set; }

        [Display(Name = "Категорія")]
        public int CategoryId { get; set; }

        [Display(Name = "Постачальник")]
        public int SupplierId { get; set; }

        [Display(Name = "Категорія")]
        public string CategoryName { get; set; } = string.Empty;

        [Display(Name = "Постачальник")]
        public string SupplierName { get; set; } = string.Empty;
    }

    public class ProductCreateViewModel
    {
        [Required(ErrorMessage = "Назва обов'язкова")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Назва має бути від 3 до 100 символів")]
        [Display(Name = "Назва")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ціна обов'язкова")]
        [Range(0.01, 1000000, ErrorMessage = "Ціна має бути більше 0")]
        [Display(Name = "Ціна")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Кількість обов'язкова")]
        [Range(0, 10000, ErrorMessage = "Кількість має бути від 0 до 10000")]
        [Display(Name = "Кількість на складі")]
        public int QuantityInStock { get; set; }

        [Required(ErrorMessage = "Категорія обов'язкова")]
        [Display(Name = "Категорія")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Постачальник обов'язковий")]
        [Display(Name = "Постачальник")]
        public int SupplierId { get; set; }
    }

    public class ProductEditViewModel
    {
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Назва обов'язкова")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Назва має бути від 3 до 100 символів")]
        [Display(Name = "Назва")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ціна обов'язкова")]
        [Range(0.01, 1000000, ErrorMessage = "Ціна має бути більше 0")]
        [Display(Name = "Ціна")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Кількість обов'язкова")]
        [Range(0, 10000, ErrorMessage = "Кількість має бути від 0 до 10000")]
        [Display(Name = "Кількість на складі")]
        public int QuantityInStock { get; set; }

        [Required(ErrorMessage = "Категорія обов'язкова")]
        [Display(Name = "Категорія")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Постачальник обов'язковий")]
        [Display(Name = "Постачальник")]
        public int SupplierId { get; set; }
    }
}