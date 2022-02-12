using AutoMapper;
using StoreManager.Application.Model;
using System;

namespace StoreManager.Application.Dto
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<StoreDto, Store>();  // StoreDto --> Store
            CreateMap<Store, StoreDto>();  // Store --> StoreDto
            CreateMap<OfferDto, Offer>();
            CreateMap<Offer, OfferDto>();
            CreateMap<ProductDto, Product>();
            CreateMap<Product, ProductDto>();
        }
    }
}
