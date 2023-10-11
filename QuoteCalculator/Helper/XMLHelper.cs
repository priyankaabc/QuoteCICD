using Nexmo.Api.Logging;
using NLog;
using QuoteCalculator.Common;
using QuoteCalculator.Data;
using QuoteCalculator.Data.Repository;
using QuoteCalculator.XmlService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using QuoteCalculator.Helper;
using System.Threading.Tasks;
using QuoteCalculator.Models;
using System.Xml;

namespace QuoteCalculator.Helper
{
    public class XMLHelper
    {
        private static NLog.Logger logger = LogManager.GetCurrentClassLogger();
        #region private variables
        private static GenericRepository<tbl_Vehicle> _dbRepositoryVehicle = new GenericRepository<tbl_Vehicle>();
        private static GenericRepository<tbl_FailedXML> _dbRepositoryFailedXML = new GenericRepository<tbl_FailedXML>();
        private static GenericRepository<tbl_EmailTemplate> _dbRepositoryEmailTemplete = new GenericRepository<tbl_EmailTemplate>();
        public static GenericRepository<tbl_EmailSettings> _dbRepositoryEmailSettings = new GenericRepository<tbl_EmailSettings>();
        //public static GenericRepository<tbl_QuoteAmount> _dbRepositoryQuoteAmount = new GenericRepository<tbl_QuoteAmount>();
        #endregion

        #region Methods
        public static async Task<string> GenerateXmlFCLFile(SP_GetXmlData_Result xmlResult, string file, bool isBookNow = false)
        {
            string QuoteStatus = string.Empty;
            if (isBookNow == true)
            {
                QuoteStatus = "BOOK NOW";
            }
            else if (xmlResult.QuoteNo == null && xmlResult.EnquiryNo != null)
            {
                QuoteStatus = "ENQUIRY";
            }
            else
            {
                QuoteStatus = "QUOTE";
            }
            XNamespace url = "http://www.moveware.com.au/MovewareConnect.xsd";
            XDocument xdoc = new XDocument(
                //new XDeclaration("1.0", "UTF-8",null),
                new XElement(url + "MWConnect",
                    new XElement("Details",
                        new XElement("Comments", (Math.Round(xmlResult.FCL ?? 0, 2, MidpointRounding.ToEven)).ToString()),
                        new XElement("LiveTest", "TEST") //Static for Testing
                    ),
                    new XElement("MessageType",
                        new XElement("WebQuote",
                            new XElement("Removal",
                                new XElement("CompanyCode", "AP"),
                                new XElement("Branch", xmlResult.Branch),
                                new XElement("Type", "EXV"),
                                new XElement("QuoteStatus", QuoteStatus),
                                new XElement("Salesrep", "WEB"),
                                new XElement("File", file),
                                new XElement("Method", "SEA"),
                                new XElement("Service", "EXFCL"),
                                new XElement("ixType", "DTDE"),
                                new XElement("Referral", xmlResult.Referral),
                                new XElement("EstMoveDate", xmlResult.EstimatedMoveDate),
                                new XElement("Origin",
                                    new XElement("Name",
                                        new XElement("Title", xmlResult.TitleName),
                                        new XElement("FullName", xmlResult.Firstname + '/' + xmlResult.Lastname),
                                        new XElement("Email", xmlResult.Email),
                                        new XElement("Phone", xmlResult.Telephone),
                                        new XElement("Mobile", xmlResult.Mobile),
                                        new XElement("Company", "")
                                    ),
                                    new XElement("Address",
                                        new XElement("Postcode", xmlResult.PostCode),
                                        new XElement("CountryCode",
                                            new XElement("CCode", "GB")
                                        )
                                    )
                                ),
                                new XElement("Destination",
                                    new XElement("Address",
                                        new XElement("Suburb", xmlResult.Suburb),
                                        new XElement("CountryCode",
                                            new XElement("CCode", xmlResult.Des_CountryCode)
                                        )
                                    )
                                ),
                                new XElement("Make", xmlResult.Make),
                                new XElement("Model", xmlResult.Model),
                                new XElement("SpecialRequirement", new XCData(xmlResult.SpecialRequirement)),
                                new XElement("EstimatedVolume", (Math.Round(xmlResult.Volume_FCL ?? 0, 2, MidpointRounding.ToEven)).ToString()),
                                new XElement("VehicleType", xmlResult.VehicleType),
                                new XElement("NoVehicles", "1"),
                                new XElement("DuplicateEnquiry", xmlResult.Duplicate)
                            )
                        )
                    )
                )
            );

            string xml = xdoc.ToString();
            string token = await Navision.GetValidAccessToken();
            string quoteType = QuoteStatus == "QUOTE" ? "VehicleQuote" : "VehicleBookNow";
            string status;
            if (quoteType != "VehicleBookNow")
            {
                status = await Navision.SubmitXmlAsync(token, xml, quoteType);
            }
            else
            {
                status = "CREATED";
            }            

            string extension = Path.GetExtension("FCL_Quote.xml");
            string fileName = Guid.NewGuid() + "_" + Path.GetFileName("FCL_Quote.xml");
            string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploads/FCLQuote_xml/" + fileName);

            saveXMLStatus(physicalPath, status, quoteType);

            xdoc.Save(physicalPath);

            var XMLEmail = System.Configuration.ConfigurationManager.AppSettings["XMLEmail"];

            if (!string.IsNullOrEmpty(XMLEmail))
            {
                var subject = "Vehicle " + QuoteStatus + " (" + file + ")";
                SendXMLEmail(XMLEmail, "Vehicle", subject, physicalPath);
            }

            return physicalPath;
        }

        public static async Task<string> GenerateXmlGDPFile(SP_GetXmlData_Result xmlResult, string file, bool isBookNow = false)
        {
            string QuoteStatus = string.Empty;
            if (isBookNow == true)
            {
                QuoteStatus = "BOOK NOW";
            }
            else if (xmlResult.QuoteNo == null && xmlResult.EnquiryNo != null)
            {
                QuoteStatus = "Enquiry";
            }
            else
            {
                QuoteStatus = "QUOTE";
            }
            XNamespace url = "http://www.moveware.com.au/MovewareConnect.xsd";
            XDocument xdoc = new XDocument(
                //new XDeclaration("1.0", "UTF-8",null),
                new XElement(url + "MWConnect",
                    new XElement("Details",
                        new XElement("Comments", (Math.Round(xmlResult.GPG ?? 0, 2, MidpointRounding.ToEven)).ToString()),
                        new XElement("LiveTest", "TEST") //Static for Testing
                    ),
                    new XElement("MessageType",
                        new XElement("WebQuote",
                            new XElement("Removal",
                                new XElement("CompanyCode", "AP"),
                                new XElement("Branch", xmlResult.Branch),
                                new XElement("Type", "EXV"),
                                new XElement("QuoteStatus", QuoteStatus),
                                new XElement("Salesrep", "WEB"),
                                new XElement("File", file),
                                new XElement("Method", "SEA"),
                                new XElement("Service", "EXP-GPGE"),
                                new XElement("ixType", "DTDE"),
                                new XElement("Referral", xmlResult.Referral),
                                new XElement("EstMoveDate", xmlResult.EstimatedMoveDate),
                                new XElement("Origin",
                                    new XElement("Name",
                                        new XElement("Title", xmlResult.TitleName),
                                        new XElement("FullName", xmlResult.Firstname + '/' + xmlResult.Lastname),
                                        new XElement("Email", xmlResult.Email),
                                        new XElement("Phone", xmlResult.Telephone),
                                        new XElement("Mobile", xmlResult.Mobile),
                                        new XElement("Company", "")
                                    ),
                                    new XElement("Address",
                                        new XElement("Postcode", xmlResult.PostCode),
                                        new XElement("CountryCode",
                                            new XElement("CCode", "GB")
                                        )
                                    )
                                ),
                                new XElement("Destination",
                                    new XElement("Address",
                                        new XElement("Suburb", xmlResult.Suburb),
                                        new XElement("CountryCode",
                                            new XElement("CCode", xmlResult.Des_CountryCode) //Destination Country Code
                                        )
                                    )
                                ),
                                new XElement("Make", xmlResult.Make),
                                new XElement("Model", xmlResult.Model),
                                new XElement("SpecialRequirement", new XCData(xmlResult.SpecialRequirement)),
                                new XElement("EstimatedVolume", (Math.Round(xmlResult.Volume_GRP ?? 0, 2, MidpointRounding.ToEven)).ToString()),
                                new XElement("VehicleType", xmlResult.VehicleType),
                                new XElement("NoVehicles", "1"), //Static value
                                new XElement("DuplicateEnquiry", xmlResult.Duplicate)
                            )
                        )
                    )
                )
            );

            string xml = xdoc.ToString();
            string token = await Navision.GetValidAccessToken();
            string quoteType = QuoteStatus == "QUOTE" ? "VehicleQuote" : "VehicleBookNow";
            string status;
            if (quoteType != "VehicleBookNow")
            {
                status = await Navision.SubmitXmlAsync(token, xml, quoteType);
            }
            else
            {
                status = "CREATED";
            }

            //Guid id = new Guid();
            string extension = Path.GetExtension("GDP_Quote.xml");
            string fileName = Guid.NewGuid() + "_" + Path.GetFileName("GDP_Quote.xml");
            string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploads/GDPQuote_XML/" + fileName);

            saveXMLStatus(physicalPath, status, quoteType);

            var XMLEmail = System.Configuration.ConfigurationManager.AppSettings["XMLEmail"];
            xdoc.Save(physicalPath);
            if (!string.IsNullOrEmpty(XMLEmail))
            {
                var subject = "Vehicle " + QuoteStatus + " (" + file + ")";
                SendXMLEmail(XMLEmail, "Vehicle", subject, physicalPath);
            }
            return physicalPath;
        }

