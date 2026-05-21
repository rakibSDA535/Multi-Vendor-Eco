using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shop111.Repositories;
using Shop111.Shared;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)//ApplicationUser er bodole IdentityUser silo
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultUI()
    .AddDefaultTokenProviders();


//==============Dependencey Injection.============(AddScoped or AddSingleton or AddTransient )//
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IHomeRepository, HomeRepository>();
builder.Services.AddTransient<ICartRepository, CartRepository>();
builder.Services.AddTransient<IUserOrderRepository, UserOrderRepository>();
builder.Services.AddTransient<IStockRepository, StockRepository>();
builder.Services.AddTransient<IGenreRepository, GenreRepository>();
builder.Services.AddTransient<IFileService, FileService>();
builder.Services.AddTransient<IProductRepository, ProductRepository>();
builder.Services.AddTransient<IReportRepository, ReportRepository>();
var app = builder.Build();



using (var scope = app.Services.CreateScope())//
{
    await DbSeeder.SeedDefaultData(scope.ServiceProvider);//
}
//or==============================================
//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;

//    await DbSeeder.SeedDefaultData(services);

//    var roleManager =
//        services.GetRequiredService<RoleManager<IdentityRole>>();

//    string[] roles =
//    {
//        nameof(Roles.Admin),
//        nameof(Roles.Manager),
//        nameof(Roles.User)
//    };

//    foreach (var role in roles)
//    {
//        bool exists =
//            await roleManager.RoleExistsAsync(role);

//        if (!exists)
//        {
//            await roleManager.CreateAsync(
//                new IdentityRole(role)
//            );
//        }
//    }
//}
//or
// রোল এবং ডিফল্ট ডাটা সিড করার জন্য এই ব্লকটি ব্যবহার করুন
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // ১. ডিফল্ট ডাটা সিড করা (যেমন অ্যাডমিন ইউজার বা ক্যাটাগরি)
        await DbSeeder.SeedDefaultData(services);

        // ২. রোলগুলো নিশ্চিত করা (Admin, Manager, User)
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        string[] roles = { "Admin", "Manager", "User" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}
//or==============================================

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();//
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();
