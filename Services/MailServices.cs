
using System.Net;
using System.Net.Mail;
using ForumApi.Models;
using Microsoft.Extensions.Options;


namespace ForumApi.Services
{
    public class MailServices
    { 
          string smtpServer;
           string port;
           string senderEmail;
           string password;
        public MailServices(IOptions<EmailStorage> EmailStorage){
         smtpServer = EmailStorage.Value.SmtpServer;
         port = EmailStorage.Value.Port;
         senderEmail = EmailStorage.Value.SenderEmail;
         password   = EmailStorage.Value.Pass;
            
        }
  

        public bool SendSimpleMessage(string recipientEmail,string Random)
        { 
           
            
            try
            {
                MailMessage mail = new MailMessage
                {
                    From = new MailAddress(senderEmail)
                };
                mail.To.Add(new MailAddress(recipientEmail));
                mail.Subject = "Email Authentication"; //E-posta konusu
                mail.Body = $"{Random}";
                 //E-posta içeriği
              
                SmtpClient smtpClient = new SmtpClient(smtpServer,int.Parse(port));
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(senderEmail, password);
                smtpClient.EnableSsl = true;

                smtpClient.Send(mail);
                Console.WriteLine("E-posta gönderildi.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("E-posta gönderirken bir hata oluştu: " + ex.Message);
                return false;
            }
        }
    }
}
