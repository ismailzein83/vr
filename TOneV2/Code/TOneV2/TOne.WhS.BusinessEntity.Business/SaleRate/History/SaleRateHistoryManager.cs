using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SaleRateHistoryManager
    {
        public Vanrise.Entities.IDataRetrievalResult<SaleRateHistoryRecordDetail> GetFilteredSaleRateHistoryRecords(Vanrise.Entities.DataRetrievalInput<SaleRateHistoryQuery> input)
        {
            var saleRateHistoryRequestHandler = new SaleRateHistoryRequestHandler()
            {
                CurrencyId = input.Query.CurrencyId,
                CurrencySymbol = new Vanrise.Common.Business.CurrencyManager().GetCurrencySymbol(input.Query.CurrencyId)
            };
            return Vanrise.Common.Business.BigDataManager.Instance.RetrieveData(input, saleRateHistoryRequestHandler);
        }

        private class SaleRateHistoryRequestHandler : Vanrise.Common.Business.BigDataRequestHandler<SaleRateHistoryQuery, SaleRateHistoryRecord, SaleRateHistoryRecordDetail>
        {
            #region Fields / Constructors

            private SaleRateManager _saleRateManager;
            private Vanrise.Common.Business.CurrencyExchangeRateManager _currencyExchangeRateManager;
            private SellingProductManager _sellingProductManager;
            private CurrencyManager _currencyManager;

            public SaleRateHistoryRequestHandler()
            {
                _saleRateManager = new SaleRateManager();
                _currencyExchangeRateManager = new Vanrise.Common.Business.CurrencyExchangeRateManager();
                _sellingProductManager = new SellingProductManager();
                _currencyManager = new CurrencyManager();
            }

            #endregion

            public int CurrencyId { get; set; }

            public string CurrencySymbol { get; set; }

            public override IEnumerable<SaleRateHistoryRecord> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<SaleRateHistoryQuery> input)
            {
                IEnumerable<long> zoneIds = GetZoneIds(input.Query.OwnerType, input.Query.OwnerId, input.Query.SellingNumberPlanId, input.Query.CountryId, input.Query.ZoneName);

                if (input.Query.OwnerType == SalePriceListOwnerType.SellingProduct)
                {
                    var spZoneRateHistoryLocator = new SellingProductZoneRateHistoryLocator(new SellingProductZoneRateHistoryReader(input.Query.OwnerId, zoneIds, true, false));
                    return spZoneRateHistoryLocator.GetSaleRateHistory(input.Query.ZoneName, null, CurrencyId);
                }
                else
                {
                    var customerZoneRateHistoryLocator = new CustomerZoneRateHistoryLocator(new CustomerZoneRateHistoryReader(input.Query.OwnerId, zoneIds, true, false));
                    return customerZoneRateHistoryLocator.GetSaleRateHistory(input.Query.ZoneName, input.Query.CountryId, null, CurrencyId);
                }
            }

            public override SaleRateHistoryRecordDetail EntityDetailMapper(SaleRateHistoryRecord entity)
            {
                return new SaleRateHistoryRecordDetail
                {
                    Entity = entity,
                    CurrencySymbol = _currencyManager.GetCurrencySymbol(entity.CurrencyId),
                    ConvertedToCurrencySymbol = CurrencySymbol,
                    SellingProductName = (entity.SellingProductId.HasValue) ? _sellingProductManager.GetSellingProductName(entity.SellingProductId.Value) : null
                };
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

            #endregion
        }
    }
}
