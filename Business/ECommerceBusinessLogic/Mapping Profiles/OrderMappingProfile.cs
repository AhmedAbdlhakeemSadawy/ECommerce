using AutoMapper;
using ECommerceBuinessDTO;
using ECommerceDataAccessDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceBusinessLogic.Mapping_Profiles
{
    public class OrderMappingProfile : Profile
    {
        public OrderMappingProfile()
        {


            CreateMap<OrderDataDto, OrderBusinessDTO>()
             .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status));

           CreateMap<ProductBusinessDTO, OrderProductDataDto>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Id))  // Map Id to ProductId
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))  // Map Name to ProductName
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));

            CreateMap<OrderBusinessDTO, OrderDataDto>()
               .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status));

            CreateMap<OrderProductDataDto, ProductBusinessDTO>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ProductId))  // Map Id to ProductId
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))  // Map Name to ProductName
            .ForMember(dest => dest.StockQuantity, opt => opt.MapFrom(src => src.Quantity))
            .ForMember(dest => dest.Quantity, opt => opt.UseDestinationValue());
        }
    }
}
