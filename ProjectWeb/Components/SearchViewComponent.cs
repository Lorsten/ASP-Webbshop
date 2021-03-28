using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectWeb.Data;
using Microsoft.EntityFrameworkCore;
using ProjectWeb.Models;

//Viewcomponet for searchfunction. The component retrivie all products and pass it to the view

namespace ProjectWeb.Components
{
    public class SearchViewComponent : ViewComponent
    {
        private readonly ProjectContext _context;

        public SearchViewComponent(ProjectContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var item = await GetItems();
            return View(item);
        }
        public async Task<List<Product>> GetItems()
        {
            var items = from m in _context.Product
                        select m;

            return await items.ToListAsync();
        }
    }
}
