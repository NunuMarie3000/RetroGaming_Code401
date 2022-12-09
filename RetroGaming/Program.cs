using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RetroGaming.Areas.Admin.Models.Interfaces;
using RetroGaming.Areas.Admin.Models.Options;
using RetroGaming.Areas.Admin.Models.Services;
using RetroGaming.Areas.Identity.Data;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("RetroGamingDataContextConnection") ?? throw new InvalidOperationException("Connection string 'RetroGamingDataContextConnection' not found.");

builder.Services.AddDbContext<RetroGamingDataContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
  .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<RetroGamingDataContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();

// getting rid of making values not explicitly stated [required] from being required
// so ModelState.IsValid is true when we create a new category and don't include a list of categoryproducts
builder.Services.AddControllers(
  options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);

builder.Services.AddTransient<IImageService, ImageService>();
builder.Services.Configure<AzureOptions>(builder.Configuration.GetSection("AzureStorageConfig"));

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
app.UseAuthentication();;

app.UseAuthorization();

// for admin area
app.UseEndpoints(endpoints =>
{
  endpoints.MapControllerRoute(
    name: "Admin",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
  );
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
