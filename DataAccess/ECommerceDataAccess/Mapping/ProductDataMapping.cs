using ECommerceDataAccess.DataEntities;
using ECommerceDataAccessDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceDataAccess.Mapping
{
    public static class ProductDataMapping
    {
        public static ProductDataDto ToDataDto(this Product entity)
        {
            if (entity is null) return null;

            return new ProductDataDto
            {
                Id = entity.Id,
                name = entity.name,
                description = entity.description,
                price = entity.price,
                StockQuantity = entity.StockQuantity
            };
        }

        public static Product ToEntity(this ProductDataDto dto)
        {
            if (dto is null) return null;

            return new Product
            {
                Id = dto.Id,
                name = dto.name,
                description = dto.description,
                price = dto.price,
                StockQuantity = dto.StockQuantity
            };
        }

        public static List<ProductDataDto> ToDataDtos(this IEnumerable<Product> entities)
        {
            if (entities is null) return new List<ProductDataDto>();

            if (entities is ICollection<Product> col)
            {
                var result = new List<ProductDataDto>(col.Count);
                foreach (var e in col)
                    result.Add(e.ToDataDto());
                return result;
            }

            var list = new List<ProductDataDto>();
            foreach (var e in entities)
                list.Add(e.ToDataDto());
            return list;
        }

        // DTO list -> Entity list
        public static List<Product> ToEntities(this IEnumerable<ProductDataDto> dtos)
        {
            if (dtos is null) return new List<Product>();

            if (dtos is ICollection<ProductDataDto> col)
            {
                var result = new List<Product>(col.Count);
                foreach (var d in col)
                    result.Add(d.ToEntity());
                return result;
            }

            var list = new List<Product>();
            foreach (var d in dtos)
                list.Add(d.ToEntity());
            return list;
        }
    }
}
