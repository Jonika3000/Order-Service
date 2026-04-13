using OrderService.Application.Services;
using OrderService.Infrastructure.Persistence;
using OrderService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("OrderDatabase")));
builder.Services.AddScoped<IOrderRepository, PostgresOrderRepository>();
builder.Services.AddScoped<OrderApplicationService>();

var app = builder.Build();

app.MapGrpcService<OrderGrpcService>();
app.MapGet("/",
    () =>
        "Use a gRPC client to work with the Order service.");

var dbContext = app.Services.GetRequiredService<OrderDbContext>();
dbContext.Database.Migrate();

app.Run();
