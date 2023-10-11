using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Core.EntityClient;
using QuoteCalculator.Data;

namespace QuoteCalculator.Service.Helper
{
    public class EFConnection
    {
        public static string EFConnectionString
        {
            get
            {
                SqlConnectionStringBuilder provider = new SqlConnectionStringBuilder();
                provider.ConnectionString = AESEncryptionDecryptionHelper.Decrypt(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
                //  provider.InitialCatalog= ConfigurationManager.AppSettings["DefaultConnection"]
                provider.MultipleActiveResultSets = true;
                provider.TrustServerCertificate = false;

                EntityConnectionStringBuilder ecsb = new EntityConnectionStringBuilder();
                ecsb.Provider = "System.Data.SqlClient";
                ecsb.ProviderConnectionString = provider.ToString();
                ecsb.Metadata= string.Format("res://*/Data.QuoteCalculator.csdl|res://*/Data.QuoteCalculator.ssdl|res://*/Data.QuoteCalculator.msl");
                return ecsb.ToString();
            }
        }
            
    }
}
