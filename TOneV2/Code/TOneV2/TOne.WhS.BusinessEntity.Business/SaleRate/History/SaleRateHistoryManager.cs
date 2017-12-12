using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SaleRateHistoryManager
    {
        public Vanrise.Entities.IDataRetrievalResult<SaleRateHistoryRecordDetail> GetFilteredSaleRateHistoryRecords(Vanrise.Entities.DataRetrievalInput<SaleRateHistoryQuery> input)
        {

            return Vanrise.Common.Business.BigDataManager.Instance.RetrieveData(input, new SaleRateHistoryRequestHandler());
        }

        private class SaleRateHistoryRequestHandler : Vanrise.Common.Business.BigDataRequestHandler<SaleRateHistoryQuery, SaleRateHistoryRecord, SaleRateHistoryRecordDetail>
        {
            #region Fields / Constructors
            private SellingProductManager _sellingProductManager;
            private CurrencyManager _currencyManager;
            private Vanrise.Common.Business.ConfigManager _configManager;

            public SaleRateHistoryRequestHandler()
            {
                _sellingProductManager = new SellingProductManager();
                _currencyManager = new CurrencyManager();
                _configManager = new Vanrise.Common.Business.ConfigManager();
            }

            #endregion

            public override IEnumerable<SaleRateHistoryRecord> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<SaleRateHistoryQuery> input)
            {
                IEnumerable<long> zoneIds = GetZoneIds(input.Query.OwnerType, input.Query.OwnerId, input.Query.SellingNumberPlanId, input.Query.CountryId, input.Query.ZoneName);
                int currencyId = (input.Query.IsSystemCurrency) ? _configManager.GetSystemCurrencyId() : input.Query.CurrencyId;
                if (input.Query.OwnerType == SalePriceListOwnerType.SellingProduct)
                {
                    var spZoneRateHistoryLocator = new SellingProductZoneRateHistoryLocator(new SellingProductZoneRateHistoryReader(input.Query.OwnerId, zoneIds, true, false));
                    return spZoneRateHistoryLocator.GetSaleRateHistory(input.Query.ZoneName, null, currencyId);
                }
                else
                {
                    int sellingProductId = new CarrierAccountManager().GetSellingProductId(input.Query.OwnerId);
                    var customerZoneRateHistoryLocator = new CustomerZoneRateHistoryLocatorV2(new CustomerZoneRateHistoryReaderV2(CreateListFromItem(input.Query.OwnerId), CreateListFromItem(sellingProductId), zoneIds));

                    int longPrecision = new Vanrise.Common.Business.GeneralSettingsManager().GetLongPrecisionValue();
                    return customerZoneRateHistoryLocator.GetCustomerZoneRateHistory(input.Query.OwnerId, sellingProductId, input.Query.ZoneName, input.Query.CountryId, input.Query.CurrencyId, longPrecision);
                    //GetSaleRateHistory(input.Query.ZoneName, input.Query.CountryId, null, currencyId);
                }
            }

            protected override BigResult<SaleRateHistoryRecordDetail> AllRecordsToBigResult(DataRetrievalInput<SaleRateHistoryQuery> input, IEnumerable<SaleRateHistoryRecord> allRecords)
            {
                int? systemCurrencyId = (input.Query.IsSystemCurrency) ? (int?)_configManager.GetSystemCurrencyId() : null;
                return allRecords.ToBigResult(input, null, (entity) => SaleRateHistoryEntityDetailMapper(entity, systemCurrencyId));
            }

            private SaleRateHistoryRecordDetail SaleRateHistoryEntityDetailMapper(SaleRateHistoryRecord entity, int? systemCurrencyId)
            {
                int currencyValueId = systemCurrencyId.HasValue ? systemCurrencyId.Value : entity.CurrencyId;
                return new SaleRateHistoryRecordDetail
                {
                    Entity = entity,
                    DisplayedCurrency = _currencyManager.GetCurrencySymbol(currencyValueId),
                    DisplayedRate = (systemCurrencyId.HasValue) ? entity.ConvertedRate : entity.Rate,
                    SellingProductName = (entity.SellingProductId.HasValue) ? _sellingProductManager.GetSellingProductName(entity.SellingProductId.Value) : null
                };
            }
            public override SaleRateHistoryRecordDetail EntityDetailMapper(SaleRateHistoryRecord entity)
            {
                throw new NotImplementedException();
            }

            #region Private Methods

            private IEnumerable<long> GetZoneIds(SalePriceListOwnerType ownerType, int ownerId, int? sellingNumberPlanId, int countryId, string zoneName)
            {
                int ownerSellingNumberPlanId = (sellingNumberPlanId.HasValue) ? sellingNumberPlanId.Value : GetOwnerSellingNumberPlanId(ownerType, ownerId);
                IEnumerable<long> zoneIds = new SaleZoneManager().GetSaleZoneIdsBySaleZoneName(ownerSellingNumberPlanId, countryId, zoneName);

                if (zoneIds == null || zoneIds.Count() == 0)
                    throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("SaleZoneIds of Zone '{0}' were not found", zoneName));

                return zoneIds;
            }

            private int GetOwnerSellingNumberPlanId(SalePriceListOwnerType ownerType, int ownerId)
            {
                if (ownerType == SalePriceListOwnerType.SellingProduct)
                {
                    int? sellingNumberPlanId = new SellingProductManager().GetSellingNumberPlanId(ownerId);
                    if (!sellingNumberPlanId.HasValue)
                        throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("SellingNumberPlanId of SellingProduct '{0}' was not found", ownerId));
                    return sellingNumberPlanId.Value;
                }
                else
                {
                    return new CarrierAccountManager().GetSellingNumberPlanId(ownerId, CarrierAccountType.Customer);
                }
            }

            private List<T> CreateListFromItem<T>(T item)
            {
                return new List<T>() { item };
            }

            #endregion
        }
    }
}
