using RangoAgil.API.EndpointHandlers;

namespace RangoAgil.API.Extensions
{
    public static class EndpointRouteBuilderExtensions
    {

        
        
        public static void RegisterRangosEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
        {
            var rangoEndpoinrs = endpointRouteBuilder.MapGroup("/rangos");
            var rangosComIdEndpoints = rangoEndpoinrs.MapGroup("/{rangoId:int}");

            rangoEndpoinrs.MapGet("", RangosHandlers.GetRangoAsync);
            rangosComIdEndpoints.MapGet("", RangosHandlers.GetRangoById).WithName("GetRangos");
            rangoEndpoinrs.MapPost("", RangosHandlers.CreateRangoAsync);
            rangosComIdEndpoints.MapPut("", RangosHandlers.UpdateRangoAsync);
            rangosComIdEndpoints.MapDelete("", RangosHandlers.DeleteRangoAsync);
        }

        public static void RegisterIngredientesEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
        {
            var ingredienteEndpoints = endpointRouteBuilder.MapGroup("/rangos/{Id:int}/ingredientes");
            ingredienteEndpoints.MapGet("", IngredientesHandlers.GetIngredientesAsync);
        }
    }
}