        public static async Task<string> GenerateXmlEnquiryFile(SP_GetXmlData_Result xmlResult, string file)
        {
            XNamespace url = "http://www.moveware.com.au/MovewareConnect.xsd";
            XDocument xdoc = new XDocument(
                //new XDeclaration("1.0", "UTF-8",null),
                new XElement(url + "MWConnect",
                    new XElement("Details",
                        new XElement("LiveTest", "TEST") //Static for Testing
                    ),
                    new XElement("MessageType",
                        new XElement("WebQuote",
                            new XElement("Removal",
                                new XElement("CompanyCode", "AP"),
                                new XElement("Branch", xmlResult.Branch),
                                new XElement("Type", "EXV"),
                                new XElement("QuoteStatus", "Enquiry"),
                                new XElement("Salesrep", "WEB"),
                                new XElement("File", file),
                                //new XElement("Method", "SEA"),
                                //new XElement("Service", "FCL20GP"),
                                //new XElement("ixType", "TOPORT"),
                                new XElement("Referral", xmlResult.Referral),
                                new XElement("EstMoveDate", xmlResult.EstimatedMoveDate),
                                new XElement("Origin",
                                    new XElement("Name",
                                        new XElement("Title", xmlResult.TitleName),
                                        new XElement("FullName", xmlResult.Firstname + '/' + xmlResult.Lastname),
                                        new XElement("Email", xmlResult.Email),
                                        new XElement("Phone", xmlResult.Telephone),
                                        new XElement("Mobile", xmlResult.Mobile),
                                        new XElement("Company", "")
                                    ),
                                    new XElement("Address",
                                        new XElement("Postcode", xmlResult.PostCode),
                                        new XElement("CountryCode",
                                            new XElement("CCode", "GB")
                                        )
                                    )
                                ),
                                new XElement("Destination",
                                    new XElement("Address",
                                        new XElement("Suburb", xmlResult.Suburb),
                                        new XElement("CountryCode",
                                            new XElement("CCode", xmlResult.Des_CountryCode)
                                        )
                                    )
                                ),
                                new XElement("Make", xmlResult.Make),
                                new XElement("Model", xmlResult.Model),
                                new XElement("Year", ""),
                                new XElement("SpecialRequirement", new XCData(xmlResult.SpecialRequirement)),
                                new XElement("EstimatedVolume", (Math.Round(xmlResult.Volume_FCL ?? 0, 2, MidpointRounding.ToEven)).ToString()),
                                new XElement("VehicleType", xmlResult.VehicleType),
                                new XElement("NoVehicles", "1"),
                                new XElement("DuplicateEnquiry", xmlResult.Duplicate)
                            )
                        )
                    )
                )
            );

            string xml = xdoc.ToString();
            string token = await Navision.GetValidAccessToken();
            string status = await Navision.SubmitXmlAsync(token, xml, "VehicleEnquiry");

            string extension = Path.GetExtension("Vehicle_Enquiry.xml");
            string fileName = Guid.NewGuid() + "_" + Path.GetFileName("Vehicle_Enquiry.xml");
            string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploads/VehicleEnquiry_XML/" + fileName);

            saveXMLStatus(physicalPath, status, "VehicleEnquiry");

            xdoc.Save(physicalPath);

            var XMLEmail = System.Configuration.ConfigurationManager.AppSettings["XMLEmail"];
            if (!string.IsNullOrEmpty(XMLEmail))
            {
                var subject = "Vehicle Enquiry (" + file + ")";
                SendXMLEmail(XMLEmail, "Vehicle", subject, physicalPath);
            }
            return physicalPath;
        }

        public static void XmlAPICall(string path, int id)
        {
            try
            {
                //int k = 0;
                //int i = 1 / k;

                string xmlString = System.IO.File.ReadAllText(path);
                string email = "quotes@anglopacific.co.uk";
                Guid sessionid = new Guid();

                mwweb_ttRow ttrow = new mwweb_ttRow();
                ttrow.tid = 0;
                ttrow.t12 = sessionid.ToString();
                ttrow.t13 = xmlString;
                ttrow.t14 = "MoveQuote";
                ttrow.t15 = "Moveware";
                ttrow.t16 = email;

                mwweb_ttRow[] array = new mwweb_ttRow[1];
                array[0] = ttrow;

                mwservicesObjClient client = new mwservicesObjClient();
                string response = client.mwweb(
                    "",
                    "",
                    "setRemovalsAdd",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "0",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    ref array,
                    out string status
                );

                logger.Info("respone: " + response + ". status" + status);

                if (id != 0)
                {
                    tbl_FailedXML model = _dbRepositoryFailedXML.SelectById(id);
                    model.Status = "success";
                    string msg = _dbRepositoryFailedXML.Update(model);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);

                //logger.Error(ex.InnerException == null?"":ex);
                if (id == 0)
                {
                    tbl_FailedXML model = new tbl_FailedXML();
                    model.Path = path;
                    if (path.Contains("VehicleBook_XML"))
                    {
                        model.Type = "VehicleBook_XML";
                    }
                    else if (path.Contains("GDPQuote_XML"))
                    {
                        model.Type = "GDPQuote_XML";
                    }
                    else if (path.Contains("FCLQuote_XML"))
                    {
                        model.Type = "FCLQuote_XML";
                    }
                    else if (path.Contains("Baggage_XML"))
                    {
                        model.Type = "Baggage_XML";
                    }
                    else if (path.Contains("Removal_XML"))
                    {
                        model.Type = "Removal_XML";
                    }
                    else { }
                    model.Status = "failed";
                    string status = _dbRepositoryFailedXML.Insert(model);
                }
            }
        }

