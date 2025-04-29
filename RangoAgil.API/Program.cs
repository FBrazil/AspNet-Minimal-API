using Microsoft.EntityFrameworkCore;
using RangoAgil.API.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(
    options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnections")));


var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
