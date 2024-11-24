using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ECommerceBuinessDTO;
using ECommerceDataAccessDTO;

namespace ECommerceBusinessLogic.Mapping_Profiles
{
    public class ProductMappingProfile : Profile
    {
        public ProductMappingProfile()
        {
           CreateMap<ProductBusinessDTO, ProductDataDto>();

           CreateMap<ProductDataDto, ProductBusinessDTO>();

        }



    }
}
