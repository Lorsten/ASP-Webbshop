using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectWeb.Models;
using ProjectWeb.Data;

namespace ProjectWeb.Controllers
{
    public class ContactController : Controller
    {

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(EmailSend Mail)
        {
            ViewBag.Message = "Ditt meddelande har skickats tack för att du kontaktade oss";
  
            return View();
        }
    }
}
