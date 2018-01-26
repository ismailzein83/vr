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
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.Sales.BP.Activities
{
    public class StructureDataByZones : CodeActivity
    {
        #region Input Arguments

        [RequiredArgument]
        public InArgument<bool> IsAdditionalOwner { get; set; }

        [RequiredArgument]
        public InArgument<int> CurrencyId { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<RateToChange>> RatesToChange { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<RateToClose>> RatesToClose { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<SaleZoneRoutingProductToAdd>> SaleZoneRoutingProductsToAdd { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<SaleZoneRoutingProductToClose>> SaleZoneRoutingProductsToClose { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<SaleZoneServiceToAdd>> SaleZoneServicesToAdd { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<SaleZoneServiceToClose>> SaleZoneServicesToClose { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<CustomerCountryToAdd>> CustomerCountriesToAdd { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<CustomerCountryToChange>> CustomerCountriesToChange { get; set; }

        #endregion

        #region Output Arguments

        [RequiredArgument]
        public OutArgument<IEnumerable<DataByZone>> DataByZone { get; set; }

        [RequiredArgument]
        public OutArgument<AllDataByZone> AllDataByZone { get; set; }

        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            bool isAdditionalOwner = IsAdditionalOwner.Get(context);
            IRatePlanContext ratePlanContext = context.GetRatePlanContext();
            SalePriceListOwnerType ownerType = ratePlanContext.OwnerType;
            int ownerId = ratePlanContext.OwnerId;
            int currencyId = CurrencyId.Get(context);

            IEnumerable<CustomerCountryToAdd> countriesToAdd = CustomerCountriesToAdd.Get(context);
            IEnumerable<CustomerCountryToChange> countriesToChange = CustomerCountriesToChange.Get(context);

            var endedCountryIds = new List<int>();
            if (countriesToChange != null)
                endedCountryIds.AddRange(countriesToChange.MapRecords(x => x.CountryId));
            if (ratePlanContext.OwnerType == SalePriceListOwnerType.Customer)
            {
                IEnumerable<CustomerCountry2> soldEndedCountries = new CustomerCountryManager().GetEndedCustomerCountriesEffectiveAfter(ratePlanContext.OwnerId, ratePlanContext.EffectiveDate);
                if (soldEndedCountries != null && soldEndedCountries.Count() > 0)
                    endedCountryIds.AddRange(soldEndedCountries.MapRecords(x => x.CountryId));
            }

            IEnumerable<RateToChange> ratesToChange = this.RatesToChange.Get(context);
            IEnumerable<RateToClose> ratesToClose = this.RatesToClose.Get(context);

            IEnumerable<SaleZoneRoutingProductToAdd> saleZoneRoutingProductsToAdd = this.SaleZoneRoutingProductsToAdd.Get(context);
            IEnumerable<SaleZoneRoutingProductToClose> saleZoneRoutingProductsToClose = this.SaleZoneRoutingProductsToClose.Get(context);

            IEnumerable<SaleZoneServiceToAdd> saleZoneServicesToAdd = this.SaleZoneServicesToAdd.Get(context);
            IEnumerable<SaleZoneServiceToClose> saleZoneServicesToClose = this.SaleZoneServicesToClose.Get(context);

            Dictionary<int, RatePlanCustomerCountry> customerCountriesByCountryId =
                (ownerType == SalePriceListOwnerType.Customer) ? GetCustomerCountriesByCountryId(ownerId, DateTime.Now.Date, countriesToAdd) : null;

            if (ownerType == SalePriceListOwnerType.Customer && customerCountriesByCountryId == null && !isAdditionalOwner)
                throw new DataIntegrityValidationException(string.Format("No countries are sold to Customer '{0}'", ownerId));

            Dictionary<string, DataByZone> dataByZoneName = new Dictionary<string, DataByZone>();
            
            if (ownerType == SalePriceListOwnerType.SellingProduct || customerCountriesByCountryId != null)
            {
                var saleZoneManager = new SaleZoneManager();

                DataByZone dataByZone;

                var ratePlanManager = new RatePlanManager();
                var currencyExchangeManager = new CurrencyExchangeRateManager();
                var saleRateManager = new SaleRateManager();

                foreach (RateToChange rateToChange in ratesToChange)
                {
                    if (!dataByZoneName.TryGetValue(rateToChange.ZoneName, out dataByZone))
                        AddEmptyDataByZone(dataByZoneName, rateToChange.ZoneName, rateToChange.ZoneId, endedCountryIds, out dataByZone, saleZoneManager);

                    if (rateToChange.RateTypeId.HasValue)
                        dataByZone.OtherRatesToChange.Add(rateToChange);
                    else
                        dataByZone.NormalRateToChange = rateToChange;

                    if (dataByZone.ZoneRateGroup == null)
                        dataByZone.ZoneRateGroup = GetZoneRateGroup(ownerType, ownerId, rateToChange.ZoneId, DateTime.Now, currencyId, ratePlanContext.LongPrecision, ratePlanManager, currencyExchangeManager, saleRateManager);

                    if (ownerType == SalePriceListOwnerType.Customer && !dataByZone.SoldOn.HasValue)
                    {
                        RatePlanCustomerCountry ratePlanCustomerCountry = GetRatePlanCustomerCountry(customerCountriesByCountryId, saleZoneManager, rateToChange.ZoneId);
                        if (ratePlanCustomerCountry != null)
                        {
                            dataByZone.SoldOn = ratePlanCustomerCountry.BED;
                            dataByZone.IsCustomerCountryNew = ratePlanCustomerCountry.IsNew;
                        }
                    }
                }

                foreach (RateToClose rateToClose in ratesToClose)
                {
                    if (!dataByZoneName.TryGetValue(rateToClose.ZoneName, out dataByZone))
                        AddEmptyDataByZone(dataByZoneName, rateToClose.ZoneName, rateToClose.ZoneId, endedCountryIds, out dataByZone, saleZoneManager);

                    if (rateToClose.RateTypeId.HasValue)
                        dataByZone.OtherRatesToClose.Add(rateToClose);
                    else
                        dataByZone.NormalRateToClose = rateToClose;

                    if (dataByZone.ZoneRateGroup == null)
                        dataByZone.ZoneRateGroup = GetZoneRateGroup(ownerType, ownerId, rateToClose.ZoneId, DateTime.Now, currencyId, ratePlanContext.LongPrecision, ratePlanManager, currencyExchangeManager, saleRateManager);

                    if (ownerType == SalePriceListOwnerType.Customer && !dataByZone.SoldOn.HasValue)
                    {
                        RatePlanCustomerCountry ratePlanCustomerCountry = GetRatePlanCustomerCountry(customerCountriesByCountryId, saleZoneManager, rateToClose.ZoneId);
                        if (ratePlanCustomerCountry != null)
                        {
                            dataByZone.SoldOn = ratePlanCustomerCountry.BED;
                            dataByZone.IsCustomerCountryNew = ratePlanCustomerCountry.IsNew;
                        }
                    }
                }

                foreach (SaleZoneRoutingProductToAdd routingProductToAdd in saleZoneRoutingProductsToAdd)
                {
                    if (!dataByZoneName.TryGetValue(routingProductToAdd.ZoneName, out dataByZone))
                        AddEmptyDataByZone(dataByZoneName, routingProductToAdd.ZoneName, routingProductToAdd.ZoneId, endedCountryIds, out dataByZone, saleZoneManager);

                    if (dataByZone.ZoneRateGroup == null)
                        dataByZone.ZoneRateGroup = GetZoneRateGroup(ownerType, ownerId, routingProductToAdd.ZoneId, DateTime.Now, currencyId, ratePlanContext.LongPrecision, ratePlanManager, currencyExchangeManager, saleRateManager);

                    dataByZone.SaleZoneRoutingProductToAdd = routingProductToAdd;
                }

                foreach (SaleZoneRoutingProductToClose routingProductToClose in saleZoneRoutingProductsToClose)
                {
                    if (!dataByZoneName.TryGetValue(routingProductToClose.ZoneName, out dataByZone))
                        AddEmptyDataByZone(dataByZoneName, routingProductToClose.ZoneName, routingProductToClose.ZoneId, endedCountryIds, out dataByZone, saleZoneManager);
                    dataByZone.SaleZoneRoutingProductToClose = routingProductToClose;
                }
            }
            this.DataByZone.Set(context, dataByZoneName.Values);
            AllDataByZone.Set(context, new AllDataByZone() { DataByZoneList = dataByZoneName.Values });
        }

        #region Private Members

        private class RatePlanCustomerCountry
        {
            public int CountryId { get; set; }

            public DateTime BED { get; set; }

            public bool IsNew { get; set; }
        }

        private void AddEmptyDataByZone(Dictionary<string, DataByZone> dataByZoneName, string zoneName, long zoneId, IEnumerable<int> endedCountryIds, out DataByZone dataByZone, SaleZoneManager saleZoneManager)
        {
            int? countryId = saleZoneManager.GetSaleZoneCountryId(zoneId);
            if (!countryId.HasValue)
                throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("Could not find the Country of Sale Zone '{0}'", zoneId));
            dataByZone = new DataByZone()
            {
                ZoneId = zoneId,
                ZoneName = zoneName,
                CountryId = countryId.Value,
                IsCountryEnded = endedCountryIds.Contains(countryId.Value),
                OtherRatesToChange = new List<RateToChange>(),
                OtherRatesToClose = new List<RateToClose>()
            };
            dataByZoneName.Add(zoneName, dataByZone);
        }

        private Dictionary<int, RatePlanCustomerCountry> GetCustomerCountriesByCountryId(int customerId, DateTime effectiveOn, IEnumerable<CustomerCountryToAdd> countriesToAdd)
        {
            var customerCountryManager = new CustomerCountryManager();
            IEnumerable<CustomerCountry2> soldCountries = customerCountryManager.GetCustomerCountriesEffectiveAfter(customerId, effectiveOn);

            IEnumerable<RatePlanCustomerCountry> newCountries = null;
            if (countriesToAdd != null)
            {
                IEnumerable<int> newCountryIds = countriesToAdd.MapRecords(x => x.CountryId);
                newCountries = countriesToAdd.MapRecords(x => new RatePlanCustomerCountry() { CountryId = x.CountryId, BED = x.BED, IsNew = true });
            }

            var allCountries = new List<RatePlanCustomerCountry>();

            if (soldCountries != null)
                allCountries.AddRange(soldCountries.MapRecords(x => new RatePlanCustomerCountry() { CountryId = x.CountryId, BED = x.BED, IsNew = false }));

            if (newCountries != null)
                allCountries.AddRange(newCountries);

            if (allCountries.Count == 0)
                return null;

            var countriesByCountryId = new Dictionary<int, RatePlanCustomerCountry>();
            var countryManager = new Vanrise.Common.Business.CountryManager();
            var duplicatedCountryNames = new List<string>();

            foreach (RatePlanCustomerCountry country in allCountries)
            {
                if (countriesByCountryId.ContainsKey(country.CountryId))
                {
                    string countryName = countryManager.GetCountryName(country.CountryId);
                    duplicatedCountryNames.Add(countryName);
                    continue;
                }
                countriesByCountryId.Add(country.CountryId, country);
            }

            if (duplicatedCountryNames.Count > 0)
            {
                throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("The following countries are duplicated: {0}", string.Join(", ", duplicatedCountryNames)));
            }

            return countriesByCountryId;
        }

        private RatePlanCustomerCountry GetRatePlanCustomerCountry(Dictionary<int, RatePlanCustomerCountry> customerCountriesByCountryId, SaleZoneManager saleZoneManager, long saleZoneId)
        {
            SaleZone saleZone = saleZoneManager.GetSaleZone(saleZoneId);
            if (saleZone == null)
                throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("SaleZone '{0}' was not found", saleZoneId));
            RatePlanCustomerCountry ratePlanCustomerCountry = customerCountriesByCountryId.GetRecord(saleZone.CountryId);
            if (ratePlanCustomerCountry == null)
                throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("Country of SaleZone '{0}' is not sold to the Customer", saleZone.Name));
            return ratePlanCustomerCountry;
        }

        private ZoneRateGroup GetZoneRateGroup(SalePriceListOwnerType ownerType, int ownerId, long zoneId, DateTime effectiveOn, int targetCurrencyId, int longPrecision, RatePlanManager ratePlanManager, CurrencyExchangeRateManager exchangeRateManager, SaleRateManager saleRateManager)
        {
            ZoneRateGroup zoneRateGroup = null;
            SaleEntityZoneRate currentRate = ratePlanManager.GetRate(ownerType, ownerId, zoneId, effectiveOn);
            if (currentRate != null)
            {
                zoneRateGroup = new ZoneRateGroup();
                if (currentRate.Rate != null)
                {
                    zoneRateGroup.NormalRate = new ZoneRate();

                    decimal convertedNormalRate = UtilitiesManager.ConvertToCurrencyAndRound(currentRate.Rate.Rate, saleRateManager.GetCurrencyId(currentRate.Rate), targetCurrencyId, effectiveOn, longPrecision, exchangeRateManager);

                    zoneRateGroup.NormalRate.Source = currentRate.Source;
                    zoneRateGroup.NormalRate.Rate = convertedNormalRate;
                    zoneRateGroup.NormalRate.BED = currentRate.Rate.BED;
                    zoneRateGroup.NormalRate.EED = currentRate.Rate.EED;
                }
                if (currentRate.RatesByRateType != null)
                {
                    zoneRateGroup.OtherRatesByType = new Dictionary<int, ZoneRate>();
                    foreach (KeyValuePair<int, SaleRate> kvp in currentRate.RatesByRateType)
                    {
                        if (kvp.Value != null)
                        {
                            var otherRate = new ZoneRate();

                            SalePriceListOwnerType otherRateSource;
                            currentRate.SourcesByRateType.TryGetValue(kvp.Key, out otherRateSource);

                            decimal convertedOtherRate = UtilitiesManager.ConvertToCurrencyAndRound(kvp.Value.Rate, saleRateManager.GetCurrencyId(kvp.Value), targetCurrencyId, effectiveOn, longPrecision, exchangeRateManager);

                            otherRate.Source = otherRateSource;
                            otherRate.RateTypeId = kvp.Key;
                            otherRate.Rate = convertedOtherRate;
                            otherRate.BED = kvp.Value.BED;
                            otherRate.EED = kvp.Value.EED;

                            zoneRateGroup.OtherRatesByType.Add(kvp.Key, otherRate);
                        }
                    }
                }
            }
            return zoneRateGroup;
        }

        #endregion
    }
}
