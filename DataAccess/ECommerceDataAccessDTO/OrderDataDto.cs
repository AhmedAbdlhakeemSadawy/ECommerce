using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceDataAccessDTO
{
    public class OrderDataDto
    {
        public int Id { get; set; }
        public long OrderNumber { get; set; }
        public decimal TotalPrice { get; set; }
        public int Status { get; set; }
        public List<ProductOrderDataDto> products { get; set; } = new List<ProductOrderDataDto>();
        public int CustomerId { get; set; }
    }
}
