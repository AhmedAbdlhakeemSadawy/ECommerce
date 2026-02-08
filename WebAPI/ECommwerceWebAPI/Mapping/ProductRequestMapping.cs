using ECommerceBuinessDTO;
using ECommerceWebApiDto;

namespace ECommwerceWebAPI.Mapping
{
    public static class ProductRequestMapping
    {
        public static ProductBusinessDTO ToBusinessDto(this ProductRequestDto request)
        {
            if (request == null) return null;

            return new ProductBusinessDTO
            {
                Id = request.Id,
                Quantity = request.Quantity,

                // Will be filled later (DB / service layer)
                Name = string.Empty,
                Price = 0m,
                StockQuantity = 0
            };
        }


        public static ProductRequestDto ToRequestDto(this ProductBusinessDTO business)
        {
            if (business == null) return null;

            return new ProductRequestDto
            {
                Id = business.Id,
                Quantity = business.Quantity
            };
        }


        public static List<ProductRequestDto> ToRequestDtos( this IEnumerable<ProductBusinessDTO> businessDtos)
        {
            if (businessDtos == null)
                return new List<ProductRequestDto>();

            var list = new List<ProductRequestDto>();
            foreach (var b in businessDtos)
                list.Add(b.ToRequestDto());

            return list;
        }

        public static List<ProductBusinessDTO> ToBusinessDtos(this IEnumerable<ProductRequestDto> requests)
        {
            if (requests == null)
                return new List<ProductBusinessDTO>();

            if (requests is ICollection<ProductRequestDto> col)
            {
                var result = new List<ProductBusinessDTO>(col.Count);
                foreach (var r in col)
                    result.Add(r.ToBusinessDto());

                return result;
            }

            var list = new List<ProductBusinessDTO>();
            foreach (var r in requests)
                list.Add(r.ToBusinessDto());

            return list;
        }
    }
}
