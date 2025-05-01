using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RangoAgil.API.Data;
using RangoAgil.API.DTO;
using RangoAgil.API.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(
    options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnections")));

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/rangos", async Task <Results<NoContent, Ok <IEnumerable<RangoDTO>>>>(
    AppDbContext context,
    IMapper mapper,
    [FromQuery(Name = "name")] string? rangoNome) =>
{
    var rangoEntity = await context.Rangos
                     .Where(x => rangoNome == null|| x.Nome.ToLower().Contains(rangoNome.ToLower()))
                     .ToListAsync();

    if (rangoEntity.Count <= 0 || rangoEntity is null)
        return TypedResults.NoContent();
    else
        return TypedResults.Ok(mapper.Map<IEnumerable<RangoDTO>>(rangoEntity));
});

app.MapGet("/rango/{rangoId:int}/ingredientes", async (
    AppDbContext context, 
    IMapper mapper,
    int rangoId) =>
{
    return mapper.Map<IEnumerable<IngredientesDTO>> ((await context.Rangos
                .Include(i => i.Ingredientes)
                .FirstOrDefaultAsync(x=> x.Id == rangoId))?.Ingredientes);
});

app.MapGet("/rango/{id:int}", async (
    AppDbContext context,
    IMapper mapper,
    int id) =>
{
    return mapper.Map<RangoDTO> (await context.Rangos.FirstOrDefaultAsync(r => r.Id == id));
});

app.MapGet("/rango/{nome}", (AppDbContext context, string nome) =>
{
    return context.Rangos.FirstOrDefault(r => r.Nome == nome);
});

app.Run();