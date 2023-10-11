using QuoteCalculatorPickfords.Data;
using QuoteCalculatorPickfords.Data.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace QuoteCalculatorPickfords.Helper
{
    public class EmailHelper
    {
        #region private variables
        public static GenericRepository<EmailSettings> _dbRepositoryEmailSettings = new GenericRepository<EmailSettings>();
        #endregion

        #region Send Mail Method
        public static bool SendMail(string to, string subject, string bodyTemplate, string fromEmailKey, string displayKey, bool isHtml = false, string bcc = "", string ccMail = "", string attachmentFileName = "")
        {
            List<EmailSettings> list = _dbRepositoryEmailSettings.GetEntities().ToList();
            fromEmailKey = string.IsNullOrEmpty(fromEmailKey) ? "EmailFrom" : fromEmailKey;
            var toEmail = _dbRepositoryEmailSettings.GetEntities().Where(m => m.Key == "CustomEmail").Select(m => m.Value).FirstOrDefault();
            //if (toEmail != null) { to = toEmail; }
            if(to == null) { to = toEmail; }
            var fromEmail = list.Where(m => m.Key == fromEmailKey).FirstOrDefault().Value;
            var email = list.Where(m => m.Key == "EmailUserName").FirstOrDefault().Value;
            var password = list.Where(m => m.Key == "EmailPasssword").FirstOrDefault().Value;
            int PortNumber = Convert.ToInt32(list.Where(m => m.Key == "PortNumber").FirstOrDefault().Value);
            string HostName = list.Where(m => m.Key == "HostName").FirstOrDefault().Value.ToString();
            var DisplayName = "";
            if (displayKey != "")
            {
                displayKey = string.IsNullOrEmpty(displayKey) ? "EmailFrom" : displayKey;
                DisplayName = list.Where(m => m.Key == displayKey).FirstOrDefault().Value;
            }
            else
            {
                DisplayName = "Pickfords";
            }

            MailMessage mail = new MailMessage();
            mail.To.Add(to);
            mail.From = new MailAddress(fromEmail, DisplayName);
            mail.Subject = subject;
            mail.Body = bodyTemplate;
            mail.IsBodyHtml = true;

            if (ccMail != "" && ccMail != null)
            {
                mail.CC.Add(ccMail);
            }

            SmtpClient smtp = new SmtpClient();
            smtp.Host = HostName;
            smtp.Port = PortNumber;

            smtp.UseDefaultCredentials = true;
            smtp.EnableSsl = true;

            smtp.Credentials = new System.Net.NetworkCredential(email, password);// Enter seders User name and password
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            try
            {
                smtp.Send(mail);
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        public static void SendAsyncEmail(string to, string subject, string body, string fromEmailKey, string displayKey, bool isHtml = false, string bcc = "",
            string cc = "", string attachmentFileName = "")
        {
            Task task = new Task(() => SendMail(to, subject, body, fromEmailKey, displayKey, isHtml, bcc, cc, attachmentFileName));
            task.Start();
        }
        #endregion
    }
}