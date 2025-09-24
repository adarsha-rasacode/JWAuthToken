using JWAuthTokenDotNet9.Entities;
using Microsoft.EntityFrameworkCore;

namespace JWAuthTokenDotNet9.Data
{
    public class UserDbContext : DbContext
    {
        // Constructor
        public UserDbContext(DbContextOptions<UserDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Image> Images { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Make Username case-sensitive
            modelBuilder.Entity<User>()
                .Property(u => u.Username)
                .UseCollation("SQL_Latin1_General_CP1_CS_AS");
        }
        


    }
}