        public static mwwebResponse ShipmentXmlAPICall(string RefNumber, string Consig)
        {
            try
            {
                mwweb_ttRow ttrow = new mwweb_ttRow();
                ttrow.tid = 0;

                mwweb_ttRow[] tt = new mwweb_ttRow[1];
                tt[0] = ttrow;

                mwservicesObjClient obj = new mwservicesObjClient();

                QuoteCalculator.XmlService.mwwebRequest inValue = new QuoteCalculator.XmlService.mwwebRequest();
                inValue.pUser = Consig;
                inValue.pPass = RefNumber;
                inValue.pCall = "getarp";
                inValue.pRP = RefNumber;
                inValue.pWaybill = "";
                inValue.pIQID = "";
                inValue.pCustCode = "";
                inValue.pCredCode = "";
                inValue.pAgentID = "";
                inValue.pPage = "";
                inValue.pResults = "";
                inValue.pParam1 = "Waybill";
                inValue.pParam2 = "";
                inValue.pParam3 = "";
                inValue.pParam4 = "";
                inValue.pParam5 = "";
                inValue.pParam6 = "";
                inValue.pParam7 = "";
                inValue.pParam8 = "";
                inValue.pParam9 = "";
                inValue.pParam10 = "";
                inValue.tt = tt;
                QuoteCalculator.XmlService.mwwebResponse retVal = ((QuoteCalculator.XmlService.mwservicesObj)(obj)).mwweb(inValue);
                return retVal;
            }
            catch (Exception e)
            {
                logger.Error(e);
                mwwebResponse result = new mwwebResponse();
                return result;
            }
        }

        public static string SendEmail(sp_GetdataForEmailSending_Result model, int ServiceId, string XMLPath, string ReferenceId)
        {
            int companyId = SessionHelper.COMPANY_ID;
            string message = string.Empty;
            string to = string.Empty;
            var BookNowXMLEmail = System.Configuration.ConfigurationManager.AppSettings["BookNowXMLEmail"];
            tbl_EmailTemplate emailModel = new tbl_EmailTemplate();
            try
            {
                if (ServiceId > 0)
                {
                    emailModel = _dbRepositoryEmailTemplete.GetEntities().Where(m => m.ServiceId == ServiceId).FirstOrDefault();
                    if (ServiceId == 1)
                    {
                        var toEmail = _dbRepositoryEmailSettings.GetEntities().Where(m => m.Key == "LON").Select(m => m.Value).FirstOrDefault();
                        if (toEmail != null) { to = toEmail; }
                    }
                    else if (ServiceId == 3)
                    {
                        var toEmail = _dbRepositoryEmailSettings.GetEntities().Where(m => m.Key == "EmailBaggage_" + companyId).Select(m => m.Value).FirstOrDefault();
                        if (toEmail != null) { to = toEmail; }
                    }
                    else if (ServiceId == 5)
                    {
                        var toEmail = _dbRepositoryEmailSettings.GetEntities().Where(m => m.Key == "EmailRemovals").Select(m => m.Value).FirstOrDefault();
                        if (toEmail != null) { to = toEmail; }
                    }
                    else if (ServiceId == 1012)
                    {
                        var toEmail = _dbRepositoryEmailSettings.GetEntities().Where(m => m.Key == "EmailBaggage_" + companyId).Select(m => m.Value).FirstOrDefault();
                        if (toEmail != null) { to = toEmail; }
                    }
                    else if (ServiceId == 1024)
                    {
                        var toEmail = _dbRepositoryEmailSettings.GetEntities().Where(m => m.Key == "EmailBaggage_" + companyId).Select(m => m.Value).FirstOrDefault();
                        if (toEmail != null) { to = toEmail; }
                    }
                    else
                    {
                        to = model.Email;
                        BookNowXMLEmail = "";
                    }

                    //    to = model.Email;
                    //if (str.Contains("User"))
                    //{
                    //    emailModel = _dbRepositoryEmailTemplete.GetEntities().Where(m => m.ServiceId == 2).FirstOrDefault();
                    //    to = model.Email;
                    //}
                    //else
                    //{
                    //    emailModel = _dbRepositoryEmailTemplete.GetEntities().Where(m => m.ServiceId == 1).FirstOrDefault();
                    //    to = model.Email;
                    //    //to = _dbRepositoryEmailSettings.GetEntities().Where(m => m.Key.Contains(model.Branch)).FirstOrDefault().Value;
                    //}
                }

                string bodyTemplate = string.Empty;

                bodyTemplate = emailModel.HtmlContent;
                bodyTemplate = bodyTemplate.Replace("#refID#", model.ReferenceNo);
                bodyTemplate = bodyTemplate.Replace("#CustName#", model.Firstname + " " + model.Lastname);
                bodyTemplate = bodyTemplate.Replace("#fromCountry#", model.FromCountry);
                if (string.IsNullOrWhiteSpace(model.FromCity))
                    bodyTemplate = bodyTemplate.Replace("#fromCity#, ", model.FromCity);
                else
                    bodyTemplate = bodyTemplate.Replace("#fromCity#", model.FromCity);
                bodyTemplate = bodyTemplate.Replace("#toCountry#", model.ToCountry);
                bodyTemplate = bodyTemplate.Replace("#toCity#", model.ToCity);
                bodyTemplate = bodyTemplate.Replace("#quoteName#", model.quoteName);
                bodyTemplate = bodyTemplate.Replace("#salesRepName#", model.SalesRepName);
                bodyTemplate = bodyTemplate.Replace("#salesRepEmail#", model.SalesRepEmail);

                //bodyTemplate = bodyTemplate.Replace("#logo#", System.Web.Hosting.HostingEnvironment.MapPath("~/Content/images/logo.png"));
                //bodyTemplate = bodyTemplate.Replace("#Port_Code#", model.Port_Code);
                //bodyTemplate = bodyTemplate.Replace("#PostCode#", model.PostCode);
                //bodyTemplate = bodyTemplate.Replace("#OriginCountry#", model.FromCountryName);
                //bodyTemplate = bodyTemplate.Replace("#City#", model.Suburb);
                //bodyTemplate = bodyTemplate.Replace("#Des_Country#", model.Des_Country);
                //bodyTemplate = bodyTemplate.Replace("#DestinationCountry#", model.Des_CountryCode);
                //bodyTemplate = bodyTemplate.Replace("#Title#", model.TitleName);
                //bodyTemplate = bodyTemplate.Replace("#Firstname#", model.Firstname);
                //bodyTemplate = bodyTemplate.Replace("#Surname#", model.Lastname);
                //bodyTemplate = bodyTemplate.Replace("#BranchName#", model.BranchName);
                //bodyTemplate = bodyTemplate.Replace("#company#", "");
                //bodyTemplate = bodyTemplate.Replace("#Email#", model.Email);
                //bodyTemplate = bodyTemplate.Replace("#Telephone#", model.Telephone);
                //bodyTemplate = bodyTemplate.Replace("#Movedate#", model.EstimatedMoveDate.ToString());
                //bodyTemplate = bodyTemplate.Replace("#Make#", model.Make);
                //bodyTemplate = bodyTemplate.Replace("#Model#", model.Model);
                //bodyTemplate = bodyTemplate.Replace("#Branch#", model.Branch);
                //bodyTemplate = bodyTemplate.Replace("#Specialrequirements#", model.SpecialRequirement);

                //bodyTemplate = bodyTemplate.Replace("#FCLPort#", model.PostCode);
                //if (model.IsFCL == true || model.IsFCL == null)
                //{
                //    bodyTemplate = bodyTemplate.Replace("#FCLprice#", Math.Round((decimal)model.FCL, 3).ToString());
                //}
                //else
                //{
                //    bodyTemplate = bodyTemplate.Replace("custom1", "custom");
                //}

                //bodyTemplate = bodyTemplate.Replace("#GRPPort#", model.PostCode);
                //if (model.IsGRP == true || model.IsFCL == null)
                //{
                //    bodyTemplate = bodyTemplate.Replace("#Groupageprice#", Math.Round((decimal)model.GPG, 3).ToString());
                //}
                //else
                //{
                //    bodyTemplate = bodyTemplate.Replace("custom2", "custom");
                //}

                //Attach Files
                string fromEmailKey = "";
                string displayKey = "";
                if (ServiceId == 2)
                {
                    fromEmailKey = "LON";
                    displayKey = "DisplayVehicle";
                }
                else if (ServiceId == 4)
                {
                    fromEmailKey = "EmailBaggage_" + companyId;
                    displayKey = "DisplayBaggage";
                }
                else if (ServiceId == 6)
                {
                    fromEmailKey = "EmailRemovals";
                    displayKey = "DisplayRemoval";
                }
                else if (ServiceId == 1011)
                {
                    fromEmailKey = "EmailBaggage_" + companyId;
                    displayKey = "DisplayPickfords";
                }
                else if (ServiceId == 1023)
                {
                    fromEmailKey = "EmailBaggage_" + companyId;
                    displayKey = "DisplayExcessBaggage";
                }
                if(ReferenceId != "" && ReferenceId != null)
                {
                    emailModel.Subject = emailModel.Subject + " BOOK NOW - " + ReferenceId;
                }
                

                bool status = EmailHelper.SendMail(companyId, to, emailModel.Subject, bodyTemplate, fromEmailKey, displayKey, true, BookNowXMLEmail, "", XMLPath.ToString());

                return message;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                message = CommonHelper.GetErrorMessage(ex);
                return message;
                //TempData[Enums.NotifyType.Error.GetDescription()] = message;
            }
        }

