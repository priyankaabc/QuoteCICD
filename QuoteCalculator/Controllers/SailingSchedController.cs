using NLog;
using QuoteCalculator.Common;
using QuoteCalculator.Data;
using QuoteCalculator.Data.Repository;
using QuoteCalculator.Helper;
using QuoteCalculator.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace QuoteCalculator.Controllers
{
    public class SailingSchedController : Controller
    {

        #region private variables
        private static NLog.Logger logger = LogManager.GetCurrentClassLogger();

        private readonly GenericRepository<sailingsched> _dbRepositorySailingSched;
        
        quotesEntities quotesDBEntities = new quotesEntities();

        #endregion

        #region Constructor
        public SailingSchedController()
        {
            _dbRepositorySailingSched = new GenericRepository<sailingsched>();
        }
        #endregion

        public ActionResult Index(string location)
        {
            List<SailingSchedModel> modelList = new List<SailingSchedModel>();
            try
            {
                var sailingscheds = _dbRepositorySailingSched.GetEntities().Where(x => x.location.ToUpper() == location.ToUpper()).GroupBy(x => x.country);

                foreach (var sailingSchedGroup in sailingscheds)
                {
                    SailingSchedModel model = new SailingSchedModel();

                    model.CountryName = sailingSchedGroup.Key;
                    model.SailingSchedules = new List<sailingsched>();
                    model.SailingSchedules.AddRange(sailingSchedGroup);
                    modelList.Add(model);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = CommonHelper.GetErrorMessage(ex);
            }
            return View(modelList);
        }
    }
}