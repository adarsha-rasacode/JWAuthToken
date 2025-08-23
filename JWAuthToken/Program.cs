using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi("Ideal");

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(); // This line is used to enable Scalar API references in development mode -> download the Scalar API reference file in the preference  https://localhost:7261/openapi/v1.json
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();     
