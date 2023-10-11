using Hangfire;
using Owin;
using QuoteCalculatorPickfords.Common;
using QuoteCalculatorPickfords.Helper;
using System;
using System.Configuration;

namespace QuoteCalculatorPickfords
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            GlobalConfiguration.Configuration
                .UseSqlServerStorage(ConfigurationManager.ConnectionStrings["quoteCalculatorHangfire"].ConnectionString);
            app.UseHangfireDashboard();
            app.UseHangfireServer();

            RecurringJob.AddOrUpdate(() => EmailReminder.SendEmail(), "* * 7 * * *", TimeZoneInfo.Local);  //every 1 min - 0 0/1 0 ? * *  
            //every 30 seconds -  0/30 0 0 ? * * *  +++++++ every 7 AM * * 7 * * *
            //0/10 * * * * * -- every 10 seconds
            RecurringJob.AddOrUpdate(() => EmailReminder.InCompleteQuote(), "* * 5 * * *", TimeZoneInfo.Local); // every 10 second
            RecurringJob.AddOrUpdate(() => XMLHelper.ResendFailedXML(), "* * */2 * * *", TimeZoneInfo.Local); // every 2 hours
        }
    }
}