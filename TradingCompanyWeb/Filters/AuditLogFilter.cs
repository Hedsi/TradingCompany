// Filters/AuditLogFilter.cs
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
using System.Security.Claims;

namespace TradingCompanyWeb.Filters
{
    public class AuditLogFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Логування перед виконанням дії
            var user = context.HttpContext.User;
            var actionName = context.ActionDescriptor.RouteValues["action"];
            var controllerName = context.ActionDescriptor.RouteValues["controller"];
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = user.FindFirstValue(ClaimTypes.Name);

            Log.Information($"User {userName} (ID: {userId}) started action {controllerName}.{actionName}");
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Логування після виконання дії
            if (context.Exception != null)
            {
                Log.Error(context.Exception, $"Action failed: {context.ActionDescriptor.RouteValues["controller"]}.{context.ActionDescriptor.RouteValues["action"]}");
            }
        }
    }
}