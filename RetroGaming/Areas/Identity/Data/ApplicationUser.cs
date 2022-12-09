using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace RetroGaming.Areas.Identity.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser: IdentityUser
{
  [StringLength(250)]
  public string FirstName { get; set; }
  [StringLength(250)]
  public string LastName { get; set; }
  [StringLength(250)]
  public string Email { get; set; }
  [StringLength(250)]
  public string Address1 { get; set; }
  [StringLength(250)]
  public string Address2 { get; set; }
  [StringLength(250)]
  public string ZipCode { get; set; }
  //public List<Product> ShoppingCart { get; set; }
}

