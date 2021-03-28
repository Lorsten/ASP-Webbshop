using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading.Tasks;
using MailKit;
using MimeKit;
using ProjectWeb.Models;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace ProjectWeb.Data
{
    public interface IMailer
    {
        Task SendEmailAsync(string email, string adress, string subject, string body);
    }
    public class MailHandler : IMailer
    {

        private readonly Email _email;

        private readonly IWebHostEnvironment _env;

        public MailHandler(IWebHostEnvironment env,  IOptions<Email> email)
        {
            _env = env;
            _email = email.Value;

        }
        public async Task SendEmailAsync(string email, string adress, string subject, string body){

            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_email.SenderName, _email.SenderEmail));
                message.To.Add(new MailboxAddress(email, adress));
                message.Subject = subject;
                message.Body = new TextPart("html")
                {
                    Text = body
                };
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(_email.Server, _email.Port, SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(_email.Username, _email.Password);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
            }
            catch(Exception e)
            {
                throw new InvalidOperationException(e.Message);
            }
        }

    }
}
