using System;

namespace JWAuthTokenDotNet9.Entities
{
    public class Image
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? FileName { get; set; }  // Original file name
        public string? FilePath { get; set; } // Path in the server
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public Guid UploadedBy { get; set; }
        public User? UploadedByUser { get; set; }
    }
}
