// ProductManager.API/Program.cs

using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ProductManager.API.Middleware;
using ProductManager.Application.Interfaces;
using ProductManager.Application.Mappings;
using ProductManager.Application.Services;
using ProductManager.Application.Validators;
using ProductManager.Core.DTOs;
using ProductManager.Core.Interfaces;
using ProductManager.Infrastructure.Data;
using ProductManager.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

/*
 * Servis kayıtları ve konfigürasyonlar
 */
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// PostgreSQL DbContext Configuration
builder.Services.AddDbContext<ProductManagerContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repository ve Service kayıtları
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IValidator<CreateProductDto>, CreateProductDtoValidator>();

// AutoMapper Configuration
builder.Services.AddAutoMapper(typeof(MappingProfile));

var app = builder.Build();

// Middleware pipeline konfigürasyonu
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Global exception middleware
app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();