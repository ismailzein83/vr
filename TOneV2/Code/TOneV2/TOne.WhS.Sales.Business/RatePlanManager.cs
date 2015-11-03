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
using TOne.WhS.Sales.Entities.Queries;
using Vanrise.Common;
using Vanrise.Entities;

namespace TOne.WhS.Sales.Business
{
    public class RatePlanManager
    {
        #region Get Rate Plan Items
        public IEnumerable<RatePlanItem> GetRatePlanItems(RatePlanItemInput input)
        {
            List<RatePlanItem> ratePlanItems = new List<RatePlanItem>();

            IEnumerable<SaleZone> saleZones = GetSaleZones(input.Filter.CustomerId);

            if (saleZones != null)
            {
                saleZones =
                    saleZones.Where(x => x.Name == null || (x.Name.Length > 0 && char.ToLower(x.Name[0]) == char.ToLower(input.Filter.ZoneLetter))).ToList();

                saleZones = this.GetPagedSaleZones(saleZones, input.FromRow, input.ToRow);

                List<long> saleZoneIds = saleZones.Select(item => item.SaleZoneId).ToList();
                List<SaleRate> saleRates = GetSaleRatesByCustomerZoneIds(input.Filter.CustomerId, saleZoneIds);

                foreach (SaleZone saleZone in saleZones)
                {
                    RatePlanItem ratePlanItem = new RatePlanItem();

                    ratePlanItem.ZoneId = saleZone.SaleZoneId;
                    ratePlanItem.ZoneName = saleZone.Name;

                    var rate = saleRates.FindRecord(item => item.ZoneId == saleZone.SaleZoneId);

                    if (rate != null)
                    {
                        ratePlanItem.SaleRateId = rate.SaleRateId;
                        ratePlanItem.Rate = rate.NormalRate;
                        ratePlanItem.BeginEffectiveDate = rate.BeginEffectiveDate;
                        ratePlanItem.EndEffectiveDate = rate.EndEffectiveDate;
                    }

                    ratePlanItems.Add(ratePlanItem);
                }
            }

            return ratePlanItems;
        }

        private IEnumerable<SaleZone> GetSaleZones(int customerId)
        {
            IEnumerable<SaleZone> saleZones = null;
            IEnumerable<int> countryIds = new CustomerZoneManager().GetCountryIds(customerId);

            if (countryIds != null)
            {
                int sellingNumberPlanId = new CarrierAccountManager().GetSellingNumberPlanId(customerId, CarrierAccountType.Customer);
                saleZones = new SaleZoneManager().GetSaleZonesByCountryIds(sellingNumberPlanId, countryIds);
            }

            return saleZones;
        }

        private int GetSellingNumberPlanId(int customerId)
        {
            CarrierAccountManager manager = new CarrierAccountManager();
            CarrierAccountDetail customer = manager.GetCarrierAccount(customerId);

            return customer.CustomerSettings.SellingNumberPlanId;
        }

        private IEnumerable<SaleZone> GetPagedSaleZones(IEnumerable<SaleZone> saleZones, int fromRow, int toRow)
        {
            List<SaleZone> pagedSaleZones = new List<SaleZone>();

            for (int i = fromRow - 1; i < saleZones.Count() && i < toRow; i++)
            {
                pagedSaleZones.Add(saleZones.ElementAt(i));
            }

            return pagedSaleZones;
        }

        private List<SaleRate> GetSaleRatesByCustomerZoneIds(int customerId, List<long> customerZoneIds)
        {
            SaleRateManager manager = new SaleRateManager();
            return manager.GetSaleRatesByCustomerZoneIds(SalePriceListOwnerType.Customer, customerId, customerZoneIds, DateTime.Now);
        }

        #endregion

        #region Save Price List

        public void SavePriceList(SalePriceListInput input)
        {
            int salePriceListId = CreateSalePriceList(input.CustomerId);

            foreach (SaleRate saleRate in input.NewSaleRates)
            {
                saleRate.PriceListId = salePriceListId;
            }

            IRatePlanDataManager dataManager = SalesDataManagerFactory.GetDataManager<IRatePlanDataManager>();
            bool succeeded = dataManager.CloseThenInsertSaleRates(input.CustomerId, input.NewSaleRates);
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

        #endregion
    }
}
