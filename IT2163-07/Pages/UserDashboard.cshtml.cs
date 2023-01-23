using IT2163_07.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Text.Encodings.Web;

namespace IT2163_07.Pages
{
    [Authorize]
    public class UserDashboardModel : PageModel
    {
        [BindProperty]
        public ApplicationUser AppUser { get; set; } = new ApplicationUser();
        [BindProperty]
        public string Roles { get; set; } = string.Empty;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly EmailSender _emailSender;

        public UserDashboardModel(UserManager<ApplicationUser> userManager, EmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }
        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var allRoles = await _userManager.GetRolesAsync(user);

            var dataProtectionProvider = DataProtectionProvider.Create("EncryptData");
            var protector = dataProtectionProvider.CreateProtector("MySecretKey");

            user.Photo = Encoding.UTF8.GetString(Convert.FromBase64String(user.Photo));
            user.AboutMe = Encoding.UTF8.GetString(Convert.FromBase64String(user.AboutMe));
            user.DeliveryAddress = Encoding.UTF8.GetString(Convert.FromBase64String(user.DeliveryAddress));
            user.CreditCardNo = protector.Unprotect(user.CreditCardNo);

            foreach (var i in allRoles)
            {
                Roles += i;
            }

            AppUser = user;

            //AppUser.Gender = user.Gender;
            //AppUser.MobileNo = user.MobileNo;
            //AppUser.DeliveryAddress= user.DeliveryAddress;
            //AppUser.Email= user.Email;
            //AppUser.Photo= user.Photo;
            //AppUser.AboutMe = user.AboutMe;

        }

        public async Task<IActionResult> OnPostAsync()
        {
            var code = await _userManager.GeneratePasswordResetTokenAsync(AppUser);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/ResetPassword",
                pageHandler: null,
                values: new { code, email = AppUser.Email },
                protocol: Request.Scheme);
           
            var sendRes = await _emailSender.SendEmailAsync(
                AppUser.Email,
                "Reset Password",
                $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
            if (sendRes)
            {
                TempData["FlashMessage.Type"] = "success";
                TempData["FlashMessage.Text"] = "Reset Password Email Sent.";
            }
            else
            {
                TempData["FlashMessage.Type"] = "danger";
                TempData["FlashMessage.Text"] = "Fail To Send Reset Password Email. Please Try Again";
            }
            return Page();
            
        }
    }
}
