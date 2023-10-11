using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;


namespace QuoteCalculator.Helper
{
    public class EmailNotification
    {
        public static bool SendMailMessage(string exepurl, Exception exdb)
        {
            StringBuilder strError = new StringBuilder();
            strError.AppendLine("<br><b>Error Date Time :</b> ");
            strError.AppendLine(DateTime.Now.ToString());
            strError.AppendLine("<br><b>Error Message :</b> ");
            strError.AppendLine(exdb.Message.ToString());
            strError.AppendLine("<br><b>Log Type:</b>");
            strError.AppendLine(exdb.GetType().Name.ToString());
            strError.AppendLine("<br><b>URL :</b>");
            strError.AppendLine(exepurl);
            strError.AppendLine("<br><b>StackTrace :</b>");
            strError.AppendLine(exdb.StackTrace.ToString());

            var recipient = ConfigurationManager.AppSettings["ToEMail"];
            var cc = ConfigurationManager.AppSettings["CCEMail"];
            var bcc = ConfigurationManager.AppSettings["BCCEMail"];
            EmailSetting emailSetting = new EmailSetting();
            if (string.IsNullOrEmpty(emailSetting.FromEmail))
            {
                return true;
            }

            // Instantiate a new instance of MailMessage 
            MailMessage mailMessage = new MailMessage();

            // Set the sender address of the mail message 
            mailMessage.From = new MailAddress(emailSetting.FromEmail, emailSetting.FromName);

            // Set the recipient address of the mail message 
            // mailMessage.To.Add(new MailAddress(recipient));
            if (!string.IsNullOrEmpty(recipient))
            {
                string[] strRecipient = recipient.Replace(";", ",").TrimEnd(',').Split(new char[] { ',' });

                // Set the Bcc address of the mail message 
                for (int intCount = 0; intCount < strRecipient.Length; intCount++)
                {
                    mailMessage.To.Add(new MailAddress(strRecipient[intCount]));
                }
            }

            // Check if the bcc value is nothing or an empty string 
            if (!string.IsNullOrEmpty(bcc))
            {
                string[] strBCC = bcc.Split(new char[] { ',' });

                // Set the Bcc address of the mail message 
                for (int intCount = 0; intCount < strBCC.Length; intCount++)
                {
                    mailMessage.Bcc.Add(new MailAddress(strBCC[intCount]));
                }
            }

            // Check if the cc value is nothing or an empty value 
            if (!string.IsNullOrEmpty(cc))
            {
                // Set the CC address of the mail message 
                string[] strCC = cc.Split(new char[] { ',' });
                for (int intCount = 0; intCount < strCC.Length; intCount++)
                {
                    mailMessage.CC.Add(new MailAddress(strCC[intCount]));
                }
            }

            // Set the subject of the mail message 
            mailMessage.Subject = "[Quote Calculator] Exception Report";

            // Set the body of the mail message 
            mailMessage.Body = strError.ToString();

            // Set the format of the mail message body as HTML 
            mailMessage.IsBodyHtml = true;

            // Set the priority of the mail message to normal 
            mailMessage.Priority = MailPriority.Normal;

            // Instantiate a new instance of SmtpClient 
            var smtpClient = new SmtpClient();
             
            try
            {
                smtpClient.EnableSsl = true;
                smtpClient.Host = emailSetting.EmailHostName;
                smtpClient.Port = emailSetting.EmailPort;
                smtpClient.Timeout = 100000;
                smtpClient.Credentials = new System.Net.NetworkCredential(emailSetting.FromEmail, emailSetting.EmailPassword);

                // Send the mail message 
                smtpClient.Send(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {

            }
        }

        /// <summary>
        /// Class EmailSetting.
        /// </summary>
        public class EmailSetting
        { /// <summary>
          /// Gets or sets from email.
          /// </summary>
          /// <value>From email.</value>
            public string FromEmail
            {
                get
                {
                    return ConfigurationManager.AppSettings["EmailFrom"];
                }
                set
                {
                    value = ConfigurationManager.AppSettings["EmailFrom"];
                }
            }

            /// <summary>
            /// Gets or sets the name of the email host.
            /// </summary>
            /// <value>The name of the email host.</value>
            public string EmailHostName
            {
                get
                {
                    return ConfigurationManager.AppSettings["HostName"];
                }
                set
                {
                    value = ConfigurationManager.AppSettings["HostName"];
                }
            }

            /// <summary>
            /// Gets or sets the email port.
            /// </summary>
            /// <value>The email port.</value>
            public int EmailPort
            {
                get
                {
                    return Convert.ToInt32(ConfigurationManager.AppSettings["PortNumber"]);
                }
                set
                {
                    value = Convert.ToInt32(ConfigurationManager.AppSettings["PortNumber"]);
                }
            }

            /// <summary>
            /// Gets or sets a value indicating whether [email enable SSL].
            /// </summary>
            /// <value><c>true</c> if [email enable SSL]; otherwise, <c>false</c>.</value>
            public bool EmailEnableSsl { get; set; }

            /// <summary>
            /// Gets or sets the email user-name.
            /// </summary>
            /// <value>The email user name.</value>
            public string EmailUsername
            {
                get
                {
                    return ConfigurationManager.AppSettings["EmailUserName"];
                }
                set
                {
                    value = ConfigurationManager.AppSettings["EmailUserName"];
                }
            }

            /// <summary>
            /// Gets or sets the email password.
            /// </summary>
            /// <value>The email password.</value>
            public string EmailPassword
            {
                get { return ConfigurationManager.AppSettings["EmailPasssword"]; }
                set { value = ConfigurationManager.AppSettings["EmailPasssword"]; }
            }

            /// <summary>
            /// Gets or sets from name.
            /// </summary>
            /// <value>From Name</value>
            public string FromName
            {
                get { return ConfigurationManager.AppSettings["EmailUserName"]; }
                set { value = ConfigurationManager.AppSettings["EmailUserName"]; }
            }
        }

    }
}