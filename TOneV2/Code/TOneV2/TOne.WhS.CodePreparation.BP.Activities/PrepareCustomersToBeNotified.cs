using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.CodePreparation.Entities;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.Common;

namespace TOne.WhS.CodePreparation.BP.Activities
{
    public sealed class PrepareCustomersToBeNotified : CodeActivity
    {

        [RequiredArgument]
        public InArgument<Dictionary<int, List<SalePLZoneChange>>> ZonesChangesByCountry { get; set; }

        [RequiredArgument]
        public InArgument<int> SellingNumberPlanId { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<CarrierAccountInfo>> CustomersToBeNotified { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            Dictionary<int, List<SalePLZoneChange>> zonesChangesByCountry = this.ZonesChangesByCountry.Get(context);
            int sellingNumberPlanId = this.SellingNumberPlanId.Get(context);

            CustomerZoneManager customerZoneManager = new CustomerZoneManager();
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            IEnumerable<CarrierAccountInfo> customers = carrierAccountManager.GetCustomersBySellingNumberPlanId(sellingNumberPlanId);
            DateTime today = DateTime.Today;

            List<CarrierAccountInfo> customersToBeNotified = new List<CarrierAccountInfo>();
            if (customers != null)
            {
                foreach (CarrierAccountInfo customer in customers)
                {
                    IEnumerable<SaleZone> saleZones = customerZoneManager.GetCustomerSaleZones(customer.CarrierAccountId, sellingNumberPlanId, today, false);
                    if (saleZones == null)
                        continue;

                    IEnumerable<int> customerSoldCountryIds = saleZones.Select(item => item.CountryId).Distinct();

                    if (customerSoldCountryIds.Intersect(zonesChangesByCountry.Keys).Count() > 0)
                        customersToBeNotified.Add(customer);
                }
            }

            CustomersToBeNotified.Set(context, customersToBeNotified);
        }

        #region Private Methods

        private IEnumerable<SalePLZoneChange> GetZoneChanges(IEnumerable<RatePreview> ratesPreview, IEnumerable<ExistingZone> existingZones, IEnumerable<AddedZone> addedZones)
        {
            Dictionary<string, List<RatePreview>> ratesPreviewByZoneName = new Dictionary<string, List<RatePreview>>();

            if (ratesPreview != null)
            {
                IEnumerable<RatePreview> newRatesPreview = ratesPreview.FindAllRecords(item => item.ChangeType == RateChangeType.New);
                ratesPreviewByZoneName = StructureRatesPreviewByZoneName(newRatesPreview);
            }

            List<SalePLZoneChange> zonesChanges = new List<SalePLZoneChange>();
            SaleZoneManager zoneManager = new SaleZoneManager();

            if (existingZones != null)
            {
                foreach (ExistingZone existingZone in existingZones)
                {
                    if (existingZone.AddedCodes.Count > 0 || existingZone.ExistingCodes.Any(item => item.ChangedEntity != null))
                    {

                        SalePLZoneChange zoneChange = new SalePLZoneChange()
                        {
                            ZoneName = existingZone.Name,
                            CountryId = existingZone.CountryId,
                            HasCodeChange = true,
                            CustomersHavingRateChange = this.GetCustomersHavingRateChange(ratesPreviewByZoneName, existingZone.Name)
                        };
                        zonesChanges.Add(zoneChange);
                    }
                }
            }

            if (addedZones != null)
            {
                foreach (AddedZone addedZone in addedZones)
                {
                    SalePLZoneChange zoneChange = new SalePLZoneChange()
                    {
                        ZoneName = addedZone.Name,
                        CountryId = addedZone.CountryId,
                        HasCodeChange = true,
                        CustomersHavingRateChange = this.GetCustomersHavingRateChange(ratesPreviewByZoneName, addedZone.Name)
                    };
                    zonesChanges.Add(zoneChange);
                }
            }

            return zonesChanges;
        }

        private IEnumerable<int> GetCustomersHavingRateChange(Dictionary<string, List<RatePreview>> ratesPreviewByZoneName, string zoneName)
        {
            HashSet<int> customersIds = new HashSet<int>();
            IEnumerable<RatePreview> newRates = ratesPreviewByZoneName.GetRecord(zoneName);

            if (newRates != null)
            {
                foreach (RatePreview rate in newRates)
                {
                    if (rate.OnwerType == SalePriceListOwnerType.Customer)
                        customersIds.Add(rate.OwnerId);
                    else
                    {
                        CustomerSellingProductManager manager = new CustomerSellingProductManager();
                        IEnumerable<CarrierAccountInfo> customersAssignedToSellingProduct = manager.GetCustomersBySellingProductId(rate.OwnerId);
                        if (customersAssignedToSellingProduct != null)
                        {
                            IEnumerable<int> ids = customersAssignedToSellingProduct.Select(itm => itm.CarrierAccountId);
                            customersIds.UnionWith(ids);
                        }
                    }
                }
            }


            return customersIds;
        }

        private Dictionary<string, List<RatePreview>> StructureRatesPreviewByZoneName(IEnumerable<RatePreview> ratesPreview)
        {
            Dictionary<string, List<RatePreview>> ratesPreviewByZoneName = new Dictionary<string, List<RatePreview>>();
            if (ratesPreview != null)
            {
                List<RatePreview> ratesPreviewList;
                foreach (RatePreview ratePreview in ratesPreview)
                {
                    if (!ratesPreviewByZoneName.TryGetValue(ratePreview.ZoneName, out ratesPreviewList))
                    {
                        ratesPreviewList = new List<RatePreview>();
                        ratesPreviewList.Add(ratePreview);
                        ratesPreviewByZoneName.Add(ratePreview.ZoneName, ratesPreviewList);
                    }
                    else
                        ratesPreviewList.Add(ratePreview);
                }
            }
            return ratesPreviewByZoneName;
        }


        #endregion
    }
}
