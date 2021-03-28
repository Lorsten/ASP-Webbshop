using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using ProjectWeb.Data;

namespace ProjectWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _env;

        private readonly ProjectContext _context;

        public HomeController(IWebHostEnvironment env, ProjectContext context)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult About()
        {
            return View();
        }

    }
}
