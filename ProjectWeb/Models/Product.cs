using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProjectWeb.Models
{
    public class Product
    {
        public int ProductID { get; set; }

        [Required]
        [Display(Name = "Produktnamn")]
        public string ProductName { get; set; }

        [Display(Name = "Kalorier(KCal)")]
        [Required]
        public float Calories { get; set; }

        [Display(Name = "Fett")]
        [Required]
        public float Fat { get; set; }

        [Display(Name = "Protein")]
        [Required]
        public float Protein { get; set; }

        [Display(Name = "Salt")]
        [Required]
        public float Salt { get; set; }

        [Required]
        [Display(Name = "Vikt")]
        public float Weight { get; set; }
        [Required]
        [Display(Name = "Pris")]
        public int Price { get; set; }

        public string ImagePath { get; set; }

        [Display(Name ="Bildbeskrivning")]

        public string ImageDesc { get; set; }

        [JsonIgnore]
        public ICollection<CartUser> Cart { get; set; }

        public DateTime DateAdded { get; set; } = DateTime.Now;
    }
}
