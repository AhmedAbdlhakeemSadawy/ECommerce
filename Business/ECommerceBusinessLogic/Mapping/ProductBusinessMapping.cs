using ECommerceBuinessDTO;
using ECommerceDataAccessDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
        public static ProductBusinessDTO MapToBusinessDto(this ProductDataDto source,ProductBusinessDTO destination
)
        {
            if (source is null) return destination;
            if (destination is null) destination = new ProductBusinessDTO();

            destination.Id = source.Id;
            destination.Name = source.name;
            destination.Price = source.price;
            destination.StockQuantity = source.StockQuantity;

            // IMPORTANT: preserve destination.Quantity
            // destination.Quantity stays as-is

            return destination;
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

        public static List<ProductBusinessDTO> MapToBusinessDtos(this IEnumerable<ProductDataDto> source, List<ProductBusinessDTO> destination)
        {
            if (source is null)
                return destination ?? new List<ProductBusinessDTO>();

            destination ??= new List<ProductBusinessDTO>();

            // Index destination by Id for O(1) lookup
            var destById = new Dictionary<int, ProductBusinessDTO>(destination.Count);
            foreach (var d in destination)
                destById[d.Id] = d;

            foreach (var src in source)
            {
                if (destById.TryGetValue(src.Id, out var existing))
                {
                    // Update existing (keeps Quantity)
                    src.MapToBusinessDto(existing);
                }
                else
                {
                    // New item (Quantity = 0)
                    destination.Add(src.ToBusinessDto());
                }
            }

            return destination;
        }

    }
}
