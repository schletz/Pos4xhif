using AutoMapper;
using StoreManager.Application.Model;

namespace StoreManager.Webapp.Dto
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<StoreDto, Store>();  // StoreDto --> Store
            CreateMap<Store, StoreDto>();  // Store --> StoreDto
            CreateMap<Offer, OfferDto>(); 
            CreateMap<OfferDtoBase, Offer>();
        }
    }
}
