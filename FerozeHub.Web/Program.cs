using FerozeHub.Web.Service.Implementations;
using FerozeHub.Web.Service.Interfaces;
using FerozeHub.Web.Utility;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
SD.CouponAPI=builder.Configuration["ServiceUrls:CouponAPI"];
builder.Services.AddHttpClient<IBaseService,BaseService>();
builder.Services.AddHttpClient<ICouponService,CouponService>();


builder.Services.AddScoped<ICouponService,CouponService>();
builder.Services.AddScoped<IBaseService, BaseService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();