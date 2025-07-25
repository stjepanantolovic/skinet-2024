using API.Middleware;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<StoreContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
    options =>
    {
        options.EnableRetryOnFailure(
             maxRetryCount: 5,
                    maxRetryDelay: System.TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null
        );
    });
}
);

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddCors(options=>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins("http://loclahost:4200", "https://loclahost:4200")
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

//Everything before this is service
var app = builder.Build();
//Everything after this is middleweare

// Configure the HTTP request pipeline.

// app.UseHttpsRedirection();

// app.UseAuthorization();

app.UseMiddleware<ExceptionMiddleware>();

app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowCredentials()
    .WithOrigins("http://localhost:4200","https://localhost:4200"));

app.MapControllers();

try
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<StoreContext>();
    await context.Database.MigrateAsync();
    await StoreContextSeed.SeedAsync(context);
}
catch (Exception ex)
{
    Console.WriteLine(ex);
    throw;
}

app.Run();
