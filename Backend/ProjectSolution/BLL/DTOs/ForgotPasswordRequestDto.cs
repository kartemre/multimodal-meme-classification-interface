using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    public class ForgotPasswordRequestDto
    {
        [Required(ErrorMessage = "Email adresi gereklidir.")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz.")]
        public string Email { get; set; }
    }
} 