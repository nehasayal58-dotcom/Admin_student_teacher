using Admin_Student_Teacher.Models;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Admin_Student_Teacher.Data.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmail(string toEmail, byte[] pdfBytes)
        {
            var message = new MailMessage();
            message.From = new MailAddress(_config["SmtpSettings:UserName"]);
            message.To.Add(toEmail);
            message.Subject = "Filtered Users PDF";
            message.Body = "Please find attached PDF.";

            message.Attachments.Add(
                new Attachment(new MemoryStream(pdfBytes),
                "FilteredUsers.pdf",
                "application/pdf"));

            var client = new SmtpClient(_config["SmtpSettings:Host"],
                int.Parse(_config["SmtpSettings:Port"]));

            client.Credentials = new NetworkCredential(
                _config["SmtpSettings:UserName"],
                _config["SmtpSettings:Password"]);

            client.EnableSsl = true;

            await client.SendMailAsync(message);
        }
    }

}

