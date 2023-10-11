using QuoteCalculator.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuoteCalculator.Service.Repository.BaggageRepository
{
    public interface IBaggageRepository
    {
        List<BaggageListModel> GetAllBaggage(int companyId, DataTablePaginationModel model);

        int DeleteBaggage(int? Id);

        List<BaggageQuoteInfoModel> ViewMyQuote(int Id, string email, int companyId);
        List<CartonsModel> GetCartonListByCompanyId(int companyId);
        List<BaggageQuoteInfoModel> GetBaggageDetailByIdAndCompanyId(int? Id, int companyId);
        List<BaggageItemModel> GetBaggageItemByQuoteId(int? quoteId);
        List<BaggageItemModel> GetBaggageItemByIds(int? quoteId, long? cartonId);
        List<BaggageItemModel> GetBaggageItems(int? quoteId);
        List<QuoteAmountList> GetQuoteIdList(int? quoteId);
        List<ukModel> GetListFromUk(string PostCode);
        List<UKbranchpostcode> GetListFromUKBranchPostCode(string PostCode);
        int InsertUpdateBaggageQuote(BaggageQuoteInfoModel QuoteObj);
        int InsertUpdateBaggageItem(BaggageItemModel moveBaggage);
        BaggageQuoteInfoModel GetBaggageQuoteById(int? quoteId);
        List<BaggageQuoteInfoModel> checkDuplicateQuoteRef(int? quoteId,string refNo);
        List<BaggageItemModel> GetMoveBaggageListByQuoteId(int? QuoteId);
        List<QuoteAmountModel> GetQuoteAmountByShippingType(int? QuoteId,string MoveType, string ShippingType);
        BaggageCalculationModel BaggageCalculation(long? QuoteId);
        BaggageCalculationLineModel GetTransitionTimeLine(string DeliveryMethodKey);
        decimal GetVolumetricsWeight(int? quoteId, string deliveryType, int? companyId);
        List<BaggageCostModel> GetBaggageCostByQuoteId(int? Id, string ShippingType); 
         List<EmailTemplateModel> GetEmailTemplateByServiceId(int? ServiceId);
        List<UserModel> GetUsersBySalesRepCode(string SalesRepCode,int? companyId);
        List<CityListModel> GetCityListByCountryId(string CountryCode, int? CompanyCode);
        int UpdateBaggageQuote(BaggageQuoteInfoModel QuoteObj);
        CollectionDelivery GetCollectionDeliveryData(long? QuoteId, int Company);
        BaggageXmlData GetBaggageXmlData(int? BaggageQuoteId, int CompanyId);
        List<BaggageQuoteInfoModel> GetallBaggageQuote(int? Company);
    }
}
