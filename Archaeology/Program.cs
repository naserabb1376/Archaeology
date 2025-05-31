using DataLayer;
using Domains;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ArchaeologyDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Archaeology"));
});
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddMvcCore(option =>
{
    option.Filters.Add(typeof(CustomExceptionFilter));
});
builder.Services.AddScoped<ArchaeologyDbContext>();
builder.Services.AddDbContext<ArchaeologyDbContext>();
builder.Services.AddIdentity<User, UserRole>().AddEntityFrameworkStores<ArchaeologyDbContext>().AddDefaultTokenProviders()/*.AddErrorDescriber<CustomIdentityErrorDescriber>()*/;

builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
    options.Lockout.MaxFailedAccessAttempts = 2;
    options.Lockout.AllowedForNewUsers = true;

    // User settings.
    options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = false;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    options.LoginPath = "/Admin/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.LogoutPath = "/Admin/Account/Logout";
    options.SlidingExpiration = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();




    // Replace the app.UseEndpoints block with top-level route registrations as follows:

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
        name: "areas",
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");


builder.WebHost.ConfigureServices((hostContext, services) =>
{
    IServiceProvider serviceProvider = services.BuildServiceProvider().GetRequiredService<IServiceProvider>();

    using (var scope = serviceProvider.CreateScope())
    {
        var scopedServiceProvider = scope.ServiceProvider;
        var userManager = scopedServiceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = scopedServiceProvider.GetRequiredService<RoleManager<UserRole>>();
        MyIdentityDataInitializer.SeedData(userManager, roleManager);
    }
});
app.Run();
