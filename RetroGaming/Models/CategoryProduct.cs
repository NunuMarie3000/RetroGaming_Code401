using System.ComponentModel.DataAnnotations;

namespace RetroGaming.Models
{
  public class CategoryProduct
  {
    public int Id { get; set; }
    [Required]
    public int ProductId { get; set; }
    [Required]
    public int CategoryId { get; set; }
  }
}