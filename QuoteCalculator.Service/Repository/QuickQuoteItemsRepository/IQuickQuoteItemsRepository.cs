using Dapper;
using QuoteCalculator.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuoteCalculator.Service.Repository.QuickQuoteItemsRepository
{
    public interface IQuickQuoteItemsRepository
    {
        List<QuickQuoteItemsModel> GetQuickQuoteItems(int? ItemId, int? Company);
        QuickQuoteItemsModel GetQuickQuoteItemsById(int? Id, int CompanyId);
        IEnumerable<QuoteAmountModel> GetQuoteAmount(); 
       QuoteAmountModel GetQuoteAmountByQuoteId(int? QuoteId,string MoveType,string ShippingType);
        IEnumerable<AdditionalQuickQuoteItemsModel> GetAdditionalQuickQuoteItems();
        int AddAdditionalQuickQuote(AdditionalQuickQuoteItemsModel AdditionalQuickQuote, int CompanyId);
        int UpdateAdditionalQuickQuote(AdditionalQuickQuoteItemsModel AdditionalQuickQuote, int CompanyId);
        //IEnumerable<RatesDestinationsModel> GetRatesDestinationsList();
        IEnumerable<GuideLinkModel> GetGuideLink();
        int AddQuoteAmount(QuoteAmountModel QuoteAmountModel);
        int UpdateQuoteAmount(QuoteAmountModel QuoteAmountModel);

    }
}
