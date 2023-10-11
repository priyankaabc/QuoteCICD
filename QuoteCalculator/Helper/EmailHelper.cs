using NLog;
using QuoteCalculator.Common;
using QuoteCalculator.Data;
using QuoteCalculator.Data.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace QuoteCalculator.Helper
{
    public class EmailHelper
    {
        #region private variables
        public static GenericRepository<tbl_EmailSettings> _dbRepositoryEmailSettings = new GenericRepository<tbl_EmailSettings>();
        private static NLog.Logger _logger = LogManager.GetCurrentClassLogger();
        #endregion

        #region Send Mail Method
        public static bool SendMail(int companyId, string to, string subject, string bodyTemplate, string fromEmailKey, string displayKey, bool isHtml = false, string bcc = "", string ccMail = "", string attachmentFileName = "")
        {
            List<tbl_EmailSettings> list = _dbRepositoryEmailSettings.GetEntities().ToList();
            fromEmailKey = string.IsNullOrEmpty(fromEmailKey) ? "EmailFrom_" + companyId : fromEmailKey;
            var toEmail = _dbRepositoryEmailSettings.GetEntities().Where(m => m.Key == "EmailBaggage_" + companyId).Select(m => m.Value).FirstOrDefault();
            //if (toEmail != null) { to = toEmail; }
            if (to == null) { to = toEmail; }
            var fromEmail = list.Where(m => m.Key == fromEmailKey).FirstOrDefault().Value;
            var email = list.Where(m => m.Key == "EmailUserName_" + companyId).FirstOrDefault().Value;
            var password = list.Where(m => m.Key == "EmailPasssword_" + companyId).FirstOrDefault().Value;
            int PortNumber = Convert.ToInt32(list.Where(m => m.Key == "PortNumber").FirstOrDefault().Value);
            string HostName = list.Where(m => m.Key == "HostName").FirstOrDefault().Value.ToString();
            var DisplayName = "";
            if (displayKey != "")
            {
                displayKey = string.IsNullOrEmpty(displayKey) ? "EmailFrom_" + companyId : displayKey;
                DisplayName = list.Where(m => m.Key == displayKey).FirstOrDefault().Value;
            }
            else
            {
                if (companyId == 1)
                    DisplayName = "Anglo Pacific";
                else if (companyId == 2)
                    DisplayName = "Pickfords";
                else if (companyId == 3)
                    DisplayName = "Excess International Movers";
            }

            MailMessage mail = new MailMessage();
            mail.To.Add(to);
            mail.From = new MailAddress(fromEmail, DisplayName);
            mail.Subject = subject;
            mail.Body = bodyTemplate;
            mail.IsBodyHtml = true;
            //mail.CC.Add("it@anglopacific.co.uk");

            if (ccMail != "" && ccMail != null)
            {
                mail.CC.Add(ccMail);
            }

            if (bcc != "" && bcc != null)
            {
                mail.Bcc.Add(bcc);
            }

            if (attachmentFileName != null && attachmentFileName != "")
                mail.Attachments.Add(new Attachment(attachmentFileName));

            SmtpClient smtp = new SmtpClient();
            smtp.Host = HostName;
            smtp.Port = PortNumber;

            smtp.UseDefaultCredentials = false;
            smtp.EnableSsl = true;

            smtp.Credentials = new System.Net.NetworkCredential(email, password);// Enter seders User name and password
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            try
            {
                smtp.Send(mail);
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return false;
            }

            return true;
        }

        public static void SendAsyncEmail(string to, string subject, string body, string fromEmailKey, string displayKey, bool isHtml = false, string bcc = "",
            string cc = "", string attachmentFileName = "")
        {
            Task task = new Task(() => SendMail( SessionHelper.COMPANY_ID, to, subject, body, fromEmailKey, displayKey, isHtml, bcc, cc, attachmentFileName));
            task.Start();
        }
        #endregion
    }
}