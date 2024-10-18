namespace ECommerceBuinessDTO
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public decimal TotalPrice { get; set; }
        public int Status { get; set; }
        public List<ProductDTO> products { get; set; }
    }
}