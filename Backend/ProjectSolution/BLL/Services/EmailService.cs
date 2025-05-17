using System;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using BLL.Interfaces;

namespace BLL.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        private readonly string _fromEmail;
        private readonly string _fromName;
        private readonly string _smtpPassword;
        private readonly string _frontendUrl;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _fromEmail = _configuration["Email:FromAddress"];
            _fromName = _configuration["Email:FromName"];
            _smtpPassword = _configuration["Email:SmtpPassword"];
            _frontendUrl = _configuration["Frontend:BaseUrl"];

            if (string.IsNullOrEmpty(_fromEmail) || string.IsNullOrEmpty(_smtpPassword))
            {
                throw new InvalidOperationException("Email ayarları eksik. Lütfen appsettings.json dosyasını kontrol edin.");
            }
        }

        public async Task SendPasswordResetEmailAsync(string toEmail, string resetToken)
        {
            try
            {
                var resetLink = $"{_frontendUrl}/reset-password?token={resetToken}";
                var fromAddress = new MailAddress(_fromEmail, _fromName);
                var toAddress = new MailAddress(toEmail);
                
                string body = GetPasswordResetEmailTemplate(resetLink);

                using var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = "Şifre Sıfırlama İsteği",
                    Body = body,
                    IsBodyHtml = true
                };

                using var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(_fromEmail, _smtpPassword),
                    Timeout = 20000
                };

                await smtp.SendMailAsync(message);
                _logger.LogInformation($"Şifre sıfırlama e-postası gönderildi: {toEmail}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"E-posta gönderimi başarısız: {ex.Message}");
                throw new Exception("Şifre sıfırlama e-postası gönderilemedi. Lütfen daha sonra tekrar deneyin.");
            }
        }

        private string GetPasswordResetEmailTemplate(string resetLink)
        {
            return $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <h2 style='color: #333; text-align: center;'>Şifre Sıfırlama İsteği</h2>
                    <div style='background-color: #f8f9fa; padding: 20px; border-radius: 5px;'>
                        <p>Merhaba,</p>
                        <p>Hesabınız için bir şifre sıfırlama isteği aldık. Şifrenizi sıfırlamak için aşağıdaki butona tıklayın:</p>
                        <div style='text-align: center; margin: 30px 0;'>
                            <a href='{resetLink}' style='background-color: #007bff; color: white; padding: 12px 24px; text-decoration: none; border-radius: 5px; display: inline-block;'>
                                Şifremi Sıfırla
                            </a>
                        </div>
                        <p>Ya da bu bağlantıyı tarayıcınıza kopyalayın:</p>
                        <p style='background-color: #e9ecef; padding: 10px; border-radius: 3px; word-break: break-all;'>
                            {resetLink}
                        </p>
                        <p><strong>Not:</strong> Bu bağlantı 24 saat boyunca geçerlidir.</p>
                        <p>Eğer bu isteği siz yapmadıysanız, bu e-postayı görmezden gelebilirsiniz.</p>
                        <hr style='border: none; border-top: 1px solid #ddd; margin: 20px 0;'>
                        <p style='color: #666; font-size: 12px;'>Bu otomatik olarak gönderilen bir e-postadır. Lütfen yanıtlamayınız.</p>
                    </div>
                </div>";
        }
    }
} 