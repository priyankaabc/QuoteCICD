using QuoteCalculatorAdmin.Data;
using QuoteCalculatorAdmin.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using NLog;
using System.Configuration;
using QuoteCalculatorAdmin.Models;
using QuoteCalculator.Service.Repository.EmailRepository;
using QuoteCalculator.Service.Models;

namespace QuoteCalculatorAdmin.Common
{
    public class EmailHelper
    {       
        #region private variables
        public static GenericRepository<tbl_EmailSettings> _dbRepositoryEmailSettings = new GenericRepository<tbl_EmailSettings>();
        private static NLog.Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly IEmailSettingsRepository _emailSettings;
        #endregion

        static EmailHelper()
        {
            _emailSettings = new EmailSettingsRepository();
        }


        #region Send Mail Method
        public static bool SendMail(int companyId, string to, string subject, string bodyTemplate, string fromEmailKey, string displayKey, bool isHtml = false, string bcc = "", string ccMail = "", string attachmentFileName = "")
        {

            //var email = System.Configuration.ConfigurationManager.AppSettings["Email"];
            //var password = System.Configuration.ConfigurationManager.AppSettings["passsword"];
            //int PortNumber = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["PortNumber"]);
            //string HostName = System.Configuration.ConfigurationManager.AppSettings["HostName"];

            //List<tbl_EmailSettings> list = _dbRepositoryEmailSettings.GetEntities().ToList();
            List<EmailSettingsModel> list = _emailSettings.GetEmailSettings();

            fromEmailKey = string.IsNullOrEmpty(fromEmailKey) ? "EmailFrom_" + companyId : fromEmailKey;
            //var toEmail = _dbRepositoryEmailSettings.GetEntities().Where(m => m.Key == "EmailBaggage_" + companyId).Select(m => m.Value).FirstOrDefault(); //londondevoffice@gmail.com 
            var toEmail = _emailSettings.GetValueByKey("EmailBaggage");// +companyId
            if (to == null) { to = toEmail; }
            var fromEmail = list.FirstOrDefault(m => m.Key == fromEmailKey)?.Value ?? "";
            //var email = list.FirstOrDefault(m => m.Key == "EmailUserName_" + companyId)?.Value ?? "";
            var email = list.FirstOrDefault(m => m.Key == "EmailUserName")?.Value ?? "";
            var password = list.FirstOrDefault(m => m.Key == "EmailPasssword")?.Value ?? "";
            var Apppassword = ConfigurationManager.AppSettings["EmailAppPassword"].ToString();
            //var password = list.FirstOrDefault(m => m.Key == "EmailPasssword_" + companyId)?.Value ?? "";
            int? portNumberNullable = Convert.ToInt32(list.FirstOrDefault(m => m.Key == "PortNumber")?.Value);
            int PortNumber = portNumberNullable ?? 0;
            string HostName = list.FirstOrDefault(m => m.Key == "HostName")?.Value ?? "";
            
            var DisplayName = "";
            if (displayKey != "")
            {
                displayKey = string.IsNullOrEmpty(displayKey) ? "EmailFrom_" + companyId : displayKey;
                DisplayName = list.FirstOrDefault(m => m.Key == displayKey)?.Value ?? "";
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

            //var email = SettingConfig.MailServerUsername;
            //var password = SettingConfig.MailServerPassword;
            //var fromEmailAddress = SettingConfig.FromEmailAddress;
            //int PortNumber = SettingConfig.MailServerPort;
            //string HostName = SettingConfig.MailServerHost;

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

            if (bcc != "" && bcc != null)
            {
                mail.Bcc.Add(bcc);
            }

            if (attachmentFileName != null && attachmentFileName != "")
            {
                mail.Attachments.Add(new Attachment(attachmentFileName));
            }


            SmtpClient smtp = new SmtpClient();
            smtp.Host = HostName;
            smtp.Port = PortNumber;

            smtp.UseDefaultCredentials = true;
            smtp.EnableSsl = true;
            smtp.Timeout = int.MaxValue;
            smtp.Credentials = new System.Net.NetworkCredential(email, Apppassword);// Enter seders User name and password
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            try
            {
                smtp.Send(mail);
            }
            catch (Exception e)
            {
                logger.Error(e);
                return false;
            }

            return true;
        }

        public static void SendAsyncEmail(int companyId, string to, string subject, string body, string fromEmailKey, string displayKey, bool isHtml = false, string bcc = "",
            string cc = "", string attachmentFileName = "")
        {
            Task task = new Task(() => SendMail(companyId, to, subject, body, fromEmailKey, displayKey, isHtml, bcc, cc, attachmentFileName));
            task.Start();
        }


        #endregion
    }
}