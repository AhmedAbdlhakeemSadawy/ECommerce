namespace ECommerceWebApiDto
{
    public class OrderRequestDto
    {
        public int CustomerId { get; set; }
        public List<ProductDto> Products { get; set; }
    }
}
