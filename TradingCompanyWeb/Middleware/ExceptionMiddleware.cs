// Middleware/ExceptionMiddleware.cs
using Serilog;

namespace TradingCompanyWeb.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Необроблена помилка в програмі");

                context.Response.Redirect("/Home/Error");
                await Task.CompletedTask;
            }
        }
    }
}