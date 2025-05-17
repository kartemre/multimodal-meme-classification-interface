using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    public class ResetPasswordDto
    {
        [Required(ErrorMessage = "Token gereklidir.")]
        public string Token { get; set; }

        [Required(ErrorMessage = "Yeni şifre gereklidir.")]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$", 
            ErrorMessage = "Şifre en az bir büyük harf, bir küçük harf ve bir rakam içermelidir.")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Şifre tekrarı gereklidir.")]
        [Compare("NewPassword", ErrorMessage = "Şifreler eşleşmiyor.")]
        public string ConfirmPassword { get; set; }
    }
} 