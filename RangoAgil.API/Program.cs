using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RangoAgil.API.Data;
using RangoAgil.API.DTO;
using RangoAgil.API.Entities;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(
    options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnections")));

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

var rangoEndpoinrs = app.MapGroup("/rangos");
var rangosComIdEndpoints = rangoEndpoinrs.MapGroup("/{rangoId:int}");
var ingredienteEndpoints = rangosComIdEndpoints.MapGroup("ingredientes");

rangoEndpoinrs.MapGet("", async Task<Results<NoContent, Ok<IEnumerable<RangoDTO>>>> (
    AppDbContext context,
    IMapper mapper,
    [FromQuery(Name = "name")] string? rangoNome) =>
{
    var rangoEntity = await context.Rangos
                     .Where(x => rangoNome == null || x.Nome.ToLower().Contains(rangoNome.ToLower()))
                     .ToListAsync();

    if (rangoEntity.Count <= 0 || rangoEntity is null)
        return TypedResults.NoContent();
    else
        return TypedResults.Ok(mapper.Map<IEnumerable<RangoDTO>>(rangoEntity));
});

ingredienteEndpoints.MapGet("", async Task<Results<Ok<IEnumerable<IngredientesDTO>>, NoContent>> (
    AppDbContext context,
    IMapper mapper,
    int rangoId) =>
{
    
    var rangoEntity = mapper.Map<IEnumerable<IngredientesDTO>>((await context.Rangos
                .Include(i => i.Ingredientes)
                .FirstOrDefaultAsync(x => x.Id == rangoId))?.Ingredientes);

    if(rangoEntity is null)
        return TypedResults.NoContent();
    return TypedResults.Ok(rangoEntity);
});

rangosComIdEndpoints.MapGet("", async Task <Results<NoContent, Ok<RangoDTO>>> (
    AppDbContext context,
    IMapper mapper,
    int rangoId) =>
{
    var rangoEntity = mapper.Map<RangoDTO>(await context.Rangos.FirstOrDefaultAsync(r => r.Id == rangoId));

    if (rangoEntity is null)
        return TypedResults.NoContent();

    return TypedResults.Ok(rangoEntity);

}).WithName("GetRangos");


rangoEndpoinrs.MapPost("", async Task<CreatedAtRoute<RangoDTO>> (
   AppDbContext context,
   IMapper mapper,
   [FromBody] RangoParaCriacaoDTO rangoParaCriacaoDTO
  //  LinkGenerator linkGenerator,
  // HttpContext httpContext)
  ) =>
{
    var rangoEntity = mapper.Map<Rango>(rangoParaCriacaoDTO);
    context.Add(rangoEntity);
    await context.SaveChangesAsync();

    var rangoToReturn = mapper.Map<RangoDTO>(rangoEntity);
    return TypedResults.CreatedAtRoute(rangoToReturn, "GetRangos", new { rangoId = rangoToReturn.Id });


    // REFERENCIA PARA ESTUDO
    //  var rangoToReturn = mapper.Map<RangoDTO>(rangoEntity);
    //  var linkToReturn = linkGenerator.GetUriByName(httpContext, "GetRango", new { Id = rangoToReturn.Id });
    //  return  TypedResults.Created($"http://localhost:5026/rango/{rangoToReturn.Id}",rangoToReturn);

});

rangosComIdEndpoints.MapPut("", async Task<Results<NotFound, Ok>> (
    AppDbContext context,
    IMapper mapper,
    int rangoId,
    [FromBody] RangoParaEdicaoDTO rangoEdicaoDTO) =>
{
    var rangoEntity = await context.Rangos.FirstOrDefaultAsync(r => r.Id == rangoId);
    if (rangoEntity is null)
        return TypedResults.NotFound();

    mapper.Map(rangoEdicaoDTO, rangoEntity);
    await context.SaveChangesAsync();

    return TypedResults.Ok();
});

rangosComIdEndpoints.MapDelete("", async Task<Results<NotFound, NoContent>> (
    AppDbContext context,
    int rangoId) =>

{
    var rangoEntity = await context.Rangos.FirstOrDefaultAsync(r => r.Id == rangoId);
    if (rangoEntity is null)
        return TypedResults.NotFound();

    context.Rangos.Remove(rangoEntity);
    await context.SaveChangesAsync();

    return TypedResults.NoContent();
});



app.Run();