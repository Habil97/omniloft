using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using Serilog;
using Verveo.DataAccess;
using Verveo.Services;

var builder = WebApplication.CreateBuilder(args);

// Serilog yapılandırması
builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console()
    .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day)
    .ReadFrom.Configuration(ctx.Configuration));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDbContext<VerveoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IProductRepository, EfProductRepository>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<ICategoryRepository, EfCategoryRepository>();
builder.Services.AddScoped<IOrderRepository, EfOrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddAuthentication("MyCookieAuth")
    .AddCookie("MyCookieAuth", options =>
    {
        options.LoginPath = "/User/Login";
        options.AccessDeniedPath = "/User/AccessDenied";
    });
builder.Services.AddScoped<VerveoDbContext>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<SliderService>();
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<ISliderRepository, EfSliderRepository>();
builder.Services.AddScoped<IReturnRequestRepository, EfReturnRequestRepository>();
builder.Services.AddScoped<ReturnRequestService>();
builder.Services.AddScoped<ICouponService, CouponService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<RoleService>();
builder.Services.AddScoped<SliderService>();
builder.Services.AddScoped<ReturnRequestService>();
builder.Services.AddScoped<IUserRepository, EfUserRepository>();
builder.Services.AddScoped<IRoleRepository, EfRoleRepository>();
builder.Services.AddScoped<IProductReviewRepository, EfProductReviewRepository>();
builder.Services.AddScoped<ProductReviewService>();
builder.Services.AddScoped<IFavoriteProductRepository, EfFavoriteProductRepository>();
builder.Services.AddScoped<FavoriteProductService>();
builder.Services.AddScoped<Verveo.DataAccess.SessionCartRepository>();
builder.Services.AddScoped<ICartRepository, EfCartRepository>();


var app = builder.Build();
// Eski ürün görsel yollarını düzelt
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<Verveo.DataAccess.VerveoDbContext>();
    var products = db.Products.Where(p => !string.IsNullOrEmpty(p.ImagePath) && !p.ImagePath.StartsWith("/images/")).ToList();
    foreach (var product in products)
    {
        product.ImagePath = "/images/" + product.ImagePath;
    }
    if (products.Count > 0)
    {
        db.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=604800");
    }
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
