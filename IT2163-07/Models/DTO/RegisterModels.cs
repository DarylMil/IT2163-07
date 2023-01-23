using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace IT2163_07.Models.DTO
{
    public class RegisterModels
    {
        [Required, MaxLength(50), DisplayName("Full Name"), RegularExpression("[A-z ]+", ErrorMessage ="Name can only contain alphabets and space.")]   
        public string FullName { get; set; } = string.Empty;
        [Required, DisplayName("Credit Card Number"), RegularExpression("[0-9]{16}", ErrorMessage ="Credit Cards Number Without Spacing. eg: 4444111122223333")]
        public string CreditCardNo { get; set; } = string.Empty;
        [Required, MaxLength(1, ErrorMessage ="Gender must be either M or F"), RegularExpression("(M|F)",ErrorMessage = "Gender must be either M or F")]
        public string Gender { get; set; } = string.Empty;
        [Required, MaxLength(8), DisplayName("Mobile Number"), DataType(DataType.PhoneNumber), RegularExpression("[0-9]{8}", ErrorMessage ="Invalid Mobile Number Format. Please key in eg: 88888888")]
        public string MobileNo { get; set; } = string.Empty;
        [Required, DisplayName("Delivery Address")]
        public string DeliveryAddress { get; set; } = string.Empty;
        [Required, EmailAddress, RegularExpression(@"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*" + "@" + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$",ErrorMessage ="Invalid Email Format")]
        public string Email { get; set; } = string.Empty;
        [Required, DataType(DataType.Password), DisplayName("Password"), 
            RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{12,}$", ErrorMessage = "Password requires At least one upper case, at least one lower case, at least one digit, at least one special character (#?!@$%^&*-), minimum 12 characters")]
        public string Password { get; set; } = string.Empty;
        [Required, DataType(DataType.Password), DisplayName("Confirm Password"), Compare("Password", ErrorMessage ="The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
        public string PhotoURL { get; set; } = string.Empty;
        [Required, DisplayName("About Me")]
        public string AboutMe { get; set; } = string.Empty;
        [Required, DisplayName("Membership Rank"), RegularExpression(@"^(Bronze|Silver|Gold)$", ErrorMessage ="Rank can only be Bronze, Silver or Gold")]
        public string Role { get; set; } = "Bronze";
    }
}
