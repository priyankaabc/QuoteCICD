using NLog;
using Kendo.Mvc.Extensions;
using QuoteCalculator.Data;
using QuoteCalculator.Data.Repository;
using QuoteCalculator.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

namespace QuoteCalculator.Common
{
    public class EmailReminder
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Send Weekly followup emails and one for sending the day before estimated move date
        /// </summary>
        public static void SendEmail()
        {
            try
            {
                quotesEntities context = BaseContext.GetDbContext();

                List<tbl_baggageQuote> quoteList = context.tbl_baggageQuote.Where(m => m.NextExecutionDate != null).ToList();
                List<string> optoutEmails = context.tbl_OptOut.ToList().Select(x => x.Email.ToUpper()).ToList();
                
                for (int i = 0; i < quoteList.Count; i++)
                {
                    //If Optout - do not send email
                    if (optoutEmails.Contains(quoteList[i].Email.ToUpper()) || quoteList[i].Company == 0 || string.IsNullOrEmpty(quoteList[i].Email))
                        continue;

                    int quoteId = quoteList[i].Id;
                    int currentCompanyId = quoteList[i].Company.Value;

                    tbl_QuoteAmount quoteAmountModel = context.tbl_QuoteAmount.Where(m => m.QuoteId == quoteId && m.MoveType == "EXB").FirstOrDefault();

                    //IS BOOKED - Do not send auot email
                    if(quoteList[i].Price.HasValue)
                        continue;

                    //if there any any booking for same customer reference, do not send email.
                    if (quoteAmountModel == null ||
                            context.tbl_QuoteAmount.Where(x => x.CustomerReferenceNo == quoteAmountModel.CustomerReferenceNo && x.IsBooked == true).Count() > 0)
                        continue;

                    DateTime? estimatedMoveDate = quoteList[i].EstimatedMoveDate;
                    DateTime? lastExecutionDate = quoteList[i].LastExecutionDate;
                    DateTime? nextExecutionDate = quoteList[i].NextExecutionDate;

                    //Do not send emails if estimated move date is either today or in past
                    if ((estimatedMoveDate ?? DateTime.Today) <= DateTime.Today)
                        continue;

                    //Do not sent emails if company is not aglo or pickfords or excess baggage
                    if (quoteList[i].Company != 1 && quoteList[i].Company != 2 && quoteList[i].Company != 3)
                        continue;

                    tbl_EmailTemplate emailModel = null;

                    //Choose template - if move date is tomorrow
                    if (estimatedMoveDate.Value.AddDays(-1) == DateTime.Today)
                        emailModel = context.tbl_EmailTemplate.Where(m => m.ServiceId == (currentCompanyId == 1 ? 1008 : (currentCompanyId == 2) ? 1009 : 2021)).FirstOrDefault();
                    //Choose template for weekly followup - execution date is today and email is not already sent today
                    else if (nextExecutionDate == DateTime.Today && (lastExecutionDate == null || lastExecutionDate < DateTime.Today))
                        emailModel = context.tbl_EmailTemplate.Where(m => m.ServiceId == (currentCompanyId == 1 ? 1006 : (currentCompanyId == 2) ? 1009 : 2021)).FirstOrDefault();

                    string bodyTemplate = string.Empty;
                    if (emailModel != null)
                    {
                        bodyTemplate = emailModel.HtmlContent;
                        bodyTemplate = bodyTemplate.Replace("#CustName#", quoteList[i].Firstname + " " + quoteList[i].Lastname);
                        //bodyTemplate = bodyTemplate.Replace("#custName#", quoteList[i].Firstname + " " + quoteList[i].Lastname); // pickfords template has custName and changing template has impact on emails
                        bodyTemplate = bodyTemplate.Replace("#QuoteNo#", quoteAmountModel.CustomerReferenceNo + "/" + quoteAmountModel.CustomerQuoteNo);
                        bodyTemplate = bodyTemplate.Replace("#ToCountry#", quoteList[i].ToCountry);
                        string salesRepName = string.Empty;
                        string salesRepEmail = string.Empty;
                        if (!string.IsNullOrEmpty(quoteList[i].SalesRep))
                        {
                            string saleRepCode = quoteList[i].SalesRep;
                            user userModel = context.user.Where(m => m.SalesRepCode == saleRepCode && m.CompanyId == quoteList[i].Company).FirstOrDefault();
                            salesRepName = userModel.username;
                            salesRepEmail = userModel.email;
                        }
                        else
                        {
                            if (quoteList[i].Company == 1)
                            {
                                salesRepName = "Baggage Team";
                                salesRepEmail = "baggage@anglopacific.co.uk";
                            }
                            else if (quoteList[i].Company == 2)
                            {
                                salesRepName = "Pickfords Baggage Team";
                                salesRepEmail = "shipping@pickfords-baggage.co.uk";
                            }
                            else if (quoteList[i].Company == 3)
                            {
                                salesRepName = "Sales Team";
                                salesRepName = "sales@excess-international.com";
                            }
                        }
                        bodyTemplate = bodyTemplate.Replace("#salesRepName#", salesRepName);
                        bodyTemplate = bodyTemplate.Replace("#salesRepEmail#", salesRepEmail);

                        //Update database first and then send email. In case if saving to database fails, it'll send email in next run.
                        // If send emails first and save to database, on databse failure, system will end up sendign 1000s emails

                        var quoteObj = context.tbl_baggageQuote.Find(quoteId);
                        if (quoteObj != null)
                        {
                            quoteObj.LastExecutionDate = DateTime.Today;
                            quoteObj.NextExecutionDate = DateTime.Today.AddDays(7);
                            //If estimated move date is tomorrow or before, set the next execution date to null
                            if (quoteObj.EstimatedMoveDate.Value <= DateTime.Today.AddDays(1))
                                quoteObj.NextExecutionDate = null;
                            //Only allow to send 4 follow up emails and hence anything after 28 days, 
                            //do not send any follow up emails. In that case, set the next date to
                            //the day before estimated move date - which will send that email on that day
                            else if (quoteObj.NextExecutionDate.Value.AddDays(-29) > quoteObj.CreatedDate)
                                quoteObj.NextExecutionDate = quoteObj.EstimatedMoveDate.Value.AddDays(-1);

                            context.SaveChanges();
                        }

                        if (quoteList[i].Company.HasValue)
                        {
                            logger.Info("SENDING REMINDER EMAIL TO: " + quoteList[i].Email + " [" + quoteList[i].Price + "]");
                            bool status = EmailHelper.SendMail(quoteList[i].Company.Value, quoteList[i].Email, emailModel.Subject, bodyTemplate, "EmailFrom_" + quoteList[i].Company, "", true, "", "", "");
                            logger.Info("REMINDER EMAIL SENT TO: " + quoteList[i].Email + " [" + quoteList[i].Price + "]");

                            //Undo DB Changes if send mail was not successful
                            if (status == false)
                            {
                                quoteObj.LastExecutionDate = lastExecutionDate;
                                quoteObj.NextExecutionDate = nextExecutionDate;
                                context.SaveChanges();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public static void InCompleteQuote()
        {
            try
            {
                quotesEntities context = BaseContext.GetDbContext();
                var bagggaQuoteList = context.tbl_baggageQuote.Where(x => (DbFunctions.TruncateTime(x.CreatedDate) == DateTime.Today) && x.Company == 1).Select(m => m.Id.ToString()).ToList();
                var baggageItemList = context.tbl_BaggageItem.Select(m => m.QuoteId.ToString()).ToList();
                var list = bagggaQuoteList.Except(baggageItemList).ToList();

                for (int i = 0; i < list.Count; i++)
                {
                    var quoteObj = context.tbl_baggageQuote.Find(Convert.ToInt32(list[i]));
                    if (quoteObj != null)
                    {
                        if (quoteObj.IsSendMailForIncompleteQuote != true && !string.IsNullOrEmpty(quoteObj.Email) && string.IsNullOrEmpty(quoteObj.SalesRep))
                        {
                            tbl_EmailTemplate emailModel = context.tbl_EmailTemplate.Where(m => m.ServiceId == 1007).FirstOrDefault();
                            string bodyTemplate = string.Empty;
                            bodyTemplate = emailModel.HtmlContent;
                            bodyTemplate = bodyTemplate.Replace("#CustName#", quoteObj.Firstname + " " + quoteObj.Lastname);
                            string quoteId = CommonHelper.Encode(quoteObj.Id.ToString());
                            bodyTemplate = bodyTemplate.Replace("#IncompleteQuoteLink#", "<a href='https://quotes.anglopacific.co.uk/Baggage?baggageId="+ quoteId +"'>click here</a>");    
                            var display = quoteObj.Company == 1 ? "DisplayAngloPacific" : (quoteObj.Company == 2 ? "DisplayPickfords" : "DisplayExcessBaggage");                            if (quoteObj.Company.HasValue)
                            {
                                bool status = EmailHelper.SendMail(quoteObj.Company.Value, quoteObj.Email, emailModel.Subject, bodyTemplate, "EmailFrom_" + quoteObj.Company, display, true, "", "", "");
                                if (status == true)
                                {
                                    quoteObj.IsSendMailForIncompleteQuote = true;
                                    context.SaveChanges();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
    }
}