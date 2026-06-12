using McpaApi.Jobs;
using McpaApi.Models;
using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;
using MimeKit;

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
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
                message.To.Add(new MailboxAddress("", to));
                message.Subject = subject;

                message.Body = new TextPart("html")
                {
                    Text = body
                };

                // Copias (CC)
                if (ccs != null)
                {
                    foreach (var cc in ccs)
                    {
                        message.Cc.Add(MailboxAddress.Parse(cc));
                    }
                }

                using var client = new SmtpClient();

                await client.ConnectAsync(_settings.SmtpServer, _settings.Port, true); // true = SSL
                await client.AuthenticateAsync(_settings.Username, _settings.Password);

                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation("Correo enviado con éxito");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                _logger.LogError(ex, $"Error al enviar correo: {ex.Message}");
            }
        }
    }
}