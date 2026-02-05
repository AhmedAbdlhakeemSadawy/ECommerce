using ECommerceBuinessDTO;
using ECommerceDataAccessDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceBusinessLogic.Mapping
{
    public static class ProductBusinessMapping
    {
        public static ProductBusinessDTO ToBusinessDto(this ProductDataDto dataDto)
        {
            if (dataDto is null) return null;

            return new ProductBusinessDTO
            {
                Id = dataDto.Id,
                Name = dataDto.name,
                Price = dataDto.price,

                // Business-specific meaning
                StockQuantity = dataDto.StockQuantity,

                // Quantity is business-context specific
                // Default = 0 (e.g., before adding to cart)
                Quantity = 0
            };
        }

        // Business -> Data
        public static ProductDataDto ToDataDto(this ProductBusinessDTO businessDto)
        {
            if (businessDto is null) return null;

            return new ProductDataDto
            {
                Id = businessDto.Id,
                name = businessDto.Name,
                price = businessDto.Price,
                StockQuantity = businessDto.StockQuantity,

                // description does not exist in Business DTO
                description = null
            };
        }

        // ---------- List mapping (explicit, non-generic) ----------

        // Data list -> Business list
        public static List<ProductBusinessDTO> ToBusinessDtos(this IEnumerable<ProductDataDto> dataDtos)
        {
            if (dataDtos is null) return new List<ProductBusinessDTO>();

            if (dataDtos is ICollection<ProductDataDto> col)
            {
                var result = new List<ProductBusinessDTO>(col.Count);
                foreach (var d in col)
                    result.Add(d.ToBusinessDto());
                return result;
            }

            var list = new List<ProductBusinessDTO>();
            foreach (var d in dataDtos)
                list.Add(d.ToBusinessDto());
            return list;
        }

        // Business list -> Data list
        public static List<ProductDataDto> ToDataDtos(this IEnumerable<ProductBusinessDTO> businessDtos)
        {
            if (businessDtos is null) return new List<ProductDataDto>();

            if (businessDtos is ICollection<ProductBusinessDTO> col)
            {
                var result = new List<ProductDataDto>(col.Count);
                foreach (var b in col)
                    result.Add(b.ToDataDto());
                return result;
            }

            var list = new List<ProductDataDto>();
            foreach (var b in businessDtos)
                list.Add(b.ToDataDto());
            return list;
        }
    }
}