        public static async Task<string> GenerateBaggageXml(SP_GetBaggageXmlData_Result xmlResult, List<tbl_BaggageItem> baggageItems, double QuotePrice, SP_GetCollectionDelivery_Result xmlColDelResult, string file, string methodName, List<BaggageCostModel> baggageCostModel = null)
        {
            XNamespace url = "http://www.moveware.com.au/MovewareConnect.xsd";

            string serviceName = (methodName == "Sea" ? "EXBGS" : ((methodName == "Courier" || methodName == "CourierExpress") ? "EXBGCOU" : (methodName == "Road" ? "EXBGRD" : (methodName == "AIRTOPORT" ? "EXBGAP" : "EXBGAD"))));
            string MethodName = (methodName == "Sea" ? "Sea" : (methodName == "Courier" ? "COURIER" : (methodName == "Road" ? "ROAD" : (methodName == "CourierExpress" ? "COURIER" : "AIR"))));
            //string IxType = (methodName == "AirFreightToAirport" ? "ToPort" : "ToDoor");
            decimal? dest = 0, freight = 0, origin = 0, additional = 0;
            if (baggageCostModel != null && baggageCostModel.Count > 0)
            {
                dest = baggageCostModel.FirstOrDefault(x => x.CostType == "Destination Charge")?.Cost;
                freight = baggageCostModel.FirstOrDefault(x => x.CostType == "Freight")?.Cost;
                origin = baggageCostModel.FirstOrDefault(x => x.CostType == "Origin")?.Cost;
                additional = baggageCostModel.FirstOrDefault(x => x.CostType == "Additional Charge")?.Cost;
            }
            List<XElement> inventoriesSubElemens = new List<XElement>();
            double TotalCuft = 0;
            foreach (var item in baggageItems)
            {
                inventoriesSubElemens.Add(
                    new XElement("Inventory",
                                    new XElement("Description", item.Description),
                                    new XElement("Width", item.Length),
                                    new XElement("Depth", item.Breadth),
                                    new XElement("Height", item.Height),
                                    new XElement("Cube", item.Volume),
                                    new XElement("Quantity", item.Quantity),
                                    new XElement("CubeTotal", (item.Volume * item.Quantity)),
                                    new XElement("Weight", item.Groweight),
                                    new XElement("MesGK", item.Groweight)
                                    ));
                TotalCuft += item.Volume * item.Quantity;
            }

            XDocument xdoc = new XDocument(
                //new XDeclaration("1.0", "UTF-8",null),
                new XElement(url + "MWConnect",
                    new XElement("Details",
                        new XElement("Comments", QuotePrice),
                        new XElement("DeliveryCharges", xmlColDelResult.DeliveryCharge),
                        new XElement("CollectionCharges", xmlColDelResult.CollectionCharge),
                        (dest != null && dest > 0) ? new XElement("DestinationCharges", dest) : null,
                        (freight != null && freight > 0) ? new XElement("SeaFreight", freight) : null,
                        //(origin != null && origin > 0) ? new XElement("AddCharge02", origin) : null,
                        //(additional != null && additional > 0) ? new XElement("AddCharge01", dest) : null,
                        new XElement("LiveTest", "TEST")
                    ),
                    new XElement("MessageType",
                        new XElement("WebQuote",
                            new XElement("Removal",
                                new XElement("CompanyCode", xmlResult.CompanyCode),
                                new XElement("Branch", xmlResult.Branch),
                                new XElement("Type", "EXB"),
                                new XElement("QuoteStatus", xmlResult.QuoteStatus),
                                new XElement("Salesrep", xmlResult.SalesRep),
                                new XElement("File", file),
                                new XElement("Method", MethodName),
                                new XElement("Service", serviceName),
                                new XElement("IxType", xmlResult.IxType),
                                new XElement("Items", xmlResult.Items),
                                new XElement("Referral", xmlResult.Referral),
                                new XElement("EstMoveDate", xmlResult.EstimatedMoveDate),
                                new XElement("InternalNote", xmlResult.InternalNotes),
                                new XElement("DuplicateEnquiry", xmlResult.DuplicateEnquiry),
                                new XElement("Cuft", TotalCuft),
                                new XElement("Origin",
                                    new XElement("Name",
                                        new XElement("Title", xmlResult.TitleName),
                                        new XElement("FullName", xmlResult.Firstname + '/' + xmlResult.Lastname),
                                        new XElement("Email", xmlResult.Email),
                                        new XElement("Phone", xmlResult.Telephone),
                                        new XElement("Mobile", xmlResult.Mobile)
                                    ),
                                    new XElement("Address",
                                        //new XElement("Suburb", xmlResult.FromSuburb),
                                        new XElement("Postcode", xmlResult.PostCode),
                                        new XElement("CountryCode",
                                            new XElement("CCode", xmlResult.FromCountryCode)
                                        )
                                    )
                                ),
                                new XElement("Destination",
                                    new XElement("Address",
                                        new XElement("Suburb", xmlResult.Suburb),
                                        new XElement("Postcode", xmlResult.ToPostCode),
                                        new XElement("CountryCode",
                                            //new XElement(xmlResult.ToCountryCode == "GB" ? "CountryCountry" : "CCode", xmlResult.ToCountryCode)
                                            new XElement("CCode", xmlResult.ToCountryCode)
                                        )
                                    )
                                ),
                                new XElement("AdditionalCosts",
                                    (additional != null && additional > 0) ? new XElement("AddCharge01", additional) : null,
                                    (origin != null && origin > 0) ? new XElement("AddCharge02", origin) : null
                                ),
                                inventoriesSubElemens.ToArray()
                            )
                        )
                    )
                )
            );

            string xml = xdoc.ToString();
            string token = await Navision.GetValidAccessToken();
            string quoteType = xmlResult.QuoteStatus == "QUOTE" ? "BaggageQuote" : "BaggageBookNow";
            string status;
            if (quoteType != "BaggageBookNow")
            {
                status = await Navision.SubmitXmlAsync(token, xml, quoteType);
            }
            else
            {
                status = "CREATED";
            }

            string extension = Path.GetExtension("Baggage_Quote.xml");
            string fileName = Guid.NewGuid() + "_" + Path.GetFileName("Baggage_Quote.xml");
            string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploads/Baggage_XML/" + fileName);

            saveXMLStatus(physicalPath, status, quoteType);

            xdoc.Save(physicalPath);

            var XMLEmail = System.Configuration.ConfigurationManager.AppSettings["XMLEmail"];
            if (!string.IsNullOrEmpty(XMLEmail))
            {
                var subject = "Baggage "+ xmlResult.QuoteStatus + " (" + file + ")";
                SendXMLEmail(XMLEmail, "Baggage", subject, physicalPath);
            }
            return physicalPath;
        }

