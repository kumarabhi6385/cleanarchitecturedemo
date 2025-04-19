using System.Threading.RateLimiting;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Polly;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddDbContext<ProductDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseSqlite("Data Source=products.db"));

// Register the combined Polly policy
builder.Services.AddSingleton<IAsyncPolicy>(ResiliencePolicies.WrapRetryAndCircuitBreaker());

builder.Services.AddScoped<IProductRepository, SqlProductRepository>(); // Switch to InMemoryProductRepository for in-memory DB
//builder.Services.AddSingleton<IProductRepository, InMemoryProductRepository>(); 
builder.Services.AddMediatR(typeof(CreateProductCommandHandler).Assembly);

builder.Services.AddOpenApi();
builder.Services.AddOpenApiDocument();

// Add ASP.NET Core Rate Limiting (for .NET 7+)
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter("global", _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 1, // requests
            Window = TimeSpan.FromMinutes(1),
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 0
        }));
    options.RejectionStatusCode = 429;
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi();
}

app.UseRouting();

app.UseRateLimiter();

// Call the middleware to map endpoints
app.MapEndpoints();

app.UseHttpsRedirection();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
    db.Database.EnsureCreated();
}

app.Run();
