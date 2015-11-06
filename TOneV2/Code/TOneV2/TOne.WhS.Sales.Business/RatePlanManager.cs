using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Data;
using TOne.WhS.Sales.Entities;
using TOne.WhS.Sales.Entities.Input;
using TOne.WhS.Sales.Entities.Queries;
using TOne.WhS.Sales.Entities.RatePlanning;
using Vanrise.Common;
using Vanrise.Entities;

namespace TOne.WhS.Sales.Business
{
    public class RatePlanManager
    {
        public IEnumerable<ZoneItem> GetZoneItems(ZoneItemInput input)
        {
            IEnumerable<ZoneItem> zoneItems = new List<ZoneItem>();
            return zoneItems;
        }

        private IEnumerable<SaleZone> GetSaleZones(RatePlanOwnerType ownerType, int ownerId)
        {
            IEnumerable<SaleZone> saleZones = null;

            if (ownerType == RatePlanOwnerType.SellingProduct)
            {
                CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                int sellingNumberPlanId = carrierAccountManager.GetSellingNumberPlanId(ownerId, CarrierAccountType.Customer);

                SaleZoneManager saleZoneManager = new SaleZoneManager();
                saleZones = saleZoneManager.GetSaleZones(sellingNumberPlanId, DateTime.Now);
            }
            else if (ownerType == RatePlanOwnerType.Customer)
            {
                CustomerZoneManager manager = new CustomerZoneManager();
                saleZones = manager.GetCustomerSaleZones(ownerId, DateTime.Now, false);
            }

            return saleZones;
        }

        public IEnumerable<RatePlanItem> GetRatePlanItems(RatePlanItemInput input)
        {
            IEnumerable<RatePlanItem> items = null;
            IEnumerable<SaleZone> saleZones = this.GetSaleZones(RatePlanOwnerType.Customer, input.Filter.CustomerId);

            if (saleZones != null)
            {
                saleZones = this.GetFilteredSaleZones(saleZones, input.Filter);

                if (saleZones != null)
                {
                    saleZones = this.GetPagedSaleZones(saleZones, input.FromRow, input.ToRow);

                    if (saleZones != null)
                    {
                        IEnumerable<SaleRate> saleRates = this.GetSaleRates(input.Filter.CustomerId, saleZones);
                        items = this.GetRatePlanItems(saleZones, saleRates);
                    }
                }
            }

            return items;
        }

        private IEnumerable<SaleZone> GetFilteredSaleZones(IEnumerable<SaleZone> saleZones, RatePlanItemFilter filter)
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

        private IEnumerable<SaleRate> GetSaleRates(int customerId, IEnumerable<SaleZone> saleZones)
        {
            SaleRateManager manager = new SaleRateManager();
            IEnumerable<long> saleZoneIds = saleZones.MapRecords(z => z.SaleZoneId);
            return manager.GetSaleRatesByCustomerZoneIds(SalePriceListOwnerType.Customer, customerId, saleZoneIds, DateTime.Now);
        }

        private IEnumerable<RatePlanItem> GetRatePlanItems(IEnumerable<SaleZone> saleZones, IEnumerable<SaleRate> saleRates)
        {
            List<RatePlanItem> items = new List<RatePlanItem>();

            foreach (SaleZone saleZone in saleZones)
            {
                RatePlanItem item = new RatePlanItem();

                item.ZoneId = saleZone.SaleZoneId;
                item.ZoneName = saleZone.Name;

                SaleRate rate = saleRates.FindRecord(r => r.ZoneId == saleZone.SaleZoneId);

                if (rate != null)
                {
                    item.SaleRateId = rate.SaleRateId;
                    item.Rate = rate.NormalRate;
                    item.BeginEffectiveDate = rate.BeginEffectiveDate;
                    item.EndEffectiveDate = rate.EndEffectiveDate;
                }

                items.Add(item);
            }

            return items;
        }

        public void SavePriceList(SalePriceListInput input)
        {
            IRatePlanDataManager dataManager = SalesDataManagerFactory.GetDataManager<IRatePlanDataManager>();
            dataManager.SetRatePlanStatusIfExists(RatePlanOwnerType.Customer, input.CustomerId, RatePlanStatus.Completed);

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

            SalePriceList salePriceList = new SalePriceList() {
                OwnerType = SalePriceListOwnerType.Customer,
                OwnerId = customerId
            };

            IRatePlanDataManager dataManager = SalesDataManagerFactory.GetDataManager<IRatePlanDataManager>();
            bool added = dataManager.InsertSalePriceList(salePriceList, out salePriceListId);

            return salePriceListId;
        }

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
    }
}