        public static async Task<string> GenerateBaggageEnquiryXml(SP_GetBaggageXmlData_Result xmlResult, List<tbl_BaggageItem> baggageItems, string file)
        {
            XNamespace url = "http://www.moveware.com.au/MovewareConnect.xsd";

            //string serviceName = (methodName == "Sea" ? "GRB" : (methodName == "Courier" ? "EXP" : "AIR"));
            //string IxType = (methodName == "AirFreightToAirport" ? "ToDoor" : "ToPort");

            List<XElement> inventoriesSubElemens = new List<XElement>();

            foreach (var item in baggageItems)
            {
                inventoriesSubElemens.Add(
                    new XElement("Inventory",
                                    new XElement("Description", item.Description),
                                    new XElement("Width", item.Length),
                                    new XElement("Depth", item.Breadth),
                                    new XElement("Height", item.Height),
                                    new XElement("Cube", item.Volume),
                                    new XElement("Quantity", item.Quantity),
                                    new XElement("CubeTotal", (item.Volume * item.Quantity)),
                                    new XElement("Weight", item.Groweight),
                                    new XElement("MesGK", item.Groweight)
                                    ));
            }

            XDocument xdoc = new XDocument(
                //new XDeclaration("1.0", "UTF-8",null),
                new XElement(url + "MWConnect",
                    new XElement("Details",
                        //new XElement("Comments", QuotePrice),
                        //new XElement("DeliveryCharge", xmlColDelResult.DeliveryCharge),
                        //new XElement("CollectionCharge", xmlColDelResult.CollectionCharge),
                        new XElement("LiveTest", "TEST") //Static for Testing
                    ),
                    new XElement("MessageType",
                        new XElement("WebQuote",
                            new XElement("Removal",
                                new XElement("CompanyCode", xmlResult.CompanyCode),
                                new XElement("Branch", xmlResult.Branch),
                                new XElement("Type", "EXB"),
                                new XElement("QuoteStatus", xmlResult.QuoteStatus),
                                new XElement("Salesrep", xmlResult.SalesRep),
                                new XElement("File", file),
                                //new XElement("Method", methodName),
                                //new XElement("Service", serviceName), //Static value
                                //new XElement("ixType", IxType),
                                new XElement("Items", xmlResult.Items),
                                new XElement("Referral", xmlResult.Referral),
                                new XElement("EstMoveDate", xmlResult.EstimatedMoveDate),
                                new XElement("InternalNote", ""),
                                new XElement("DuplicateEnquiry", xmlResult.DuplicateEnquiry),
                                new XElement("Origin",
                                    new XElement("Name",
                                        new XElement("Title", xmlResult.TitleName),
                                        new XElement("FullName", xmlResult.Firstname + '/' + xmlResult.Lastname),
                                        new XElement("Email", xmlResult.Email),
                                        new XElement("Phone", xmlResult.Telephone),
                                        new XElement("Mobile", xmlResult.Mobile)
                                    ),
                                    new XElement("Address",
                                        new XElement("Postcode", xmlResult.PostCode),
                                        new XElement("CountryCode",
                                            new XElement("CCode", xmlResult.FromCountryCode)
                                        )
                                    )
                                ),
                                new XElement("Destination",
                                    new XElement("Address",
                                        new XElement("Suburb", xmlResult.Suburb),
                                        new XElement("CountryCode",
                                            new XElement("CCode", xmlResult.ToCountryCode)
                                        )
                                    )
                                ),
                                inventoriesSubElemens.ToArray()
                            )

                        )
                    )
                )
            );

            string xml = xdoc.ToString();
            string token = await Navision.GetValidAccessToken();
            string status = await Navision.SubmitXmlAsync(token, xml, "BaggageEnquiry");

            string extension = Path.GetExtension("Baggage_Quote.xml");
            string fileName = Guid.NewGuid() + "_" + Path.GetFileName("Baggage_Quote.xml");
            string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploads/Baggage_XML/" + fileName);

            saveXMLStatus(physicalPath, status, "BaggageEnquiry");

            xdoc.Save(physicalPath);
            
            var XMLEmail = System.Configuration.ConfigurationManager.AppSettings["XMLEmail"];
            if (!string.IsNullOrEmpty(XMLEmail))
            {
                var subject = "Baggage " + xmlResult.QuoteStatus + " (" + file + ")";
                SendXMLEmail(XMLEmail, "Baggage", subject, physicalPath);
            }
            return physicalPath;
        }

        //public static string GenerateVehicleBookNowXML(sp_GetdataForEmailSending_Result xmlResult, string file)
        //{
        //    string QuoteStatus = string.Empty;
        //    string ServiceNo = string.Empty;
        //    string File = string.Empty;
        //    XNamespace url = "http://www.moveware.com.au/MovewareConnect.xsd";
        //    XDocument xdoc = new XDocument(
        //        //new XDeclaration("1.0", "UTF-8",null),
        //        new XElement(url + "MWConnect",
        //            new XElement("Details",
        //                new XElement("Comments", "125.00"), //Static value
        //                new XElement("CollectionCharge", "12.00"), //Static value
        //                new XElement("LiveTest", "TEST") //Static for Testing
        //            ),
        //            new XElement("MessageType",
        //                new XElement("WebQuote",
        //                    new XElement("Removal",
        //                        new XElement("Branch", xmlResult.Branch),
        //                        new XElement("Type", "EXV"), 
        //                        new XElement("QuoteStatus", "Book Now"),
        //                        new XElement("Salesrep", "WEB"),
        //                        new XElement("File", file),
        //                        new XElement("Method", "SEA"),
        //                        new XElement("Service", xmlResult.Service),
        //                        new XElement("IxType", "TOPORT"),
        //                        new XElement("Referral", ""),
        //                        new XElement("EstMoveDate", xmlResult.EstimatedMoveDate),
        //                        new XElement("DuplicateEnquiry", xmlResult.DuplicateEnquiry),
        //                         new XElement("Origin",
        //                            new XElement("Name",
        //                                new XElement("Name",
        //                                    new XElement("Title", xmlResult.TitleName),
        //                                    new XElement("First", xmlResult.Firstname),
        //                                    new XElement("Last", xmlResult.Lastname)
        //                                ),
        //                                new XElement("Email", xmlResult.Email),
        //                                new XElement("Phone", xmlResult.Telephone)
        //                            ),
        //                            new XElement("Address",
        //                                new XElement("Postcode", xmlResult.PostCode),
        //                                new XElement("CountryCode",
        //                                    new XElement("CountryCode", xmlResult.OriginCountryCode)
        //                                )
        //                            )
        //                        ),
        //                        new XElement("Destination",
        //                            new XElement("Address",
        //                                new XElement("Suburb", xmlResult.Suburb),
        //                                new XElement("CountryCode",
        //                                    new XElement("CountryCode", xmlResult.DestCountryCode)
        //                                )
        //                            )
        //                        )
        //                    )
        //                )
        //            )
        //        )
        //    );

        //    string extension = Path.GetExtension("FCL_Quote.xml");
        //    string fileName = Guid.NewGuid() + "_" + Path.GetFileName("Quote_XML.xml");
        //    string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploads/VehicleBook_XML/" + fileName);
        //    xdoc.Save(physicalPath);
        //    return physicalPath;
        //}

