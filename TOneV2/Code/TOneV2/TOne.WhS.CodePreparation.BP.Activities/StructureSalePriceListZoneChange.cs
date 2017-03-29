using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.CodePreparation.Entities;
using TOne.WhS.CodePreparation.Entities.Processing;

namespace TOne.WhS.CodePreparation.BP.Activities
{
    public sealed class StructureSalePriceListZoneChange : CodeActivity
    {
        [RequiredArgument]
        public InArgument<int> SellingNumberPlanId { get; set; }

        public InArgument<IEnumerable<CountryToProcess>> CountriesToProcess { get; set; }
        public InArgument<DateTime> EffectiveDate { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<CustomerPriceListChange>> CustomerChange { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<CountryToProcess> countriesToProcess = CountriesToProcess.Get(context);
            int sellingNumberPlanId = this.SellingNumberPlanId.Get(context);
            DateTime effectiveDate = this.EffectiveDate.Get(context);
            IEnumerable<CustomerPriceListChange> cutsomerChanges = GetCustomerPriceListChanges(countriesToProcess, sellingNumberPlanId, effectiveDate);
            CustomerChange.Set(context, cutsomerChanges);
        }
        #region private Methodes
        private IEnumerable<CustomerPriceListChange> GetCustomerPriceListChanges(IEnumerable<CountryToProcess> countryToProcess, int sellingNumberPlanId, DateTime effectiveDate)
        {
            var customerPriceListChanges = new List<CustomerPriceListChange>();
            var carrierAccountManager = new CarrierAccountManager();
            var customerSellingProductManager = new CustomerSellingProductManager();
            IEnumerable<CarrierAccountInfo> customers = carrierAccountManager.GetCustomersBySellingNumberPlanId(sellingNumberPlanId);

            if (customers == null)
                return customerPriceListChanges;

            Changes structureData = GetChangesByCountry(countryToProcess);

            var customerCountryManager = new CustomerCountryManager();
            foreach (CarrierAccountInfo customer in customers)
            {
                int? sellingProductId = customerSellingProductManager.GetEffectiveSellingProductId(customer.CarrierAccountId, effectiveDate, false);
                string ownerKey = string.Format("{0}|{1}", customer.CarrierAccountId, (int)SalePriceListOwnerType.Customer);
                RateByZone ratebyZone;
                if (!structureData.SaleRateByOwner.TryGetValue(ownerKey, out ratebyZone))
                {
                    ownerKey = string.Format("{0}|{1}", sellingProductId, (int)SalePriceListOwnerType.SellingProduct);
                    structureData.SaleRateByOwner.TryGetValue(ownerKey, out ratebyZone);
                }
                var countryRateChanges = new List<SalePricelistRateChange>();
                var countryCodeChanges = new List<SalePricelistCodeChange>();
                IEnumerable<CustomerCountry2> soldCountries = customerCountryManager.GetCustomerCountriesEffectiveAfter(customer.CarrierAccountId, effectiveDate);
                if (soldCountries == null) continue;
                foreach (var country in soldCountries)
                {
                    if (ratebyZone != null)
                    {
                        List<SalePricelistRateChange> rateChange;
                        if (ratebyZone.TryGetValue(country.CountryId, out rateChange))
                            countryRateChanges.AddRange(rateChange);
                    }
                    List<SalePricelistCodeChange> codeChange;
                    if (structureData.SaleCode.TryGetValue(country.CountryId, out codeChange))
                        countryCodeChanges.AddRange(codeChange);
                }
                if (countryRateChanges.Count == 0 && countryCodeChanges.Count == 0) continue;

                customerPriceListChanges.Add(new CustomerPriceListChange
                {
                    RateChanges = countryRateChanges,
                    CodeChanges = countryCodeChanges,
                    CustomerId = customer.CarrierAccountId
                });
            }
            return customerPriceListChanges;
        }
        private Changes GetChangesByCountry(IEnumerable<CountryToProcess> countries)
        {
            var rateByOwner = new RateByOwner();
            var codeChangeDictionary = new Dictionary<int, List<SalePricelistCodeChange>>();
            var closeZonesByCountryId = new Dictionary<int, List<string>>();
            foreach (var country in countries)
            {
                foreach (var zone in country.ZonesToProcess)
                {
                    ManageRateChange(zone, rateByOwner, country);
                    ManageCodeChange(zone, country, codeChangeDictionary);
                    if (zone.ChangeType == ZoneChangeType.Deleted || zone.ChangeType == ZoneChangeType.PendingClosed)
                    {
                        List<string> closedZoneIds;
                        if (!closeZonesByCountryId.TryGetValue(country.CountryId, out closedZoneIds))
                        {
                            closedZoneIds = new List<string>();
                            closeZonesByCountryId.Add(country.CountryId, closedZoneIds);
                        }
                        closedZoneIds.Add(zone.ZoneName);
                    }
                }
            }
            return new Changes
            {
                SaleRateByOwner = rateByOwner,
                SaleCode = codeChangeDictionary,
                ClosedzoneBycountryId = closeZonesByCountryId
            };
        }
        private void ManageRateChange(ZoneToProcess zone, RateByOwner rateByOwner, CountryToProcess country)
        {
            foreach (var rateToAdd in zone.RatesToAdd)
            {
                string ownerKey = string.Format("{0}|{1}", rateToAdd.PriceListToAdd.OwnerId, (int)rateToAdd.PriceListToAdd.OwnerType);
                RateByZone rateByZone;
                if (!rateByOwner.TryGetValue(ownerKey, out rateByZone))
                {
                    rateByZone = new RateByZone();
                    rateByOwner.Add(ownerKey, rateByZone);
                }
                List<SalePricelistRateChange> tempChanges;
                if (!rateByZone.TryGetValue(country.CountryId, out tempChanges))
                {
                    tempChanges = new List<SalePricelistRateChange>();
                    rateByZone.Add(country.CountryId, tempChanges);
                }
                tempChanges.Add(new SalePricelistRateChange
                {
                    ZoneName = rateToAdd.ZoneName,
                    ChangeType = RateChangeType.New,
                    Rate = rateToAdd.Rate,
                    CountryId = country.CountryId,
                    BED = zone.BED
                });
            }
        }
        private void ManageCodeChange(ZoneToProcess zone, CountryToProcess country, Dictionary<int, List<SalePricelistCodeChange>> codeChangeDictionary)
        {
            List<SalePricelistCodeChange> codeChanges = new List<SalePricelistCodeChange>();
            codeChanges.AddRange(zone.CodesToAdd.Select(code => new SalePricelistCodeChange
            {
                CountryId = country.CountryId,
                ZoneName = code.ZoneName,
                Code = code.Code,
                ChangeType = CodeChange.New,
                BED = code.BED,
                EED = code.EED
            }));
            foreach (var code in zone.CodesToMove)
            {
                codeChanges.Add(new SalePricelistCodeChange
                {
                    CountryId = country.CountryId,
                    ZoneName = code.ZoneName,
                    Code = code.Code,
                    ChangeType = CodeChange.Moved,
                    RecentZoneName = code.OldZoneName,
                    BED = code.BED,
                    EED = code.EED
                });
                codeChanges.Add(new SalePricelistCodeChange
                {
                    CountryId = country.CountryId,
                    ZoneName = code.OldZoneName,
                    Code = code.Code,
                    ChangeType = CodeChange.Closed,
                    EED = code.BED,
                    BED = code.OldCodeBED
                });
            }
            codeChanges.AddRange(zone.CodesToClose.Select(code => new SalePricelistCodeChange
            {
                CountryId = country.CountryId,
                ZoneName = code.ZoneName,
                Code = code.Code,
                ChangeType = CodeChange.Closed,
                EED = code.CloseEffectiveDate,
                BED = zone.BED
            }));
            if (codeChanges.Count > 0)
            {
                List<SalePricelistCodeChange> foundCodes;
                if (!codeChangeDictionary.TryGetValue(country.CountryId, out foundCodes))
                {
                    foundCodes = new List<SalePricelistCodeChange>();
                    codeChangeDictionary.Add(country.CountryId, foundCodes);
                }
                foundCodes.AddRange(codeChanges);
            }
        }

        #endregion

        #region private class
        public class RateByOwner : Dictionary<string, RateByZone> { }
        public class RateByZone : Dictionary<int, List<SalePricelistRateChange>> { }
        private class Changes
        {
            public RateByOwner SaleRateByOwner { get; set; }
            public Dictionary<int, List<SalePricelistCodeChange>> SaleCode { get; set; }
            public Dictionary<int, List<string>> ClosedzoneBycountryId { get; set; }
        }
        #endregion
    }
}
