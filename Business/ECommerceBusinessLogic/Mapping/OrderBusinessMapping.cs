using ECommerceBuinessDTO;
using ECommerceDataAccessDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceBusinessLogic.Mapping
{
    public static class OrderBusinessMapping
    {
        public static OrderBusinessDTO ToBusinessDto(this OrderDataDto dataDto)
        {
            if (dataDto is null) return null;

            return new OrderBusinessDTO
            {
                Id = dataDto.Id,
                OrderNumber = dataDto.OrderNumber,
                TotalPrice = dataDto.TotalPrice,

                // int -> enum conversion (business meaning)
                Status = (OrderStatus)dataDto.Status,

                CustomerId = dataDto.CustomerId,

                // CustomerEmail does NOT exist in Data DTO
                // This is business-context data; fill it later if needed
                CustomerEmail = null,

                products = dataDto.products is null
                    ? new()
                    : dataDto.products
                        .Select(p => p.ToBusinessDto())
                        .ToList()
            };
        }

        // Business -> Data
        public static OrderDataDto ToDataDto(this OrderBusinessDTO businessDto)
        {
            if (businessDto is null) return null;

            return new OrderDataDto
            {
                Id = businessDto.Id,
                OrderNumber = businessDto.OrderNumber,
                TotalPrice = businessDto.TotalPrice,

                // enum -> int conversion (storage)
                Status = (int)businessDto.Status,

                CustomerId = businessDto.CustomerId,

                products =businessDto.products.ToOrderProductDataDtos().ToList()
            };
        }

        // ---------- List mapping (explicit, non-generic) ----------

        // Data list -> Business list
        public static List<OrderBusinessDTO> ToBusinessDtos(this IEnumerable<OrderDataDto> dataDtos)
        {
            if (dataDtos is null) return new List<OrderBusinessDTO>();

            if (dataDtos is ICollection<OrderDataDto> col)
            {
                var result = new List<OrderBusinessDTO>(col.Count);
                foreach (var d in col)
                    result.Add(d.ToBusinessDto());
                return result;
            }

            var list = new List<OrderBusinessDTO>();
            foreach (var d in dataDtos)
                list.Add(d.ToBusinessDto());
            return list;
        }

        // Business list -> Data list
        public static List<OrderDataDto> ToDataDtos(this IEnumerable<OrderBusinessDTO> businessDtos)
        {
            if (businessDtos is null) return new List<OrderDataDto>();

            if (businessDtos is ICollection<OrderBusinessDTO> col)
            {
                var result = new List<OrderDataDto>(col.Count);
                foreach (var b in col)
                    result.Add(b.ToDataDto());
                return result;
            }

            var list = new List<OrderDataDto>();
            foreach (var b in businessDtos)
                list.Add(b.ToDataDto());
            return list;
        }
    }
}
