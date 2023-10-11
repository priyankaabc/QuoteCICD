using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using NLog;
using QuoteCalculator.Data;
using QuoteCalculator.Data.Repository;
using QuoteCalculator.Models;

namespace QuoteCalculator.Helper
{
    public class Navision
    {
        private static NLog.Logger logger = LogManager.GetCurrentClassLogger();
        private static GenericRepository<tbl_CommonSettings> _dbRepositoryCommonSettings = new GenericRepository<tbl_CommonSettings>();

        public static async Task<string> GetValidAccessToken()
        {
            try
            {
                //Get Access Token from datbase with timestamp
                //If AT is null or timestamp < 300 minutes - Generate new Token ELSE Use Token from database
                var commonSettings = _dbRepositoryCommonSettings.GetEntities().ToList();
                tbl_CommonSettings AccessToken = commonSettings.Where(m => m.Key == "NavisionToken").FirstOrDefault();
                tbl_CommonSettings AccessTokenCreatedOn = commonSettings.Where(m => m.Key == "NavisionTokenCreatedOn").FirstOrDefault();

                DateTime TokenExpiryTime = Convert.ToDateTime(AccessTokenCreatedOn.Value).AddMinutes(300);
                DateTime now = DateTime.Now;

                if (now > TokenExpiryTime)
                {
                    //File.WriteAllLines("log.txt", new string[] { File.ReadAllText("log.txt") + "\r\n" + "Hello World!" });

                    using (var client = new HttpClient())
                    {
                        var webUrl = "https://stagingangloapi.pickfords.co.uk/";

                        client.BaseAddress = new Uri(webUrl);
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.DefaultRequestHeaders.ConnectionClose = true;

                        //Set Basic Auth
                        var user = "PF-AngloRAWNET";
                        var password = "n#wSC#8b";
                        var base64String = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{user}:{password}"));
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64String);

                        var result = await client.PostAsync("https://stagingangloapi.pickfords.co.uk/requestaccesstokens.asmx/requestaccesstoken", null);
                        //File.WriteAllLines("log.txt", new string[] { File.ReadAllText("log.txt") + "\r\n" + ("Status: " + result.StatusCode) });
                        //File.WriteAllLines("log.txt", new string[] { File.ReadAllText("log.txt") + "\r\n" + "Result: " + (await result.Content.ReadAsStringAsync()) });

                        string response = await result.Content.ReadAsStringAsync();
                        JavaScriptSerializer json_serializer = new JavaScriptSerializer();
                        NavisionModel[] navision = json_serializer.Deserialize<NavisionModel[]>(response);
                        

                        AccessToken.Value = navision[0].accessToken;
                        AccessTokenCreatedOn.Value = now.ToString();

                        string newAccessToken = _dbRepositoryCommonSettings.Update(AccessToken);
                        string accessTokenCreatedOn = _dbRepositoryCommonSettings.Update(AccessTokenCreatedOn);
                        logger.Info("navisionAccessToken: " + navision[0].accessToken + " | CreatedTime:" + AccessTokenCreatedOn.Value);
                        return navision[0].accessToken;
                    }
                }
                return AccessToken.Value;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                //insert ex to log
                return null;
            }
        }

        public static async Task<string> SubmitXmlAsync(string token, string xml, string quoteType)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var webUrl = "https://stagingangloapi.pickfords.co.uk/";
                    client.BaseAddress = new Uri(webUrl);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));//ACCEPT header

                    NavisionXMLRequestModel requestData = new NavisionXMLRequestModel();

                    requestData.accessToken = token;
                    xml = xml.Replace(" xmlns=\"\"", "");
                    requestData.XMLData = xml;
                    requestData.QuoteType = quoteType;

                    string data = Newtonsoft.Json.JsonConvert.SerializeObject(requestData);
                    var content = new StringContent(data, Encoding.UTF8, "application/xml");

                    logger.Info("navisionXMLData: " + data + "");

                    var result = await client.PostAsync("https://stagingangloapi.pickfords.co.uk/importangloxmldata.asmx/importangloxmlresponse", content);

                    logger.Info("navisionStatus: " + result.StatusCode.ToString() + "");
                    return result.StatusCode.ToString();
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    return null;
                }
            }
        }
    }
}