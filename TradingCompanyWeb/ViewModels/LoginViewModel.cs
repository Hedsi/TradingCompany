// ViewModels/LoginViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace TradingCompanyWeb.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Логін обов'язковий")]
        [Display(Name = "Логін")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Пароль обов'язковий")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Display(Name = "Запам'ятати мене")]
        public bool RememberMe { get; set; }
    }
}