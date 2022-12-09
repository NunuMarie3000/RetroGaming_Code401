using System.ComponentModel;

namespace RetroGaming.Areas.Admin.Models
{
  public class ImageFileModel
  {
    [DisplayName("Upload Image")]
    public string FileDetails { get; set; }
    public IFormFile File { get; set; }
  }
}
