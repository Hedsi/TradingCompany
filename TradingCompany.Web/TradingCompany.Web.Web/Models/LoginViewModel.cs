using System.ComponentModel.DataAnnotations;

namespace TradingCompany.Web.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Логін обов'язковий")]
        [Display(Name = "Логін")]
        public string Login { get; set; } = string.Empty;

        [Required(ErrorMessage = "Пароль обов'язковий")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Запам'ятати мене")]
        public bool RememberMe { get; set; }
    }
}