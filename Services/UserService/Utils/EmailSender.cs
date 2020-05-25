﻿using Kernel.CustomExceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;
using System.Threading;
using UserService.Interfaces;

namespace UserService.Utils
{
    public class EmailSender : IEmailSender
    {
        private readonly ILogger<EmailSender> logger;

        public EmailSender([FromServices] ILogger<EmailSender> logger)
        {
            this.logger = logger;
        }
        public void SendEmail(string email, ISecretTokenEngine secretTokenEngine)
        {
            var from = new MailAddress("t.platform@mail.ru", "Trading Station");
            var to = new MailAddress(email);
            string secretToken = secretTokenEngine.GetToken(email).ToString();
            string link = $"http://localhost:8080/confirm/{secretToken}";
            string htmlCode = $"<p>Please, click this <a href ={link}>link</a> to confirm registration.</p>";
            var m = new MailMessage(from, to);
            m.Subject = "Registration confirmation";
            m.Body = htmlCode;
            m.IsBodyHtml = true;
            var smtp = new SmtpClient("smtp.mail.ru", 25);
            smtp.Credentials = new NetworkCredential("t.platform@mail.ru", "t123plat");
            smtp.EnableSsl = true;

            var flag=true;

            for (int i = 0, k = 1; i < k; ++i)
            {
                try
                {
                    flag = true;
                    smtp.Send(m);
                }
                catch (SmtpException)
                {
                    Thread.Sleep(10000 * k);
                    flag = false;
                    if (k < 5)
                        k++;
                }
            }

            if(!flag)
            {
                var e = new InternalServerException("Internal Server. Email didn't send");
                logger.LogWarning(e, "InternalServer thrown while trying to Send Eamil to confirm");
                throw e;
            }
        }
    }
}
