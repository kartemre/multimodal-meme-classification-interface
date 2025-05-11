using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.DTOs;
using FluentValidation;

namespace BLL.Validators
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequestDto>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Ad alanı zorunludur.")
                .MaximumLength(50).WithMessage("Ad 50 karakterden uzun olamaz.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Soyad alanı zorunludur.")
                .MaximumLength(50).WithMessage("Soyad 50 karakterden uzun olamaz.");

            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Kullanıcı adı zorunludur.")
                .MaximumLength(50).WithMessage("Kullanıcı adı 50 karakterden uzun olamaz.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Şifre zorunludur.")
                .MinimumLength(3).WithMessage("Şifre en az 3 karakter olmalıdır."); // Şimdilik 3 karakter, daha sonra 8 yapılacak
                //.Matches("[A-Z]").WithMessage("Şifre en az bir büyük harf içermelidir.")
                //.Matches("[a-z]").WithMessage("Şifre en az bir küçük harf içermelidir.")
                //.Matches("[0-9]").WithMessage("Şifre en az bir rakam içermelidir.")
                //.Matches("[^a-zA-Z0-9]").WithMessage("Şifre en az bir özel karakter içermelidir (!@#$%^&* vb.).");

            RuleFor(x => x.PasswordControl)
                .Equal(x => x.Password).WithMessage("Şifreler uyuşmuyor.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("E-posta zorunludur.")
                .EmailAddress().WithMessage("Geçersiz e-posta formatı.");
        }
    }
}
