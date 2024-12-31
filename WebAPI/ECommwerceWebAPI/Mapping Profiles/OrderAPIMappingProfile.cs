using AutoMapper;
using ECommerceBuinessDTO;
using ECommerceDataAccessDTO;
using ECommerceWebApiDto;

namespace ECommwerceWebAPI.Mapping_Profiles
{
    public class OrderAPIMappingProfile : Profile
    {
        public OrderAPIMappingProfile()
        {
            CreateMap<OrderRequestDto, OrderBusinessDTO>();


            CreateMap<ProductRequestDto, ProductBusinessDTO>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))  // Map Id to ProductId
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));
        }
    }
}
