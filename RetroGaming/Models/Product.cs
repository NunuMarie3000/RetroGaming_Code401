using System.ComponentModel.DataAnnotations;

namespace RetroGaming.Models
{
  public class Product
  {
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    public Category Category { get; set; }
    public int CategoryId { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; }
    public Condition Condition { get; set; }
    public Rating Rating { get; set; }
    public string ImagePath { get; set; }

  }
  public enum Condition
  {
    New = 5, UsedLikeNew = 4, UsedGood = 3, UsedFair = 2, Poor = 1
  }
  public enum Rating
  {
    Low = 1, LowMedium = 2, Medium = 3, MediumHigh = 4, High = 5
  }
}