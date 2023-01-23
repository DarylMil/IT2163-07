using System.ComponentModel.DataAnnotations;

namespace IT2163_07.Models.DTO
{
    public class TwoFactorModels
    {
        [Required]
        public string OTP { get; set; } = string.Empty;
    }
}
