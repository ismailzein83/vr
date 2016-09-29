﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SaleEntityServiceLocator
    {
        private ISaleEntityServiceReader _reader;

        public SaleEntityServiceLocator(ISaleEntityServiceReader reader)
        {
            _reader = reader;
        }

        public SaleEntityService GetSellingProductDefaultService(int sellingProductId)
        {
            SaleEntityService saleEntityService;
            HasDefaultService(SalePriceListOwnerType.SellingProduct, sellingProductId, out saleEntityService);
            return saleEntityService;
        }
        public SaleEntityService GetSellingProductZoneService(int sellingProductId, long zoneId)
        {
            SaleEntityService saleEntityService;
            if (!HasZoneService(SalePriceListOwnerType.SellingProduct, sellingProductId, zoneId, out saleEntityService))
                HasDefaultService(SalePriceListOwnerType.SellingProduct, sellingProductId, out saleEntityService);
            return saleEntityService;
        }

        public SaleEntityService GetCustomerDefaultService(int customerId, int sellingProductId)
        {
            SaleEntityService saleEntityService;
            if (!HasDefaultService(SalePriceListOwnerType.Customer, customerId, out saleEntityService))
                HasDefaultService(SalePriceListOwnerType.SellingProduct, sellingProductId, out saleEntityService);
            return saleEntityService;
        }
        public SaleEntityService GetCustomerZoneService(int customerId, int sellingProductId, long zoneId)
        {
            SaleEntityService saleEntityService;
            if (!HasZoneService(SalePriceListOwnerType.Customer, customerId, zoneId, out saleEntityService))
                if (!HasDefaultService(SalePriceListOwnerType.Customer, customerId, out saleEntityService))
                    if (!HasZoneService(SalePriceListOwnerType.SellingProduct, sellingProductId, zoneId, out saleEntityService))
                        HasDefaultService(SalePriceListOwnerType.SellingProduct, sellingProductId, out saleEntityService);
            return saleEntityService;
        }
        
        public SaleEntityService GetRoutingCustomerZoneService(int customerId, int sellingProductId, long zoneId)
        {
            SaleEntityService saleEntityService = GetCustomerZoneService(customerId, sellingProductId, zoneId);

            if (saleEntityService == null)
                throw new NullReferenceException(string.Format("customerService. Customer ID: {0} having Selling Product ID: {1} does not contain default services.", customerId, sellingProductId));

            if (saleEntityService.Services == null)
                throw new NullReferenceException(string.Format("customerService.Services. Customer ID: {0} having Selling Product ID: {1} does not contain default services.", customerId, sellingProductId));

            if (saleEntityService.Services.Count == 0)
                throw new Exception(string.Format("customerService.Services count is 0. Customer ID: {0} having Selling Product ID: {1} does not contain default services.", customerId, sellingProductId));

            return saleEntityService;
        }

        private bool HasDefaultService(SalePriceListOwnerType ownerType, int ownerId, out SaleEntityService saleEntityService)
        {
            SaleEntityDefaultService defaultService = _reader.GetSaleEntityDefaultService(ownerType, ownerId);

            if (defaultService != null)
            {
                saleEntityService = new SaleEntityService()
                {
                    Services = defaultService.Services,
                    Source = ownerType == SalePriceListOwnerType.SellingProduct ? SaleEntityServiceSource.ProductDefault : SaleEntityServiceSource.CustomerDefault,
                    BED = defaultService.BED,
                    EED = defaultService.EED
                };
                return true;
            }

            saleEntityService = null;
            return false;
        }
        private bool HasZoneService(SalePriceListOwnerType ownerType, int ownerId, long zoneId, out SaleEntityService saleEntityService)
        {
            SaleEntityZoneServicesByZone zoneServicesByZone = _reader.GetSaleEntityZoneServicesByZone(ownerType, ownerId);

            if (zoneServicesByZone != null)
            {
                SaleEntityZoneService zoneService;
                if (zoneServicesByZone.TryGetValue(zoneId, out zoneService))
                {
                    saleEntityService = new SaleEntityService()
                    {
                        Services = zoneService.Services,
                        Source = ownerType == SalePriceListOwnerType.SellingProduct ? SaleEntityServiceSource.ProductZone : SaleEntityServiceSource.CustomerZone,
                        BED = zoneService.BED,
                        EED = zoneService.EED
                    };
                    return true;
                }
            }

            saleEntityService = null;
            return false;
        }
    }
}
