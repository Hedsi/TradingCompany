// Program.cs
using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using TradingCompany.BLL.Concrete;
using TradingCompany.BLL.Interfaces;
using TradingCompany.DAL.Interfaces;
using TradingCompany.DALEF.AutoMapper;
using TradingCompany.DALEF.Concrete;

var builder = WebApplication.CreateBuilder(args);

// Налаштування Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Додавання послуг
builder.Services.AddControllersWithViews();
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<ProductMap>();
    cfg.AddProfile<EmployeeMap>();
    cfg.AddProfile<RoleMap>(); 
    cfg.AddProfile<SupplierMap>();
    cfg.AddProfile<CategoryMap>();
});
// Реєстрація DAL
builder.Services.AddScoped<IRoleDAL>(provider =>
{
    var mapper = provider.GetRequiredService<IMapper>();
    var connStr = builder.Configuration.GetConnectionString("DefaultConnection");
    return new RoleDALEF(connStr, mapper);
});
builder.Services.AddScoped<IProductDAL>(provider =>
{
    var mapper = provider.GetRequiredService<IMapper>();
    var connStr = builder.Configuration.GetConnectionString("DefaultConnection");
    return new ProductDALEF(connStr, mapper);
});

builder.Services.AddScoped<IEmployeeDAL>(provider =>
{
    var mapper = provider.GetRequiredService<IMapper>();
    var connStr = builder.Configuration.GetConnectionString("DefaultConnection");
    return new EmployeeDALEF(connStr, mapper);
});
builder.Services.AddScoped<ICategoryDAL>(provider =>
{
    var mapper = provider.GetRequiredService<IMapper>();
    var connStr = builder.Configuration.GetConnectionString("DefaultConnection");
    return new CategoryDALEF(connStr, mapper);
});
builder.Services.AddScoped<ISupplierDAL>(provider =>
{
    var mapper = provider.GetRequiredService<IMapper>();
    var connStr = builder.Configuration.GetConnectionString("DefaultConnection");
    return new SupplierDALEF(connStr, mapper);
});

// Реєстрація BLL
builder.Services.AddScoped<IProductManager, ProductManager>();
builder.Services.AddScoped<IAuthorizationManager, AuthorizationManager>();

// Налаштування автентифікації
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("WarehouseManager", policy =>
        policy.RequireRole("Менеджер"));
});

builder.Services.AddSession();

var app = builder.Build();

// Конфігурація HTTP pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();