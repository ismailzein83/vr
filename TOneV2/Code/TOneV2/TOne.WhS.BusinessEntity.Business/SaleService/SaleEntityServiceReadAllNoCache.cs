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
    public class SaleEntityServiceReadAllNoCache : ISaleEntityServiceReader
    {
        #region ctor/Local Variables
        ISaleEntityServiceDataManager _saleEntityServiceDataManager;
        SalePriceListManager _salePriceListManager;
        SaleEntityZoneServicesByOwner _allSaleEntityZoneServicesByOwner;
        #endregion


        #region Public Methods
        public SaleEntityServiceReadAllNoCache(IEnumerable<RoutingCustomerInfoDetails> customerInfos, DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            _saleEntityServiceDataManager = BEDataManagerFactory.GetDataManager<ISaleEntityServiceDataManager>();
            _salePriceListManager = new SalePriceListManager();
            _allSaleEntityZoneServicesByOwner = GetAllSaleEntityZoneServicesByOwner(customerInfos, effectiveOn, isEffectiveInFuture);
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
            //var defaultServicesByOwner = new SaleEntityDefaultServicesByOwner();
            //defaultServicesByOwner.DefaultServicesByProduct = new Dictionary<int, SaleEntityDefaultService>();
            //defaultServicesByOwner.DefaultServicesByCustomer = new Dictionary<int, SaleEntityDefaultService>();

            SaleEntityDefaultService _defaultService = null;

            //var dataManager = BEDataManagerFactory.GetDataManager<ISaleEntityServiceDataManager>();
            //IEnumerable<SaleEntityDefaultService> defaultServices = dataManager.GetEffectiveSaleEntityDefaultServices(_effectiveOn);

            //if (defaultServices != null)
            //{
            //    var salePriceListManager = new SalePriceListManager();

            //    foreach (SaleEntityDefaultService defaultService in defaultServices)
            //    {
            //        SalePriceList priceList = salePriceListManager.GetPriceList(defaultService.PriceListId);
            //        if (priceList == null)
            //            throw new NullReferenceException("priceList");

            //        if (priceList.OwnerType == SalePriceListOwnerType.SellingProduct)
            //        {
            //            if (!defaultServicesByOwner.DefaultServicesByProduct.ContainsKey(priceList.OwnerId))
            //                defaultServicesByOwner.DefaultServicesByProduct.Add(priceList.OwnerId, defaultService);
            //        }
            //        else
            //        {
            //            if (!defaultServicesByOwner.DefaultServicesByCustomer.ContainsKey(priceList.OwnerId))
            //                defaultServicesByOwner.DefaultServicesByCustomer.Add(priceList.OwnerId, defaultService);
            //        }
            //    }
            //}

            //Dictionary<int, SaleEntityDefaultService> defaultServicesByTargetOwner = ownerType == SalePriceListOwnerType.SellingProduct ?
            //        defaultServicesByOwner.DefaultServicesByProduct :
            //        defaultServicesByOwner.DefaultServicesByCustomer;

            //defaultServicesByTargetOwner.TryGetValue(ownerId, out _defaultService);

            return _defaultService;
        }
        #endregion


        #region Private Methods
        private SaleEntityZoneServicesByOwner GetAllSaleEntityZoneServicesByOwner(IEnumerable<RoutingCustomerInfoDetails> customerInfos, DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            SaleEntityZoneServicesByOwner result = new SaleEntityZoneServicesByOwner();
            result.SaleEntityZoneServicesByCustomer = new Dictionary<int, SaleEntityZoneServicesByZone>();
            result.SaleEntityZoneServicesByProduct = new Dictionary<int, SaleEntityZoneServicesByZone>();
            SaleEntityZoneServicesByZone saleEntityZoneServicesByZone;

            IEnumerable<SaleEntityZoneService> saleEntityZoneServices = _saleEntityServiceDataManager.GetEffectiveSaleEntityZoneServicesByOwner(customerInfos, effectiveOn, isEffectiveInFuture);
            SaleEntityZoneService tempSaleEntityZoneService;

            foreach (SaleEntityZoneService saleEntityZoneService in saleEntityZoneServices)
            {
                SalePriceList priceList = _salePriceListManager.GetPriceList(saleEntityZoneService.PriceListId);
                Dictionary<int, SaleEntityZoneServicesByZone> SaleEntityZoneServicesByOwner = priceList.OwnerType == SalePriceListOwnerType.Customer ? result.SaleEntityZoneServicesByCustomer : result.SaleEntityZoneServicesByProduct;

                if (!SaleEntityZoneServicesByOwner.TryGetValue(priceList.OwnerId, out saleEntityZoneServicesByZone))
                {
                    saleEntityZoneServicesByZone = new SaleEntityZoneServicesByZone();
                    SaleEntityZoneServicesByOwner.Add(priceList.OwnerId, saleEntityZoneServicesByZone);
                }

                if (!saleEntityZoneServicesByZone.TryGetValue(saleEntityZoneService.ZoneId, out tempSaleEntityZoneService))
                {
                    saleEntityZoneServicesByZone.Add(saleEntityZoneService.ZoneId, saleEntityZoneService);
                }
            }
            return result;
        }
        #endregion
    } 
}
