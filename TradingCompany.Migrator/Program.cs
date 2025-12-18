using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using TradingCompany.DALEF.Concrete.ctx;

namespace TradingCompany.Migrator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "Server=localhost\\SQLEXPRESS;Database=Trading_Company;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True";

            Console.WriteLine("=== МІГРАЦІЯ ПАРОЛІВ ===");

            try
            {
                using (var ctx = new TradingCompContext(connectionString))
                {
                    var usersToUpdate = ctx.Employees.Where(u => u.Salt == null).ToList();

                    if (!usersToUpdate.Any())
                    {
                        Console.WriteLine("Всі користувачі вже мають нові паролі.");
                        Console.ReadLine();
                        return;
                    }

                    Console.WriteLine($"Знайдено {usersToUpdate.Count} користувачів для оновлення.");

                    foreach (var user in usersToUpdate)
                    {
                        Console.Write($"Обробка користувача '{user.Login}'... ");

                        var salt = Guid.NewGuid();
                        var hashedPassword = HashPassword(user.Password, salt.ToString());

                        user.Salt = salt;
                        user.Password = hashedPassword;

                        Console.WriteLine("OK");
                    }

                    ctx.SaveChanges();
                    Console.WriteLine("Міграція завершена успішно.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ПОМИЛКА: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Деталі: {ex.InnerException.Message}");
                }
            }

            Console.WriteLine("Натисніть Enter для виходу...");
            Console.ReadLine();
        }

        private static string HashPassword(string password, string salt)
        {
            using (var alg = SHA512.Create())
            {
                var bytes = alg.ComputeHash(Encoding.UTF8.GetBytes(password + salt));
                return Convert.ToBase64String(bytes);
            }
        }
    }
}