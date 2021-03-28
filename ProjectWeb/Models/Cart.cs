using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProjectWeb.Models
{
    public class Cart
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SessionKey { get; set; }

        public virtual List<CartUser> ItemsInCart { get; set; }

        [ForeignKey("OrderRef")]
        public int? OrderRef { get; set; }

        [JsonIgnore]
        public Order OrderItem { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.Now;

    }
}
