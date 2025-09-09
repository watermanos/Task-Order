using System.Net;
using System.Net.Mail;

public static class EmailHelper
{
    public static void SendEmail(string to, string subject, string body)
    {
        var mail = new MailMessage();
        mail.From = new MailAddress(Appsettings.Current.SenderEmail);
        mail.To.Add(to);

        if (Appsettings.Current.PartnerEmails != null)
        {
            foreach (var partner in Appsettings.Current.PartnerEmails)
            {
                mail.To.Add(partner.Trim());
            }
        }

        mail.Subject = subject;
        mail.Body = body;

        var smtp = new SmtpClient("smtp.gmail.com", 587)
        {
            Credentials = new NetworkCredential(Appsettings.Current.SenderEmail, Appsettings.Current.SenderPassword),
            EnableSsl = true
        };

        smtp.Send(mail);
    }
}
