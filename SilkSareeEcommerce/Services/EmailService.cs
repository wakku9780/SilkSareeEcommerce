using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace SilkSareeEcommerce.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SendEmail(string toEmail, string subject, string body)
        {
            var smtpSettings = _configuration.GetSection("EmailSettings");

            string host = smtpSettings["SMTPServer"];
            int port = int.Parse(smtpSettings["SMTPPort"]);
            string username = smtpSettings["SenderEmail"];
            string password = smtpSettings["SenderPassword"];

            var smtpClient = new SmtpClient(host)
            {
                Port = port,
                Credentials = new NetworkCredential(username, password),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(username),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            smtpClient.Send(mailMessage);
        }
    }
}
