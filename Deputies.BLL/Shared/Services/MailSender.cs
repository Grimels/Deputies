using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Deputies.BLL.Shared.Services
{
    public class MailSender : IMailSender
    {
        public void Send(string inputEmail, string subject, string body)
        {
            MailMessage msg = new MailMessage();

            msg.From = new MailAddress("inquiriesua@gmail.com");
            msg.To.Add(inputEmail);
            msg.Subject = subject;
            msg.Body = body;
            msg.IsBodyHtml = true;
            SmtpClient client = new SmtpClient();
            client.UseDefaultCredentials = true;
            client.Host = "smtp.gmail.com";
            client.Port = 587;
            client.EnableSsl = true;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Credentials = new NetworkCredential("inquiriesua@gmail.com", "00bc00bc00");
            client.Timeout = 20000;
            try
            {
                client.Send(msg);
            }
            catch (Exception ex)
            {

            }
            finally
            {
                msg.Dispose();
            }
        }
    }
}
