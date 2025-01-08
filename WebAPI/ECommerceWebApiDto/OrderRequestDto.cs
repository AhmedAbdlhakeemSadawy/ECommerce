namespace ECommerceWebApiDto
{
    public class OrderRequestDto
    {
        public int CustomerId { get; set; }
        public List<ProductRequestDto> Products { get; set; }
    }
}
