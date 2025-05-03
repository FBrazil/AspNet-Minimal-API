using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RangoAgil.API.Data;
using RangoAgil.API.DTO;

namespace RangoAgil.API.EndpointHandlers
{
    public static class IngredientesHandlers
    {
        public static async Task<Results<Ok<IEnumerable<IngredientesDTO>>, NoContent>> GetIngredientesAsync
            (AppDbContext context,
            IMapper mapper,
            int rangoId)
            {

            var rangoEntity = mapper.Map<IEnumerable<IngredientesDTO>>((await context.Rangos
                        .Include(i => i.Ingredientes)
                        .FirstOrDefaultAsync(x => x.Id == rangoId))?.Ingredientes);

            if (rangoEntity is null)
                return TypedResults.NoContent();
            return TypedResults.Ok(rangoEntity);
        }

    }
}
