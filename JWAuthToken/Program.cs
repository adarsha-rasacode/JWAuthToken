using JWAuthTokenDotNet9.Data;
using JWAuthTokenDotNet9.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;


using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi("Ideal"); 


//Db registration
builder.Services.AddDbContext<UserDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("UserDatabase")
    ));



// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["AppSettings:Issuer"],

            ValidateAudience = true,
            ValidAudience = builder.Configuration["AppSettings:Audience"],

            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:Token"]!)),
            ValidateIssuerSigningKey = true

        };
    });




//reg 
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Map OpenAPI & Scalar only in Development
    app.MapOpenApi();
    app.MapScalarApiReference();

    app.UseHttpsRedirection();


    // Important: order matters
    app.UseAuthentication();
    app.UseAuthorization();

    // Map controllers
    app.MapControllers();

    app.Run();
}
//https://localhost:7261/Scalar/Ideal