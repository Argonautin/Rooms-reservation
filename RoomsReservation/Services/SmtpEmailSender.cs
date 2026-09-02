using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using RoomsReservation.Data.Models;

namespace RoomsReservation.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task<bool> SendAsync(string to, string subject, string htmlBody)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.Name, _settings.From));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;
            message.Body = new TextPart(TextFormat.Html) { Text = htmlBody };

            using var smtp = new SmtpClient();
            try
            {
                await smtp.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_settings.Username, _settings.Password);
                await smtp.SendAsync(message);
                await smtp.DisconnectAsync(true);
                return true;
            }
            catch (Exception ex)
            {
                // Tu możesz zalogować błąd (ILogger, Serilog itp.)
                Console.WriteLine($"Błąd wysyłki maila: {ex.Message}");
                return false;
            }
        }
    }
}