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

builder.Services.AddSingleton<IAsyncPolicy>(GetRetryPolicy());
builder.Services.AddSingleton<IAsyncPolicy>(GetCircuitBreakerPolicy());

builder.Services.AddScoped<IProductRepository, SqlProductRepository>(); // Switch to InMemoryProductRepository for in-memory DB
//builder.Services.AddSingleton<IProductRepository, InMemoryProductRepository>(); 
builder.Services.AddMediatR(typeof(CreateProductCommandHandler).Assembly);

builder.Services.AddOpenApi();
builder.Services.AddOpenApiDocument();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi();
}

app.UseRouting();

// Call the middleware to map endpoints
app.MapEndpoints();

app.UseHttpsRedirection();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
    db.Database.EnsureCreated();
}

app.Run();

IAsyncPolicy GetRetryPolicy()
{
    return Policy.Handle<SqliteException>()
        .WaitAndRetryAsync(new[]
            {
                 TimeSpan.FromSeconds(1),
                 TimeSpan.FromSeconds(5),
                 TimeSpan.FromSeconds(10)
            });
}

IAsyncPolicy GetCircuitBreakerPolicy()
{
    return Policy.Handle<SqliteException>()
                 .CircuitBreakerAsync(2, TimeSpan.FromMinutes(1));
}


