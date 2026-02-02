using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.EquivalencyExpression;
using ECommerceBuinessDTO;
using ECommerceDataAccessDTO;

namespace ECommerceBusinessLogic.Mapping_Profiles
{
    public class ProductMappingProfile : Profile
    {

        public ProductMappingProfile()
        {

            CreateMap<ProductBusinessDTO, ProductDataDto>();

            CreateMap<ProductDataDto, ProductBusinessDTO>()
                .EqualityComparison((src, dest) => src.Id == dest.Id)
                .ForMember(d => d.Quantity, o => o.Ignore())
                .ForMember(dest => dest.StockQuantity, opt => opt.MapFrom(src => src.StockQuantity));



        }

    }
}
