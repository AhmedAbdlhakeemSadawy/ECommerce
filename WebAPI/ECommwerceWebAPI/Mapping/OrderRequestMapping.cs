using ECommerceBuinessDTO;
using ECommerceWebApiDto;

namespace ECommwerceWebAPI.Mapping
{
    public static class OrderRequestMapping
    {
        public static OrderBusinessDTO ToBusinessDto(this OrderRequestDto request)
        {
            if (request == null) return null;

            return new OrderBusinessDTO
            {
                CustomerId = request.CustomerId,
                products = request.Products.ToBusinessDtos(),  // uses ProductRequestDto -> ProductBusinessDTO
                                                               // Remaining fields are set later:
                                                               // Id, OrderNumber, TotalPrice, Status, CustomerEmail
            };
        }

        public static OrderRequestDto ToRequestDto(this OrderBusinessDTO business)
        {
            if (business == null) return null;

            return new OrderRequestDto
            {
                CustomerId = business.CustomerId,
                Products = business.products.ToRequestDtos()
            };
        }
    }
}