        public static async Task<string> GenerateRemovalXml(SP_GetRemovalXmlData_Result xmlResult, string QuoteStatus, string file)
        {
            XNamespace url = "http://www.moveware.com.au/MovewareConnect.xsd";
            List<XElement> inventoriesSubElemens = new List<XElement>();

            XDocument xdoc = new XDocument(
                //new XDeclaration("1.0", "UTF-8",null),
                new XElement(url + "MWConnect",
                    new XElement("Details",
                        new XElement("Comments", xmlResult.Comments),
                        new XElement("Vehicle", xmlResult.Vehicle),
                        new XElement("Labour", xmlResult.Labour),
                        new XElement("ExcessMiles", xmlResult.ExcessMiles),
                        new XElement("PackingMaterials", xmlResult.PackingMaterials),
                        new XElement("SeaFreight", xmlResult.SeaFreight),
                        new XElement("DestinationCharges", xmlResult.DestinationCharges),
                        new XElement("OriginMarkup", xmlResult.OriginMarkup),
                        //new XElement("AddCharge01", xmlResult.ReceivingHandling),
                        new XElement("LiveTest", "TEST") //Static for Testing
                    ),
                    new XElement("MessageType",
                        new XElement("WebQuote",
                            new XElement("Removal",
                                new XElement("CompanyCode", "AP"),
                                new XElement("Branch", xmlResult.Branch),
                                new XElement("Type", "EXR"),
                                new XElement("QuoteStatus", QuoteStatus),
                                new XElement("Salesrep", "WEB"),
                                new XElement("File", file),
                                new XElement("Method", "SEA"),
                                new XElement("Service", "EXBGCOU"),
                                new XElement("IxType", "DTDE"),
                                new XElement("Referral", xmlResult.Referral),
                                new XElement("EstMoveDate", xmlResult.EstimatedMoveDate),
                                new XElement("EstVolume", xmlResult.Volume),
                                new XElement("RemovalType", "Quick Estimate"),
                                new XElement("QuickEstimateType", xmlResult.QuickEstimateType),
                                new XElement("SpecialRequirement", new XCData(xmlResult.SpecialRequirements)),
                                new XElement("DuplicateEnquiry", xmlResult.DuplicateEnquiry),
                                new XElement("Origin",
                                    new XElement("Name",
                                        new XElement("Title", xmlResult.TitleName),
                                        new XElement("FullName", xmlResult.Firstname + '/' + xmlResult.Lastname),
                                        new XElement("Email", xmlResult.Email),
                                        new XElement("Phone", xmlResult.Telephone),
                                        new XElement("Mobile", xmlResult.Mobile)
                                    ),
                                    new XElement("Address",
                                        new XElement("Postcode", xmlResult.PostCode),
                                        new XElement("CountryCode",
                                            new XElement("CCode", xmlResult.FromCountryCode)
                                        )
                                    )
                                ),
                                new XElement("Destination",
                                    new XElement("Address",
                                        new XElement("Suburb", xmlResult.Suburb),
                                        new XElement("CountryCode",
                                            new XElement("CCode", xmlResult.ToCountryCode)
                                        )
                                    )
                                ),
                                new XElement("AdditionalCosts",
                                    new XElement("AddCharge01", xmlResult.ReceivingHandling)
                                ),
                                inventoriesSubElemens.ToArray()
                            )
                        )
                    )
                )
            );

            string xml = xdoc.ToString();
            string token = await Navision.GetValidAccessToken();
            string quoteType = QuoteStatus == "Quote" ? "RemovalQuote" : "RemovalBookNow";
            string status;
            if (quoteType != "RemovalBookNow")
            {
                status = await Navision.SubmitXmlAsync(token, xml, quoteType);
            }
            else
            {
                status = "CREATED";
            }
            

            string extension = Path.GetExtension("Removal_Quote.xml");
            string fileName = Guid.NewGuid() + "_" + Path.GetFileName("Removal_Quote.xml");
            string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploads/Removal_Xml/" + fileName);

            saveXMLStatus(physicalPath, status, quoteType);

            xdoc.Save(physicalPath);

            var XMLEmail = System.Configuration.ConfigurationManager.AppSettings["XMLEmail"];
            if (!string.IsNullOrEmpty(XMLEmail))
            {
                var subject = "Removal " + QuoteStatus + " (" + file + ")";
                SendXMLEmail(XMLEmail, "Removal", subject, physicalPath);
            }
            return physicalPath;
        }

        public static async Task<string> GenerateRemovalEnquiryXml(SP_GetRemovalXmlData_Result xmlResult, string QuoteStatus, string file)
        {
            XNamespace url = "http://www.moveware.com.au/MovewareConnect.xsd";
            List<XElement> inventoriesSubElemens = new List<XElement>();

            XDocument xdoc = new XDocument(
                //new XDeclaration("1.0", "UTF-8",null),
                new XElement(url + "MWConnect",
                    new XElement("Details",
                        new XElement("Comments", xmlResult.Comments),
                        new XElement("Vehicle", xmlResult.Vehicle),
                        new XElement("Labour", xmlResult.Labour),
                        new XElement("ExcessMiles", xmlResult.ExcessMiles),
                        new XElement("PackingMaterials", xmlResult.PackingMaterials),
                        new XElement("SeaFreight", xmlResult.SeaFreight),
                        new XElement("DestinationCharges", xmlResult.DestinationCharges),
                        new XElement("OriginMarkup", xmlResult.OriginMarkup),
                        //new XElement("AddCharge01", xmlResult.ReceivingHandling),
                        new XElement("LiveTest", "TEST") //Static for Testing
                    ),
                    new XElement("MessageType",
                        new XElement("WebQuote",
                            new XElement("Removal",
                                new XElement("CompanyCode", "AP"),
                                new XElement("Branch", xmlResult.Branch),
                                new XElement("Type", "EXR"),
                                new XElement("QuoteStatus", QuoteStatus),
                                new XElement("Salesrep", "WEB"),
                                new XElement("File", file),
                                new XElement("Method", "SEA"),
                                //new XElement("Service", ""),
                                //new XElement("IxType", "TODOOR"),
                                new XElement("Referral", xmlResult.Referral),
                                new XElement("EstMoveDate", xmlResult.EstimatedMoveDate),
                                new XElement("EstVolume", xmlResult.Volume),
                                new XElement("RemovalType", "Quick Estimate"),
                                new XElement("FullHousehold", ""),
                                new XElement("SpecialRequirement", new XCData(xmlResult.SpecialRequirements)),
                                new XElement("DuplicateEnquiry", xmlResult.DuplicateEnquiry),
                                new XElement("Origin",
                                    new XElement("Name",
                                        new XElement("Title", xmlResult.TitleName),
                                        new XElement("FullName", xmlResult.Firstname + '/' + xmlResult.Lastname),
                                        new XElement("Email", xmlResult.Email),
                                        new XElement("Phone", xmlResult.Telephone),
                                        new XElement("Mobile", xmlResult.Mobile)
                                    ),
                                    new XElement("Address",
                                        new XElement("Postcode", xmlResult.PostCode),
                                        new XElement("CountryCode",
                                            new XElement("CCode", xmlResult.FromCountryCode)
                                        )
                                    )
                                ),
                                new XElement("Destination",
                                    new XElement("Address",
                                        new XElement("Suburb", xmlResult.Suburb),
                                        new XElement("CountryCode",
                                            new XElement("CCode", xmlResult.ToCountryCode)
                                        )
                                    )
                                ),
                                new XElement("AdditionalCosts",
                                    new XElement("AddCharge01", xmlResult.ReceivingHandling)
                                ),
                                inventoriesSubElemens.ToArray()
                            )
                        )
                    )
                )
            );

            string xml = xdoc.ToString();
            string token = await Navision.GetValidAccessToken();
            string status = await Navision.SubmitXmlAsync(token, xml, "RemovalEnquiry");

            string extension = Path.GetExtension("Removal_Quote.xml");
            string fileName = Guid.NewGuid() + "_" + Path.GetFileName("Removal_Quote.xml");
            string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploads/Removal_Xml/" + fileName);

            saveXMLStatus(physicalPath, status, "RemovalEnquiry");

            xdoc.Save(physicalPath);
            
            var XMLEmail = System.Configuration.ConfigurationManager.AppSettings["XMLEmail"];
            if (!string.IsNullOrEmpty(XMLEmail))
            {
                var subject = "Removal " + QuoteStatus + " (" + file + ")";
                SendXMLEmail(XMLEmail, "Removal", subject, physicalPath);
            }
            return physicalPath;
        }

