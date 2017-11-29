using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SaleEntityZoneRoutingProductHistoryManager
    {
        #region Public Methods

        public Vanrise.Entities.IDataRetrievalResult<SaleEntityZoneRoutingProductHistoryRecordDetail> GetFilteredSaleEntityZoneRoutingProductHistoryRecords(Vanrise.Entities.DataRetrievalInput<SaleEntityZoneRoutingProductHistoryQuery> input)
        {
            return Vanrise.Common.Business.BigDataManager.Instance.RetrieveData(input, new SaleEntityZoneRoutingProductHistoryRequestHandler());
        }

        #endregion

        #region Private Classes

        private class SaleEntityZoneRoutingProductHistoryRequestHandler : Vanrise.Common.Business.BigDataRequestHandler<SaleEntityZoneRoutingProductHistoryQuery, SaleEntityZoneRoutingProductHistoryRecord, SaleEntityZoneRoutingProductHistoryRecordDetail>
        {
            #region Fields / Constructors

            private IEnumerable<SaleEntityZoneRoutingProductSource> _orderedRPSources;

            private RoutingProductManager _routingProductManager = new RoutingProductManager();
            private SellingProductManager _sellingProductManager = new SellingProductManager();
            private CarrierAccountManager _carrierAccountManager = new CarrierAccountManager();

            public SaleEntityZoneRoutingProductHistoryRequestHandler()
            {
                _orderedRPSources = new List<SaleEntityZoneRoutingProductSource>()
                {
                    SaleEntityZoneRoutingProductSource.ProductDefault,
                    SaleEntityZoneRoutingProductSource.ProductZone,
                    SaleEntityZoneRoutingProductSource.CustomerDefault,
                    SaleEntityZoneRoutingProductSource.CustomerZone,
                };
                _routingProductManager = new RoutingProductManager();
                _sellingProductManager = new SellingProductManager();
                _carrierAccountManager = new CarrierAccountManager();
            }

            #endregion

            public override IEnumerable<SaleEntityZoneRoutingProductHistoryRecord> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<SaleEntityZoneRoutingProductHistoryQuery> input)
            {
                IEnumerable<long> zoneIds = GetZoneIds(input.Query.OwnerType, input.Query.OwnerId, input.Query.SellingNumberPlanId, input.Query.CountryId, input.Query.ZoneName);
                ZoneRoutingProductHistoryLocator locator = new ZoneRoutingProductHistoryLocator(new ZoneRoutingProductHistoryReader(input.Query.OwnerType, input.Query.OwnerId, zoneIds));

                if (input.Query.OwnerType == SalePriceListOwnerType.SellingProduct)
                    return locator.GetSellingProductZoneRoutingProductHistory(input.Query.OwnerId, zoneIds);
                else
                {
                    int sellingProductId = new CarrierAccountManager().GetSellingProductId(input.Query.OwnerId);
                    return locator.GetCustomerZoneRoutingProductHistory(input.Query.OwnerId, sellingProductId, input.Query.CountryId, zoneIds);
                }
            }

            public override SaleEntityZoneRoutingProductHistoryRecordDetail EntityDetailMapper(SaleEntityZoneRoutingProductHistoryRecord entity)
            {
                var detail = new SaleEntityZoneRoutingProductHistoryRecordDetail()
                {
                    Entity = entity,
                    RoutingProductName = _routingProductManager.GetRoutingProductName(entity.RoutingProductId),
                };

                detail.ServiceIds = (entity.SaleZoneId.HasValue) ?
                    _routingProductManager.GetZoneServiceIds(entity.RoutingProductId, entity.SaleZoneId.Value) : _routingProductManager.GetDefaultServiceIds(entity.RoutingProductId);

                if (entity.Source == SaleEntityZoneRoutingProductSource.ProductDefault || entity.Source == SaleEntityZoneRoutingProductSource.ProductZone)
                    detail.SaleEntityName = _sellingProductManager.GetSellingProductName(entity.SaleEntityId);

                return detail;
            }

            #region Common Methods

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

        #endregion
    }
}
