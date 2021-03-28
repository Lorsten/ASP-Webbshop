using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectWeb.Models;
using ProjectWeb.Data;
using Microsoft.EntityFrameworkCore;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace ProjectWeb.Data
{
    //Service for the shopping card
    public class ShoppingCartService
    {
        private readonly ProjectContext _context;
        private  IHttpContextAccessor _httpContextAccessor;
        private int _UserID;

        public ShoppingCartService(ProjectContext context, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }
        public async Task<Cart> RetriveItems()
        {
            GetCookieID();

            var query = _context.Cart
                .Where(x => x.SessionKey == _UserID)
               .Include(c => c.ItemsInCart)
               .ThenInclude(s => s.ProductItem)
               .SingleOrDefaultAsync();
            return await query;
        }

        public void GetCookieID()
        {
            //retrive the cookie id using httpconextAccessor
            try
            {
                _UserID = Int32.Parse(_httpContextAccessor.HttpContext.Request.Cookies["UserID"]);
            }
            catch(Exception err) {
                Console.WriteLine(err);
           }
        }
        public async Task UpdateVIew()
        {
            await RetriveItems();
        }
        public int CountItems()
        {
            GetCookieID();
            var query =  _context.CartUser.Where(s => s.CartItemRef == _UserID).ToList();
            return query.Count;
        }
        public void UpdateCount()
        {
            CountItems();
        }
    }
}
