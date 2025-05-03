using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RangoAgil.API.Data;
using RangoAgil.API.DTO;
using RangoAgil.API.Entities;

namespace RangoAgil.API.EndpointHandlers;

public static class RangosHandlers
{
    public static async Task<Results<NoContent, Ok<IEnumerable<RangoDTO>>>> GetRangoAsync
        (AppDbContext context,
        IMapper mapper,
        [FromQuery(Name = "name")] string? rangoNome)
    {
        var rangoEntity = await context.Rangos
                         .Where(x => rangoNome == null || x.Nome.ToLower().Contains(rangoNome.ToLower()))
                         .ToListAsync();

        if (rangoEntity.Count <= 0 || rangoEntity is null)
            return TypedResults.NoContent();
        else
            return TypedResults.Ok(mapper.Map<IEnumerable<RangoDTO>>(rangoEntity));
    }


    public static async Task<Results<NoContent, Ok<RangoDTO>>> GetRangoById
        (AppDbContext context,
        IMapper mapper,
        int rangoId)
    {
        var rangoEntity = mapper.Map<RangoDTO>(await context.Rangos.FirstOrDefaultAsync(r => r.Id == rangoId));

        if (rangoEntity is null)
            return TypedResults.NoContent();

        return TypedResults.Ok(rangoEntity);

    }


    public static async Task<CreatedAtRoute<RangoDTO>> CreateRangoAsync
         (AppDbContext context,
         IMapper mapper,
         [FromBody] RangoParaCriacaoDTO rangoParaCriacaoDTO
            //  LinkGenerator linkGenerator,
             // HttpContext httpContext)
         )
    {
        var rangoEntity = mapper.Map<Rango>(rangoParaCriacaoDTO);
        context.Add(rangoEntity);
        await context.SaveChangesAsync();

        var rangoToReturn = mapper.Map<RangoDTO>(rangoEntity);
        return TypedResults.CreatedAtRoute(rangoToReturn, "GetRangos", new
        {
            rangoId = rangoToReturn.Id
        });


        // REFERENCIA PARA ESTUDO
        //  var rangoToReturn = mapper.Map<RangoDTO>(rangoEntity);
        //  var linkToReturn = linkGenerator.GetUriByName(httpContext, "GetRango", new { Id = rangoToReturn.Id });
        //  return  TypedResults.Created($"http://localhost:5026/rango/{rangoToReturn.Id}",rangoToReturn);

    }

    public static async Task<Results<NotFound, Ok>> UpdateRangoAsync
        (AppDbContext context,
        IMapper mapper,
        int rangoId,
        [FromBody] RangoParaEdicaoDTO rangoEdicaoDTO)
    {
        var rangoEntity = await context.Rangos.FirstOrDefaultAsync(r => r.Id == rangoId);
        if (rangoEntity is null)
            return TypedResults.NotFound();

        mapper.Map(rangoEdicaoDTO, rangoEntity);
        await context.SaveChangesAsync();

        return TypedResults.Ok();
    }

    public static async Task<Results<NotFound, NoContent>> DeleteRangoAsync
        (AppDbContext context,
        int rangoId)

    {
        var rangoEntity = await context.Rangos.FirstOrDefaultAsync(r => r.Id == rangoId);
        if (rangoEntity is null)
            return TypedResults.NotFound();

        context.Rangos.Remove(rangoEntity);
        await context.SaveChangesAsync();

        return TypedResults.NoContent();
    }
}

