using FerozeHub.MessageBus.Bus;
using FerozeHub.RabbitMqBus;
using FerozeHub.Web.Service.Implementations;
using FerozeHub.Web.Service.Interfaces;
using FerozeHub.Web.Utility;
using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
SD.CouponAPI=builder.Configuration["ServiceUrls:CouponAPI"];
SD.AuthAPI=builder.Configuration["ServiceUrls:AuthAPI"];
SD.ProductAPI=builder.Configuration["ServiceUrls:ProductAPI"];
SD.CartAPI=builder.Configuration["ServiceUrls:CartAPI"];
builder.Services.AddHttpClient<IBaseService,BaseService>();
builder.Services.AddHttpClient<ICouponService,CouponService>();
builder.Services.AddHttpClient<IAuthService,AuthService>();
builder.Services.AddHttpClient<IProductService,ProductService>();
builder.Services.AddHttpClient<ICartService,CartService>();
builder.Services.AddScoped<HttpContextAccessor>();

builder.Services.AddScoped<ICouponService,CouponService>();
builder.Services.AddScoped<IBaseService, BaseService>();
builder.Services.AddScoped<IAuthService,AuthService>();
builder.Services.AddScoped<ITokenProvider,TokenProvider>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICartService,CartService>();



builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromHours(10);
    options.LoginPath = "/Auth/Login";
    options.AccessDeniedPath = "/Auth/AccessDenied";
});

var app = builder.Build();

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();