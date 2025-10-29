using DogsHouseService.Application.Middleware;
using DogsHouseService.Application.RateLimiting;
using DogsHouseService.Application.Services;
using DogsHouseService.Infrastructure;
using DogsHouseService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register DbContext with SQLite
builder.Services.AddDbContext<DogsDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// RateLimitOptions from config
builder.Services.Configure<RateLimitOptions>(
    builder.Configuration.GetSection("RateLimitOptions"));

// services
builder.Services.AddScoped<IDogRepository, DogRepository>();
builder.Services.AddScoped<IDogService, DogService>();
builder.Services.AddSingleton<IRateLimiter>(sp =>
{
    var options = sp.GetRequiredService<IOptions<RateLimitOptions>>().Value;
    return new InMemoryRateLimiter(options.RequestsPerSecond, TimeSpan.FromSeconds(1));
});

var app = builder.Build();

//checks limits
app.UseMiddleware<RateLimitingMiddleware>();

//catches any errors in business logic
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Auto-migrate DB 
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DogsDbContext>();
    db.Database.Migrate(); 
}

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();