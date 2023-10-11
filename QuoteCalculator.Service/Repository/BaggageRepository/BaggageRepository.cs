using Dapper;
using QuoteCalculator.Service.Common;
using QuoteCalculator.Service.Models;
using QuoteCalculator.Service.Repository.CommonRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuoteCalculator.Service.Repository.BaggageRepository
{
    public class BaggageRepository : BaseRepository, IBaggageRepository
    {
        private readonly ICommonRepository _commonRepository;

        public BaggageRepository(ICommonRepository commonRepository)
        {
            _commonRepository = commonRepository;
        }

        public List<BaggageListModel> GetAllBaggage(int companyId, DataTablePaginationModel model)
        {
            List<BaggageListModel> baggageList;

            DynamicParameters parameter = new DynamicParameters();

            parameter.Add("@CompanyId", companyId);
            parameter.Add("@PageNumber", model.DtPageNumber);
            parameter.Add("@PageSize", model.DtPageSize);
            parameter.Add("@StrSearch", model.DtSearch != null ? model.DtSearch.Trim() : "");
            parameter.Add("@SortColumn", model.DtSortColumn);
            parameter.Add("@SortOrder", model.DtSortOrder);

            baggageList = GetAll<BaggageListModel>("SP_GetBaggageQuoteList", parameter);
            return baggageList;
        }

        public int DeleteBaggage(int? Id)
        {
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@Id", Id);

            int result = ExecuteStoredProcedure("sp_DeleteBaggage", parameter);
            return result;
        }

        public List<CartonsModel> GetCartonListByCompanyId(int companyId)
        {
            List<CartonsModel> cartons = new List<CartonsModel>();

            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@CompanyId", companyId);

            cartons = GetAll<CartonsModel>("SP_GetCartonListByCompanyId", parameter);
            return cartons;
        }

        public List<BaggageQuoteInfoModel> GetBaggageDetailByIdAndCompanyId(int? Id, int companyId)
        {
            List<BaggageQuoteInfoModel> baggageQuotes = new List<BaggageQuoteInfoModel>();

            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@Id", Id);
            parameter.Add("@CompanyId", companyId);

            baggageQuotes = GetAll<BaggageQuoteInfoModel>("SP_GetBaggageDetailsByIdAndCompanyId", parameter);
            return baggageQuotes;
        }

        public List<BaggageItemModel> GetBaggageItemByQuoteId(int? quoteId)
        {
            List<BaggageItemModel> baggageItems = new List<BaggageItemModel>();

            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@QuoteId", quoteId);

            baggageItems = GetAll<BaggageItemModel>("SP_GetBaggageItemsByQuoteId", parameter);

            return baggageItems;
        }
        public List<BaggageItemModel> GetBaggageItemByIds(int? quoteId, long? cartonId)
        {
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@QuoteId", quoteId);
            parameter.Add("@CartonId", cartonId);

            List<BaggageItemModel> baggageItems = GetAll<BaggageItemModel>("GetBaggageItemByIds", parameter);
            if (quoteId > 0 && baggageItems != null && baggageItems.Count > 0 && baggageItems[0].Id < 0)
            {
                baggageItems[0].Id = 0;
            }
            return baggageItems;
        }

        public List<BaggageItemModel> GetBaggageItems(int? quoteId)
        {
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@QuoteId", quoteId);
            List<BaggageItemModel> baggageItems = GetAll<BaggageItemModel>("SP_GetBaggageItems", parameter);
            return baggageItems;
        }

        public List<QuoteAmountList> GetQuoteIdList(int? quoteId)
        {
            List<QuoteAmountList> baggageQuotes = new List<QuoteAmountList>();
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@QuoteId", quoteId);

            baggageQuotes = GetAll<QuoteAmountList>("SP_GetQuoteIdList", parameter);

            return baggageQuotes;
        }

        public List<ukModel> GetListFromUk(string PostCode)
        {
            List<ukModel> ukModelList = new List<ukModel>();
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@PostCode", PostCode);

            ukModelList = GetAll<ukModel>("Sp_PostalCodeCheckUK", parameter);
            return ukModelList;
        }
        public List<UKbranchpostcode> GetListFromUKBranchPostCode(string PostCode)
        {
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@PostCode", PostCode);

            List<UKbranchpostcode> ukModelList = GetAll<UKbranchpostcode>("Sp_PostalCodeCheckUK_branch_postcode", parameter);
            return ukModelList;
        }
        public int InsertUpdateBaggageQuote(BaggageQuoteInfoModel QuoteObj)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", QuoteObj.Id);
            parameters.Add("@BranchId", QuoteObj.BranchId);
            parameters.Add("@CityName", QuoteObj.CityName);
            parameters.Add("@Company", QuoteObj.Company);
            parameters.Add("@CountryCode", QuoteObj.CountryCode);
            parameters.Add("@CreatedDate", QuoteObj.CreatedDate);
            parameters.Add("@Email", QuoteObj.Email);
            parameters.Add("@EstimatedMoveDate", QuoteObj.EstimatedMoveDate);
            parameters.Add("@FirstName", QuoteObj.Firstname);
            parameters.Add("@FromCountry", QuoteObj.FromCountry);
            parameters.Add("@FromCity", QuoteObj.FromCity);
            parameters.Add("@InternalNotes", QuoteObj.InternalNotes);
            parameters.Add("@IsConditionApply", QuoteObj.IsConditionApply);
            parameters.Add("@LastName", QuoteObj.Lastname);
            parameters.Add("@NextExecutionDate", QuoteObj.NextExecutionDate);
            parameters.Add("@PostCode", QuoteObj.PostCode);
            parameters.Add("@SalesRep", QuoteObj.SalesRep);
            parameters.Add("@Telephone", QuoteObj.Telephone);
            parameters.Add("@TitleId", QuoteObj.TitleId);
            parameters.Add("@ToCountry", QuoteObj.ToCountry);
            parameters.Add("@IsInquiry", QuoteObj.IsInquiry);
            parameters.Add("@GeneratedId", dbType: DbType.Int32, direction: ParameterDirection.Output);


            ExecuteStoredProcedure("SP_InsertUpdateBaggageQuote", parameters);

            if (QuoteObj.Id == 0)
            {
                int generatedId = parameters.Get<int>("@GeneratedId");
                return generatedId;
            }
            else
            {
                return 0;
            }
        }

        public int InsertUpdateBaggageItem(BaggageItemModel moveBaggage)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", moveBaggage.Id);
            parameters.Add("@QuoteId", moveBaggage.QuoteId);
            parameters.Add("@CartonId", moveBaggage.CartonId);
            parameters.Add("@Type", moveBaggage.Type);
            parameters.Add("@Description", moveBaggage.Description);
            parameters.Add("@Length", moveBaggage.Length);
            parameters.Add("@Breadth", moveBaggage.Breadth);
            parameters.Add("@Height", moveBaggage.Height);
            parameters.Add("@Volume", moveBaggage.Volume);
            parameters.Add("@UserVolume", moveBaggage.UserVolume);
            parameters.Add("@Quantity", moveBaggage.Quantity);
            parameters.Add("@Groweight", moveBaggage.Groweight);
            parameters.Add("@MovewareDescription", moveBaggage.MovewareDescription);
            //parameters.Add("@AirtFreightToAirport", airtFreightToAirport);
            //parameters.Add("@Courier", courier);
            //parameters.Add("@SeaFreight", seaFreight);
            parameters.Add("@Company", moveBaggage.Company);
            parameters.Add("@GeneratedId", dbType: DbType.Int32, direction: ParameterDirection.Output);



           // if (moveBaggage.Id == 0 && moveBaggage.CartonId != 0)
            //{
                ExecuteStoredProcedure("SP_InsertUpdateBaggageItem", parameters);
                int generatedId = parameters.Get<int>("@GeneratedId");
                return generatedId;
            //}
            //else
            //{
            //    return 0;
            //}

        }

        public List<BaggageQuoteInfoModel> ViewMyQuote(int Id, string email, int companyId)
        {
            //SessionHelper.BaggageQuoteId = Id;
            var firstDate = DateTime.Today.AddMonths(-6);

            List<BaggageQuoteInfoModel> baggageList;

            DynamicParameters parameter = new DynamicParameters();

            parameter.Add("@CompanyId", companyId);
            parameter.Add("@Email", email);


            baggageList = GetAll<BaggageQuoteInfoModel>("SP_GetBaggageQuoteInfo", parameter);

            //_dbRepositoryBaggageQuote.GetEntities().Where(m => m.Email == email && m.IsDelete != true && m.CreatedDate >= firstDate && m.IsInquiry != true && m.Company == companyId).OrderByDescending(m => m.CreatedDate).ToList();

            List<BaggageQuoteInfoModel> baggageQuoteList = new List<BaggageQuoteInfoModel>();
            int volume = 0;

            //bool IsAnyMethodSelected = baggageList != null && baggageList.Count(d => d.AirFreightToAirport == true || d.AirFreightToDoor == true || d.Courier == true || d.SeaFreight == true || d.RoadFreightToDoor == true || d.CourierExpressToDoor == true) > 0;
            for (int i = 0; i < baggageList.Count(); i++)
            {
                if (baggageList[i].AirFreightToAirportFinal > 0 || baggageList[i].AirFreightToDoorFinal > 0 || baggageList[i].SeaFreightFinal > 0 || baggageList[i].CourierFinal > 0 || baggageList[i].RoadFreightToDoorFinal > 0 || baggageList[i].CourierExpressToDoorFinal > 0)
                {
                    BaggageQuoteInfoModel baggageObj = new BaggageQuoteInfoModel
                    {
                        Id = baggageList[i].Id,
                        PostCode = baggageList[i].PostCode,
                        FromCity = baggageList[i].FromCity,
                        FromCountry = baggageList[i].FromCountry,
                        ToPostCode = baggageList[i].ToPostCode,
                        ToCountry = baggageList[i].ToCountry,
                        CityName = baggageList[i].CityName,
                        CreatedDate = baggageList[i].CreatedDate,
                        AirFreightToAirport = baggageList[i].AirFreightToAirport == true,
                        AirFreightToDoor = baggageList[i].AirFreightToDoor == true,
                        Courier = baggageList[i].Courier == true,
                        SeaFreight = baggageList[i].SeaFreight == true,
                        RoadFreightToDoor = baggageList[i].RoadFreightToDoor == true,
                        CourierExpressToDoor = baggageList[i].CourierExpressToDoor == true,
                        InternalNotes = string.IsNullOrEmpty(baggageList[i].InternalNotes) ? null : baggageList[i].InternalNotes,
                        isMethodSelected = baggageList[i].AirFreightToAirport == true || baggageList[i].AirFreightToDoor == true || baggageList[i].Courier == true || baggageList[i].SeaFreight == true || baggageList[i].RoadFreightToDoor == true || baggageList[i].CourierExpressToDoor == true //IsAnyMethodSelected
                    };
                    //if (baggageObj.AirFreightToAirport == true || baggageObj.AirFreightToDoor == true || baggageObj.Courier == true || baggageObj.SeaFreight == true || baggageObj.RoadFreightToDoor == true || baggageObj.CourierExpressToDoor == true)
                    //{
                    //    baggageObj.isMethodSelected = true;
                    //}

                    string FromCity = string.IsNullOrEmpty(baggageObj.FromCity) ? baggageObj.PostCode : baggageObj.FromCity;
                    string ToCity = string.IsNullOrEmpty(baggageObj.CityName) ? baggageObj.ToPostCode : baggageObj.CityName;

                    SP_GetCollectionDelivery_Result collectionDeliveryResult = _commonRepository.GetCollectionDelivery(baggageList[i].Id, companyId);  //CustomRepository.GetCollectionDeliveryData(baggageList[i].Id);
                    baggageObj.DeliveryCharge = Math.Round(Convert.ToDecimal(collectionDeliveryResult.DeliveryCharge), 2, MidpointRounding.ToEven);
                    baggageObj.CollectionCharge = Math.Round(Convert.ToDecimal(collectionDeliveryResult.CollectionCharge), 2, MidpointRounding.ToEven);


                    long id = Convert.ToInt64(baggageList[i].Id);

                    DynamicParameters dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@Id", id);


                    var cartonObjList = GetAll<BaggageItemModel>("Sp_GetBaggageItemListById", dynamicParameters); //_dbRepositoryMoveBaggage.GetEntities().Where(m => m.QuoteId == id).ToList();

                    foreach (var cartonItem in cartonObjList)
                    {
                        string FullSizeCartonStr = "";
                        FullSizeCartonStr = Convert.ToString(cartonItem.Quantity + " X " + cartonItem.Description + " (" + cartonItem.Volume + " cubic feet" + (cartonItem.Quantity > 1 ? " each)" : ")"));
                        if (cartonItem.Length > 0 && cartonItem.Breadth > 0 && cartonItem.Height > 0)
                        {
                            FullSizeCartonStr += Convert.ToString(", " + cartonItem.Length + " X " + cartonItem.Breadth + " X " + cartonItem.Height + " Cms");
                        }
                        if (cartonItem.UserVolume > 0)
                        {
                            FullSizeCartonStr += Convert.ToString(", " + cartonItem.UserVolume + " Kgs");
                        }
                        baggageObj.CartonList += FullSizeCartonStr + System.Environment.NewLine;
                    }
                    if (cartonObjList != null && cartonObjList.Count > 0)
                        baggageObj.HasMainCartons = cartonObjList.Count(x => string.Compare(x.Type, "MAIN", true) == 0) > 0;

                    baggageObj.BaggagePriceList = new List<BaggageCalculationLineModel>();

                    //quotesEntities entityObj = new quotesEntities();




                    //var QuoteId = new SqlParameter
                    //{
                    //    ParameterName = "QuoteId",
                    //    DbType = DbType.Int64,
                    //    Value = baggageList[i].Id
                    //};

                    int quoteId = baggageList[i].Id;

                    DynamicParameters quoteAmountParam = new DynamicParameters();

                    quoteAmountParam.Add("@QuoteId", quoteId);
                    quoteAmountParam.Add("@MoveType", "EXB");

                    var tblQuoteAmounts = GetAll<BaggageAmountModel>("SP_GetAmountDetails", quoteAmountParam);
                    //_dbRepositoryQuoteAmount.GetEntities().Where(x => x.QuoteId == quoteId && x.MoveType == "EXB" && (x.QuoteAmount ?? 0) > 0).ToList();

                    if (tblQuoteAmounts.Count() == 0)
                        continue;
                    double cubicFeet = 0;
                    volume = 0;
                    for (int j = 0; j < cartonObjList.Count(); j++)
                    {
                        volume += cartonObjList[j].UserVolume ?? 0;
                        cubicFeet += (cartonObjList[j].Volume * cartonObjList[j].Quantity);
                    }

                    foreach (var tblQuoteAmount in tblQuoteAmounts)
                    {
                        //string Desc = string.Empty;
                        //if (tblQuoteAmount.ShippingTypeDescription != "Sea Freight" && tblQuoteAmount.ShippingTypeDescription != "Road freight To Door")
                        //{
                        //    string deliveryType = tblQuoteAmount.ShippingTypeDescription == "Courier" ? "COURIER" : (tblQuoteAmount.ShippingTypeDescription == "Courier Express To Door" ? "COURIEREXPRESS" : "AIR");

                        //    string sqlQuery = "SELECT [dbo].[FN_GetVolumetricsWeight] ('" + quoteId + "','" + deliveryType + "','" + companyId + "')";

                        //    decimal VolumetricsWeight = ExecuteQuery(sqlQuery); //CustomRepository.GetVolumetricsWeight(baggageList[i].Id, deliveryType);
                        //    Desc = VolumetricsWeight.ToString() + " Vol kgs" + (volume > 0 ? ("/" + volume + " Kgs gross") : "");
                        //}
                        string Desc = string.Empty;
                        if (tblQuoteAmount.ShippingTypeDescription != "Sea Freight" && tblQuoteAmount.ShippingTypeDescription != "Road freight To Door")
                        {
                            string deliveryType = tblQuoteAmount.ShippingTypeDescription == "Courier" ? "COURIER" : (tblQuoteAmount.ShippingTypeDescription == "Courier Express To Door" ? "COURIEREXPRESS" : "AIR");

                            string sqlQuery = "SELECT [dbo].[FN_GetVolumetricsWeight] (@quoteId, @deliveryType, @companyId)";

                            using (var connection = GetDbConnection())
                            {
                                var param = new { quoteId, deliveryType, companyId };
                                decimal VolumetricsWeight = ExecuteQuery<decimal>(sqlQuery, param);
                                Desc = VolumetricsWeight.ToString() + " Vol kgs" + (volume > 0 ? ("/" + volume + " Kgs gross") : "");
                            }
                        }

                        else
                        {
                            Desc = string.Concat(cubicFeet, " cubic feet");
                        }

                        baggageObj.BaggagePriceList.Add(new BaggageCalculationLineModel()
                        {
                            DeliveryMethodName = tblQuoteAmount.ShippingTypeDescription == "Courier" ? "Courier Economy To Door" : (tblQuoteAmount.ShippingTypeDescription == "Sea Freight" ? "Sea Freight To Door" : tblQuoteAmount.ShippingTypeDescription),
                            Amount = tblQuoteAmount.QuoteAmount ?? 0,
                            TransitionTime = tblQuoteAmount.TransitionTime,
                            CalcDescription = Desc
                        });
                        baggageObj.CustomerRefNo = string.Concat(tblQuoteAmount.CustomerReferenceNo + "/" + tblQuoteAmount.CustomerQuoteNo);
                    }
                    baggageObj.BaggagePriceList = baggageObj.BaggagePriceList.OrderBy(m => m.Amount).ToList();

                    if (baggageQuoteList.Count(x => x.CustomerRefNo == baggageObj.CustomerRefNo) == 0)
                        baggageQuoteList.Add(baggageObj);
                }
            }
            //myquote.bagageQuoteInfo = baggageQuoteList;
            return (baggageQuoteList);
        }
        public BaggageQuoteInfoModel GetBaggageQuoteById(int? quoteId)
        {
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@Id", quoteId);
            //parameter.Add("@companyId", companyId);
            var List = Get<BaggageQuoteInfoModel>("SP_GetBaggageQuoteById", parameter);
            return List;

        }
        public List<BaggageQuoteInfoModel> checkDuplicateQuoteRef(int? quoteId, string refNo)
        {
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@Id", quoteId);
            parameter.Add("@refNo", refNo);
            var List = GetAll<BaggageQuoteInfoModel>("SP_checkDuplicateQuoteRef", parameter);
            return List;
        }
        public List<BaggageItemModel> GetMoveBaggageListByQuoteId(int? QuoteId)
        {
            try
            {
                DynamicParameters parameter = new DynamicParameters();
                parameter.Add("@QuoteId", QuoteId);
                var List = GetAll<BaggageItemModel>("SP_GetMoveBaggageListByQuoteId", parameter);
                return List;
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        public List<QuoteAmountModel> GetQuoteAmountByShippingType(int? QuoteId, string MoveType, string ShippingType)
        {
            try
            {
                DynamicParameters parameter = new DynamicParameters();
                parameter.Add("@QuoteId", QuoteId);
                parameter.Add("@MoveType", MoveType);
                parameter.Add("@ShippingType", ShippingType);
                var List = GetAll<QuoteAmountModel>("SP_GetQuoteAmountByShipping", parameter);
                return List;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public BaggageCalculationModel BaggageCalculation(long? QuoteId)
        {
            try
            {
                DynamicParameters parameter = new DynamicParameters();
                parameter.Add("@QuoteId", QuoteId);
                var List = Get<BaggageCalculationModel>("SP_BaggegeCalculation", parameter);
                return List;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public BaggageCalculationLineModel GetTransitionTimeLine(string DeliveryMethodKey)
        {
            try
            {
                DynamicParameters parameter = new DynamicParameters();
                parameter.Add("@DeliveryMethodKey", DeliveryMethodKey);
                var List = Get<BaggageCalculationLineModel>("SP_TransitionTimeLine", parameter);
                return List;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public decimal GetVolumetricsWeight(int? quoteId, string deliveryType, int? companyId)
        {
            try
            {
                DynamicParameters parameter = new DynamicParameters();
                parameter.Add("@quoteId", quoteId);
                parameter.Add("@deliveryType", deliveryType);
                parameter.Add("@companyId", companyId);
                var result = Get<decimal>("SP_GetVolumetricsWeight", parameter);
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public List<BaggageCostModel> GetBaggageCostByQuoteId(int? Id, string ShippingType)
        {
            try
            {
                DynamicParameters parameter = new DynamicParameters();
                parameter.Add("@QuoteId", Id);
                parameter.Add("@ShippingType", ShippingType);
                var result = GetAll<BaggageCostModel>("SP_GetBaggageCostByQuoteId", parameter);
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public List<EmailTemplateModel> GetEmailTemplateByServiceId(int? ServiceId)
        {
            try
            {
                DynamicParameters parameter = new DynamicParameters();
                parameter.Add("@ServiceId", ServiceId);
                var result = GetAll<EmailTemplateModel>("SP_GetEmailTemplateByServiceId", parameter);
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public List<UserModel> GetUsersBySalesRepCode(string SalesRepCode, int? companyId)
        {
            try
            {
                DynamicParameters parameter = new DynamicParameters();
                parameter.Add("@SalesRepCode", SalesRepCode);
                parameter.Add("@companyId", companyId);
                var result = GetAll<UserModel>("SP_GetUsersBySalesRepCode", parameter);
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public List<CityListModel> GetCityListByCountryId(string CountryCode, int? CompanyCode)
        {
            try
            {
                DynamicParameters parameter = new DynamicParameters();
                parameter.Add("@CountryCode", CountryCode);
                parameter.Add("@CompanyCode", CompanyCode);
                var result = GetAll<CityListModel>("GetCityForBaggae", parameter);
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public int UpdateBaggageQuote(BaggageQuoteInfoModel QuoteObj)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", QuoteObj.Id);

            parameters.Add("@Price", QuoteObj.Price);
            parameters.Add("@AirFreightToAirport", QuoteObj.AirFreightToAirport);
            parameters.Add("@AirFreightToDoor", QuoteObj.AirFreightToDoor);
            parameters.Add("@Courier", QuoteObj.Courier);
            parameters.Add("@SeaFreight", QuoteObj.SeaFreight);
            parameters.Add("@RoadFreightToDoor", QuoteObj.RoadFreightToDoor);
            parameters.Add("@CourierExpressToDoor", QuoteObj.CourierExpressToDoor);
            int result = ExecuteStoredProcedure("SP_UpdateBaggageQuote", parameters);
            return result;
        }

        public CollectionDelivery GetCollectionDeliveryData(long? QuoteId, int Company)
        {
            try
            {
                DynamicParameters parameter = new DynamicParameters();
                parameter.Add("@QuoteId", QuoteId);
                parameter.Add("@Company", Company);
                var result = Get<CollectionDelivery>("SP_GetCollectionDelivery", parameter);
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public BaggageXmlData GetBaggageXmlData(int? BaggageQuoteId, int CompanyId)
        {
            try
            {
                DynamicParameters parameter = new DynamicParameters();
                parameter.Add("@BaggageQuoteId", BaggageQuoteId);
                parameter.Add("@Company", CompanyId);
                var result = Get<BaggageXmlData>("SP_GetBaggageXmlData", parameter);
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public List<BaggageQuoteInfoModel> GetallBaggageQuote(int? Company)
        {
            try
            {
                DynamicParameters parameter = new DynamicParameters();
                parameter.Add("@Company", Company);

                var result = GetAll<BaggageQuoteInfoModel>("SP_GetAllBaggageQuote", parameter);
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
