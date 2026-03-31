using ECommerceDataAccess.DataEntities;
using ECommerceDataAccessDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceDataAccess.Mapping
{
    public static class OrderProductDataMapping
    {
        public static OrderProductDataDto ToDataDto(this OrderProduct entity)
        {
            if (entity is null) return null;

            return new OrderProductDataDto
            {
                ProductId = entity.ProductId,
                Quantity = entity.ProductQuantity,

                // Name comes from navigation property
                // Make sure Product is included or projected
                Name = entity.Product?.name
            };
        }

        public static OrderProduct ToEntity(this OrderProductDataDto dto, int orderId)
        {
            if (dto is null) return null;

            return new OrderProduct
            {
                ProductId = dto.ProductId,
                ProductQuantity = dto.Quantity,
                OrderId = orderId
            };
        }

        public static List<OrderProductDataDto> ToDataDtos(this IEnumerable<OrderProduct> entities)
        {
            if (entities is null) return new List<OrderProductDataDto>();

            if (entities is ICollection<OrderProduct> col)
            {
                var result = new List<OrderProductDataDto>(col.Count);
                foreach (var e in col)
                    result.Add(e.ToDataDto());
                return result;
            }

            var list = new List<OrderProductDataDto>();
            foreach (var e in entities)
                list.Add(e.ToDataDto());
            return list;
        }

        // DTO list -> Entity list
        public static List<OrderProduct> ToEntities(this IEnumerable<OrderProductDataDto> dtos, int orderId)
        {
            if (dtos is null) return new List<OrderProduct>();

            if (dtos is ICollection<OrderProductDataDto> col)
            {
                var result = new List<OrderProduct>(col.Count);
                foreach (var d in col)
                    result.Add(d.ToEntity(orderId));
                return result;
            }

            var list = new List<OrderProduct>();
            foreach (var d in dtos)
                list.Add(d.ToEntity(orderId));
            return list;
        }
    }
}
