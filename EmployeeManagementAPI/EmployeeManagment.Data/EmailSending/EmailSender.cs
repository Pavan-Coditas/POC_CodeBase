using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagment.Services.EmailSending
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            try
            {
                var client = new SmtpClient("smtp.office365.com", 587)
                {
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential("vasapawan23@outlook.com", "Bittu143@")
                };
                var res = client.SendMailAsync(new MailMessage(from: "vasapawan23@outlook.com", to: email, subject, message));
                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
