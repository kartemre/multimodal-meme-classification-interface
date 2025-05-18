using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    public class CreatePostDto
    {
        [Required(ErrorMessage = "Metin alanı zorunludur.")]
        public string Text { get; set; }

        // Base64 formatında resim verisi
        public string ImageBase64 { get; set; }

        // Bu alan controller tarafından otomatik doldurulacak
        public int UserId { get; set; }
    }
} 