using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectWeb.Areas.Identity.Data;
using ProjectWeb.Data;
using ProjectWeb.Models;

namespace ProjectWeb.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<ProjectWebUser> _userManager;
        private readonly SignInManager<ProjectWebUser> _signInManager;

        private readonly ProjectContext _context;
        public IndexModel(
            UserManager<ProjectWebUser> userManager,
            SignInManager<ProjectWebUser> signInManager,
            ProjectContext context
            )
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [Required]
            [Display(Name = "Förnamn")]
            public string Firstname { get; set; }

            [Required]
            [Display(Name = "Efternamn")]
            public string Lastname { get; set; }

            [Required]
            public string Adress { get; set; }



            [Required]
            [Display(Name = "Stad")]
            public string City { get; set; }

        }

        private async Task LoadAsync(ProjectWebUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            Username = userName;


            Input = new InputModel
            {
                PhoneNumber = phoneNumber
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            if (!User.IsInRole("Admin"))
            {

                var name = _context.UserData
                    .Where(b => b.RegisteredUserID == _userManager.GetUserId(User))
                    .SingleOrDefault();

                ViewData["UserName"] = name;
                var Order = _context.Order
                    .Include(s => s.CartItems)
                    .ThenInclude(p => p.ItemsInCart)
                    .Where(s => s.CustomerID == name.UserID);
                if (Order != null)
                {
                    ViewData["Order"] = await Order.ToListAsync();
                }
            }
            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
