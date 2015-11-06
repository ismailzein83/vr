using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Data;
using TOne.WhS.Sales.Entities.Queries;
using TOne.WhS.Sales.Entities.RatePlanning;
using TOne.WhS.Sales.Entities.RatePlanning.Input;
using Vanrise.Common;

namespace TOne.WhS.Sales.Business
{
    public class RatePlanManager
    {
        #region Get Zone Letters

        public IEnumerable<char> GetZoneLetters(RatePlanOwnerType ownerType, int ownerId)
        {
            IEnumerable<char> zoneLetters = null;

            if (ownerType == RatePlanOwnerType.SellingProduct)
            {
                IEnumerable<SaleZone> saleZones = this.GetSellingProductSaleZones(ownerId, DateTime.Now);

                if (saleZones != null)
                    zoneLetters = saleZones.MapRecords(z => z.Name[0], z => z.Name != null && z.Name.Length > 0).Distinct().OrderBy(l => l);
            }
            else if (ownerType == RatePlanOwnerType.Customer)
            {
                CustomerZoneManager customerZoneManager = new CustomerZoneManager();
                zoneLetters = customerZoneManager.GetCustomerZoneLetters(ownerId);
            }

            return zoneLetters;
        }
        
        #endregion

        #region Get Zone Items

        public IEnumerable<ZoneItem> GetZoneItems(ZoneItemInput input)
        {
            IEnumerable<ZoneItem> zoneItems = null;
            IEnumerable<SaleZone> saleZones = this.GetSaleZones(input.Filter.OwnerType, input.Filter.OwnerId);

            if (saleZones != null)
            {
                saleZones = this.GetFilteredSaleZones(saleZones, input.Filter);

                if (saleZones != null)
                {
                    saleZones = this.GetPagedSaleZones(saleZones, input.FromRow, input.ToRow);

                    if (saleZones != null)
                    {
                        IEnumerable<SaleRate> saleRates = this.GetSaleRates(input.Filter.OwnerType, input.Filter.OwnerId, saleZones, DateTime.Now);
                        zoneItems = this.BuildZoneItems(saleZones, saleRates);
                    }
                }
            }

            return zoneItems;
        }

        private IEnumerable<SaleZone> GetSaleZones(RatePlanOwnerType ownerType, int ownerId)
        {
            IEnumerable<SaleZone> saleZones = null;

            if (ownerType == RatePlanOwnerType.SellingProduct)
            {
                saleZones = this.GetSellingProductSaleZones(ownerId, DateTime.Now);
            }
            else if (ownerType == RatePlanOwnerType.Customer)
            {
                CustomerZoneManager manager = new CustomerZoneManager();
                saleZones = manager.GetCustomerSaleZones(ownerId, DateTime.Now, false);
            }

            return saleZones;
        }

        private IEnumerable<SaleZone> GetSellingProductSaleZones(int sellingProductId, DateTime effectiveOn)
        {
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            int sellingNumberPlanId = carrierAccountManager.GetSellingNumberPlanId(sellingProductId, CarrierAccountType.Customer);

            SaleZoneManager saleZoneManager = new SaleZoneManager();
            return saleZoneManager.GetSaleZones(sellingNumberPlanId, effectiveOn);
        }

        private IEnumerable<SaleZone> GetFilteredSaleZones(IEnumerable<SaleZone> saleZones, ZoneItemFilter filter)
        {
            return saleZones.FindAllRecords(z => z.Name != null && z.Name.Length > 0 && char.ToLower(z.Name.ElementAt(0)) == char.ToLower(filter.ZoneLetter));
        }

        private IEnumerable<SaleZone> GetPagedSaleZones(IEnumerable<SaleZone> saleZones, int fromRow, int toRow)
        {
            List<SaleZone> pagedSaleZones = null;

            if (saleZones.Count() >= fromRow)
            {
                pagedSaleZones = new List<SaleZone>();

                for (int i = fromRow - 1; i < toRow && i < saleZones.Count(); i++)
                {
                    pagedSaleZones.Add(saleZones.ElementAt(i));
                }
            }

            return pagedSaleZones;
        }

