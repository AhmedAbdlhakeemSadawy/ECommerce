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
        public decimal TotalPrice { get; set; }
        public int Status { get; set; }
        public List<ProductDataDto> products { get; set; } = new List<ProductDataDto>();
    }
}
