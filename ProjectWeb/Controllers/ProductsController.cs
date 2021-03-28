using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectWeb.Data;
using ProjectWeb.Models;
using System.IO;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace ProjectWeb.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ProjectContext _context;

        private readonly IWebHostEnvironment _env;

        private string _ImagePath_folder;

        private string _UserID;

        public ProductsController(ProjectContext context, IWebHostEnvironment env)
        {
            _env = env;
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index(string SearchName)
        {
            var ProductsSearch = from m in _context.Product
                           select m;

            if (!string.IsNullOrEmpty(SearchName))
            {
                ProductsSearch = ProductsSearch.Where(x => x.ProductName.Contains(SearchName));
            }
            if (Request.Cookies["UserID"] != null)
            {
                _UserID = Request.Cookies["UserID"];

            }
            else
            {
                CookieOptions options = new CookieOptions();
                options.Expires = DateTime.Now.AddDays(2);
                GenerateSessionKey();
                Response.Cookies.Append("UserID", _UserID);
            }
            return View(await ProductsSearch.ToListAsync());
        }
        //Only allow admin to visit this view
        [Authorize(Roles = "Admin")]
        [Route("Hantera_Produkter")]
        public async Task<IActionResult> Admin_products()
        {
            var GetProducts = from m in _context.Product
                              select m;

            return View(await GetProducts.ToListAsync());
        }

        //Add Product to CartUSer
        [HttpPost]
        public async Task<IActionResult> AddToCart([Bind("ProductID,Quantity,UserID")] string ProductID, string Quantity, string UserID)
        {
            var CartQuery = await _context.Cart.FirstOrDefaultAsync(x => x.SessionKey == Int32.Parse(UserID));

            if (CartQuery == null)
            {
                Cart ShopCart = new Cart
                {
                    SessionKey = Int32.Parse(UserID),
                };
                _context.Cart.Add(ShopCart);
                await _context.SaveChangesAsync();
            }
            // Check if product already exists in cart
            var CartItem = await _context.CartUser.FirstOrDefaultAsync(x => x.ProductRef == Int32.Parse(ProductID) && x.CartItemRef == Int32.Parse(UserID));
            if(CartItem == null)
            {
                var Item = new CartUser();
                Item.ProductRef = Int32.Parse(ProductID);
                Item.CartItemRef = Int32.Parse(UserID);
                Item.Quantity = Int32.Parse(Quantity);
                _context.Add(Item);
            }
            // Add quantity by one if it exists
            else
            {
                CartItem.Quantity++;
                _context.Update(CartItem);
            }
                await _context.SaveChangesAsync();
                return Ok();   
        }
        //Delete CartItem based on ID
        [HttpPost]
        public async Task<IActionResult> RemoveCartItem(string CartID)
        {
            var Item = await _context.CartUser.FindAsync(Int32.Parse(CartID));
            if(Item != null)
            {
                _context.Remove(Item);
                await _context.SaveChangesAsync();
                return StatusCode(200);
            }
            return StatusCode(400);
        }
        [HttpDelete]
        public async Task<IActionResult> EmptyCart(int? id )
        {
        
            if (id == null)
            {
                return NotFound();
            }

            var CartID = await _context.Cart.FindAsync(id);
            if (CartID != null)
            {
                _context.Cart.Remove(CartID);
                await _context.SaveChangesAsync();
                return StatusCode(400);
            }
            return Ok();

        }
        //Edit the cart quantity
        [HttpPut]
        public async Task<IActionResult> EditCart(string CartID, string Quantity)
        {
            var Item = await _context.CartUser.FindAsync(Int32.Parse(CartID));
            if (Item != null)
            {
                Item.Quantity = Int32.Parse(Quantity);
                _context.Update(Item);
                await _context.SaveChangesAsync();
                return Ok();
            }
            return StatusCode(400);
        }
        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewBag.cookie = Request.Cookies["UserID"];
            List<int> CalcPerPotion = new List<int>();
            if (id == null)
            {
                return NotFound();
            }
            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.ProductID == id);
            if (product == null)
            {
                return NotFound();
            }
            ViewBag.Salt = product.Salt * (product.Weight / 100);
            ViewBag.Fat = product.Fat * (product.Weight / 100);
            ViewBag.Calories = product.Calories * (product.Weight / 100);
            ViewBag.Protein = product.Protein * (product.Weight / 100);

            return View(product);
        }
        //Method for handling file uploads
        private async Task HandleFileUpload(string ProductName, IFormFile File = null, bool UploadFile = true, bool Edit = false)
        {
            //Get Product path
            string ProductFolder = Path.Combine(_env.WebRootPath, "Images", "Products");
            if (!Directory.Exists(ProductFolder))
            {
                Directory.CreateDirectory(ProductFolder);
            }
            //Get the new path to the file 
            string UploadFolder = Path.Combine(_env.WebRootPath, "Images", "Products", ProductName.Replace(" ", ""));
            if (UploadFile)
            {
                if (Edit)
                {
                    if (Directory.Exists(UploadFolder))
                    {
                        Directory.Delete(UploadFolder, true);
                    }
                }
                //Check if directory exists, otherwise create it
                if (!Directory.Exists(UploadFolder))
                {
                    Directory.CreateDirectory(UploadFolder);
                }
                string FileExtension = Path.GetExtension(File.FileName);
                string ImageName = ProductName.Replace(" ", "") + FileExtension;
                string ImagePath = Path.Combine(UploadFolder, ImageName);

                using (var stream = System.IO.File.Create(ImagePath))
                {
                    await File.CopyToAsync(stream);
                }
                //Get the path to the img by removing everything before Images
                _ImagePath_folder = ImagePath.Replace(_env.WebRootPath, "").Replace(@"\", "/");
            }
            //remove the folder and all the subdirections
            else
            {
                Directory.Delete(UploadFolder,true);
            }
        }
        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }
        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductID,ProductName,Calories,Fat,Protein,Salt,Weight,Price,ImageDesc")] Product product, IFormFile File)
        {
            if (ModelState.IsValid)
            {
                if(File.Length > 0)
                {
                   await  HandleFileUpload(product.ProductName, File, true,false);
                   product.ImagePath = _ImagePath_folder;
                }
                if(string.IsNullOrEmpty(product.ImageDesc))
                {
                    product.ImageDesc = $"Bild på en {product.ProductName}";
                }
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Admin_products));
            }
            return View(product);
        }
        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductID,ProductName,Calories,Fat,Protein,Salt,Weight,Price,ImageDesc")] Product product, IFormFile File)
        {
            if (id != product.ProductID)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);

                    if (File.Length > 0)
                    {
                        await HandleFileUpload(product.ProductName, File, true,true);
                        product.ImagePath = _ImagePath_folder;

                    }
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }
        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.ProductID == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Product.FindAsync(id);

            _context.Product.Remove(product);
            await HandleFileUpload(product.ProductName, null, false);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.ProductID == id);
        }
        //method for generating a random key for user
        private void GenerateSessionKey()
        {
            bool CheckID = true;
            var Random = new Random();

            while (CheckID)
            {
                for (int i = 0; i < 8; i++)
                {
                    _UserID += Random.Next(1, 9).ToString();
                }
                var id = _context.Cart.SingleOrDefault(x => x.SessionKey == Int32.Parse(_UserID));
                if (id != null) _UserID = "";
                if (id == null) return;

            }
        }
    }
}