        public static async Task<string> GenerateHomeSurveyXml(SP_GetRemovalXmlData_Result xmlResult, string file)
        {
            XNamespace url = "http://www.moveware.com.au/MovewareConnect.xsd";
            List<XElement> inventoriesSubElemens = new List<XElement>();

            XDocument xdoc = new XDocument(
                //new XDeclaration("1.0", "UTF-8",null),
                new XElement(url + "MWConnect",
                    new XElement("Details",
                        new XElement("LiveTest", "TEST") //Static for Testing
                    ),
                    new XElement("MessageType",
                        new XElement("WebQuote",
                            new XElement("Removal",
                                new XElement("CompanyCode", "AP"),
                                new XElement("Branch", xmlResult.Branch),
                                new XElement("Type", "EXR"),
                                new XElement("QuoteStatus", "ENQUIRY"),
                                new XElement("Salesrep", "WEB"),
                                new XElement("File", file),
                                new XElement("Method", "SEA"),
                                new XElement("Service", "EXFCL"),
                                new XElement("IxType", "DTDE"),
                                new XElement("Referral", xmlResult.Referral),
                                new XElement("EstMoveDate", xmlResult.EstimatedMoveDate),
                                new XElement("EstVolume", xmlResult.Volume),
                                new XElement("RemovalType", "Free Home Survey"),
                                new XElement("SelectiveItems", ""),
                                //new XElement("SpecialRequirement", new XCData(xmlResult.HomeConsultationDateTime)),
                                new XElement("DuplicateEnquiry", xmlResult.DuplicateEnquiry),
                                new XElement("Origin",
                                    new XElement("Name",
                                        new XElement("Title", xmlResult.TitleName),
                                        new XElement("FullName", xmlResult.Firstname + '/' + xmlResult.Lastname),
                                        new XElement("Email", xmlResult.Email),
                                        new XElement("Phone", xmlResult.Telephone),
                                        new XElement("Mobile", xmlResult.Mobile)
                                    ),
                                    new XElement("Address",
                                        new XElement("Postcode", xmlResult.PostCode),
                                        new XElement("CountryCode",
                                            new XElement("CCode", xmlResult.FromCountryCode)
                                        )
                                    )
                                ),
                                new XElement("Destination",
                                    new XElement("Address",
                                        new XElement("Suburb", xmlResult.Suburb),
                                        new XElement("CountryCode",
                                            new XElement("CCode", xmlResult.ToCountryCode)
                                        )
                                    )
                                ),
                                new XElement("Dates",
                                    new XElement("Date", xmlResult.HomeConsultationDateTime),
                                    new XElement("Description", "Home Survey")
                                ),
                                inventoriesSubElemens.ToArray()
                            )
                        )
                    )
                )
            );

            string xml = xdoc.ToString();
            string token = await Navision.GetValidAccessToken();
            string status = await Navision.SubmitXmlAsync(token, xml, "RemovalHomeSurvey");

            string extension = Path.GetExtension("Removal_Quote.xml");
            string fileName = Guid.NewGuid() + "_" + Path.GetFileName("Removal_Quote.xml");
            string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploads/Removal_Xml/" + fileName);

            saveXMLStatus(physicalPath, status, "RemovalHomeSurvey");

            xdoc.Save(physicalPath);

            var XMLEmail = System.Configuration.ConfigurationManager.AppSettings["XMLEmail"];
            if (!string.IsNullOrEmpty(XMLEmail))
            {
                var subject = "Removal Home Survey ENQUIRY (" + file + ")";
                SendXMLEmail(XMLEmail, "RemovalHomeSurvey", subject, physicalPath);
            }
            return physicalPath;
        }

        public static async Task<string> GenerateHomeVideoSurveyXml(SP_GetRemovalXmlData_Result xmlResult, string file, DateTime? videoSurveyAppointmentTime)
        {
            string Date = videoSurveyAppointmentTime == null ? "" :
                videoSurveyAppointmentTime.Value.ToString("dd-MM-yy").Replace('-', '/'); ;

            string Time = videoSurveyAppointmentTime == null ? "" :
                videoSurveyAppointmentTime.Value.ToString("HH:mm");

            XNamespace url = "http://www.moveware.com.au/MovewareConnect.xsd";
            List<XElement> inventoriesSubElemens = new List<XElement>();

            XDocument xdoc = new XDocument(
                //new XDeclaration("1.0", "UTF-8",null),
                new XElement(url + "MWConnect",
                    new XElement("Details",
                        new XElement("LiveTest", "TEST") //Static for Testing
                    ),
                    new XElement("MessageType",
                        new XElement("WebQuote",
                            new XElement("Removal",
                                new XElement("CompanyCode", "AP"),
                                new XElement("Branch", xmlResult.Branch),
                                new XElement("Type", "EXR"),
                                new XElement("QuoteStatus", "ENQUIRY"),
                                new XElement("Salesrep", "WEB"),
                                new XElement("File", file),
                                new XElement("Method", "SEA"),
                                new XElement("Service", "EXP-GPGE"),
                                new XElement("IxType", "DTDE"),
                                new XElement("Referral", xmlResult.Referral),
                                new XElement("EstMoveDate", xmlResult.EstimatedMoveDate),
                                new XElement("EstVolume", xmlResult.Volume),
                                new XElement("RemovalType", "Video Survey"),
                                new XElement("DuplicateEnquiry", xmlResult.DuplicateEnquiry),
                                new XElement("Origin",
                                    new XElement("Name",
                                        new XElement("Title", xmlResult.TitleName),
                                        new XElement("FullName", xmlResult.Firstname + '/' + xmlResult.Lastname),
                                        new XElement("Email", xmlResult.Email),
                                        new XElement("Phone", xmlResult.Telephone),
                                        new XElement("Mobile", xmlResult.Mobile)
                                    ),
                                    new XElement("Address",
                                        new XElement("Postcode", xmlResult.PostCode),
                                        new XElement("CountryCode",
                                            new XElement("CCode", xmlResult.FromCountryCode)
                                        )
                                    )
                                ),
                                new XElement("Size", "Video-Buzz"),
                                new XElement("Destination",
                                    new XElement("Address",
                                        new XElement("Suburb", xmlResult.Suburb),
                                        new XElement("CountryCode",
                                            new XElement("CCode", xmlResult.ToCountryCode)
                                        )
                                    )
                                ),
                                new XElement("Dates",
                                    new XElement("Date", Date),
                                    new XElement("Time", xmlResult.DayScheduleName),
                                    new XElement("Type", "Video Survey"),
                                    new XElement("Description", "Video Survey")
                                ),
                                inventoriesSubElemens.ToArray()
                            )
                        )
                    )
                )
            );

            string xml = xdoc.ToString();
            string token = await Navision.GetValidAccessToken();
            string status = await Navision.SubmitXmlAsync(token, xml, "RemovalHomeVideoSurvey");

            string extension = Path.GetExtension("Removal_Quote.xml");
            string fileName = Guid.NewGuid() + "_" + Path.GetFileName("Removal_Quote.xml");
            string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploads/Removal_Xml/" + fileName);

            saveXMLStatus(physicalPath, status, "RemovalHomeVideoSurvey");

            xdoc.Save(physicalPath);

            var XMLEmail = System.Configuration.ConfigurationManager.AppSettings["XMLEmail"];
            if (!string.IsNullOrEmpty(XMLEmail))
            {
                var subject = "Removal Home Video Survey ENQUIRY (" + file + ")";
                SendXMLEmail(XMLEmail, "RemovalHomeVideoSurvey", subject, physicalPath);
            }
            return physicalPath;
        }

