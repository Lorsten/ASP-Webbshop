using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProjectWeb.Areas.Identity.Data;
namespace ProjectWeb.Models
{
    public class Order
    {
        [Key]
        public int  OrderID { get; set; }

        public string OrderNumber { get; set; }
        public Cart CartItems { get; set; }

        [ForeignKey("CustomerID")]
        public int CustomerID { get; set; }

        public UserData User { get; set; }

        [Display(Name = "Order datum")]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Display(Name= "Pris")]
        public int Price { get; set; }
    }
}
