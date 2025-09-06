using System.Net;
using System.Net.Mail;

public static class EmailHelper
{
    public static void SendEmail(string to, string subject, string body)
    {
        var mail = new MailMessage();
        mail.From = new MailAddress("example@example.com"); 
        mail.To.Add(to);
        mail.Subject = subject;
        mail.Body = body;

        var smtp = new SmtpClient("smtp.gmail.com", 587)
        {
            Credentials = new NetworkCredential("example@example.com", "app password"), 
            EnableSsl = true
        };

        smtp.Send(mail);
    }
}
