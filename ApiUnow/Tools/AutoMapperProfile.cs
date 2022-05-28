using ApiUnow.DTO;
using ApiUnow.Models;
using AutoMapper;

namespace ApiUnow.Tools
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Cita, CitaDTO>().ReverseMap();
            CreateMap<ClienteDTO, Cliente>()
                .ForMember(x => x.Citas, options => options.Ignore()).ReverseMap();
            CreateMap<TallerDTO, Taller>()
                .ForMember(x => x.Citas, options => options.Ignore()).ReverseMap();
        }
    }
}
