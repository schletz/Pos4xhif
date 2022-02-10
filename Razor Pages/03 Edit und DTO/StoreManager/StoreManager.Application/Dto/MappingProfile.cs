using AutoMapper;
using StoreManager.Application.Model;

namespace StoreManager.Application.Dto
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<StoreDto, Store>();  // StoreDto --> Store
            CreateMap<Store, StoreDto>();  // Store --> StoreDto
        }
    }
}
