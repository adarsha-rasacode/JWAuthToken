using System.Security.Claims;
using JWAuthTokenDotNet9.Data;
using JWAuthTokenDotNet9.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JWAuthTokenDotNet9.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly UserDbContext _context;

        public ImageController(UserDbContext context)
        {
            _context = context;
        }

        // 1️⃣ Upload image (Admin only)
        [HttpPost("upload")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Upload([FromForm] IFormFile? file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var filePath = Path.Combine(uploadsFolder, file.FileName);

            try
            {
                // Save the file first
                await using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Create DB record
                var image = new Image
                {
                    FileName = file.FileName,
                    FilePath = $"/uploads/{file.FileName}",
                    UploadedBy = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value)
                };

                _context.Images.Add(image);
                await _context.SaveChangesAsync();

                return Ok(new { image.Id, image.FilePath });
            }
            catch (Exception ex)
            {
                // Delete file if exception occurs
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                var msg = $"Error uploading file: {ex.Message}";
                return StatusCode(500, new { msg });
            }
        }


        // 2️⃣ Get all images (User or Admin)
        [HttpGet]
        [Authorize] // both Admin & User can see
        public async Task<IActionResult> GetAll()
        {
            var images = await _context.Images
                .OrderByDescending(i => i.UploadedAt)
                .ToListAsync();

            return Ok(images);
        }


        //endpoint : https://localhost:7261/api/image/f210aab7-a238-4a71-91d8-acf8c2a196f2
        //FATCH IMAGE USING ID (User or Admin)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetImage(Guid id)
        {
            var image = await _context.Images.FindAsync(id);
            if (image == null)
                return NotFound("Image not found");

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", image.FileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound("File not found on disk");

            // Determine content type
            var ext = Path.GetExtension(filePath).ToLower();
            var contentType = ext switch
            {
                ".jpg" or ".jpeg" or ".jfif" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };

            var bytes = await System.IO.File.ReadAllBytesAsync(filePath);

            // Force browser to render inline instead of download
            Response.Headers.Add("Content-Disposition", $"inline; filename={image.FileName}");

            return File(bytes, contentType);
        }

        //CONTROLER TO FETCH IMAGE BY THE FILE NAME 
        [HttpGet("byname/{fileName}")]
        public async Task<IActionResult> GetImageByFileName(string fileName)
        {
            // Optional: decode URL in case of spaces or special chars
            fileName = Uri.UnescapeDataString(fileName);

            // Find image in DB (optional)
            var image = await _context.Images.FirstOrDefaultAsync(i => i.FileName == fileName);
            if (image == null)
                return NotFound("Image not found");

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", image.FileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound("File not found on disk");

            var ext = Path.GetExtension(filePath).ToLower();
            var contentType = ext switch
            {
                ".jpg" or ".jpeg" or ".jfif" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };

            Response.Headers.Add("Content-Disposition", $"inline; filename={image.FileName}");
            var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(bytes, contentType);
        }



    }
}
