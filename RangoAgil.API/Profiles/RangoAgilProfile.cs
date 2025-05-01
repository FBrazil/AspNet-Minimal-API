using AutoMapper;
using RangoAgil.API.DTO;
using RangoAgil.API.Entities;

namespace RangoAgil.API.Profiles
{
    public class RangoAgilProfile : Profile
    {
        public RangoAgilProfile()
        {
            CreateMap<Rango, RangoDTO>().ReverseMap();
            CreateMap<Ingrediente, IngredientesDTO>()
                .ForMember(
                dto => dto.RangoId,
                w => w.MapFrom(i => i.Rangos.First().Id));
        }
    }
}
