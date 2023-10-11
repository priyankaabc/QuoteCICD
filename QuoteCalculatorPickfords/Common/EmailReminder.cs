using Kendo.Mvc.Extensions;
using QuoteCalculatorPickfords.Data;
using QuoteCalculatorPickfords.Data.Repository;
using QuoteCalculatorPickfords.Helper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QuoteCalculatorPickfords.Common
{
    public class EmailReminder
    {
        public static void SendEmail()
        {
            try
            {
                quotesEntities context = BaseContext.GetDbContext();

                List<tbl_baggageQuote> quoteList = context.tbl_baggageQuote.Where(m => m.NextExecutionDate != null).ToList();
                List<tbl_OptOut> optoutEmailList = context.tbl_OptOut.ToList();
                for (int i = 0; i < quoteList.Count; i++)
                {
                    if (optoutEmailList.Count > 0)
                    {
                        for (int j = 0; j < optoutEmailList.Count; j++)
                        {
                            if (optoutEmailList[j].Email != quoteList[i].Email)
                            {
                                // Send mail one day before estimated date
                                if (quoteList[i].EstimatedMoveDate.Value.AddDays(-1) == DateTime.Today && quoteList[i].LastExecutionDate == null)
                                {
                                    tbl_EmailTemplate emailModel = context.tbl_EmailTemplate.Where(m => m.ServiceId == 1008).FirstOrDefault();
                                    string bodyTemplate = string.Empty;
                                    if (emailModel != null)
                                    {
                                        bodyTemplate = emailModel.HtmlContent;
                                        bool status = EmailHelper.SendMail(quoteList[i].Email, emailModel.Subject, bodyTemplate, "EmailFrom", "", true, "", "", "");
                                        var quoteObj = context.tbl_baggageQuote.Find(quoteList[i].Id);
                                        if (quoteObj != null)
                                        {
                                            quoteObj.LastExecutionDate = DateTime.Today;
                                            quoteObj.NextExecutionDate = DateTime.Today.AddDays(7);
                                            context.SaveChanges();
                                        }
                                    }
                                }
                                // Send mail on multiple of 7 days
                                else if ((quoteList[i].EstimatedMoveDate >= DateTime.Today) && (quoteList[i].LastExecutionDate != DateTime.Today.AddDays(-1)))
                                {
                                    tbl_EmailTemplate emailModel = context.tbl_EmailTemplate.Where(m => m.ServiceId == 1006).FirstOrDefault();
                                    string bodyTemplate = string.Empty;
                                    if (emailModel != null)
                                    {
                                        bodyTemplate = emailModel.HtmlContent;
                                        bodyTemplate = bodyTemplate.Replace("#CustName#", quoteList[i].Firstname + " " + quoteList[i].Lastname);
                                        bodyTemplate = bodyTemplate.Replace("#QuoteNo#", quoteList[i].Id.ToString());
                                        bodyTemplate = bodyTemplate.Replace("#ToCountry#", quoteList[i].ToCountry);
                                        bool status = EmailHelper.SendMail(quoteList[i].Email, emailModel.Subject, bodyTemplate, "EmailFrom", "",true, "", "", "");
                                        if (status == true)
                                        {
                                            var quoteObj = context.tbl_baggageQuote.Find(quoteList[i].Id);
                                            if (quoteObj != null)
                                            {
                                                quoteObj.LastExecutionDate = DateTime.Today;
                                                quoteObj.NextExecutionDate = DateTime.Today.AddDays(7);
                                                context.SaveChanges();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var error = ex.Message;
            }
        }

        public static void InCompleteQuote()
        {
            try
            {
                quotesEntities context = BaseContext.GetDbContext();
                var bagggaQuoteList = context.tbl_baggageQuote.Select(m => m.Id).ToList();
                var itemList = context.tbl_BaggageItem.Select(m => m.QuoteId).ToList();
                List<int> baggageItemList = new List<int>();

                for (int i = 0; i < itemList.Count; i++)
                {
                    baggageItemList.Add(Convert.ToInt32(itemList[i]));
                }

                var list = bagggaQuoteList.Except(baggageItemList).ToList();
                if (list.Count > 0)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        var quoteObj = context.tbl_baggageQuote.Find(list[i]);
                        if (quoteObj != null)
                        {
                            if (quoteObj.IsSendMailForIncompleteQuote != true)
                            {
                                tbl_EmailTemplate emailModel = context.tbl_EmailTemplate.Where(m => m.ServiceId == 1007).FirstOrDefault();
                                string bodyTemplate = string.Empty;
                                bodyTemplate = emailModel.HtmlContent;
                                bodyTemplate = bodyTemplate.Replace("#CustName#", quoteObj.Firstname + " " + quoteObj.Lastname);
                                bodyTemplate = bodyTemplate.Replace("#SalesRep#", quoteObj.SalesRep);
                                bool status = EmailHelper.SendMail(quoteObj.Email, emailModel.Subject, bodyTemplate, "EmailFrom", "", true, "", "", "");
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
                var error = ex.Message;
            }
        }
    }
}