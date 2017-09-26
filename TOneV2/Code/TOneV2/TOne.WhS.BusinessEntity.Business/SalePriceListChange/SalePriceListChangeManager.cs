using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.BusinessEntity.Entities.SalePricelistChanges;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SalePriceListChangeManager
    {
        public Vanrise.Entities.IDataRetrievalResult<SalePricelistRateChangeDetail> GetFilteredPricelistRateChanges(Vanrise.Entities.DataRetrievalInput<SalePriceListChangeQuery> input)
        {
            ISalePriceListChangeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISalePriceListChangeDataManager>();
            var salePriceListRateChanges = dataManager.GetFilteredSalePricelistRateChanges(input.Query.PriceListId, input.Query.Countries);
            return DataRetrievalManager.Instance.ProcessResult(input, salePriceListRateChanges.ToBigResult(input, null, SalePricelistRateChangeDetailMapper));
        }

        public Vanrise.Entities.IDataRetrievalResult<SalePricelistCodeChange> GetFilteredPricelistCodeChanges(Vanrise.Entities.DataRetrievalInput<SalePriceListChangeQuery> input)
        {
            ISalePriceListChangeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISalePriceListChangeDataManager>();
            var salePriceListRateChanges = dataManager.GetFilteredSalePricelistCodeChanges(input.Query.PriceListId, input.Query.Countries);
            return DataRetrievalManager.Instance.ProcessResult(input, salePriceListRateChanges.ToBigResult(input, null, SalePricelistCodeChangeDetailMapper));
        }
        public Vanrise.Entities.IDataRetrievalResult<SalePricelistRPChangeDetail> GetFilteredSalePriceListRPChanges(Vanrise.Entities.DataRetrievalInput<SalePriceListChangeQuery> input)
        {
            ISalePriceListChangeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISalePriceListChangeDataManager>();
            var salePriceListRateChanges = dataManager.GetFilteredSalePriceListRPChanges(input.Query.PriceListId, input.Query.Countries);
            return DataRetrievalManager.Instance.ProcessResult(input, salePriceListRateChanges.ToBigResult(input, null, SalePricelistRPChangeDetailMapper));
        }

        public List<SaleCode> GetSalePriceListSaleCodeSnapShot(int pricelistId)
        {
            ISalePriceListChangeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISalePriceListChangeDataManager>();
            var saleCodesSnapshot = dataManager.GetSalePriceListSnapShot(pricelistId);

            SaleCodeManager saleCodeManager = new SaleCodeManager();
            return saleCodeManager.GetSaleCodesByCodeId(saleCodesSnapshot.SnapShotDetail.CodeIds);
        }

        public SalePriceListOption GetOwnerOptions(int priceListId)
        {
            SalePriceListManager salePriceListManager = new SalePriceListManager();
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            var priceList = salePriceListManager.GetPriceList(priceListId);
            if (priceList != null)
            {
                var ownerId = priceList.OwnerId;
                var carrierAccount = carrierAccountManager.GetCarrierAccount(ownerId);
                return new SalePriceListOption
                {
                    OwnerName = carrierAccountManager.GetCarrierAccountName(ownerId),
                    CompressPriceListFile = carrierAccountManager.GetCustomerCompressPriceListFileStatus(ownerId),
                };
            }
            return null;
        }

        public int? GetOwnerPricelistTemplateId(int priceListId)
        {
            SalePriceListManager salePriceListManager = new SalePriceListManager();
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            var priceList = salePriceListManager.GetPriceList(priceListId);
            if (priceList != null)
            {
                var ownerId = priceList.OwnerId;
                return carrierAccountManager.GetCustomerPriceListTemplateId(ownerId);
            }
            return null;
        }

        public void BulkInsertSalePriceListSaleCodeSnapshot(IEnumerable<SalePriceListSnapShot> salePriceListSaleCodeSnapshots)
        {
            var dataManager = BEDataManagerFactory.GetDataManager<ISalePriceListChangeDataManager>();
            dataManager.SaveSalePriceListSnapshotToDb(salePriceListSaleCodeSnapshots);

        }

        public void BulkCustomerSalePriceListChanges(IEnumerable<SalePriceListCustomerChange> customerChanges, IEnumerable<SalePricelistCodeChange> codeChanges, IEnumerable<SalePricelistRateChange> rateChanges
            , IEnumerable<SalePricelistRPChange> rpChanges, long processInstanceId)
        {
            var dataManager = BEDataManagerFactory.GetDataManager<ISalePriceListChangeDataManager>();
            dataManager.SaveCustomerChangesToDb(customerChanges);
            dataManager.SaveCustomerCodeChangesToDb(codeChanges);
            dataManager.SaveCustomerRateChangesToDb(rateChanges, processInstanceId);
            dataManager.SaveCustomerRoutingProductChangesToDb(rpChanges, processInstanceId);
        }
       
        public CustomerPriceListChange GetCustomerChangesByPriceListId(int pricelistId)
        {
            ISalePriceListChangeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISalePriceListChangeDataManager>();
            var salePriceListRateChanges = dataManager.GetFilteredSalePricelistRateChanges(pricelistId, null);
            var salePriceListCodeChanges = dataManager.GetFilteredSalePricelistCodeChanges(pricelistId, null);
            var routingProductChanges = dataManager.GetFilteredSalePriceListRPChanges(pricelistId, null);
            CustomerPriceListChange changes = new CustomerPriceListChange();
            changes.CodeChanges.AddRange(salePriceListCodeChanges);
            changes.RateChanges.AddRange(salePriceListRateChanges);
            changes.RoutingProductChanges.AddRange(routingProductChanges);
            changes.PriceListId = pricelistId;

            return changes;
        }
        #region Mapper
        private SalePricelistRateChangeDetail SalePricelistRateChangeDetailMapper(SalePricelistRateChange salePricelistRateChange)
        {
            RoutingProductManager routingProductManager = new RoutingProductManager();
            CurrencyManager currencyManager = new CurrencyManager();
            var salePricelistRateChangeDetail = new SalePricelistRateChangeDetail
            {
                ZoneName = salePricelistRateChange.ZoneName,
                BED = salePricelistRateChange.BED,
                EED = salePricelistRateChange.EED,
                Rate = salePricelistRateChange.Rate,
                ChangeType = salePricelistRateChange.ChangeType,
                ServicesId = !salePricelistRateChange.ZoneId.HasValue
                    ? routingProductManager.GetDefaultServiceIds(salePricelistRateChange.RoutingProductId)
                    : routingProductManager.GetZoneServiceIds(salePricelistRateChange.RoutingProductId,
                        salePricelistRateChange.ZoneId.Value)
            };
            if (salePricelistRateChange.CurrencyId.HasValue)
                salePricelistRateChangeDetail.CurrencySymbol = currencyManager.GetCurrencySymbol(salePricelistRateChange.CurrencyId.Value);

            return salePricelistRateChangeDetail;
        }
        private SalePricelistCodeChange SalePricelistCodeChangeDetailMapper(SalePricelistCodeChange salePricelistCodeChange)
        {
            return salePricelistCodeChange;
        }
        private SalePricelistRPChangeDetail SalePricelistRPChangeDetailMapper(SalePricelistRPChange salePricelistRpChange)
        {
            RoutingProductManager routingProductManager = new RoutingProductManager();
            SalePricelistRPChangeDetail salePricelistRpChangeDetail = new SalePricelistRPChangeDetail
            {
                ZoneName = salePricelistRpChange.ZoneName,
                BED = salePricelistRpChange.BED,
                EED = salePricelistRpChange.EED,
                RoutingProductName = routingProductManager.GetRoutingProductName(salePricelistRpChange.RoutingProductId),
                RoutingProductServicesId = !salePricelistRpChange.ZoneId.HasValue
                    ? routingProductManager.GetDefaultServiceIds(salePricelistRpChange.RoutingProductId)
                    : routingProductManager.GetZoneServiceIds(salePricelistRpChange.RoutingProductId,
                        salePricelistRpChange.ZoneId.Value)
            };
            if (salePricelistRpChange.RecentRoutingProductId.HasValue)
            {
                int recentRoutingProductId = salePricelistRpChange.RecentRoutingProductId.Value;
                salePricelistRpChangeDetail.RecentRoutingProductName = routingProductManager.GetRoutingProductName(recentRoutingProductId);

                salePricelistRpChangeDetail.RecentRouringProductServicesId = !salePricelistRpChange.ZoneId.HasValue
                    ? routingProductManager.GetDefaultServiceIds(recentRoutingProductId)
                    : routingProductManager.GetZoneServiceIds(recentRoutingProductId, salePricelistRpChange.ZoneId.Value);
            }
            return salePricelistRpChangeDetail;
        }

        #endregion

    }
}
