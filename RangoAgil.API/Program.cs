using Microsoft.EntityFrameworkCore;
using RangoAgil.API.Data;
using RangoAgil.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(
    options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnections")));

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

app.RegisterRangosEndpoints();
app.RegisterIngredientesEndpoints();


app.Run();