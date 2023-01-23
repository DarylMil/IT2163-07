using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace IT2163_07.Models.DTO
{
    public class ResetPasswordModels
    {

        [Required, DataType(DataType.Password), DisplayName("Password"),
           RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{12,}$", ErrorMessage = "Password requires At least one upper case, at least one lower case, at least one digit, at least one special character (#?!@$%^&*-), minimum 12 characters")]
        public string Password { get; set; }
        [Required, DataType(DataType.Password), DisplayName("Confirm Password"), Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        [Required]
        public string Code { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
