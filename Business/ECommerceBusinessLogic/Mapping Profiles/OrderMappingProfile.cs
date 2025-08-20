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

        }
    }
}
