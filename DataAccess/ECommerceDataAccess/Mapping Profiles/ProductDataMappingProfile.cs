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
    public class ProductDataMappingProfile :Profile
    {
        public ProductDataMappingProfile()
        {
            CreateMap<ProductDataDto, Product>();
            CreateMap<Product, ProductDataDto>();
        }
    }
}
