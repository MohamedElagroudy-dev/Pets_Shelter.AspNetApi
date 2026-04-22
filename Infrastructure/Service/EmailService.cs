using Application.Account.DTOs;
using Core.Interfaces;
using Core.Sharing.Identity;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;

namespace Infrastructure.Service
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration configuration;
        public EmailService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public async Task SendEmail(EmailMessage emailMessage)
        {
            MimeMessage message = new();

            var fromAddress = configuration["EmailSetting:From"] ?? string.Empty;
            var fromName = configuration["EmailSetting:FromName"] ?? "Petopia";

            // Ensure From has the desired display name and set Sender as well
            message.From.Clear();
            message.From.Add(new MailboxAddress(fromName, fromAddress));
            message.Sender = new MailboxAddress(fromName, fromAddress);

            message.Subject = emailMessage.Subject;
            message.To.Add(new MailboxAddress(emailMessage.To, emailMessage.To));
            message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = emailMessage.Content
            };
            using (var smtp = new MailKit.Net.Smtp.SmtpClient())
            {
                try
                {
                    await smtp.ConnectAsync(
                        configuration["EmailSetting:Smtp"],
                       int.Parse(configuration["EmailSetting:Port"]), true);
                    await smtp.AuthenticateAsync(configuration["EmailSetting:Username"],
                        configuration["EmailSetting:Password"]);

                    await smtp.SendAsync(message);
                }
                catch (Exception ex)
                {

                    throw;
                }
                finally
                {
                    smtp.Disconnect(true);
                    smtp.Dispose();
                }
            }
        }
    }
}