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
            CreateMap<ProductBusinessDTO, ProductDataDto>()
                 .ForMember(dest => dest.StockQuantity, opt => opt.MapFrom(src => src.StockQuantity));

            CreateMap<ProductDataDto, ProductBusinessDTO>()
                 .ForMember(dest => dest.StockQuantity, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Quantity, opt => opt.UseDestinationValue());

        }

    }
}
