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
            CreateMap<OrderDataDto, Order>();
            CreateMap<Order, OrderDataDto>();
        }
    }
}
