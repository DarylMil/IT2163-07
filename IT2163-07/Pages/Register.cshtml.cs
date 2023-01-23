using IT2163_07.Models;
using IT2163_07.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;

namespace IT2163_07.Pages
{
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public RegisterModels registerModels { get; set; }
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly EmailSender _emailSender;
        private IWebHostEnvironment _environment;
        
        [BindProperty]
        public IFormFile? Upload { get; set; }
        public RegisterModel(UserManager<ApplicationUser> userManager, IWebHostEnvironment environment, RoleManager<IdentityRole> roleManager
            ,EmailSender emailSender)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _emailSender = emailSender;
            _environment = environment;
        }
        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPost()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingUser = await _userManager.FindByEmailAsync(registerModels.Email);

                    if (existingUser != null)
                    {
                        TempData["FlashMessage.Text"] = "Email Exist.";
                        TempData["FlashMessage.Type"] = "danger";
                        return Page();
                    }

                    var dataProtectionProvider = DataProtectionProvider.Create("EncryptData");
                    var protector = dataProtectionProvider.CreateProtector("MySecretKey");
                    var imgUrl = "";

                    if (Upload != null)
                    {
                        if (Upload.ContentType == "image/jpeg" || Upload.ContentType == "image/jpg")
                        {
                            var uploadsFolder = "uploads/images";
                            var imageFile = Guid.NewGuid() + Path.GetExtension(Upload.FileName);
                            var imagePath = Path.Combine(_environment.ContentRootPath, "wwwroot", uploadsFolder, imageFile);
                            using var fileStream = new FileStream(imagePath, FileMode.Create);
                            await Upload.CopyToAsync(fileStream);
                            imgUrl = string.Format("/{0}/{1}", uploadsFolder, imageFile);
                        }
                        else
                        {
                            ModelState.AddModelError("Upload", "Only JPG File Types.");
                            return Page();
                        }
                    }
                    else
                    {
                        var uploadsFolder = "uploads/images";
                        var imageFile = "default_pic.jpg";
                        Path.Combine(_environment.ContentRootPath, "wwwroot", uploadsFolder, imageFile);
                        imgUrl = string.Format("/{0}/{1}", uploadsFolder, imageFile);
                    }

                    //byte[] salt = new byte[16];
                    //RandomNumberGenerator.Create().GetBytes(salt);
                    

                    var user = new ApplicationUser()
                    {
                        FullName = registerModels.FullName,
                        CreditCardNo = protector.Protect(registerModels.CreditCardNo),
                        Gender = registerModels.Gender,
                        MobileNo = registerModels.MobileNo,
                        DeliveryAddress = Convert.ToBase64String(Encoding.UTF8.GetBytes(registerModels.DeliveryAddress)),
                        Email = registerModels.Email,
                        UserName = registerModels.Email,
                        Photo = Convert.ToBase64String(Encoding.UTF8.GetBytes(imgUrl)),
                        AboutMe = Convert.ToBase64String(Encoding.UTF8.GetBytes(registerModels.AboutMe)),
                        TwoFactorEnabled = true
                    };

                    var res = await _userManager.CreateAsync(user, registerModels.Password);
                    if (res.Succeeded)
                    {
                        if (!await _roleManager.RoleExistsAsync(registerModels.Role))
                        {
                            await _roleManager.CreateAsync(new IdentityRole(registerModels.Role));
                            await _userManager.AddToRoleAsync(user, registerModels.Role);
                        }
                        else
                        {
                            await _userManager.AddToRoleAsync(user, registerModels.Role);
                        }

                        //var userId = await _userManager.GetUserIdAsync(user);
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Page(
                            "/Login",
                            pageHandler: null,
                            values: new { userId = user.Id, code = code },
                            protocol: Request.Scheme);

                        var sendResult = await _emailSender.SendEmailAsync(user.Email, "Welcome to NVYRO",
                            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                        if (sendResult)
                        {
                            TempData["FlashMessage.Type"] = "success";
                            TempData["FlashMessage.Text"] = string.Format("Account {0} is added. Please follow email instructions sent to your email.", registerModels.Email);
                            return RedirectToPage("/Login");
                        }
                        else
                        {
                            TempData["FlashMessage.Type"] = "success";
                            TempData["FlashMessage.Text"] = string.Format("Account {0} is added. However, failed to send an confirmation email. Please try logging in.", registerModels.Email);
                            return RedirectToPage("/Login");
                        }
                    }
                    TempData["FlashMessage.Type"] = "danger";
                    TempData["FlashMessage.Text"] = "Error creating account.";
                    return Page();
                }
                return Page();
            }
            catch (Exception)
            {
                return RedirectToPage("/Errors/500");
            }
        }
    }
}
