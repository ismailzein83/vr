using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;

namespace TOne.WhS.Sales.BP.Activities
{
    public class SetContextIntersectedSellingProductZoneRatesByZone : CodeActivity
    {
        #region Input Arguments

        [RequiredArgument]
        public InArgument<IEnumerable<CustomerCountryToAdd>> CustomerCountriesToAdd { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> MinimumDate { get; set; }

        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            var ratePlanContext = context.GetRatePlanContext() as RatePlanContext;

            if (ratePlanContext.OwnerType == SalePriceListOwnerType.SellingProduct)
                return;

            IEnumerable<CustomerCountryToAdd> countriesToAdd = CustomerCountriesToAdd.Get(context);

            if (countriesToAdd == null || countriesToAdd.Count() == 0)
                return;

            DateTime minimumDate = MinimumDate.Get(context);

            Dictionary<int, List<ProcessedCustomerSellingProduct>> structuredSPAssignments = GetStructuredSPAssignments(ratePlanContext.OwnerId, minimumDate);
            IEnumerable<int> sellingProductIds = structuredSPAssignments.Keys;

            IEnumerable<long> countryZoneIds = GetCountryZoneIds(ratePlanContext.ExistingZonesByCountry, countriesToAdd);

            IEnumerable<SaleRate> rates =
                new SaleRateManager().GetSaleRatesEffectiveAfterByOwnersAndZones(SalePriceListOwnerType.SellingProduct, sellingProductIds, countryZoneIds, minimumDate);

            ratePlanContext.IntersectedSellingProductZoneRatesByZone = new IntersectedSellingProductZoneRatesByZone();

            if (rates == null || rates.Count() == 0)
                return;

            Dictionary<long, Dictionary<int, List<SaleRate>>> structuredRates = GetSturcturedRates(rates);
            SetIntersectedSellingProductZoneRatesByZone(ratePlanContext.IntersectedSellingProductZoneRatesByZone, countryZoneIds, sellingProductIds, structuredRates, structuredSPAssignments);
        }

        #region Private Methods

        private IEnumerable<long> GetCountryZoneIds(Dictionary<int, List<ExistingZone>> existingZonesByCountry, IEnumerable<CustomerCountryToAdd> countriesToAdd)
        {
            var allCountryZoneIds = new List<long>();

            foreach (CustomerCountryToAdd countryToAdd in countriesToAdd)
            {
                List<ExistingZone> countryZones = existingZonesByCountry.GetRecord(countryToAdd.CountryId);

                if (countryZones == null || countryZones.Count == 0)
                    throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("Zones of Country '{0}' were not found", countryToAdd.CountryId));

                IEnumerable<long> countryZoneIds = countryZones.MapRecords(x => x.ZoneId);
                allCountryZoneIds.AddRange(countryZoneIds);
            }

            return allCountryZoneIds;
        }

        private Dictionary<int, List<ProcessedCustomerSellingProduct>> GetStructuredSPAssignments(int customerId, DateTime minimumDate)
        {
            IEnumerable<ProcessedCustomerSellingProduct> allSPAssignments = new CustomerSellingProductManager().GetProcessedCustomerSellingProducts(customerId);

            if (allSPAssignments == null || allSPAssignments.Count() == 0)
                throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("Customer '{0}' has never been assigned to any selling product", customerId));

            var structuredSPAssignments = new Dictionary<int, List<ProcessedCustomerSellingProduct>>();

            foreach (ProcessedCustomerSellingProduct spAssingment in allSPAssignments.OrderBy(x => x.BED))
            {
                if (spAssingment.EED <= minimumDate)
                    continue;

                List<ProcessedCustomerSellingProduct> spAssignments;

                if (!structuredSPAssignments.TryGetValue(spAssingment.SellingProductId, out spAssignments))
                {
                    spAssignments = new List<ProcessedCustomerSellingProduct>();
                    structuredSPAssignments.Add(spAssingment.SellingProductId, spAssignments);
                }

                spAssignments.Add(spAssingment);
            }