        private IEnumerable<SaleRate> GetSaleRates(RatePlanOwnerType ownerType, int ownerId, IEnumerable<SaleZone> saleZones, DateTime effectiveOn)
        {
            List<SaleRate> saleRates = null;
            SaleEntityZoneRateLocator zoneRateLocator = new SaleEntityZoneRateLocator(new SaleRateReadWithCache(effectiveOn));
            
            if (ownerType == RatePlanOwnerType.SellingProduct)
            {
                foreach (SaleZone saleZone in saleZones)
                {
                    SaleEntityZoneRate zoneRate = zoneRateLocator.GetSellingProductZoneRate(ownerId, saleZone.SaleZoneId);
                    
                    if (zoneRate != null)
                        saleRates.Add(zoneRate.Rate);
                }
            }
            else if (ownerType == RatePlanOwnerType.Customer)
            {
                CustomerSellingProductManager customerSellingProductManager = new CustomerSellingProductManager();
                CustomerSellingProduct customerSellingProduct = customerSellingProductManager.GetEffectiveSellingProduct(ownerId, DateTime.Now, false);

                foreach (SaleZone saleZone in saleZones)
                {
                    SaleEntityZoneRate zoneRate = zoneRateLocator.GetCustomerZoneRate(ownerId, customerSellingProduct.SellingProductId, saleZone.SaleZoneId);

                    if (zoneRate != null)
                        saleRates.Add(zoneRate.Rate);
                }
            }

            return saleRates;
        }

        private IEnumerable<ZoneItem> BuildZoneItems(IEnumerable<SaleZone> saleZones, IEnumerable<SaleRate> saleRates)
        {
            List<ZoneItem> zoneItems = new List<ZoneItem>();

            foreach (SaleZone saleZone in saleZones)
            {
                ZoneItem zoneItem = new ZoneItem();

                zoneItem.ZoneId = saleZone.SaleZoneId;
                zoneItem.ZoneName = saleZone.Name;

                SaleRate rate = saleRates.FindRecord(r => r.ZoneId == saleZone.SaleZoneId);

                if (rate != null)
                {
                    zoneItem.CurrentRateId = rate.SaleRateId;
                    zoneItem.CurrentRate = rate.NormalRate;
                    zoneItem.RateBED = rate.BeginEffectiveDate;
                    zoneItem.RateEED = rate.EndEffectiveDate;
                }

                zoneItems.Add(zoneItem);
            }

            return zoneItems;
        }
        
        #endregion

        #region Save Price List

        public void SavePriceList(SalePriceListInput input)
        {
            IRatePlanDataManager dataManager = SalesDataManagerFactory.GetDataManager<IRatePlanDataManager>();
            //dataManager.SetRatePlanStatusIfExists(RatePlanOwnerType.Customer, input.CustomerId, RatePlanStatus.Completed);

            int salePriceListId = CreateSalePriceList(input.CustomerId);

            foreach (SaleRate saleRate in input.NewSaleRates)
            {
                saleRate.PriceListId = salePriceListId;
            }

            dataManager.CloseAndInsertSaleRates(input.CustomerId, input.NewSaleRates);
        }

        private int CreateSalePriceList(int customerId)
        {
            int salePriceListId;

            SalePriceList salePriceList = new SalePriceList()
            {
                OwnerType = SalePriceListOwnerType.Customer,
                OwnerId = customerId
            };

            IRatePlanDataManager dataManager = SalesDataManagerFactory.GetDataManager<IRatePlanDataManager>();
            bool added = dataManager.InsertSalePriceList(salePriceList, out salePriceListId);

            return salePriceListId;
        }
        
        #endregion

        #region Junk Code
        /*
        public RatePlan GetRatePlan(RatePlanOwnerType ownerType, int ownerId, RatePlanStatus status)
        {
            IRatePlanDataManager dataManager = SalesDataManagerFactory.GetDataManager<IRatePlanDataManager>();
            return dataManager.GetRatePlan(ownerType, ownerId, status);
        }

        public bool SaveRatePlanDraft(RatePlan draft)
        {
            IRatePlanDataManager dataManager = SalesDataManagerFactory.GetDataManager<IRatePlanDataManager>();
            return dataManager.InsertOrUpdateRatePlan(draft);
        }
        */
        #endregion
    }
}
