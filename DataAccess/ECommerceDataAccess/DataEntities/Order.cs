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

        public int orderNumber { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        ICollection<OrderProduct> orderProducts { get; set; }

    }
}
