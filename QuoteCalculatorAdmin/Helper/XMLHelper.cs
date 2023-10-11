using QuoteCalculatorAdmin.Common;
using QuoteCalculatorAdmin.Data;
using QuoteCalculatorAdmin.Data.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using NLog;
using QuoteCalculatorAdmin.XmlService;
using System.Threading.Tasks;
using QuoteCalculator.Service.Models;

using AddtionalCostModel = QuoteCalculator.Service.Models.AddtionalCostModel;

namespace QuoteCalculatorAdmin.Helper
{
    public class XMLHelper
    {
        private static NLog.Logger logger = LogManager.GetCurrentClassLogger();
        #region private variables
        private static GenericRepository<tbl_EmailTemplate> _dbRepositoryEmailTemplete = new GenericRepository<tbl_EmailTemplate>();
        public static GenericRepository<tbl_EmailSettings> _dbRepositoryEmailSettings = new GenericRepository<tbl_EmailSettings>();
        private static GenericRepository<tbl_FailedXML> _dbRepositoryFailedXML = new GenericRepository<tbl_FailedXML>();
        #endregion

        #region Methods       
        public static string SendEmail(int companyId, sp_GetdataForEmailSending_Result model, int ServiceId, string XMLPath, string ReferenceId)
        {
            string message = string.Empty;
            string to = string.Empty;
            var BookNowXMLEmail = "";
            tbl_EmailTemplate emailModel = new tbl_EmailTemplate();
            try
            {
                if (ServiceId > 0)
                {
                    emailModel = _dbRepositoryEmailTemplete.GetEntities().Where(m => m.ServiceId == ServiceId).FirstOrDefault();
                    if (ServiceId == 1 || ServiceId == 3 || ServiceId == 5 || ServiceId == 1012 || ServiceId == 1024)
                    {
                        var toEmail = _dbRepositoryEmailSettings.GetEntities().Where(m => m.Key == "EmailBaggage_" + companyId).Select(m => m.Value).FirstOrDefault();
                        if (toEmail != null) { to = toEmail; }
                        BookNowXMLEmail = System.Configuration.ConfigurationManager.AppSettings["BookNowXMLEmail"];
                    }
                    else
                        to = model.Email;
                   
                }

                string bodyTemplate = string.Empty;
                if (emailModel != null)
                {
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
                }
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


                if (ReferenceId != "" && ReferenceId != null && emailModel != null)
                {
                    emailModel.Subject = emailModel.Subject + " BOOK NOW - " + ReferenceId;
                }
                if(emailModel != null)
                {
                    bool status = EmailHelper.SendMail(companyId, to, emailModel.Subject, bodyTemplate, fromEmailKey, displayKey, true, BookNowXMLEmail, "", XMLPath.ToString());
                }


                return message;
            }
            catch (Exception ex)
            {
                message = CommonHelper.GetErrorMessage(ex);
                return message;
            }
        }