            return structuredSPAssignments;
        }

        private Dictionary<long, Dictionary<int, List<SaleRate>>> GetSturcturedRates(IEnumerable<SaleRate> rates)
        {
            var ratesByZone = new Dictionary<long, Dictionary<int, List<SaleRate>>>();
            var salePriceListManager = new SalePriceListManager();

            foreach (SaleRate rate in rates.OrderBy(x => x.BED))
            {
                Dictionary<int, List<SaleRate>> zoneRatesBySellingProduct;

                if (!ratesByZone.TryGetValue(rate.ZoneId, out zoneRatesBySellingProduct))
                {
                    zoneRatesBySellingProduct = new Dictionary<int, List<SaleRate>>();
                    ratesByZone.Add(rate.ZoneId, zoneRatesBySellingProduct);
                }

                SalePriceList salePriceList = salePriceListManager.GetPriceList(rate.PriceListId);
                if (salePriceList == null)
                    throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("SalePriceList of SaleRate '{0}' of SaleZone '{1}' was not found", rate.SaleRateId, rate.ZoneId));

                List<SaleRate> sellingProductZoneRates;

                if (!zoneRatesBySellingProduct.TryGetValue(salePriceList.OwnerId, out sellingProductZoneRates))
                {
                    sellingProductZoneRates = new List<SaleRate>();
                    zoneRatesBySellingProduct.Add(salePriceList.OwnerId, sellingProductZoneRates);
                }

                sellingProductZoneRates.Add(rate);
            }

            return ratesByZone;
        }

        private void SetIntersectedSellingProductZoneRatesByZone(IntersectedSellingProductZoneRatesByZone intersectedSellingProductZoneRatesByZone, IEnumerable<long> zoneIds, IEnumerable<int> sellingProductIds, Dictionary<long, Dictionary<int, List<SaleRate>>> structuredRates, Dictionary<int, List<ProcessedCustomerSellingProduct>> structuredSPAssignmets)
        {
            foreach (long zoneId in zoneIds)
            {
                Dictionary<int, List<SaleRate>> zoneRatesBySellingProduct = structuredRates.GetRecord(zoneId);

                if (zoneRatesBySellingProduct == null)
                    continue;

                var allIntersectedZoneRates = new List<SaleRate>();

                foreach (int sellingProductId in sellingProductIds)
                {
                    List<SaleRate> sellingProductZoneRates = zoneRatesBySellingProduct.GetRecord(sellingProductId);

                    if (sellingProductZoneRates == null)
                        continue;

                    List<ProcessedCustomerSellingProduct> spAssignments = structuredSPAssignmets.GetRecord(sellingProductId);

                    if (spAssignments == null)
                        continue;

                    IEnumerable<SaleRate> intersectedZoneRates = Vanrise.Common.Utilities.GetQIntersectT(spAssignments, sellingProductZoneRates, SaleRateMapper);

                    if (intersectedZoneRates != null)
                        allIntersectedZoneRates.AddRange(intersectedZoneRates);
                }

                if (allIntersectedZoneRates.Count > 0)
                    intersectedSellingProductZoneRatesByZone.Add(zoneId, allIntersectedZoneRates);
            }
        }

        #endregion

        #region Mappers

        private Action<SaleRate, SaleRate> SaleRateMapper = (saleRate, mappedSaleRate) =>
        {
            mappedSaleRate.SaleRateId = saleRate.SaleRateId;
            mappedSaleRate.ZoneId = saleRate.ZoneId;
            mappedSaleRate.PriceListId = saleRate.PriceListId;
            mappedSaleRate.CurrencyId = saleRate.CurrencyId;
            mappedSaleRate.RateTypeId = saleRate.RateTypeId;
            mappedSaleRate.Rate = saleRate.Rate;
            mappedSaleRate.SourceId = saleRate.SourceId;
            mappedSaleRate.RateChange = saleRate.RateChange;
        };

        #endregion
    }
}