        public static string GenerateShipmentXml(string RefNumber, string Consig)
        {
            XNamespace s = "http://schemas.xmlsoap.org/soap/envelope/";
            XNamespace ns2 = "urn:openedgeservices:wsa1:mwservices";
            XNamespace ns3 = "urn:soap-fault:details";
            XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
            XDocument xdoc = new XDocument(
            new XElement(s + "Envelope",
                new XAttribute(XNamespace.Xmlns + "S", s.NamespaceName),
                new XElement(s + "Body",
                    new XElement(ns2 + "mwweb",
                            new XAttribute(XNamespace.Xmlns + "ns2", ns2.NamespaceName),
                            new XAttribute(XNamespace.Xmlns + "ns3", ns3.NamespaceName),
                            new XElement(ns2 + "pUser", Consig),
                            new XElement(ns2 + "pPass", RefNumber),
                            new XElement(ns2 + "pCall", "getrp"),
                            new XElement(ns2 + "pRP", RefNumber),
                            new XElement(ns2 + "pWaybill",
                                new XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName),
                                new XAttribute(xsi + "nil", true)
                            ),
                            new XElement(ns2 + "pIQID",
                                new XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName),
                                new XAttribute(xsi + "nil", true)
                            ),
                            new XElement(ns2 + "pCustCode",
                                new XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName),
                                new XAttribute(xsi + "nil", true)
                            ),
                            new XElement(ns2 + "pCredCode",
                                new XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName),
                                new XAttribute(xsi + "nil", true)
                            ),
                            new XElement(ns2 + "pAgentID",
                                new XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName),
                                new XAttribute(xsi + "nil", true)
                            ),
                            new XElement(ns2 + "pPage",
                                new XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName),
                                new XAttribute(xsi + "nil", true)
                            ),
                            new XElement(ns2 + "pResults",
                                new XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName),
                                new XAttribute(xsi + "nil", true)
                            ),
                            new XElement(ns2 + "pParam1", "Waybill"),
                            new XElement(ns2 + "pParam2",
                                new XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName),
                                new XAttribute(xsi + "nil", true)
                            ),
                            new XElement(ns2 + "pParam3",
                                new XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName),
                                new XAttribute(xsi + "nil", true)
                            ),
                            new XElement(ns2 + "pParam4",
                                new XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName),
                                new XAttribute(xsi + "nil", true)
                            ),
                            new XElement(ns2 + "pParam5",
                                new XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName),
                                new XAttribute(xsi + "nil", true)
                            ),
                            new XElement(ns2 + "pParam6",
                                new XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName),
                                new XAttribute(xsi + "nil", true)
                            ),
                            new XElement(ns2 + "pParam7",
                                new XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName),
                                new XAttribute(xsi + "nil", true)
                            ),
                            new XElement(ns2 + "pParam8",
                                new XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName),
                                new XAttribute(xsi + "nil", true)
                            ),
                            new XElement(ns2 + "pParam9",
                                new XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName),
                                new XAttribute(xsi + "nil", true)
                            ),
                            new XElement(ns2 + "pParam10",
                                new XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName),
                                new XAttribute(xsi + "nil", true)
                            ),
                            new XElement(ns2 + "tt")
                        )
                    )
                )
            );

            string extension = Path.GetExtension("ShipmentTracking.xml");
            string fileName = Guid.NewGuid() + "_" + Path.GetFileName("ShipmentTracking.xml");
            string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploads/Shipment_Xml/" + fileName);
            xdoc.Save(physicalPath);
            return physicalPath;
        }

        //public static void ResendFailedXML()
        //{
        //    try
        //    {
        //        List<tbl_FailedXML> list = _dbRepositoryFailedXML.GetEntities().Where(m => m.Status.Contains("failed")).ToList();
        //        foreach (var obj in list)
        //        {
        //            XMLHelper.XmlAPICall(obj.Path, obj.Id);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        var error = ex.Message;
        //    }
        //}
        public static async Task<string> ResendFailedXML()
        {
            try
            {
                List<tbl_FailedXML> list = _dbRepositoryFailedXML.GetEntities().Where(m => m.Status.Contains("failed")).ToList();
                foreach (var obj in list)
                {
                    string xml = GetXmlString(obj.Path);
                    string token = await Navision.GetValidAccessToken();
                    string status = await Navision.SubmitXmlAsync(token, xml, obj.QuoteType);
                    if (status != null)
                    {
                        tbl_FailedXML model = _dbRepositoryFailedXML.SelectById(obj.Id);
                        model.Status = status.ToUpper() == "CREATED" ? "success" : "failed";
                        model.NavisionStatus = status;
                        string msg = _dbRepositoryFailedXML.Update(model);
                    }
                }
            }
            catch (Exception ex)
            {
                var error = ex.Message;
            }
            return null;
        }       

        public static void saveXMLStatus(string physicalPath, string status, string quoteType)
        {
            try
            {
                tbl_FailedXML model = new tbl_FailedXML();
                model.Path = physicalPath;
                model.QuoteType = quoteType;
                if (physicalPath.Contains("VehicleBook_XML"))
                {
                    model.Type = "VehicleBook_XML";
                }
                else if (physicalPath.Contains("GDPQuote_XML"))
                {
                    model.Type = "GDPQuote_XML";
                }
                else if (physicalPath.Contains("FCLQuote_XML"))
                {
                    model.Type = "FCLQuote_XML";
                }
                else if (physicalPath.Contains("Baggage_XML"))
                {
                    model.Type = "Baggage_XML";
                }
                else if (physicalPath.Contains("Removal_Xml"))
                {
                    model.Type = "Removal_XML";
                }
                else { }
                model.Status = status.ToUpper() == "CREATED" ? "success" : "failed";
                model.NavisionStatus = status;
                string xmlStatusId = _dbRepositoryFailedXML.Insert(model);
            }
            catch (Exception ex)
            {
                var error = ex.Message;
            }
        }

        public static string GetXmlString(string strFile)
        {            
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(strFile);
            }
            catch (XmlException e)
            {
                var err = e.Message;
            }            
            StringWriter sw = new StringWriter();
            XmlTextWriter xw = new XmlTextWriter(sw);
            xmlDoc.WriteTo(xw);
            return sw.ToString();
        }

        public static string SendXMLEmail(string xmlEmailAddress, string serviceFor, string subject,  string XMLPath)
        {
            int companyId = SessionHelper.COMPANY_ID;
            string message = string.Empty;
            string to = xmlEmailAddress;
            tbl_EmailTemplate emailModel = new tbl_EmailTemplate();

            emailModel = _dbRepositoryEmailTemplete.GetEntities().Where(m => m.ServiceId == 1027).FirstOrDefault();
            try
            {
                string bodyTemplate = string.Empty;

                bodyTemplate = emailModel.HtmlContent;
                string fromEmailKey = "";
                string displayKey = "";
                if (serviceFor == "Vehicle")
                {
                    fromEmailKey = "LON";
                    displayKey = "DisplayVehicle";
                }
                else if (serviceFor == "Baggage")
                {
                    fromEmailKey = "EmailBaggage_" + companyId;
                    displayKey = "DisplayBaggage";
                }
                else if (serviceFor == "RemovalHomeSurvey")
                {
                    fromEmailKey = "EmailRemovals";
                    displayKey = "DisplayRemoval";
                }
                else if (serviceFor == "RemovalHomeVideoSurvey")
                {
                    fromEmailKey = "EmailRemovals";
                    displayKey = "DisplayRemoval";
                }

                emailModel.Subject = subject;

                bool status = EmailHelper.SendMail(companyId, to, emailModel.Subject, bodyTemplate, fromEmailKey, displayKey, true, "", "", XMLPath.ToString());

                return message;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                message = CommonHelper.GetErrorMessage(ex);
                return message;
                //TempData[Enums.NotifyType.Error.GetDescription()] = message;
            }
        }

        #endregion
    }
}