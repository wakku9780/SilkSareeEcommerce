using DinkToPdf.Contracts;
using DinkToPdf;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SilkSareeEcommerce.Data;
using SilkSareeEcommerce.Models;
using SilkSareeEcommerce.Repositories;
using SilkSareeEcommerce.Services;
using Microsoft.Extensions.Options;
using Npgsql.EntityFrameworkCore.PostgreSQL;


var builder = WebApplication.CreateBuilder(args);

 

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();


builder.Services.AddDistributedMemoryCache(); // Required for session to work
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Optional timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


// Load native wkhtmltox library
var wkhtmltoxPath = Path.Combine(Directory.GetCurrentDirectory(), "wkhtmltox", "libwkhtmltox.dll");
var context = new CustomAssemblyLoadContext();
context.LoadUnmanagedLibrary(wkhtmltoxPath);

// Register DinkToPdf converter
builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));


//Add DbContext using connection string from configuration
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("dbcs")));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseNpgsql(connectionString));






//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
//    new MySqlServerVersion(new Version(8, 0, 36)))); // or your MySQL version


//options.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection"));



//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
//    new MySqlServerVersion(new Version(8, 0, 36)))); // ya 5.7 agar FreeSQL MySQL 5.7 hai


// Add Identity services
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

builder.Services.AddScoped<PdfService>();

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IWishlistRepository,WishlistRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserOrderRepository, UserOrderRepository>();
builder.Services.AddScoped<ICouponRepository, CouponRepository>();

builder.Services.AddScoped<IProductReviewRepository, ProductReviewRepository>();
builder.Services.AddScoped<ProductReviewService>();







builder.Services.AddSingleton<CloudinaryService>();
builder.Services.AddScoped<CouponService>();
builder.Services.AddSingleton<FirebaseService>();


builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<PaymentService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<WishlistService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<UserOrderService>();
builder.Services.AddScoped<PayPalService> ();


builder.Services.AddScoped<UserService>();

// Add Google Authentication
//builder.Services.AddAuthentication()
//    .AddGoogle(options =>
//    {
//        options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
//        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
//    });

// Configure Cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

var app = builder.Build();

// **Seed Roles & Admin User**
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedRolesAndAdmin(services);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseSession();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Authentication & Authorization Middleware
app.UseAuthentication();
app.UseAuthorization();

// Map Route explicitly for AccountController
app.MapControllerRoute(
    name: "account",
    pattern: "Account/{action=Login}/{id?}",
    defaults: new { controller = "Account" }
);

// Define the default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

// **Seed Roles & Admin User Method**
async Task SeedRolesAndAdmin(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    string[] roles = { "Admin", "User" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    string adminEmail = "admin@example.com";
    string adminPassword = "Admin@123";

    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new ApplicationUser { UserName = adminEmail, Email = adminEmail };
        var result = await userManager.CreateAsync(adminUser, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}






//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using SilkSareeEcommerce.Data;
//using SilkSareeEcommerce.Models;

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//builder.Services.AddControllersWithViews();

//// Add DbContext using connection string from configuration
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("dbcs")));

//// Add Identity services
//builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
//    .AddEntityFrameworkStores<ApplicationDbContext>()
//    .AddDefaultTokenProviders();

//// Add Google Authentication
//builder.Services.AddAuthentication()
//    .AddGoogle(options =>
//    {
//        options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
//        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
//    });

//// Configure Cookie settings
//builder.Services.ConfigureApplicationCookie(options =>
//{
//    options.LoginPath = "/Account/Login";
//    options.AccessDeniedPath = "/Account/AccessDenied";
//});

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error");
//    app.UseHsts();  // HTTP Strict Transport Security (HSTS)
//}

//app.UseHttpsRedirection();
//app.UseStaticFiles();
//app.UseRouting();

//// Authentication & Authorization Middleware
//app.UseAuthentication();  // Add Authentication
//app.UseAuthorization();   // Add Authorization


//// Map Route explicitly for AccountController
//app.MapControllerRoute(
//    name: "account",
//    pattern: "Account/{action=Login}/{id?}",  // Default action is Login
//    defaults: new { controller = "Account" }  // Default controller is Account
//);

//// Define the default route
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");

//app.Run();










//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using SilkSareeEcommerce.Data;
//using SilkSareeEcommerce.Models;

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//builder.Services.AddControllersWithViews();

//var provider = builder.Services.BuildServiceProvider();
//var config = provider.GetRequiredService<IConfiguration>();
//builder.Services.AddDbContext<ApplicationDbContext>(item => item.UseSqlServer(config.GetConnectionString("dbcs")));


//// Add Identity
//builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
//    .AddEntityFrameworkStores<ApplicationDbContext>()
//    .AddDefaultTokenProviders();


//// Google Authentication
//builder.Services.AddAuthentication()
//    .AddGoogle(options =>
//    {
//        options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
//        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
//    });

//builder.Services.ConfigureApplicationCookie(options =>
//{
//    options.LoginPath = "/Account/Login";
//    options.AccessDeniedPath = "/Account/AccessDenied";
//});

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}

//app.UseHttpsRedirection();
//app.UseStaticFiles();

//app.UseRouting();

//app.UseAuthorization();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");

//app.Run();

