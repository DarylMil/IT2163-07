using IT2163_07.Models;
using IT2163_07.Models.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IT2163_07.Pages
{
    public class TwoFactorModel : PageModel
    {
        [BindProperty]
        public TwoFactorModels TwoFA { get; set; }
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SMSSender _smsSender;

        public TwoFactorModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager,
            SMSSender smsSender)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _smsSender = smsSender;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                TempData["FlashMessage.Text"] = "Error while logging in";
                TempData["FlashMessage.Type"] = "danger";
                return RedirectToPage("/Login");
            }

            var providers = await _userManager.GetValidTwoFactorProvidersAsync(user);
            if (!providers.Contains("Phone"))
            {
                TempData["FlashMessage.Text"] = "Error while logging in";
                TempData["FlashMessage.Type"] = "danger";
                return RedirectToPage("/Login");
            }
            var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Phone");
            var message = "Your Security Code is: " + token;
            await _smsSender.SendSmsAsync(user.PhoneNumber,message);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if(user == null)
            {
                TempData["FlashMessage.Text"] = "Error while logging in";
                TempData["FlashMessage.Type"] = "danger";
                return RedirectToPage("/Login");
            }
            var result = await _signInManager.TwoFactorSignInAsync("Phone", TwoFA.OTP, true, false);
            if (result.Succeeded)
            {
                TempData["FlashMessage.Text"] = "Succesfully LoggedIn";
                TempData["FlashMessage.Type"] = "success";
                return RedirectToPage("/UserDashboard");
            }
            else if (result.IsLockedOut)
            {
                TempData["FlashMessage.Text"] = "Account Is Locked";
                TempData["FlashMessage.Type"] = "danger";
                return Page();
            }
            TempData["FlashMessage.Text"] = "Invalid OTP Attempt";
            TempData["FlashMessage.Type"] = "danger";
            return Page();
        }
    }
}
