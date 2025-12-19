using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using TradingCompany.Web.Models;

namespace TradingCompany.Web.Controllers
{
    public class AccountController : Controller
    {
        // Пустий конструктор
        public AccountController()
        {
        }

        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            Console.WriteLine("GET /Account/Login called");
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            Console.WriteLine($"=== LOGIN ATTEMPT START ===");
            Console.WriteLine($"Login: {model.Login}");
            Console.WriteLine($"Password length: {model.Password?.Length}");
            Console.WriteLine($"RememberMe: {model.RememberMe}");
            Console.WriteLine($"ReturnUrl: {returnUrl}");

            ViewData["ReturnUrl"] = returnUrl;

            Console.WriteLine($"ModelState.IsValid: {ModelState.IsValid}");
            Console.WriteLine($"ModelState error count: {ModelState.ErrorCount}");

            if (ModelState.IsValid)
            {
                Console.WriteLine("? ModelState is VALID");

                // ТИМЧАСОВО: проста логіка
                string role = model.Login.ToLower().Contains("manager") ? "Менеджер" : "Користувач";

                Console.WriteLine($"? Role determined: {role}");

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, "1"),
                    new Claim(ClaimTypes.Name, model.Login),
                    new Claim("FullName", model.Login),
                    new Claim(ClaimTypes.Role, role)
                };

                Console.WriteLine($"? Created {claims.Count} claims");

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                Console.WriteLine("? ClaimsIdentity created, signing in...");

                try
                {
                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        new AuthenticationProperties
                        {
                            IsPersistent = model.RememberMe,
                            ExpiresUtc = DateTime.UtcNow.AddDays(7)
                        });

                    Console.WriteLine($"? User signed in successfully!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"? Error signing in: {ex.Message}");
                    Console.WriteLine($"? Stack trace: {ex.StackTrace}");
                    ModelState.AddModelError("", "Помилка автентифікації");
                    return View(model);
                }

                Console.WriteLine($"? Redirecting to Home/Index...");

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    Console.WriteLine($"? Redirecting to returnUrl: {returnUrl}");
                    return Redirect(returnUrl);
                }

                Console.WriteLine($"? Final redirect to Home/Index");
                return RedirectToAction("Index", "Home");
            }
            else
            {
                Console.WriteLine("? ModelState is INVALID");
                Console.WriteLine($"? ModelState has {ModelState.ErrorCount} errors");

                foreach (var key in ModelState.Keys)
                {
                    var state = ModelState[key];
                    if (state.Errors.Any())
                    {
                        Console.WriteLine($"? Field '{key}' errors:");
                        foreach (var error in state.Errors)
                        {
                            Console.WriteLine($"   - {error.ErrorMessage}");
                        }
                    }
                }

                ModelState.AddModelError("", "Будь ласка, введіть логін та пароль");
            }

            Console.WriteLine($"=== LOGIN ATTEMPT END - Returning View ===");
            return View(model);
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            Console.WriteLine("Logout requested");

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            Console.WriteLine("User logged out, redirecting to Home");
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/AccessDenied
        [HttpGet]
        public IActionResult AccessDenied()
        {
            Console.WriteLine("AccessDenied page requested");
            return View();
        }

        // Тестовий метод без токена
        [HttpPost]
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> LoginTest(string login, string password)
        {
            Console.WriteLine($"?? LOGIN TEST START");
            Console.WriteLine($"Login: {login}");
            Console.WriteLine($"Password: {password}");

            if (!string.IsNullOrEmpty(login))
            {
                string role = login.ToLower().Contains("manager") ? "Менеджер" : "Користувач";
                Console.WriteLine($"Role: {role}");

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, "1"),
                    new Claim(ClaimTypes.Name, login),
                    new Claim(ClaimTypes.Role, role)
                };

                try
                {
                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)),
                        new AuthenticationProperties
                        {
                            IsPersistent = true,
                            ExpiresUtc = DateTime.UtcNow.AddDays(1)
                        });

                    Console.WriteLine($"? Test login successful!");
                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"? Test login error: {ex.Message}");
                    return Content($"Error: {ex.Message}");
                }
            }

            Console.WriteLine($"? Test login failed - no login provided");
            return Content("Потрібен логін!");
        }
    }
}