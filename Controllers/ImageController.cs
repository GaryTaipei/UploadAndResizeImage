
using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using UploadingImagesWorkspace.Models;

namespace UploadingImagesWorkspace.Controllers
{
    public class ImageController : Controller
    {
        private readonly IWebHostEnvironment _environment;


        public ImageController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }


        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Upload(ImageUploadViewModel model)
        {
            if(ModelState.IsValid)
            {
                if(model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    //Check file size < 2MB
                    if (model.ImageFile.Length > 2 * 1024 * 1024) 
                    {
                        ModelState.AddModelError("ImageFile", "The file size should not exceed 2MB.");
                        return View(model);
                    }

                    // Check file extension
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                    var fileExtension = Path.GetExtension(model.ImageFile.FileName).ToLowerInvariant();
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        ModelState.AddModelError("ImageFile", "Only .jpg, .jpeg, and .png files are allowed.");
                        return View(model);
                    }

                    try 
                    {
                        // Create uploads folder if it doesn't exist
                        var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                        if (!Directory.Exists(uploadsFolder)) 
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        // Generate a unique filename
                        var uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        // Resize and save the image
                        using (var image = Image.FromStream(model.ImageFile.OpenReadStream()))
                        {
                            var resized = new Bitmap(120, 120);
                            using (var graphics = Graphics.FromImage(resized))
                            {
                                graphics.DrawImage(image, 0, 0,120,120);

                            }
                            resized.Save(filePath, fileExtension==".png"? ImageFormat.Png : ImageFormat.Jpeg);
                        }

                        ViewBag.Message = "Image uploaded and resized successfully.";
                        return View();

                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "An error occurred while processing the image: " + ex.Message);
                    }
                }
                else
                {
                    ModelState.AddModelError("ImageFile", "Please select a file to upload.");
                }
            }
            return View(model);
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
