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
                if (input.Query.OwnerType == SalePriceListOwnerType.SellingProduct)
                    return GetProductZoneRoutingProductHistory(input.Query.OwnerId, input.Query.SellingNumberPlanId, input.Query.ZoneName, input.Query.CountryId);
                else
                    return GetCustomerZoneRoutingProductHistory(input.Query.OwnerId, input.Query.SellingNumberPlanId, input.Query.ZoneName, input.Query.CountryId);
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

            #region Private Methods
            private IEnumerable<SaleEntityZoneRoutingProductHistoryRecord> GetProductZoneRoutingProductHistory(int sellingProductId, int? sellingNumberPlanId, string zoneName, int countryId)
            {
                if (!sellingNumberPlanId.HasValue)
                {
                    int? productSellingNumberPlanId = new SellingProductManager().GetSellingNumberPlanId(sellingProductId);
                    if (!productSellingNumberPlanId.HasValue)
                        throw new NullReferenceException("productSellingNumberPlanId");
                    sellingNumberPlanId = productSellingNumberPlanId;
                }

                IEnumerable<long> zoneIds = GetZoneIds(SalePriceListOwnerType.SellingProduct, sellingProductId, sellingNumberPlanId.Value, zoneName, countryId);
                var productZoneRoutingProductHistoryLocator = new ProductZoneRoutingProductHistoryLocator(new ProductZoneRoutingProductHistoryReader(CreateListFromItem(sellingProductId), zoneIds));

                return productZoneRoutingProductHistoryLocator.GetProductZoneRoutingProductHistory(sellingProductId, zoneName);
            }
            private IEnumerable<SaleEntityZoneRoutingProductHistoryRecord> GetCustomerZoneRoutingProductHistory(int customerId, int? sellingNumberPlanId, string zoneName, int countryId)
            {
                if (!sellingNumberPlanId.HasValue)
                    sellingNumberPlanId = new CarrierAccountManager().GetSellingNumberPlanId(customerId, CarrierAccountType.Customer);

                int sellingProductId = new CarrierAccountManager().GetSellingProductId(customerId);
                IEnumerable<long> zoneIds = GetZoneIds(SalePriceListOwnerType.SellingProduct, sellingProductId, sellingNumberPlanId.Value, zoneName, countryId);
                var customerZoneRoutingProductHistoryLocator = new CustomerZoneRoutingProductHistoryLocator(new CustomerZoneRoutingProductHistoryReader(CreateListFromItem(customerId), CreateListFromItem(sellingProductId), zoneIds));

                return customerZoneRoutingProductHistoryLocator.GetCustomerZoneRoutingProductHistory(customerId, sellingProductId, zoneName, countryId);
            }
            private IEnumerable<long> GetZoneIds(SalePriceListOwnerType ownerType, int ownerId, int sellingNumberPlanId, string zoneName, int countryId)
            {
                IEnumerable<long> zoneIds = new SaleZoneManager().GetSaleZoneIdsBySaleZoneName(sellingNumberPlanId, countryId, zoneName);
                zoneIds.ThrowIfNull("zoneIds");
                return zoneIds;
            }
            private List<T> CreateListFromItem<T>(T item)
            {
                return new List<T>() { item };
            }
            #endregion
        }

        #endregion
    }
}