        public static async Task<string> GenerateBaggageXml(BaggageXmlData xmlResult, List<BaggageItemModel> baggageItems, double QuotePrice, CollectionDelivery xmlColDelResult, string file, string methodName, List<BaggageCostModel> baggageCostModel = null, int? companyId= null)
        {
            XNamespace url = "http://www.moveware.com.au/MovewareConnect.xsd";

            string serviceName = (methodName == "Sea" ? "EXBGS" : ((methodName == "Courier" || methodName == "CourierExpress") ? "EXBGCOU" : (methodName == "Road" ? "EXBGRD" : (methodName == "AIRTOPORT" ? "EXBGAP" : "EXBGAD"))));
            string MethodName = (methodName == "Sea" ? "Sea" : (methodName == "Courier" ? "COURIER ECONOMY" : (methodName == "Road" ? "ROAD" : (methodName == "CourierExpress" ? "COURIER EXPRESS" : "AIR"))));
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
                                    new XElement("Description", item.Description), //Destination Country Code
                                    new XElement("Width", item.Length),
                                    new XElement("Depth", item.Breadth),
                                    new XElement("Height", item.Height),
                                    new XElement("Cube", item.Volume),
                                    new XElement("Quantity", item.Quantity),
                                    new XElement("CubeTotal", (item.Volume * item.Quantity)), //TODO
                                    new XElement("Weight", item.Groweight),
                                    new XElement("MesGK", item.Groweight) //TODO
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
                                new XElement("File", file), //Quote No + Service No
                                new XElement("Method", MethodName),
                                new XElement("Service", serviceName), //Static value
                                new XElement("ixType", xmlResult.IxType),
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
                                            new XElement("CCode", xmlResult.ToCountryCode) //Destination Country Code
                                        )
                                    )
                                ),
                                new XElement("AdditionalCosts",
                                    //new XElement("AddCharge01", additional)
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
            string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploads/" + fileName);

            saveXMLStatus(physicalPath, status, quoteType);

            xdoc.Save(physicalPath);
            
            var XMLEmail = System.Configuration.ConfigurationManager.AppSettings["XMLEmail"];
            if (!string.IsNullOrEmpty(XMLEmail))
            {
                var subject = "Admin's Baggage "+ xmlResult.QuoteStatus + " (" + file + ")";
                SendXMLEmail((int)companyId, XMLEmail, "Baggage", subject, physicalPath);
            }
            return physicalPath;
        }

        public static async Task<string> GenerateImportQuoteXml(QuoteCalculator.Service.Models.ImportsQuoteModel _ImportQuoteModel, List<QuoteCalculator.Service.Models.DestinationDetails> _ConsigneeModel = null, List<AddtionalCostModel> _AdditionalCostModel = null)
        {
            XNamespace url = "http://www.moveware.com.au/MovewareConnect.xsd";


            List<XElement> consigneeSubElemens = new List<XElement>();
            foreach (var item in _ConsigneeModel)
            {
                List<XElement> additionalCostElements = new List<XElement>();
                long destId = item.Id;
                if (_AdditionalCostModel != null && _AdditionalCostModel.Count > 0)
                {
                    List<AddtionalCostModel> model = new List<AddtionalCostModel>();
                    model = _AdditionalCostModel.Where(x => x.DestId == destId && x.Cost > 0).ToList();
                    foreach (var costItem in model)
                    {
                        //costItem.Type = costItem.Type.Replace(" ", "");
                        costItem.Type = costItem.Type.Replace(" ", "").Replace("/", "");
                        additionalCostElements.Add(
                            new XElement(costItem.Type, costItem.Cost)
                        );
                    }
                }
                consigneeSubElemens.Add(
                    new XElement("Consignee",
                        new XElement("ConsigneeName", item.ConsigneeName),
                        new XElement("VolumeKgs", item.Kgs),
                        new XElement("Type", _ImportQuoteModel.IxType),
                        new XElement("CollectFromBranch", item.CollectFromBranch),
                        new XElement("ConsigneeAddress",
                            new XElement("ConsigneeAddress1", item.DestAddress1),
                            new XElement("ConsigneeAddress2", item.DestAddress2),
                            new XElement("ConsigneePostcode", item.DestPostcode)
                        ),
                        new XElement("Tariff", item.Tariff),
                        new XElement("AdditionalCosts",
                            additionalCostElements.ToArray()
                        ),
                        new XElement("SpecialRequirements", item.VehicleName)
                    ));
            }

            XDocument xdoc = new XDocument(
                new XElement(url + "MWConnect",
                    new XElement("Details",
                        new XElement("Comments", _ImportQuoteModel.TotalCost),
                        new XElement("LiveTest", "TEST") //Static for Testing
                    ),
                    new XElement("MessageType",
                        new XElement("WebQuote",
                            new XElement("Removal",
                                new XElement("CompanyCode", _ImportQuoteModel.CompanyCode),
                                //new XElement("Type", "EXT"),
                                new XElement("Salesrep", _ImportQuoteModel.SalesRep),
                                new XElement("File", _ImportQuoteModel.RefNo),
                                //new XElement("Method", ),
                                new XElement("Service", _ImportQuoteModel.Service),
                                new XElement("ixType", _ImportQuoteModel.IxType),
                                new XElement("Port", _ImportQuoteModel.POE),
                                new XElement("Branch", _ImportQuoteModel.Branch),
                                 new XElement("SpecialRequirements", _ImportQuoteModel.Note),
                                new XElement("EstMoveDate", ""),
                                new XElement("ContainerSize", _ImportQuoteModel.StrContainerSize),
                                new XElement("Consignees", _ImportQuoteModel.TotalConsignee),
                                new XElement("Origin",
                                    new XElement("CustomerName", _ImportQuoteModel.CustomerName),
                                    new XElement("Agent",
                                        new XElement("AgentName", _ImportQuoteModel.AgentName),
                                        new XElement("AgentCode", _ImportQuoteModel.AgentCode)
                                    ),
                                     new XElement("Address",
                                        new XElement("CountryCode",
                                           new XElement("CCode", _ImportQuoteModel.OriginCountry)
                                        ),
                                        new XElement("Town", _ImportQuoteModel.OriginTown)
                                    )
                                ),
                            consigneeSubElemens.ToArray()
                            )
                        )
                    )
                )
            );

            string xml = xdoc.ToString();
            string token = await Navision.GetValidAccessToken();
            string status = await Navision.SubmitXmlAsync(token, xml, "ImportQuote");

            string extension = Path.GetExtension("Import_Quote.xml");
            string fileName = Guid.NewGuid() + "_" + Path.GetFileName("Import_Quote.xml");
         //   string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploads/ImportQuote/" + fileName);
            string physicalPath1 = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploads/ImportQuote");
            if (!Directory.Exists(physicalPath1))
            {
                Directory.CreateDirectory(physicalPath1);
            }
            string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploads/ImportQuote/" + fileName);
            saveXMLStatus(physicalPath, status, "ImportQuote");

            xdoc.Save(physicalPath);

            var XMLEmail = System.Configuration.ConfigurationManager.AppSettings["XMLEmail"];
            if (!string.IsNullOrEmpty(XMLEmail))
            {
                var subject = "Admin's Import Quote " + _ImportQuoteModel.RefNo;
                SendXMLEmail(SessionHelper.CompanyId, XMLEmail, "ImportQuote", subject, physicalPath);
            }

            return physicalPath;
        }

        public static async Task<string> GenerateTradeQuoteXml(TradeQuoteModel _TradeQuoteModel, List<QuoteCalculator.Service.Models.AddtionalCostModel> _AdditionalCostModel = null)
        {
            List<XElement> additionalCostElements = new List<XElement>();
            foreach (var item in _AdditionalCostModel)
            {
                item.Type = item.Type.Replace(" ", "");
                additionalCostElements.Add(
                    new XElement(item.Type, item.Cost)
                );
            }

            XNamespace url = "http://www.moveware.com.au/MovewareConnect.xsd";

            XDocument xdoc = new XDocument(
                new XElement(url + "MWConnect",
                    new XElement("Details",
                        new XElement("Tariff", _TradeQuoteModel.Tariff),
                        new XElement("Comments", _TradeQuoteModel.AdjustedPrice),
                        new XElement("ProfitMargin", _TradeQuoteModel.AdjustedProfit),
                        new XElement("LiveTest", "TEST") //Static for Testing
                    ),
                    new XElement("MessageType",
                        new XElement("WebQuote",
                            new XElement("Removal",
                            new XElement("CompanyCode", _TradeQuoteModel.CompanyCode),
                                new XElement("Type", "EXT"),
                                new XElement("Salesrep", _TradeQuoteModel.SalesRep),
                                new XElement("File", _TradeQuoteModel.RefNo),
                                //new XElement("Method", ""),
                                new XElement("Service", _TradeQuoteModel.StrService),
                                new XElement("ixType", _TradeQuoteModel.IxType),
                                new XElement("Branch", _TradeQuoteModel.Branch),
                                new XElement("Volume", _TradeQuoteModel.Volume),
                                new XElement("Origin",
                                    new XElement("Agent",
                                        new XElement("AgentName", _TradeQuoteModel.AgentName),
                                        new XElement("AgentCode", _TradeQuoteModel.AgentCode),
                                         new XElement("Address",
                                            new XElement("CountryCode",
                                                new XElement("CCode", "GB")
                                            )
                                         )
                                    )
                                ),
                                new XElement("Destination",
                                    new XElement("Address",
                                        new XElement("Suburb", _TradeQuoteModel.Suburb),
                                        new XElement("CountryCode",
                                            new XElement("CCode", _TradeQuoteModel.DestCountry)
                                        )
                                    )
                                ),
                                new XElement("Fees",
                                    new XElement("HandlingFee", _TradeQuoteModel.HandlingFee),
                                    new XElement("DestinationFee", _TradeQuoteModel.DestinationFee),
                                    new XElement("FreightFee", _TradeQuoteModel.FreightFee)
                                ),
                                new XElement("AdditionalCosts",
                                    additionalCostElements.ToArray()
                                )
                            )
                        )
                    )
                )
            );

            string xml = xdoc.ToString();
            string token = await Navision.GetValidAccessToken();
            string status = await Navision.SubmitXmlAsync(token, xml, "TradeQuote");

            string extension = Path.GetExtension("Trade_Quote.xml");
            string fileName = Guid.NewGuid() + "_" + Path.GetFileName("Trade_Quote.xml");
    
            string physicalPath1 = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploads/TradeQuote");
            if (!Directory.Exists(physicalPath1))
            {
                Directory.CreateDirectory(physicalPath1);
            }
            string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploads/TradeQuote/" + fileName);
            saveXMLStatus(physicalPath, status, "TradeQuote");

            xdoc.Save(physicalPath);

            var XMLEmail = System.Configuration.ConfigurationManager.AppSettings["XMLEmail"];
            if (!string.IsNullOrEmpty(XMLEmail))
            {
                var subject = "Admin's Trade Quote " + _TradeQuoteModel.RefNo;
                SendXMLEmail(SessionHelper.CompanyId, XMLEmail, "TradeQuote", subject, physicalPath);
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
                    else if (path.Contains("TradeQuote"))
                    {
                        model.Type = "TradeQuote_XML";
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

        public static async Task<string> GenerateRemovalXml(SP_GetRemovalXmlData_Result xmlResult, string QuoteStatus, string file,int companyId)
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
                                new XElement("Salesrep", xmlResult.SalesRepCode),
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
            string physicalPath1 = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploads/InternationalRemoval");
            if (!Directory.Exists(physicalPath1))
            {
                Directory.CreateDirectory(physicalPath1);
            }
            string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploads/InternationalRemoval/" + fileName);

            saveXMLStatus(physicalPath, status, quoteType);

            xdoc.Save(physicalPath);
            var XMLEmail = System.Configuration.ConfigurationManager.AppSettings["XMLEmail"];
            if (!string.IsNullOrEmpty(XMLEmail))
            {
                var subject = "Admin's Removal " + status + " (" + file + ")";
                SendXMLEmail((int)companyId, XMLEmail, "Vehicle", subject, physicalPath);
            }
            return physicalPath;
        }

        public static async Task<string> GenerateRemovalEnquiryXml(SP_GetRemovalXmlData_Result xmlResult, string QuoteStatus, string file, int companyId)
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
                                new XElement("Salesrep", xmlResult.SalesRepCode),
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
            string physicalPath1 = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploads/InternationalRemoval");
            if (!Directory.Exists(physicalPath1))
            {
                Directory.CreateDirectory(physicalPath1);
            }
            string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploads/InternationalRemoval/" + fileName);

            saveXMLStatus(physicalPath, status, "RemovalEnquiry");

            xdoc.Save(physicalPath);
            var XMLEmail = System.Configuration.ConfigurationManager.AppSettings["XMLEmail"];
            if (!string.IsNullOrEmpty(XMLEmail))
            {
                var subject = "Admin's Removal Enquiry" + status + " (" + file + ")";
                SendXMLEmail((int)companyId, XMLEmail, "Vehicle", subject, physicalPath);
            }
            return physicalPath;
        }

        public static async Task<string> GenerateHomeSurveyXml(SP_GetRemovalXmlData_Result xmlResult, string file, int companyId)
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
                                new XElement("Salesrep", xmlResult.SalesRepCode),
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
            string physicalPath1 = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploads/InternationalRemoval");
            if (!Directory.Exists(physicalPath1))
            {
                Directory.CreateDirectory(physicalPath1);
            }
            string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploads/InternationalRemoval/" + fileName);

            saveXMLStatus(physicalPath, status, "RemovalHomeSurvey");

            xdoc.Save(physicalPath);
            var XMLEmail = System.Configuration.ConfigurationManager.AppSettings["XMLEmail"];
            if (!string.IsNullOrEmpty(XMLEmail))
            {
                var subject = "Admin's Removal Home Survey" + status + " (" + file + ")";
                SendXMLEmail(companyId, XMLEmail, "RemovalHomeSurvey", subject, physicalPath);
            }
            return physicalPath;
        }

        public static async Task<string> GenerateHomeVideoSurveyXml(SP_GetRemovalXmlData_Result xmlResult, string file, DateTime? videoSurveyAppointmentTime, int companyId)
        {
            string Date = videoSurveyAppointmentTime == null ? "" :
                videoSurveyAppointmentTime.Value.ToString("dd-MM-yy").Replace('-', '/'); ;

            string Time = videoSurveyAppointmentTime == null ? "" :
                videoSurveyAppointmentTime.Value.ToString("HH:mm");

            XNamespace url = "http://www.moveware.com.au/MovewareConnect.xsd";
            List<XElement> inventoriesSubElemens = new List<XElement>();

            XDocument xdoc = new XDocument(
                new XElement(url + "MWConnect",
                    new XElement("Details",
                        new XElement("LiveTest", "TEST") 
                    ),
                    new XElement("MessageType",
                        new XElement("WebQuote",
                            new XElement("Removal",
                                new XElement("CompanyCode", "AP"),
                                new XElement("Branch", xmlResult.Branch),
                                new XElement("Type", "EXR"),
                                new XElement("QuoteStatus", "ENQUIRY"),
                                new XElement("Salesrep", xmlResult.SalesRepCode),
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
            string fileName = Guid.NewGuid() + "_" + Path.GetFileName("Removal_Quote.xml");
            string physicalPath1 = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploads/InternationalRemoval");
            if (!Directory.Exists(physicalPath1))
            {
                Directory.CreateDirectory(physicalPath1);
            }
            string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploads/InternationalRemoval/" + fileName);
            saveXMLStatus(physicalPath, status, "RemovalHomeVideoSurvey");

            xdoc.Save(physicalPath); 
            var XMLEmail = System.Configuration.ConfigurationManager.AppSettings["XMLEmail"];
            if (!string.IsNullOrEmpty(XMLEmail))
            {
                var subject = "Admin's Removal Home Video Survey" + status + " (" + file + ")";
                SendXMLEmail(companyId, XMLEmail, "RemovalHomeVideoSurvey", subject, physicalPath);
            }
            return physicalPath;
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
                else if (physicalPath.Contains("Baggage_Quote"))
                {
                    model.Type = "Baggage_XML";
                }
                else if (physicalPath.Contains("InternationalRemoval"))
                {
                    model.Type = "Removal_XML";
                }
                else if (physicalPath.Contains("ImportQuote"))
                {
                    model.Type = "ImportQuote_XML";
                }
                else if (physicalPath.Contains("TradeQuote"))
                {
                    model.Type = "TradeQuote_XML";
                }
                model.Status = status.ToUpper() == "CREATED" ? "success" : "failed";
                model.NavisionStatus = status;
                _dbRepositoryFailedXML.Insert(model);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
               
            }
        }

        public static string SendXMLEmail(int companyId,string xmlEmailAddress, string serviceFor, string subject, string XMLPath)
        {
            string message = string.Empty;
            string to = xmlEmailAddress;
            tbl_EmailTemplate emailModel = new tbl_EmailTemplate();

            emailModel = _dbRepositoryEmailTemplete.GetEntities().Where(m => m.ServiceId == 1027).FirstOrDefault();
            try
            {
                string bodyTemplate = string.Empty;
                if (emailModel != null)
                {
                    bodyTemplate = emailModel.HtmlContent;
                }
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
                if (emailModel != null)
                {
                    emailModel.Subject = subject;

                    bool status = EmailHelper.SendMail(companyId, to, emailModel.Subject, bodyTemplate, fromEmailKey, displayKey, true, "", "", XMLPath.ToString());
                }
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