using AutoMapper;
using MagicVilla_API.Modelos;
using MagicVilla_API.Modelos.Dto;

namespace MagicVilla_API
{
    public class MappingConfig : Profile
    {
        //Se necesita crear constructor
        public MappingConfig()
        {
            //fuente villa, destino villadto
            CreateMap<Villa, VillaDto>();
            //lo inverso
            CreateMap <VillaDto, Villa>();

            //es lo mismo que arriba pero en una sola linea de codigo
            CreateMap<Villa, VillaCreateDto>().ReverseMap();
            CreateMap<Villa, VillaUpdateDto>().ReverseMap();


        }
    }
}
