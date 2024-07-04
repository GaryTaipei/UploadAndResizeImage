using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace UploadingImagesWorkspace.Models
{
    public class ImageUploadViewModel
    {
        [Required(ErrorMessage = "Please select an image file.")]
        [Display(Name = "Image File")]
        public IFormFile ImageFile {  get; set; }

    }
}
