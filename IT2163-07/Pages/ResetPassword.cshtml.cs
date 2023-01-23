using IT2163_07.Models;
using IT2163_07.Models.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace IT2163_07.Pages
{
    public class ResetPasswordModel : PageModel
    {
        [BindProperty]
        public ResetPasswordModels ResetPassword { get; set; }
        private readonly UserManager<ApplicationUser> _userManager;
        public ResetPasswordModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult OnGet(string code = null, string email = null)
        {
            if (code == null || email == null)
            {
                TempData["FlashMessage.Text"] = "A code or email must be supplied for password reset.";
                TempData["FlashMessage.Type"] = "danger";
                return RedirectToPage("/UserDashboard");
            }
            else
            {
                ResetPassword = new ResetPasswordModels
                {
                    Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code)),
                    Email = email
                };
                return Page();
            }
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(ResetPassword.Email);
                if (user == null)
                {
                    // Don't reveal that the user does not exist
                    TempData["FlashMessage.Text"] = "Reset Password Fail";
                    TempData["FlashMessage.Type"] = "danger";
                    return RedirectToPage("/UserDashboard");
                }

                var result = await _userManager.ResetPasswordAsync(user, ResetPassword.Code, ResetPassword.Password);

                if (result.Succeeded)
                {
                    TempData["FlashMessage.Text"] = "Reset Password Success";
                    TempData["FlashMessage.Type"] = "danger";
                    return RedirectToPage("/UserDashboard");
                }
                TempData["FlashMessage.Text"] = "Reset Password Fail";
                TempData["FlashMessage.Type"] = "danger";
                return Page();
            }
            return Page();
        }
    }
}
