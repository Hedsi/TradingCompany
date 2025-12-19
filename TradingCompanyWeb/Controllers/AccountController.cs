using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Security.Claims;
using TradingCompany.BLL.Interfaces;
using TradingCompany.DAL.Interfaces;
using TradingCompanyWeb.ViewModels;

namespace TradingCompany.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthorizationManager _authManager;
        private readonly IEmployeeDAL _employeeDal;

        public AccountController(
            IAuthorizationManager authManager,
            IEmployeeDAL employeeDal)
        {
            _authManager = authManager;
            _employeeDal = employeeDal;
        }

        // GET: Account/Login
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var employee = _authManager.Login(model.Login, model.Password);

                if (employee == null)
                {
                    ModelState.AddModelError(string.Empty, "Невірний логін або пароль");
                    Log.Warning("Невдала спроба входу з логіном {Login}", model.Login);
                    return View(model);
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, employee.Login),
                    new Claim(ClaimTypes.NameIdentifier, employee.EmployeeId.ToString()),
                    new Claim(ClaimTypes.Role, employee.Role?.RoleName ?? "Користувач"),
                    new Claim("FullName", employee.FirstName)
                };

                var claimsIdentity = new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                Log.Information("Користувач {Login} успішно увійшов у систему", employee.Login);

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Products");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Помилка при вході для користувача {Login}", model.Login);
                ModelState.AddModelError(string.Empty, "Сталася помилка при вході в систему");
                return View(model);
            }
        }

        // GET: Account/Logout
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // GET: Account/AccessDenied
        public IActionResult AccessDenied()
        {
            return View();
        }

        // GET: Account/Register
        [Authorize(Policy = "WarehouseManager")]
        public IActionResult Register()
        {
            return View();
        }
    }
}
