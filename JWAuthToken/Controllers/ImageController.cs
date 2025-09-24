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
        private readonly IWebHostEnvironment _env;

        public ImageController(UserDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
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
    }
}
