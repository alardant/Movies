using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Movies.Data;
using Movies.Models;
using Movies.Repository;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Add Entity Framework
builder.Services.AddDbContext<DataContext>(options =>
  options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();

// Register SeedData as a service
builder.Services.AddScoped<SeedData>();

// Configure Logging
builder.Logging.ClearProviders(); // Remove the default logging providers

Log.Logger = new LoggerConfiguration()
    .WriteTo.File("app.log")
    .CreateLogger();

builder.Logging.AddSerilog();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<MovieRepository>();
builder.Services.AddScoped<UserRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Populate the database with seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<DataContext>();
    dbContext.Database.Migrate(); // Ensure the database is created/updated

    var seedData = services.GetRequiredService<SeedData>();
    await seedData.SeedDataAsync();
}

app.Run();
