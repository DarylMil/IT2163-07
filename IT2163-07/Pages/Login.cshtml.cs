using IT2163_07.Models;
using IT2163_07.Models.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace IT2163_07.Pages
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        [BindProperty]
        public LoginModels LoginModels { get; set; }
        public LoginModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;

        }
        public async Task<IActionResult> OnGetAsync(string userId, string code)
        {
            try
            {
                if (userId == null || code == null)
                {
                    return Page();
                }
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    TempData["FlashMessage.Text"] = "Unable to load the user";
                    TempData["FlashMessage.Type"] = "danger";
                    return Page();
                }

                code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
                var result = await _userManager.ConfirmEmailAsync(user, code);
                if (result.Succeeded)
                {
                    TempData["FlashMessage.Text"] = "Thank you for confirming your email";
                    TempData["FlashMessage.Type"] = "success";
                }
                else
                {
                    TempData["FlashMessage.Text"] = "Error confirming your email";
                    TempData["FlashMessage.Type"] = "danger";
                }
                return Page();
            }
            catch (Exception)
            {
                return RedirectToPage("/Errors/500");
            }
        }
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _signInManager.PasswordSignInAsync(LoginModels.Email, LoginModels.Password, true, lockoutOnFailure: true);
                    if (result.IsLockedOut)
                    {
                        TempData["FlashMessage.Text"] = "Account is Locked";
                        TempData["FlashMessage.Type"] = "danger";
                        return Page();
                    }
                    if (result.RequiresTwoFactor)
                    {
                        return RedirectToPage("/TwoFactor");
                    }
                    else
                    {
                        TempData["FlashMessage.Text"] = "Invalid login credentials";
                        TempData["FlashMessage.Type"] = "danger";
                        return Page();
                    }
                }
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return Page();
            }
            catch (Exception)
            {
                return RedirectToPage("/Errors/500");
            }
        }
    }
}
