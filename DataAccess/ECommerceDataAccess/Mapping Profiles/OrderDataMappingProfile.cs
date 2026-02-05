using AutoMapper;
using ECommerceDataAccess.DataEntities;
using ECommerceDataAccessDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceDataAccess.Mapping_Profiles
{
    public class OrderDataMappingProfile : Profile
    {
        public OrderDataMappingProfile()
        {
            CreateMap<OrderDataDto, Order>()
                .ForMember(dest => dest.orderProducts, opt => opt.MapFrom(src => src.products));


            CreateMap<OrderProductDataDto, OrderProduct>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))  // Map Id to ProductId
            .ForMember(dest => dest.ProductQuantity, opt => opt.MapFrom(src => src.Quantity));

            CreateMap<Order, OrderDataDto>()
                .ForMember(dest => dest.products, opt => opt.MapFrom(src => src.orderProducts));


            CreateMap<OrderProduct , OrderProductDataDto>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))  // Map Id to ProductId
             .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.ProductQuantity));
        }
    }
}
