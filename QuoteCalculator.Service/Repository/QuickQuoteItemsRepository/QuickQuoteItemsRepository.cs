using Dapper;
using QuoteCalculator.Service.Common;
using QuoteCalculator.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuoteCalculator.Service.Repository.QuickQuoteItemsRepository
{
    public class QuickQuoteItemsRepository : BaseRepository, IQuickQuoteItemsRepository
    {
        public List<QuickQuoteItemsModel> GetQuickQuoteItems(int? ItemId, int? Company)
        {
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@prmItemId", ItemId);
            parameter.Add("@prmCompany", Company);
            var result = GetAll<QuickQuoteItemsModel>("Sp_QuickQuoteItemsList", parameter);
            return result;
        }
        public QuickQuoteItemsModel GetQuickQuoteItemsById(int? Id, int CompanyId)
        {
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@prmItemId", Id);
            parameter.Add("@prmCompany", CompanyId);

            var result = Get<QuickQuoteItemsModel>("Sp_QuickQuoteItemsList", parameter);
            return result;
        }

        public IEnumerable<QuoteAmountModel> GetQuoteAmount()
        {
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@prmId", 0);
            parameter.Add("@prmCompany", 0);
            var result = GetAll<QuoteAmountModel>("Sp_QuoteAmountList", parameter);
            return result;
        }
        public QuoteAmountModel GetQuoteAmountByQuoteId(int? QuoteId, string MoveType, string ShippingType)
        {
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@QuoteId", QuoteId);
            parameter.Add("@MoveType", MoveType);
            parameter.Add("@ShippingType", ShippingType);
            var result = Get<QuoteAmountModel>("Sp_QuoteAmountListByQuoteId", parameter);
            return result;

        }
        public IEnumerable<AdditionalQuickQuoteItemsModel> GetAdditionalQuickQuoteItems()
        {
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@prmId", 0);
            var result = GetAll<AdditionalQuickQuoteItemsModel>("Sp_AdditionalQuickQuoteItemslist", parameter);
            return result;
        }
        public int AddAdditionalQuickQuote(AdditionalQuickQuoteItemsModel AdditionalQuickQuote, int CompanyId)
        {
            return addupdateAdditionalQuickQuote(AdditionalQuickQuote, CompanyId);
        }

        public int UpdateAdditionalQuickQuote(AdditionalQuickQuoteItemsModel AdditionalQuickQuote, int CompanyId)
        {
            return addupdateAdditionalQuickQuote(AdditionalQuickQuote, CompanyId);
        }

        public int addupdateAdditionalQuickQuote(AdditionalQuickQuoteItemsModel model, int CompanyId)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@RemovalId", model.InternationalRemovalId);
            parameters.Add("@QuickQuoteItemId", model.QuickQuoteItemId);
            parameters.Add("@Cu_ft", model.Cuft);
            parameters.Add("@SpecialRequirements", model.SpecialRequirements);
            parameters.Add("@Company", CompanyId);
            int result = ExecuteStoredProcedure("SP_InsertUpdateAdditionalQuickQuoteItems", parameters);
            return result;
        }

        //public IEnumerable<RatesDestinationsModel> GetRatesDestinationsList()
        //{
        //    DynamicParameters parameter = new DynamicParameters();
        //    parameter.Add("@prmId", 0);
        //    parameter.Add("@prmCompany", 0);
        //    var result = GetAll<RatesDestinationsModel>("Sp_rates_destinationslist", parameter);
        //    return result;
        //}

        public IEnumerable<GuideLinkModel> GetGuideLink()
        {
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@prmId", 0);
            var result = GetAll<GuideLinkModel>("Sp_GuideLinklist", parameter);
            return result;
        }

        public int AddQuoteAmount(QuoteAmountModel QuoteAmountModel)
        {
            return addupdateQuoteAmount(QuoteAmountModel);
        }

        public int UpdateQuoteAmount(QuoteAmountModel QuoteAmountModel)
        {
            return addupdateQuoteAmount(QuoteAmountModel);
        }

        public int addupdateQuoteAmount(QuoteAmountModel model)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@prmId", model.Id);
            parameters.Add("@prmQuoteId", model.QuoteId);
            parameters.Add("@prmMoveType", model.MoveType);
            parameters.Add("@prmQuoteAmount", model.QuoteAmount);
            parameters.Add("@prmShippingType", model.ShippingType);
            parameters.Add("@prmShippingTypeDescription", model.ShippingTypeDescription);
            parameters.Add("@prmTransitionTime", model.TransitionTime);
            parameters.Add("@prmCreatedOn", model.CreatedOn);
            parameters.Add("@prmIsBooked", model.IsBooked);
            parameters.Add("@prmCustomerReferenceNo", model.CustomerReferenceNo);
            parameters.Add("@prmCustomerQuoteNo", model.CustomerQuoteNo);
            parameters.Add("@prmQuoteSeqNo", model.QuoteSeqNo);
            parameters.Add("@prmCompany", model.Company);

            int result = ExecuteStoredProcedure("Sp_QuoteAmountList_addEdit", parameters);
            return result;
        }

    }
}
