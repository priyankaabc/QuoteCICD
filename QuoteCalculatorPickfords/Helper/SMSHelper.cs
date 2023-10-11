using Nexmo.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuoteCalculatorPickfords.Helper
{
    public class SMSHelper
    {
        public static string SendSMS(string toMobileNo,string smsText)
        {
            const string API_KEY = "0863e13c";
            const string API_SECRET = "99699919183ac75b";
            const string FROM_NAME = "Pickfords";
            var client = new Client(creds: new Nexmo.Api.Request.Credentials(
                nexmoApiKey: API_KEY, nexmoApiSecret: API_SECRET));

            
            var results = client.SMS.Send(new SMS.SMSRequest
            {
                from = FROM_NAME,
                to = toMobileNo,
                text = smsText
            });

            return results.message_count;
        }
    }
}