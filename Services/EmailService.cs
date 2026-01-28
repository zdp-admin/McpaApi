using McpaApi.Jobs;
using McpaApi.Models;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace McpaApi.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;
        private readonly ILogger<EmailJob> _logger;

        public EmailService(IOptions<EmailSettings> settings, ILogger<EmailJob> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string body, IEnumerable<string>? ccs = null)
        {
            var client = new SmtpClient(_settings.SmtpServer, _settings.Port)
            {
                Credentials = new NetworkCredential(_settings.Username, _settings.Password),
                EnableSsl = true
            };

            var message = new MailMessage
            {
                From = new MailAddress(_settings.SenderEmail, _settings.SenderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
                BodyEncoding = Encoding.UTF8,
                SubjectEncoding = Encoding.UTF8
            };

            message.To.Add(to);

            if (ccs != null)
            {
                foreach (string cc in ccs)
                {
                    message.To.Add(cc);
                }
            }

            try
            {
                await client.SendMailAsync(message);
                _logger.LogInformation("Correo enviado con éxito");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al enviar correo: {ex.Message}");
            }
        }
    }
}