using ExamManager.App.Entities;
using ExamManager.App.Mappings;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StoreManager.Application.Infrastructure;

using (var context = new ExamContext())
{
    context.Database.EnsureDeleted();
    context.Database.EnsureCreated();
    context.Seed(new CryptService());
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<ExamContext>();
builder.Services.AddAutoMapper(typeof(DtoMappings));

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(
            builder =>
            {
                builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
            });
    });
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCors();
}
app.UseHttpsRedirection();
app.MapControllers();
app.UseStaticFiles();
app.MapFallbackToFile("index.html");
app.Run();
