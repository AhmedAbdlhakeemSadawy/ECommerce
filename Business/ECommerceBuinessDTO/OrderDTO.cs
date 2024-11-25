namespace ECommerceBuinessDTO
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public decimal TotalPrice { get; set; }
        public OrderStatus Status { get; set; }
        public List<ProductBusinessDTO> products { get; set; } = new List<ProductBusinessDTO>();
    }
}