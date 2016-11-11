using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SaleEntityServiceReadAllNoCache : ISaleEntityServiceReader
    {
        #region ctor/Local Variables
        ISaleEntityServiceDataManager _saleEntityServiceDataManager;
        SalePriceListManager _salePriceListManager;
        SaleEntityZoneServicesByOwner _allSaleEntityZoneServicesByOwner;
        SaleEntityDefaultServicesByOwner _defaultSaleEntityServicesByOwner;
        #endregion


        #region Public Methods
        public SaleEntityServiceReadAllNoCache(IEnumerable<RoutingCustomerInfoDetails> customerInfoDetails, DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            _saleEntityServiceDataManager = BEDataManagerFactory.GetDataManager<ISaleEntityServiceDataManager>();
            _salePriceListManager = new SalePriceListManager();
            GetAllSaleEntityServicesByOwner(customerInfoDetails, effectiveOn, isEffectiveInFuture, out _allSaleEntityZoneServicesByOwner, out _defaultSaleEntityServicesByOwner);
        }

        public SaleEntityZoneServicesByZone GetSaleEntityZoneServicesByZone(SalePriceListOwnerType ownerType, int ownerId)
        {
            if (_allSaleEntityZoneServicesByOwner == null)
                return null;

            var saleEntityZoneServicesByOwnerType = ownerType == SalePriceListOwnerType.Customer ? _allSaleEntityZoneServicesByOwner.SaleEntityZoneServicesByCustomer : _allSaleEntityZoneServicesByOwner.SaleEntityZoneServicesByProduct;

            if (saleEntityZoneServicesByOwnerType == null)
                return null;

            return saleEntityZoneServicesByOwnerType.GetRecord(ownerId);
        }

        public SaleEntityDefaultService GetSaleEntityDefaultService(BusinessEntity.Entities.SalePriceListOwnerType ownerType, int ownerId)
        {
            if (_defaultSaleEntityServicesByOwner == null)
                return null;

            var saleEntityServicesByOwnerType = ownerType == SalePriceListOwnerType.Customer ? _defaultSaleEntityServicesByOwner.DefaultServicesByCustomer : _defaultSaleEntityServicesByOwner.DefaultServicesByProduct;

            if (saleEntityServicesByOwnerType == null)
                return null;

            return saleEntityServicesByOwnerType.GetRecord(ownerId);
        }
        #endregion


        #region Private Methods
        private void GetAllSaleEntityServicesByOwner(IEnumerable<RoutingCustomerInfoDetails> customerInfoDetails, DateTime? effectiveOn, bool isEffectiveInFuture,
                                                     out SaleEntityZoneServicesByOwner saleEntityZoneServicesByOwner, out SaleEntityDefaultServicesByOwner saleEntityDefaultServicesByOwner)
        {
            saleEntityZoneServicesByOwner = new SaleEntityZoneServicesByOwner();
            saleEntityZoneServicesByOwner.SaleEntityZoneServicesByCustomer = new Dictionary<int, SaleEntityZoneServicesByZone>();
            saleEntityZoneServicesByOwner.SaleEntityZoneServicesByProduct = new Dictionary<int, SaleEntityZoneServicesByZone>();

            saleEntityDefaultServicesByOwner = new SaleEntityDefaultServicesByOwner();
            saleEntityDefaultServicesByOwner.DefaultServicesByCustomer = new Dictionary<int, SaleEntityDefaultService>();
            saleEntityDefaultServicesByOwner.DefaultServicesByProduct = new Dictionary<int, SaleEntityDefaultService>();

            SaleEntityZoneServicesByZone saleEntityZoneServicesByZone;
            SaleEntityDefaultService saleEntityDefaultServices;

            IEnumerable<SaleEntityZoneService> saleEntityZoneServices = _saleEntityServiceDataManager.GetEffectiveSaleEntityZoneServicesByOwner(customerInfoDetails, effectiveOn, isEffectiveInFuture);

            foreach (SaleEntityZoneService saleEntityZoneService in saleEntityZoneServices)
            {
                SalePriceList priceList = _salePriceListManager.GetPriceList(saleEntityZoneService.PriceListId);

                if (saleEntityZoneService.ZoneId != default(long))
                {
                    Dictionary<int, SaleEntityZoneServicesByZone> saleEntityZoneServicesByOwnerType = priceList.OwnerType == SalePriceListOwnerType.Customer ? saleEntityZoneServicesByOwner.SaleEntityZoneServicesByCustomer : saleEntityZoneServicesByOwner.SaleEntityZoneServicesByProduct;

                    saleEntityZoneServicesByZone = saleEntityZoneServicesByOwnerType.GetOrCreateItem(priceList.OwnerId);

                    if (!saleEntityZoneServicesByZone.ContainsKey(saleEntityZoneService.ZoneId))
                        saleEntityZoneServicesByZone.Add(saleEntityZoneService.ZoneId, saleEntityZoneService);
                }
                else
                {
                    Dictionary<int, SaleEntityDefaultService> saleEntityDefaultServicesByOwnerType = priceList.OwnerType == SalePriceListOwnerType.Customer ? saleEntityDefaultServicesByOwner.DefaultServicesByCustomer : saleEntityDefaultServicesByOwner.DefaultServicesByProduct;

                    if (!saleEntityDefaultServicesByOwnerType.ContainsKey(priceList.OwnerId))
                    {
                        saleEntityDefaultServices = new SaleEntityDefaultService()
                        {
                            BED = saleEntityZoneService.BED,
                            EED = saleEntityZoneService.EED,
                            PriceListId = saleEntityZoneService.PriceListId,
                            SaleEntityServiceId = saleEntityZoneService.SaleEntityServiceId,
                            Services = saleEntityZoneService.Services
                        };
                        saleEntityDefaultServicesByOwnerType.Add(priceList.OwnerId, saleEntityDefaultServices);
                    }
                }
            }
        }
        #endregion
    }
}
