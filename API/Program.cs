using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<StoreContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
    options=>
    {
        options.EnableRetryOnFailure(
             maxRetryCount: 5,
                    maxRetryDelay: System.TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null
        );
    });
    
    
}
);


//Everything before this is service
var app = builder.Build();
//Everything after this is middleweare

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

// app.UseHttpsRedirection();

// app.UseAuthorization();

app.MapControllers();

app.Run();
