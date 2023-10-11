using QuoteCalculatorPickfords.Common;
using QuoteCalculatorPickfords.Data;
using QuoteCalculatorPickfords.Data.Repository;
using QuoteCalculatorPickfords.Helper;
using QuoteCalculatorPickfords.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace QuoteCalculatorPickfords.Controllers
{
    public class SailingSchedController : Controller
    {

        #region private variables
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
            var sailingscheds = _dbRepositorySailingSched.GetEntities().Where(x => x.location.ToUpper() == location.ToUpper()).GroupBy(x=>x.country);

            List<SailingSchedModel> modelList = new List<SailingSchedModel>();

            foreach (var sailingSchedGroup in sailingscheds)
            {
                SailingSchedModel model = new SailingSchedModel();

                model.CountryName = sailingSchedGroup.Key;
                model.SailingSchedules = new List<sailingsched>();
                model.SailingSchedules.AddRange(sailingSchedGroup);
                modelList.Add(model);
            }

            return View(modelList);
        }
    }
}