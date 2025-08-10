using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SilkSareeEcommerce.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var smtpSettings = _configuration.GetSection("EmailSettings");

                string host = smtpSettings["SMTPServer"];
                int port = int.Parse(smtpSettings["SMTPPort"]);
                string username = smtpSettings["SenderEmail"];
                string password = smtpSettings["SenderPassword"];

                if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    _logger.LogError("SMTP settings are not configured properly");
                    return false;
                }

                using var smtpClient = new SmtpClient(host)
                {
                    Port = port,
                    Credentials = new NetworkCredential(username, password),
                    EnableSsl = true,
                    Timeout = 10000 // 10 seconds timeout
                };

                using var mailMessage = new MailMessage
                {
                    From = new MailAddress(username),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                await smtpClient.SendMailAsync(mailMessage);
                
                _logger.LogInformation($"Order confirmation email sent successfully to {toEmail}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email to {toEmail}: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SendOrderConfirmationEmailAsync(string toEmail, string customerName, int orderNumber, decimal totalAmount)
        {
            var subject = $"Order Confirmation - Order #{orderNumber}";
            var body = $@"
                <h2>Thank you for your order!</h2>
                <p>Dear {customerName},</p>
                <p>Your order #{orderNumber} has been successfully placed.</p>
                <p>Total Amount: ₹{totalAmount}</p>
                <p>We'll start processing your order right away!</p>
                <p>Best regards,<br>Silk Saree Team</p>";

            return await SendEmailAsync(toEmail, subject, body);
        }
    }
}
