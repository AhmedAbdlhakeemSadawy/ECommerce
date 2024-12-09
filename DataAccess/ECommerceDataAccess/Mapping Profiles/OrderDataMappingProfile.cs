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
            CreateMap<Order, OrderDataDto>()
                .ForMember(dest => dest.products, opt => opt.MapFrom(src => src.orderProducts));
        }
    }
}
