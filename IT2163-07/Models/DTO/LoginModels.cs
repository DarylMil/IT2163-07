using System.ComponentModel.DataAnnotations;

namespace IT2163_07.Models.DTO
{
    public class LoginModels
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
