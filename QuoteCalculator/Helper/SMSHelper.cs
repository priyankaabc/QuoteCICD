using Nexmo.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuoteCalculator.Helper
{
    public class SMSHelper
    {
        public static string SendSMS(int companyId, string toMobileNo,string smsText)
        {
            //const string API_KEY = "0863e13c";
            //const string API_SECRET = "99699919183ac75b";
            try
            {
                string SMS_KEY = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["NEXMO_SMS_KEY"]);
                string SECRET_KEY = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["NEXMO_SECRET_KEY"]);
                string FROM_NAME;
                if (companyId == 1)
                    FROM_NAME = "ANGLO PACIFIC";
                else if (companyId == 2)
                    FROM_NAME = "PICKFORDS";
                else if (companyId == 3)
                    FROM_NAME = "EXCESS INTERNATIONAL MOVERS";
                else
                    FROM_NAME = "";

                var client = new Client(creds: new Nexmo.Api.Request.Credentials(
                    nexmoApiKey: SMS_KEY, nexmoApiSecret: SECRET_KEY));


                var results = client.SMS.Send(new SMS.SMSRequest
                {
                    from = FROM_NAME,
                    to = toMobileNo,
                    text = smsText
                });

                return results.message_count;
            }
            catch(Exception ex)
            {
                return null;
            }
        }
    }
}