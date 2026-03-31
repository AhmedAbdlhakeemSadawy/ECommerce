using ECommerceDataAccess.DataEntities;
using ECommerceDataAccessDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceDataAccess.Mapping
{
    public static class OrderDataMapping
    {
        public static OrderDataDto ToDataDto(this Order entity)
        {
            if (entity is null) return null;

            return new OrderDataDto
            {
                Id = entity.Id,
                OrderNumber = entity.OrderNumber,
                TotalPrice = entity.TotalPrice,
                Status = entity.Status,
                CustomerId = entity.CustomerId,

                // If you need products list, you must map from entity.orderProducts
                // NOTE: Requires you to have ProductOrderDataDto mapping available.
                products = entity.orderProducts is null
                    ? new()
                    : entity.orderProducts
                        .Select(op => op.ToDataDto()) // implement this mapper
                        .ToList()
            };
        }

        public static Order ToEntity(this OrderDataDto dto)
        {
            if (dto is null) return null;

            return new Order
            {
                Id = dto.Id,
                OrderNumber = dto.OrderNumber,
                TotalPrice = dto.TotalPrice,
                Status = dto.Status,
                CustomerId = dto.CustomerId,

                // CreatedDate: decide your rule:
                // - For create: leave default (DateTime.Now) or set explicitly in service
                // - For update: do NOT overwrite CreatedDate unless you intend to
                // CreatedDate = ???

                // orderProducts mapping depends on how you model OrderProduct.
                // Usually you build OrderProduct rows from dto.products.
                orderProducts = dto.products?.Select(p =>p.ToEntity(dto.Id)).ToList() // implement this mapper
            };
        }


        public static List<OrderDataDto> ToDataDtos(this IEnumerable<Order> entities)
        {
            if (entities is null) return new List<OrderDataDto>();

            // pre-allocate if possible
            if (entities is ICollection<Order> col)
            {
                var result = new List<OrderDataDto>(col.Count);
                foreach (var e in col)
                    result.Add(e.ToDataDto());
                return result;
            }

            var list = new List<OrderDataDto>();
            foreach (var e in entities)
                list.Add(e.ToDataDto());
            return list;
        }

        public static List<Order> ToEntities(this IEnumerable<OrderDataDto> dtos)
        {
            if (dtos is null) return new List<Order>();

            if (dtos is ICollection<OrderDataDto> col)
            {
                var result = new List<Order>(col.Count);
                foreach (var d in col)
                    result.Add(d.ToEntity());
                return result;
            }

            var list = new List<Order>();
            foreach (var d in dtos)
                list.Add(d.ToEntity());
            return list;
        }

    }
}
