using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectWeb.Data;
using ProjectWeb.Models;
using Microsoft.AspNetCore.Identity;
using ProjectWeb.Areas.Identity.Data;

namespace ProjectWeb.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ProjectContext _context;
        private readonly UserManager<ProjectWebUser> _userManager;

        private readonly IMailer _mail;

        private string RandomNumbers;

        private int _UserID;

        private string _IDCookie;

        private int _TotalPrice;

        public CheckoutController(ProjectContext context, UserManager<ProjectWebUser> userManager, IMailer mail )
        {
            _context = context;
            _userManager = userManager;
            _mail = mail;
        }
        [Route("Checkout")]
        public async Task<IActionResult> Index()
        {
            _UserID = Int32.Parse(Request.Cookies["UserID"]);
            var query = await _context.Cart
                .Include(c => c.ItemsInCart)
                .ThenInclude(s => s.ProductItem)
                .FirstOrDefaultAsync(x => x.SessionKey == _UserID);

            if (query != null && query.ItemsInCart.Any()) {
                ViewBag.Cart =  query;
                ViewBag.UserID = _UserID;
                return View();
            }
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        [Route("Checkout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>Index([Bind("UserID,Firstname,Lastname,Adress,City,Email")] UserData User)
        {
 
            if (ModelState.IsValid)
            {
                _context.Add(User);
                await _context.SaveChangesAsync();
                var cart = await _context.Cart
                    .Include(s => s.ItemsInCart)
                    .ThenInclude(p => p.ProductItem)
                    .FirstOrDefaultAsync(x => x.SessionKey == Int32.Parse(Request.Cookies["UserID"]));

                CalculateTotalPrice(cart.ItemsInCart.ToList());

                int id = User.UserID;
                //Create an ordernumber
                var Random = new Random();
                for (int i = 0; i < 8; i++)
                {
                    RandomNumbers += Random.Next(1, 8).ToString();
                }
                Order Order = new Order();
                Order.Price = _TotalPrice;
                Order.OrderNumber = RandomNumbers;
                Order.CustomerID = id;

                string OrderNumber = Order.OrderNumber;
                _context.Add(Order);

                await _context.SaveChangesAsync();

                if (cart != null)
                {
                    cart.OrderRef = Order.OrderID;

                    _context.Update(cart);
                    await _context.SaveChangesAsync();
                    /*
                    int OrderID = Order.OrderID;
                    */
                    return RedirectToAction("Order", new { id, OrderNumber });
                }

                //Redirect to Create order to create the order by passing the user id
            }
            var query = _context.Cart
                 .Where(x => x.SessionKey == Int32.Parse(Request.Cookies["UserID"]))
                .Include(c => c.ItemsInCart)
                .ThenInclude(s => s.ProductItem)
                .SingleOrDefaultAsync();

            ViewBag.Cart = await query;

            return View(User);
        }
        //Method for getting the total sum of the order
        private void CalculateTotalPrice(List<CartUser> CartModel)
        {
            foreach(var item in CartModel)
            {
                _TotalPrice += (item.ProductItem.Price * item.Quantity);
            }
        }

        //Method for logged in users
        public async Task<IActionResult> CreateOrderUser(int UserIDCookie)
        {
            var userID = _context.UserData.SingleOrDefault(x => x.RegisteredUserID == _userManager.GetUserId(User));
      
            if(userID != null)
            {
                int id = userID.UserID;
                var cart = await _context.Cart.FirstOrDefaultAsync(x => x.SessionKey == Int32.Parse(Request.Cookies["UserID"]));
                if (cart != null)
                {
                    //Create an ordernumber
                    var Random = new Random();
                    for (int i = 0; i < 8; i++)
                    {
                        RandomNumbers += Random.Next(1, 8);
                    }
                    //Find the cart table and calculate total price of the orders 
                    var CartPrice = await _context.Cart
                       .Include(s => s.ItemsInCart)
                       .ThenInclude(p => p.ProductItem)
                       .FirstOrDefaultAsync(x => x.SessionKey == Int32.Parse(Request.Cookies["UserID"]));

                    CalculateTotalPrice(cart.ItemsInCart.ToList());
                    Order Order = new Order();

                    string RandomOrder = $"#{RandomNumbers}";

                    Order.Price = _TotalPrice;
                    Order.OrderNumber = RandomOrder;
                    Order.CustomerID = userID.UserID;

                    _context.Add(Order);
                    await _context.SaveChangesAsync();

                    string OrderNumber = Order.OrderNumber;
                    cart.OrderRef = Order.OrderID;
                    int OrderID = Order.OrderID;
                    _context.Update(cart);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Order", new { id, OrderNumber });
                }
            }
             return RedirectToAction(nameof(Index));
        }

        //Method for displaying the order after purchase
        public async Task<IActionResult> Order(int id, string OrderNumber)
        {
            //Get the Order with LINQ
            var OrderCreated = await _context.Order
                .Include(s => s.CartItems)
                .FirstOrDefaultAsync(b => b.OrderNumber == OrderNumber && b.CustomerID == id);
         
            if (OrderCreated != null)
            {
                //Get the products 
                var Products = await _context.CartUser
                    .Where(b => b.CartItemRef == OrderCreated.CartItems.SessionKey)
                    .Include(s => s.ProductItem)
                    .ToListAsync();

                //Display the products using ViewData
                ViewData["Products"] = Products;


                //Count how many products in the Query and use a viewbag
                ViewBag.NumberOfItems = Products.Count;


                var UserInfo = await _context.UserData.SingleOrDefaultAsync(x => x.UserID == id);
                ViewBag.Userinfo = UserInfo;

  
                //Generate new Cookie id
                GenerateSessionKey();
                Response.Cookies.Append("UserID", _IDCookie.ToString());

                await _mail.SendEmailAsync(UserInfo.Email, UserInfo.Email, $"Din order hos oss har skapats med ordernummer {OrderCreated.OrderNumber}", $"Totala kostnaden för din order är {OrderCreated.Price}kr");
                return View(OrderCreated);
            }

            return RedirectToAction("Index");
        }

       private void GenerateSessionKey()
        {
            bool CheckID = true;
            var Random = new Random();

            while (CheckID)
            {
                for (int i = 0; i < 9; i++)
                {
                    _IDCookie += Random.Next(1, 9).ToString();
                }
                var id = _context.Cart.SingleOrDefault(x => x.SessionKey == Int32.Parse(_IDCookie));
                if (id != null) _IDCookie = "";
                if (id == null) return;
            }
        }
    }
}
