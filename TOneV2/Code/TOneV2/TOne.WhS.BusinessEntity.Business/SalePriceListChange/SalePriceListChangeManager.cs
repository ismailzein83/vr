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
        public Vanrise.Entities.IDataRetrievalResult<CustomerRatePreviewDetail> GetFilteredCustomerRatePreviews(Vanrise.Entities.DataRetrievalInput<CustomerRatePreviewQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new CustomerRatePreviewRequestHandler());
        }
        public IEnumerable<int> GetAffectedCustomerIdsRPChangesByProcessInstanceId(long ProcessInstanceId)
        {
            ISalePriceListChangeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISalePriceListChangeDataManager>();
            return dataManager.GetAffectedCustomerIdsRPChangesByProcessInstanceId(ProcessInstanceId);
        }

        public IEnumerable<int> GetAffectedCustomerIdsRateChangesByProcessInstanceId(long ProcessInstanceId)
        {
            ISalePriceListChangeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISalePriceListChangeDataManager>();
            return dataManager.GetAffectedCustomerIdsRateChangesByProcessInstanceId(ProcessInstanceId);
        }
        public Vanrise.Entities.IDataRetrievalResult<RoutingProductPreviewDetail> GetFilteredRoutingProductPreviews(Vanrise.Entities.DataRetrievalInput<RoutingProductPreviewQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new RoutingProductPreviewRequestHandler());
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

        private class CustomerRatePreviewRequestHandler : BigDataRequestHandler<CustomerRatePreviewQuery, CustomerRatePreview, CustomerRatePreviewDetail>
        {


            public override CustomerRatePreviewDetail EntityDetailMapper(CustomerRatePreview entity)
            {
                 RoutingProductManager routingProductManager = new RoutingProductManager();
            CurrencyManager currencyManager = new CurrencyManager();
                CarrierAccountManager carrierAccountManager =  new CarrierAccountManager();
                var entityDetail = new CustomerRatePreviewDetail()
                {
                      ZoneName = entity.ZoneName,
                      Rate = entity.Rate, 
                      BED = entity.BED,
                      EED = entity.EED,
                      ChangeType = entity.ChangeType ,
                      ServicesId = !entity.ZoneId.HasValue
                    ? routingProductManager.GetDefaultServiceIds(entity.RoutingProductId)
                    : routingProductManager.GetZoneServiceIds(entity.RoutingProductId,
                        entity.ZoneId.Value),
                      CustomerName = carrierAccountManager.GetCarrierAccountName(entity.CustomerId)
                    
                };
                 if (entity.CurrencyId.HasValue)
                entityDetail.CurrencySymbol = currencyManager.GetCurrencySymbol(entity.CurrencyId.Value);
                return entityDetail;
            }

            public override IEnumerable<CustomerRatePreview> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<CustomerRatePreviewQuery> input)
            {
                var dataManager = BEDataManagerFactory.GetDataManager<ISalePriceListChangeDataManager>();
                return dataManager.GetCustomerRatePreviews(input.Query);
            }
        }

        private class RoutingProductPreviewRequestHandler : BigDataRequestHandler<RoutingProductPreviewQuery, RoutingProductPreview, RoutingProductPreviewDetail>
        {


            public override RoutingProductPreviewDetail EntityDetailMapper(RoutingProductPreview entity)
            {
                RoutingProductManager routingProductManager = new RoutingProductManager();
                CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                var entityDetail = new RoutingProductPreviewDetail()
                {
                    ZoneName = entity.ZoneName,
                    BED = entity.BED,
                    EED = entity.EED,
                    RoutingProductName = routingProductManager.GetRoutingProductName(entity.RoutingProductId),
                     CustomerName = carrierAccountManager.GetCarrierAccountName(entity.CustomerId),
                    RoutingProductServicesId = !entity.ZoneId.HasValue
                        ? routingProductManager.GetDefaultServiceIds(entity.RoutingProductId)
                        : routingProductManager.GetZoneServiceIds(entity.RoutingProductId,
                            entity.ZoneId.Value)
                };
                if (entity.RecentRoutingProductId.HasValue)
                {
                    int recentRoutingProductId = entity.RecentRoutingProductId.Value;
                    entityDetail.RecentRoutingProductName = routingProductManager.GetRoutingProductName(recentRoutingProductId);

                    entityDetail.RecentRouringProductServicesId = !entity.ZoneId.HasValue
                        ? routingProductManager.GetDefaultServiceIds(recentRoutingProductId)
                        : routingProductManager.GetZoneServiceIds(recentRoutingProductId, entity.ZoneId.Value);
                }
                return entityDetail;
            }

            public override IEnumerable<RoutingProductPreview> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<RoutingProductPreviewQuery> input)
            {
                var dataManager = BEDataManagerFactory.GetDataManager<ISalePriceListChangeDataManager>();
                return dataManager.GetRoutingProductPreviews(input.Query);
            }
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
