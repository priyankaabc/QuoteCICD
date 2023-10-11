using QuoteCalculator.Service.Models;
using QuoteCalculator.Service.Repository.CommonRepository;
using QuoteCalculatorAdmin.Common;
using QuoteCalculatorAdmin.Data;
using QuoteCalculatorAdmin.Data.Repository;
using QuoteCalculatorAdmin.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuoteCalculatorAdmin.Controllers
{
    public class CommonController : Controller
    {
        #region private variables
        private readonly GenericRepository<rates_destinations> _dbRepositoryCountry;
        public readonly GenericRepository<tbl_CountryCode> _dbRepositoryCountryCode;
        private readonly ICommonRepository _commonRepository;

        #endregion
        #region Constructor
        public CommonController(ICommonRepository commonRepository)
        {
            _dbRepositoryCountry = new GenericRepository<rates_destinations>();
            _dbRepositoryCountryCode = new GenericRepository<tbl_CountryCode>();
            _commonRepository = commonRepository;
        }
        #endregion
        //public ActionResult GetServiceList()
        //{
        //    using (var context = BaseContext.GetDbContext())
        //    {
        //        var list = context.tbl_Service.Select(m => new { m.Id, m.Name }).OrderBy(m => new { m.Name }).ToList();
        //        return Json(list, JsonRequestBehavior.AllowGet);
        //    }
        //}
        public IEnumerable<CommonModel> getServiceList()
        {
            var serviceList = _commonRepository.GetList("service", null);
            return serviceList;
        }

        //Unutilized
        //public ActionResult GetCountryList()
        //{
        //    using (var context = BaseContext.GetDbContext())
        //    {
        //        var list = context.rates_destinations.Where(m => m.country_code != "").Select(m => new { m.country_code, m.country }).OrderBy(m => new { m.country }).Distinct().ToList();
        //        list.Insert(list.Count, new { country_code = "UK", country = "UNITED KINGDOM" });
        //        return Json(list, JsonRequestBehavior.AllowGet);
        //    }
        //}

        public IEnumerable<SelectListItem> GetHeadingContentlList()
        {
            //using (var context = BaseContext.GetDbContext())
            //{
            //    var list = context.tbl_HeadingContent.ToList();
            //    IEnumerable<SelectListItem> CountryList = list.Select(c => new SelectListItem { Value = Convert.ToString(c.HeadingContentId), Text = c.Heading });
            //    return CountryList;
            //}
            var list = _commonRepository.GetList("HeadingList", null);
            IEnumerable<SelectListItem> CountryList = list.Select(c => new SelectListItem { Value = Convert.ToString(c.Value), Text = c.Text });
            return CountryList;
        }

        public IEnumerable<SelectListItem> GetTitleList()
        {
            //using (var context = BaseContext.GetDbContext())
            //{
            //    var list = context.tbl_Title.OrderBy(x => x.DisplayOrder).Select(m => new { TitleId = m.Id, m.TitleName }).ToList();
            //}
            //List<CommonModel> list = new List<CommonModel>();

            var list = _commonRepository.GetList("Title", null);
            IEnumerable<SelectListItem> TitleList = list.Select(m => new SelectListItem { Value = m.Value, Text = m.Text });
            return TitleList;
        }

        public IEnumerable<SelectListItem> GetVehicleTypeList()
        {
            //using (var context = BaseContext.GetDbContext())
            //{
            //    var list = context.tbl_VehicleType.Select(m => new { m.Id, m.TypeName }).ToList();
            //    return Json(list, JsonRequestBehavior.AllowGet);
            //}
            var list = _commonRepository.GetList("VehicleType", null);
            IEnumerable<SelectListItem> VehicleTypeList = list.Select(m => new SelectListItem { Value = m.Value, Text = m.Text});
            return VehicleTypeList;
        }

        //Unutilized
        //public ActionResult GetCountryCodeList()
        //{
        //    using (var context = BaseContext.GetDbContext())
        //    {
        //        var list = context.tbl_CountryCode.Select(m => new { CountryCodeId = m.CountryCode, CountryCode = m.CountryName + " (" + m.CountryCode + ")" }).OrderBy(x => x.CountryCode).ToList();
        //        list.Insert(0, new { CountryCodeId = "+44", CountryCode = "United Kingdom (+44)" });
        //        return Json(list, JsonRequestBehavior.AllowGet);
        //    }
        //}

        //Unutilized
        //public ActionResult GetBranchList()
        //{
        //    using (var context = BaseContext.GetDbContext())
        //    {
        //        var list = context.branch.Select(m => new { m.br_id, m.br_branch, m.br_code }).ToList();
        //        return Json(list, JsonRequestBehavior.AllowGet);
        //    }
        //}

        //Unutilized
        //public ActionResult GetQuoteTypeList()
        //{
        //    using (var context = BaseContext.GetDbContext())
        //    {
        //        var list = context.tbl_QuoteType.Select(m => new { m.Id, m.QuoteType }).Distinct().ToList();
        //        return Json(list, JsonRequestBehavior.AllowGet);
        //    }
        //}

        public IEnumerable<SelectListItem> GetCompanyList()
        {
            //using (var context = BaseContext.GetDbContext())
            //{
            //var list = context.tbl_Company.Distinct().ToList();
            //}
            var list = _commonRepository.GetList("companyCode", null);

            IEnumerable<SelectListItem> cmpList = list.Select(c => new SelectListItem { Value = Convert.ToString(c.Value), Text = c.Text });
            return cmpList;
        }

        //Unutilized
        //public ActionResult BaggageFromCountryList()
        //{
        //    using (var context = BaseContext.GetDbContext())
        //    {
        //        var list = context.rates_destinations.Where(m => m.bag_dest == 1).Select(m => new { m.country_code, m.country }).Distinct().ToList();
        //        list.Insert(0, new { country_code = "UK", country = "UNITED KINGDOM" });
        //        return Json(list, JsonRequestBehavior.AllowGet);
        //    }

        //    //var fromCountryList = _dbRepositoryCountry.GetEntities().Where(m => m.dcr_id > 0).Select(m => new { m.country_code, m.country }).Distinct().OrderBy(m => m.country).ToList();
        //    //fromCountryList.Insert(0, new { country_code = "UK", country = "UNITED KINGDOM" });
        //    //ViewBag.FromCountryList = fromCountryList;                        
        //}

        //Unutilized
        //public ActionResult BaggageToCountryList()
        //{
        //    using (var context = BaseContext.GetDbContext())
        //    {
        //        var list = context.rates_destinations.Where(m => m.bag_dest == 1).Select(m => new { m.country_code, m.country }).OrderBy(m => new { m.country }).Distinct().ToList();
        //        list.Insert(list.Count, new { country_code = "UK", country = "UNITED KINGDOM" });
        //        //var toCountryList = _dbRepositoryCountry.GetEntities().Where(m => m.bag_dest == 1).Select(m => new { m.country_code, m.country }).Distinct().OrderBy(m => m.country).ToList();
        //        //toCountryList.Insert(0, new { country_code = "UK", country = "UNITED KINGDOM" });
        //        //ViewBag.CountryList = toCountryList;
        //        return Json(list, JsonRequestBehavior.AllowGet);
        //    }
        //}


        //Unutilized
        //public ActionResult GetSalesRepCodeList()
        //{
        //    using (var context = BaseContext.GetDbContext())
        //    {
        //        var list = context.user.Where(m => m.SalesRepCode != null && m.CompanyId == SessionHelper.CompanyId).Select(m => new { userId = m.id, code = m.SalesRepCode }).ToList();
        //        return Json(list, JsonRequestBehavior.AllowGet);
        //    }
        //}

        //Unutilized
        //public ActionResult GetDayScheduleList()
        //{
        //    using (var context = BaseContext.GetDbContext())
        //    {
        //        var list = context.tbl_DaySchedule.Select(m => new { m.Id, m.Name }).Distinct().ToList();
        //        return Json(list, JsonRequestBehavior.AllowGet);
        //    }
        //}

        //Unutilized
        [HttpGet]
        public ActionResult QuoteDetails(string fromCountryCode, string countryCode, string quoteType)
        {
            List<SP_GetQuoteContents_Result> Details = CustomRepository.GetQuoteContents(fromCountryCode, countryCode, quoteType).OrderBy(m => m.DisplayOrder).ToList();
            return PartialView("_QuoteContentView", Details);
        }

        public IEnumerable<SelectListItem> GetCountryListCommon()
        {
            var countryList = _commonRepository.GetList("GetFromcountry", null);
            //var countryList = _dbRepositoryCountry.GetEntities().Where(m => m.rem_dest == 1).Select(m => new { m.country_code, m.country }).Distinct().ToList();

            //countryList.Insert(0, new { country_code = "US", country = "USA" });
            //countryList.Insert(0, new { country_code = "AE", country = "UNITED ARAB EMIRATES" });
            //countryList.Insert(0, new { country_code = "TH", country = "THAILAND" });
            //countryList.Insert(0, new { country_code = "ZA", country = "SOUTH AFRICA" });
            //countryList.Insert(0, new { country_code = "SG", country = "SINGAPORE" });
            //countryList.Insert(0, new { country_code = "NZ", country = "NEW ZEALAND" });
            //countryList.Insert(0, new { country_code = "MT", country = "MALTA" });
            //countryList.Insert(0, new { country_code = "MY", country = "MALAYSIA" });
            //countryList.Insert(0, new { country_code = "IN", country = "INDIA" });
            //countryList.Insert(0, new { country_code = "HK", country = "HONG KONG" });
            //countryList.Insert(0, new { country_code = "CY", country = "CYPRUS" });
            //countryList.Insert(0, new { country_code = "CA", country = "CANADA" });
            //countryList.Insert(0, new { country_code = "AU", country = "AUSTRALIA" });
            IEnumerable<SelectListItem> CountryList = countryList.Select(c => new SelectListItem { Value = Convert.ToString(c.Value), Text = c.Text }).Distinct().OrderBy(x => x.Text);
            return CountryList;
        }

        public IEnumerable<CommonModel> GetCountryCodeListCommon()
        {
            IEnumerable<CommonModel> CountryCode;
            List<CommonModel> CountryCodeList = new List<CommonModel>();

            try
            {
                //CountryCodeList = _dbRepositoryCountryCode.GetEntities().Distinct().OrderBy(m => m.CountryName).ToList();
                //for (int i = 0; i < CountryCodeList.Count; i++)
                //{
                //    CountryCodeList[i].CountryName = CountryCodeList[i].CountryName + " (" + CountryCodeList[i].CountryCode + ")";
                //}

                CountryCodeList = _commonRepository.GetList("countryCode", null);
            }
            catch (Exception ex)
            {

            }
            CountryCode = CountryCodeList.Select(c => new CommonModel { Value = c.Value, Text = c.Text });
            return CountryCode;
        }
        public IEnumerable<CommonModel> GetSalesRepCodeListCommon()
        {
            //using (var context = BaseContext.GetDbContext())
            //{
            //    var list = context.user.Where(m => m.SalesRepCode != null && m.CompanyId == SessionHelper.CompanyId).Select(m => new { userId = m.id, code = m.SalesRepCode }).ToList();
            //    return list.Select(c => new SelectListItem { Value = Convert.ToString(c.code), Text = c.code });
            //}
            var list = _commonRepository.GetList("SalesRepDropdown", SessionHelper.CompanyId);
            return list.Select(c => new CommonModel { Value = Convert.ToString(c.Value), Text = c.Text });
        }

        //public IEnumerable<SelectListItem> GetAgentListCommon()
        //{
        //    IEnumerable<SelectListItem> AgentList;
        //    List<CommonDDLModel> list = new List<CommonDDLModel>();
        //    try
        //    {
        //        quotesEntities entityObj = new quotesEntities();
        //        list = entityObj.Database.SqlQuery<CommonDDLModel>("SP_GetImportAgentList").ToList();
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    AgentList = list.Select(c => new SelectListItem { Value = Convert.ToString(c.Value), Text = c.Text });
        //    return AgentList;
        //}

        public IEnumerable<CommonModel> GetCountryListImportQuoteCommon()
        {
            //var countryList = context.rates_destinations.Where(m => m.country_code != "").Select(m => new { m.country_code, m.country }).OrderBy(m => new { m.country }).Distinct().ToList();
            //countryList.Insert(countryList.Count, new { country_code = "UK", country = "UNITED KINGDOM" });
            var countryList = _commonRepository.GetList("CountryListImportQuote", null);

            IEnumerable<CommonModel> CountryList = countryList.Select(c => new CommonModel { Value = Convert.ToString(c.Value), Text = c.Text });
            return CountryList;
        }

        public IEnumerable<CommonModel> GetPOEList()
        {
            IEnumerable<CommonModel> POEList;
            List<CommonModel> list = new List<CommonModel>();
            try
            {
                list = _commonRepository.GetList("PoeList", null);
            }
            catch (Exception ex)
            {
            }
            POEList = list.Select(c => new CommonModel { Value = Convert.ToString(c.Value), Text = c.Text });
            return POEList;
        }

        public IEnumerable<CommonModel> GetBranchListCommon()
        {
            IEnumerable<CommonModel> BranchList;
            List<CommonModel> list = new List<CommonModel>();
            try
            {
                list = _commonRepository.GetList("Branch", null);
            }
            catch (Exception ex)
            {
            }
            BranchList = list.Select(c => new CommonModel { Value = Convert.ToString(c.Value), Text = c.Text });
            return BranchList;
        }
        public IEnumerable<CommonModel> GetBranchByAgentId(int? AgentId)
        {
            IEnumerable<CommonModel> BranchList;
            List<CommonModel> list = new List<CommonModel>();
            try
            {
                list = _commonRepository.GetBranchByAgentId(AgentId);
            }
            catch (Exception ex)
            {
            }
            BranchList = list.Select(c => new CommonModel { Value = Convert.ToString(c.Value), Text = c.Text });
            return BranchList;
        }

        public IEnumerable<SelectListItem> GetServiceListCommon(bool IsTradeQuote)
        {
            IEnumerable<SelectListItem> ServiceList;
            List<CommonModel> list = new List<CommonModel>();
            try
            {
                //IsTradeQuote = false;
                //quotesEntities entityObj = new quotesEntities();
                //var paramIsTrade = new SqlParameter
                //{
                //    ParameterName = "IsTradeQuote",
                //    DbType = DbType.Boolean,
                //    Value = IsTradeQuote
                //};

                list = _commonRepository.GetServiceListCommon(IsTradeQuote); //entityObj.Database.SqlQuery<CommonModel>("SP_GetServiceList @IsTradeQuote", paramIsTrade).ToList();
            }
            catch (Exception ex)
            {
            }
            ServiceList = list.Select(c => new SelectListItem { Value = Convert.ToString(c.Value), Text = c.Text });
            return ServiceList;
        }
        public IEnumerable<CommonModel> GetContainerSizeList()
        {
            IEnumerable<CommonModel> ContainerSizeList;
            List<CommonModel> list = new List<CommonModel>();
            try
            {
                list = _commonRepository.GetList("ContainerSizeList", null);
            }
            catch (Exception ex)
            {
            }
            ContainerSizeList = list.Select(c => new CommonModel { Value = Convert.ToString(c.Value), Text = c.Text });
            return ContainerSizeList;
        }

        public IEnumerable<CommonModel> GetVehicleList()
        {
            IEnumerable<CommonModel> VehicleList;
            List<CommonModel> list = new List<CommonModel>();
            try
            {
                list = _commonRepository.GetList("Vehicle", null); //entityObj.Database.SqlQuery<CommonModel>("SP_GetVehicleList").ToList();
            }
            catch (Exception ex)
            {
            }
            VehicleList = list.Select(c => new CommonModel { Value = Convert.ToString(c.Value), Text = c.Text });
            return VehicleList;
        }

        public static DataTablePaginationModel GetDataTablePaginationModel(DatatableModel model)
        {
            DataTablePaginationModel DtModel = new DataTablePaginationModel();
            try
            {
                DtModel.DtPageNumber = model.start / model.length + 1;
                DtModel.DtPageSize = model.length;
                DtModel.DtSearch = string.IsNullOrEmpty(model.search.value) ? string.Empty : model.search.value;
                DtModel.DtSortColumn = model.columns.Count > 0 && model.order != null && model.order.Count > 0 ? model.columns[model.order[0].column].data : string.Empty;
                DtModel.DtSortOrder = model.order != null && model.order.Count > 0 ? model.order[0].dir : string.Empty;
            }
            catch (Exception ex)
            {

            }
            return DtModel;
        }
        public IEnumerable<SelectListItem> GetQuoteList()
        {
            List<CommonModel> list = new List<CommonModel>();
            try
            {
                //quotesEntities entityObj = new quotesEntities();
                //var prmtable = new SqlParameter
                //{
                //    ParameterName = "prmtable",
                //    DbType = DbType.String,
                //    Value = "Quote"
                //};
                // list = entityObj.Database.SqlQuery<QuoteTypeModel>("sp_Get_list @prmtable", prmtable).ToList();
                list = _commonRepository.GetList("QuoteList", null);
            }
            catch (Exception ex)
            {
            }
            IEnumerable<SelectListItem> quoteTypeList = list.Select(c => new SelectListItem { Value = Convert.ToString(c.Value), Text = c.Text });
            return quoteTypeList;
        }

        public IEnumerable<CommonModel> GetRoleList()
        {
            List<CommonModel> list = new List<CommonModel>();
            try
            {
                //quotesEntities entityObj = new quotesEntities();
                //var prmtable = new SqlParameter
                //{
                //    ParameterName = "prmtable",
                //    DbType = DbType.String,
                //    Value = "Role"
                //};
                //list = entityObj.Database.SqlQuery<RoleModel>("sp_Get_list @prmtable", prmtable).ToList();
                list = _commonRepository.GetList("RoleList", null);
            }
            catch (Exception ex)
            {
            }
            IEnumerable<CommonModel> quoteTypeList = list.Select(c => new CommonModel { Value = Convert.ToString(c.Value), Text = c.Text });
            return quoteTypeList;
        }
        public IEnumerable<CommonModel> GetRolecompanyList()
        {
            List<CommonModel> list = new List<CommonModel>();
            try
            {
                //quotesEntities entityObj = new quotesEntities();
                //var prmtable = new SqlParameter
                //{
                //    ParameterName = "prmtable",
                //    DbType = DbType.String,
                //    Value = "company"
                //};
                //list = entityObj.Database.SqlQuery<CompanyModel>("sp_Get_list @prmtable", prmtable).ToList();
                list = _commonRepository.GetList("companyList", null);
            }
            catch (Exception ex)
            {
            }
            IEnumerable<CommonModel> quoteTypeList = list.Select(c => new CommonModel { Value = Convert.ToString(c.Value), Text = c.Text });
            return quoteTypeList;
        }
        public IEnumerable<SelectListItem> GetAgentList(bool IsTradeAgent)
        {
            List<CommonModel> list = new List<CommonModel>();
            try
            {
                //quotesEntities entityObj = new quotesEntities();
                //var prmtable = new SqlParameter
                //{
                //    ParameterName = "prmtable",
                //    DbType = DbType.String,
                //    Value = "Agent"
                //};
                if (IsTradeAgent)
                {
                    //list = entityObj.Database.SqlQuery<AgentModel>("sp_Get_list @prmtable", prmtable).Where(c => c.IsTradeAgent && c.IsActive).ToList();
                    list = _commonRepository.GetList("AgentListByTradeAgent", null);

                }
                else
                {
                    //list = entityObj.Database.SqlQuery<AgentModel>("sp_Get_list @prmtable", prmtable).ToList();
                    //list = entityObj.Database.SqlQuery<AgentModel>("sp_Get_list @prmtable", prmtable).Where(c => c.IsImportAgent && c.IsActive).ToList();
                    list = _commonRepository.GetList("AgentListByImportAgent", null);
                }
            }
            catch (Exception ex)
            {
            }
            IEnumerable<SelectListItem> quoteTypeList = list.Select(c => new SelectListItem { Value = Convert.ToString(c.Value), Text = c.Text });
            return quoteTypeList;
        }


    }
}