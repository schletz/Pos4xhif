using AutoMapper;
using StoreManager.Application.Model;
using System;

namespace StoreManager.Webapp.Dto
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<StoreDto, Store>();  // StoreDto --> Store
            CreateMap<Store, StoreDto>();  // Store --> StoreDto
            CreateMap<OfferDto, Offer>()
                .ForMember(
                    o => o.Guid,
                    opt => opt.MapFrom(o => o.Guid == default ? Guid.NewGuid() : o.Guid));
        }
    }
}
