using Dapper;
using QuoteCalculator.Service.Common;
using QuoteCalculator.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace QuoteCalculator.Service.Repository.CommonRepository
{
    public class CommonRepository : BaseRepository, ICommonRepository
    {
        public List<CommonModel> GetList(string ListOf, int? companyId)
        {
            List<CommonModel> list = new List<CommonModel>();

            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@prmtable", ListOf);
            parameter.Add("@CompanyId", companyId);

            list = GetAll<CommonModel>("sp_Get_list", parameter);

            return list;
        }
        public List<CommonModel> GetServiceListCommon(bool IsTradeQuote)
        {
            List<CommonModel> list = new List<CommonModel>();

            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@IsTradeQuote", IsTradeQuote);

            list = GetAll<CommonModel>("SP_GetServiceList", parameter);

            return list;
        }
        public SP_GetCollectionDelivery_Result GetCollectionDelivery(Nullable<long> quoteId, Nullable<int> company)
        {
            SP_GetCollectionDelivery_Result gcd = new SP_GetCollectionDelivery_Result();

            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@QuoteId", quoteId);
            parameter.Add("@Company", company);
                        
            gcd = Get<SP_GetCollectionDelivery_Result>("SP_GetCollectionDelivery", parameter);

            return gcd;

        }
        public List<CommonModel> GetBranchByAgentId(int? AgentId)
        {
            List<CommonModel> list = new List<CommonModel>();

            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@AgentId", AgentId);

            list = GetAll<CommonModel>("SP_GetBranchByAgentId", parameter);

            return list;
        }
        
    }
}
