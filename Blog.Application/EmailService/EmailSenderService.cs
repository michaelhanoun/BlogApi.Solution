using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Blog.Core.Services.Contracts;
using Microsoft.Extensions.Configuration;

namespace Blog.Application.EmailService
{
    public class EmailSenderService : IEmailSenderService
    {
        private readonly IConfiguration _configuration;

        public EmailSenderService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task SendEmailAsync(string to, string subject, string body)
        {
            MailMessage message = new MailMessage();
            message.To.Add(to);
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;
            message.From =new MailAddress(_configuration["EmailService:Sender"] ??"");
            SmtpClient smtpClient = new SmtpClient(_configuration["EmailService:SmtpServer"], int.Parse(_configuration["EmailService:SmtpPort"] ?? "0")) { Credentials = new NetworkCredential(_configuration["EmailService:Sender"], _configuration["EmailService:Password"]) ,EnableSsl = true };
            await smtpClient.SendMailAsync(message);    
        }
    }
}
