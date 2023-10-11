using Newtonsoft.Json;
using NLog;
using QuoteCalculator.Common;
using QuoteCalculator.Helper;
using QuoteCalculator.Models;
using QuoteCalculator.XmlService;
using System;
using System.Web.Mvc;

namespace QuoteCalculator.Controllers
{
    public class ShipmentTrackingController : Controller
    {
        #region private variables
        private static Logger logger = LogManager.GetCurrentClassLogger();
        #endregion
        // GET: ShipmentTracking
        public ActionResult Index()
        {
            try
            {
                if (HttpContext.Request.Url.GetLeftPart(UriPartial.Authority) == System.Configuration.ConfigurationManager.AppSettings["AngloUrl"])
                {
                    SessionHelper.COMPANY_ID = 1;
                }
                else if (HttpContext.Request.Url.GetLeftPart(UriPartial.Authority) == System.Configuration.ConfigurationManager.AppSettings["PicfordUrl"])
                {
                    SessionHelper.COMPANY_ID = 2;
                }
                else if (HttpContext.Request.Url.GetLeftPart(UriPartial.Authority) == System.Configuration.ConfigurationManager.AppSettings["ExcessUrl"])
                {
                    SessionHelper.COMPANY_ID = 3;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = CommonHelper.GetErrorMessage(ex);
            }
            return View();
        }

        [HttpGet]
        public ActionResult ShippingXMLDetailsPartial(string RefNumber, string Consig)
        {
            ShipmentXMLModel model = new ShipmentXMLModel();
            try
            {
                ///Generate Shipment Tracking XML....
                mwwebResponse result = XMLHelper.ShipmentXmlAPICall(RefNumber, Consig);
                if (result.pOK == "TRUE")
                {
                    if (result.tt[2].ttype != null)
                    {
                        ///Deserialize Shipment Tracking XML....
                        string str = result.tt[2].ttype;
                        var result1 = (RootObject)JsonConvert.DeserializeObject(str, typeof(RootObject));

                        //Reg Expression for Address..
                        System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"(<br />|<br/>|</ br>|</br>)");
                        string address = string.Empty;

                        //Mapping Fields....
                        model.Message = "True";
                        model.RefrenceNumber = RefNumber;
                        model.SurName = Consig;
                        model.ReceivedDate = result1.diary.departure.dddate.ToString();
                        model.VesselOrFlightName = result1.usage.vessel.entity.enname.ToString();
                        model.ContainerOrAirwaybill = result1.usage.container.entity.encode.ToString();
                        model.DestinationPort = result1.address.destination.adname.ToString();
                        model.EstimatedTimeofArrival = result1.diary.arrival.dddate.ToString();
                        model.DestinationAgentName = result1.usage.destinationagent.entity.enname.ToString();
                        model.AgentTelephone = result1.usage.destinationagent.entity.contact.phone.cccontact.ToString();
                        model.AgentFax = result1.usage.destinationagent.entity.contact.fax.cccontact.ToString();
                        model.AgentEmail = result1.usage.destinationagent.entity.contact.email.cccontact.ToString();
                        model.Address = regex.Replace(result1.usage.destinationagent.entity.address.adfulladdress.ToString(), "\r\n");
                        return PartialView("_ShipmentTrackingView", model);
                    }
                    else
                    {
                        model.Message = "Down";
                        return PartialView("_ShipmentTrackingView", model);
                    }

                }
                else
                {
                    model.Message = "False";
                    return PartialView("_ShipmentTrackingView", model);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = CommonHelper.GetErrorMessage(ex);

                model.Message = "False";
                return PartialView("_ShipmentTrackingView", model);
            }
        }
    }
}