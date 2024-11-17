using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceBuinessDTO
{
    public class CreateOrderDto
    {
        public int Id { get; set; }
        public List<ProductBusinessDTO> products { get; set; } = new List<ProductBusinessDTO>();
    }
}
