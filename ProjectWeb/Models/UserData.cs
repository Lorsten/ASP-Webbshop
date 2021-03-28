using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ProjectWeb.Areas.Identity.Data;

namespace ProjectWeb.Models
{
    public class UserData
    {

        [Key]
        public int UserID { get; set; }

        [Required]
        [Display(Name = "Förnamn")]
        public string Firstname { get; set; }

        [Required]
        [Display(Name = "Efternamn")]
        public string Lastname { get; set; }
        [Required]
        public string Adress { get; set; }

        [EmailAddress]
        [Required]
        public string Email { get; set; }

        [Display(Name ="Stad")]
        [Required]
        public string City { get; set; }

        [Display(Name ="Registerad")]
        public bool Registered { get; set; }

        public string RegisteredUserID { get; set; }
       
        public ICollection<Order> Orders { get; set; }

    }
}
