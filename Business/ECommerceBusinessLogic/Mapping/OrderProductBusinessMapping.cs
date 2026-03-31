using ECommerceBuinessDTO;
using ECommerceDataAccessDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceBusinessLogic.Mapping
{
    public static class OrderProductBusinessMapping
    {
        public static ProductBusinessDTO ToBusinessDto(this OrderProductDataDto dataDto)
        {
            if (dataDto is null) return null;

            return new ProductBusinessDTO
            {
                Id = dataDto.ProductId,
                Name = dataDto.Name,
                Quantity = dataDto.Quantity,

                // Not available at this stage
                Price = 0,
                StockQuantity = 0
            };
        }

        // Business -> Data (SAFE reverse mapping)
        public static OrderProductDataDto ToOrderProductDataDto(this ProductBusinessDTO businessDto)
        {
            if (businessDto is null) return null;

            return new OrderProductDataDto
            {
                ProductId = businessDto.Id,
                Name = businessDto.Name,
                Quantity = businessDto.Quantity
            };
        }

        // ---------- List mapping (explicit, non-generic) ----------

        // Data list -> Business list
        public static List<ProductBusinessDTO> ToBusinessDtos(
            this IEnumerable<OrderProductDataDto> dataDtos)
        {
            if (dataDtos is null) return new List<ProductBusinessDTO>();

            if (dataDtos is ICollection<OrderProductDataDto> col)
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
        public static List<OrderProductDataDto> ToOrderProductDataDtos(
            this IEnumerable<ProductBusinessDTO> businessDtos)
        {
            if (businessDtos is null) return new List<OrderProductDataDto>();

            if (businessDtos is ICollection<ProductBusinessDTO> col)
            {
                var result = new List<OrderProductDataDto>(col.Count);
                foreach (var b in col)
                    result.Add(b.ToOrderProductDataDto());
                return result;
            }

            var list = new List<OrderProductDataDto>();
            foreach (var b in businessDtos)
                list.Add(b.ToOrderProductDataDto());
            return list;
        }
    }
}
