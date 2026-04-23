using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace ECommerceDataAccess.DataEntities
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public long OrderNumber { get; set; }
        public int Status { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

         public ICollection<OrderProduct> orderProducts { get; set; }

        [ForeignKey("Customer")]
        public int CustomerId { get; set; }

        // Navigation Property (Each Order belongs to one Customer)
        public Customer Customer { get; set; }

    }
}
