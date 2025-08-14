namespace ECommerceBuinessDTO
{
    public class OrderBusinessDTO
    {
        public int Id { get; set; }
        public long OrderNumber { get; set; }
        public decimal TotalPrice { get; set; }
        public OrderStatus Status { get; set; }
        public List<ProductBusinessDTO> products { get; set; } = new List<ProductBusinessDTO>();
        public int CustomerId { get; set; }
        public string CustomerEmail { get; set; }
    }
}