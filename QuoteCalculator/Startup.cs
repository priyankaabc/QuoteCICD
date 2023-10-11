using Hangfire;
using Owin;
using QuoteCalculator.Common;
using QuoteCalculator.Helper;
using System;
using System.Configuration;

namespace QuoteCalculator
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            GlobalConfiguration.Configuration
                .UseSqlServerStorage(ConfigurationManager.ConnectionStrings["quoteCalculatorHangfire"].ConnectionString);
            app.UseHangfireDashboard();
            app.UseHangfireServer();

            RecurringJob.AddOrUpdate(() => EmailReminder.SendEmail(), " 0 7 * * * *", TimeZoneInfo.Local);  //every 7 hours
            RecurringJob.AddOrUpdate(() => EmailReminder.InCompleteQuote(), "0 5 * * * *", TimeZoneInfo.Local); // every 5 hours
            RecurringJob.AddOrUpdate(() => XMLHelper.ResendFailedXML(), "0 2 * * * *", TimeZoneInfo.Local); // every 2 hours
        }
    }
}