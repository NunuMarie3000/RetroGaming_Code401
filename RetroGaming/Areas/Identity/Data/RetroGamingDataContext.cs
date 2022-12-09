using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RetroGaming.Areas.Identity.Data;
using RetroGaming.Models;

namespace RetroGaming.Areas.Identity.Data;

public class RetroGamingDataContext: IdentityDbContext<ApplicationUser>
{
  public RetroGamingDataContext( DbContextOptions<RetroGamingDataContext> options )
      : base(options)
  {
  }

  protected override void OnModelCreating( ModelBuilder builder )
  {
    base.OnModelCreating(builder);
    // Customize the ASP.NET Identity model and override the defaults if needed.
    // For example, you can rename the ASP.NET Identity table names and more.
    // Add your customizations after calling base.OnModelCreating(builder);
  }

  public DbSet<Category> Categories { get; set; }
  public DbSet<CategoryProduct> CategoryProducts { get; set; }
  public DbSet<Product> Products { get; set; }
}
