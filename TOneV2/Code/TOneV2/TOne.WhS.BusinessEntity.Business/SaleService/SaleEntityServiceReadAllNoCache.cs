using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SaleEntityServiceReadAllNoCache : ISaleEntityServiceReader
    {
        #region ctor/Local Variables
        private DateTime _effectiveOn { get; set; }
        private bool _isEffectiveInFuture { get; set; }


        public SaleEntityServiceReadAllNoCache(IEnumerable<RoutingCustomerInfo> customerInfos, DateTime effectiveOn, bool isEffectiveInFuture)
        {
            _effectiveOn = effectiveOn;
            _isEffectiveInFuture = isEffectiveInFuture; 
        }
        #endregion


        #region Public Methods
        public SaleEntityZoneServicesByZone GetSaleEntityZoneServicesByZone(SalePriceListOwnerType ownerType, int ownerId)
        {
            var zoneServicesByZone = new SaleEntityZoneServicesByZone();

            var dataManager = BEDataManagerFactory.GetDataManager<ISaleEntityServiceDataManager>();
            IEnumerable<SaleEntityZoneService> zoneServices = dataManager.GetEffectiveSaleEntityZoneServices(ownerType, ownerId, _effectiveOn);

            if (zoneServices != null)
            {
                foreach (SaleEntityZoneService zoneService in zoneServices)
                {
                    if (!zoneServicesByZone.ContainsKey(zoneService.ZoneId))
                        zoneServicesByZone.Add(zoneService.ZoneId, zoneService);
                }
            }

            return zoneServicesByZone;
        }

        public SaleEntityDefaultService GetSaleEntityDefaultService(BusinessEntity.Entities.SalePriceListOwnerType ownerType, int ownerId)
        {
            var defaultServicesByOwner = new SaleEntityDefaultServicesByOwner();
            defaultServicesByOwner.DefaultServicesByProduct = new Dictionary<int, SaleEntityDefaultService>();
            defaultServicesByOwner.DefaultServicesByCustomer = new Dictionary<int, SaleEntityDefaultService>();

            SaleEntityDefaultService _defaultService = null;

            var dataManager = BEDataManagerFactory.GetDataManager<ISaleEntityServiceDataManager>();
            IEnumerable<SaleEntityDefaultService> defaultServices = dataManager.GetEffectiveSaleEntityDefaultServices(_effectiveOn);

            if (defaultServices != null)
            {
                var salePriceListManager = new SalePriceListManager();

                foreach (SaleEntityDefaultService defaultService in defaultServices)
                {
                    SalePriceList priceList = salePriceListManager.GetPriceList(defaultService.PriceListId);
                    if (priceList == null)
                        throw new NullReferenceException("priceList");

                    if (priceList.OwnerType == SalePriceListOwnerType.SellingProduct)
                    {
                        if (!defaultServicesByOwner.DefaultServicesByProduct.ContainsKey(priceList.OwnerId))
                            defaultServicesByOwner.DefaultServicesByProduct.Add(priceList.OwnerId, defaultService);
                    }
                    else
                    {
                        if (!defaultServicesByOwner.DefaultServicesByCustomer.ContainsKey(priceList.OwnerId))
                            defaultServicesByOwner.DefaultServicesByCustomer.Add(priceList.OwnerId, defaultService);
                    }
                }
            }

            Dictionary<int, SaleEntityDefaultService> defaultServicesByTargetOwner = ownerType == SalePriceListOwnerType.SellingProduct ?
                    defaultServicesByOwner.DefaultServicesByProduct :
                    defaultServicesByOwner.DefaultServicesByCustomer;

            defaultServicesByTargetOwner.TryGetValue(ownerId, out _defaultService);

            return _defaultService;
        }
        #endregion
    } 
}
