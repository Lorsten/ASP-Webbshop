using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ProjectWeb.Models
{
    public class CartUser
    {
        [Key]
        public int CartID { get; set; }

        [ForeignKey("Product")]
        public int ProductRef { get; set; }
        public Product ProductItem { get; set; }

        public int Quantity { get; set; }

        [ForeignKey("CartItemRef")]
        public int CartItemRef { get; set; }

        [JsonIgnore]
        public Cart ShoppingCart { get; set; }

    }
}
