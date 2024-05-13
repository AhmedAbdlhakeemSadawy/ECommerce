using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceDataAccess.DataEntities
{
    internal class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        private int Id;

        [Required]
        private string name;
        private string description { get; set; }

        [Required]
        [Range(1.0, double.MaxValue, ErrorMessage = "Price must be greater than 1.")]
        private decimal price { get; set; }

        public Product(string name,string description, decimal price)
        {
            this.name = name;
            this.description = description;
            this.price = price;
        }
    }
}
