using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceDataAccess.DataEntities
{
    //[Table("Products")]
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string name { get; set; }
        public string description { get; set; }

        [Required]
        [Column(TypeName = "money")]
        [Range(1.0, double.MaxValue, ErrorMessage = "Price must be greater than 1.")]
        public decimal price { get; set; }


        [Required]
        public int StockQuantity { get; set; }



    }
}
