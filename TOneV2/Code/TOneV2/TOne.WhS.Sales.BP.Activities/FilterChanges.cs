using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Business.Reader;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.BusinessProcess;
using Vanrise.Entities;
using TOne.WhS.Sales.Business;


namespace TOne.WhS.Sales.BP.Activities
{

    public class FilterChanges : CodeActivity
    {
        [RequiredArgument]
        public InArgument<int> CustomerId { get; set; }

        [RequiredArgument]
        public InArgument<Changes> Changes { get; set; }

        [RequiredArgument]
        public InArgument<bool> FollowPublisherRatesBED { get; set; }

        [RequiredArgument]
        public InArgument<bool> FollowPublisherRoutingProduct { get; set; }
        [RequiredArgument]
        public InArgument<DateTime> EffectiveDate { get; set; }
        [RequiredArgument]
        public InArgument<int> PublisherId { get; set; }

        [RequiredArgument]
        public OutArgument<Changes> FilteredChanges { get; set; }
        public OutArgument<List<ExcludedItem>> ExcludedItems { get; set; }

        CarrierAccountManager _carrierAccountManager;
        SaleZoneManager _saleZoneManager;
        protected override void Execute(CodeActivityContext context)
        {
            _carrierAccountManager = new CarrierAccountManager();
            _saleZoneManager = new SaleZoneManager();

            #region Context Variables

            RatePlanContext ratePlanContext = context.GetRatePlanContext() as RatePlanContext;
            long processInstanceId = context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID;
            int customerId = this.CustomerId.Get(context);
            Changes changes = this.Changes.Get(context);
            DateTime effectiveDate = this.EffectiveDate.Get(context);
            bool followPublisherRatesBED = this.FollowPublisherRatesBED.Get(context);
            bool followPublisherRoutingProduct = this.FollowPublisherRoutingProduct.Get(context);
            int publisherId = this.PublisherId.Get(context);

            #endregion

            #region Local Variables

            Dictionary<int, List<ZoneChanges>> subscriberZoneChangesByCountryId = new Dictionary<int, List<ZoneChanges>>();
            CountryManager countryManager = new CountryManager();
            Dictionary<int, List<ZoneChanges>> publisherZoneChangesByCountryId = new Dictionary<int, List<ZoneChanges>>();
            Dictionary<int, List<ExcludedItem>> excludedZones = new Dictionary<int, List<ExcludedItem>>();
            Dictionary<int, ExcludedItem> excludedCountries = new Dictionary<int, ExcludedItem>();
            List<DraftNewCountry> newCountriesToSold = new List<DraftNewCountry>();
            Changes filteredChanges = new Changes();

            string carrierName = _carrierAccountManager.GetCarrierAccountName(customerId);
            var subscriberSoldCountriesByCountryId = GetNotEndedCustomerCountriesByCountryId(customerId, effectiveDate);
            #endregion

            // Structuring Dictionary for Publisher ZoneChanges from Changes
            publisherZoneChangesByCountryId = GetPublisherZoneChangesByCountryId(changes);

            FilterCountryChanges(carrierName, customerId, effectiveDate, changes, excludedCountries, newCountriesToSold, processInstanceId, publisherZoneChangesByCountryId.Keys.ToList(), subscriberSoldCountriesByCountryId);

            if (newCountriesToSold.Count() > 0)
            {
                filteredChanges.CountryChanges = new CountryChanges();
                filteredChanges.CountryChanges.NewCountries = newCountriesToSold;
            }

            filteredChanges.ZoneChanges = FilterZoneChanges(customerId, effectiveDate, followPublisherRatesBED, followPublisherRoutingProduct, publisherId, excludedCountries, excludedZones, subscriberZoneChangesByCountryId, processInstanceId, filteredChanges, changes, subscriberSoldCountriesByCountryId,newCountriesToSold);
            filteredChanges.CurrencyId = changes.CurrencyId;
            foreach (var publisherZoneChangesCountryId in publisherZoneChangesByCountryId.Keys)
            {
                var subscriberZoneChange = subscriberZoneChangesByCountryId.GetRecord(publisherZoneChangesCountryId);
                if ((subscriberZoneChange == null || subscriberZoneChange.Count() == 0) && excludedCountries.GetRecord(publisherZoneChangesCountryId) == null && !newCountriesToSold.Any(x => x.CountryId == publisherZoneChangesCountryId))
                {
                    var countryName = countryManager.GetCountryName(publisherZoneChangesCountryId);
                    var reason = String.Format("Country '{0}' has been excluded for subscriber '{1}' because all country zone changes has been also excluded due to some errors", countryName, carrierName);
                    AddExcludedCountry(excludedCountries, publisherZoneChangesCountryId, reason, processInstanceId);
                }
            }

            List<ExcludedItem> excludedItems = new List<ExcludedItem>();
            excludedItems.AddRange(excludedCountries.Values);
            excludedItems.AddRange(excludedZones.Values.SelectMany(item => item));

            this.ExcludedItems.Set(context, excludedItems);
            this.FilteredChanges.Set(context, filteredChanges);
        }

        private void FilterCountryChanges(string subscriberName, int subscriberId, DateTime effectiveDate, Changes changes, Dictionary<int, ExcludedItem> excludedCountries, List<DraftNewCountry> newCountriesToSold, long processInstanceId, List<int> publisherChangesCountryIds, Dictionary<int, CustomerCountry2> subscriberSoldCountriesByCountryId)
        {
            List<int> notSoldCountryIds = new List<int>();
            List<int> excludedClosedCountryIds = new List<int>();
            CountryManager countryManager = new CountryManager();
            if (changes.CountryChanges != null)
            {
                //We have countries to sell or to close
                if (changes.CountryChanges.NewCountries != null && changes.CountryChanges.NewCountries.Count() > 0)
                {
                    var effectiveOrFutureCustomerCountriesByCountryId = GetEffectiveOrFutureCustomerCountriesByCountryId(subscriberId, effectiveDate);
                    foreach (var newCountry in changes.CountryChanges.NewCountries)
                    {
                        if (!effectiveOrFutureCustomerCountriesByCountryId.ContainsKey(newCountry.CountryId))
                            newCountriesToSold.Add(newCountry);
                        //to check that the existing country does not contain any zone changes
                        else if (!changes.ZoneChanges.Any(item => item.CountryId == newCountry.CountryId))
                        {
                            var countryName = countryManager.GetCountryName(newCountry.CountryId);
                            var reason = String.Format("Country '{0}' already exists for subscriber '{1}'", countryName, subscriberName);
                            AddExcludedCountry(excludedCountries, newCountry.CountryId, reason, processInstanceId);
                        }
                    }
                }
                if (changes.CountryChanges.ChangedCountries != null)
                {
                    excludedClosedCountryIds.AddRange(changes.CountryChanges.ChangedCountries.Countries.Select(item => item.CountryId));
                    List<string> closedCountryNames = countryManager.GetCountryNames(excludedClosedCountryIds);
                    closedCountryNames.ThrowIfNull("countryNames");

                    //Adding each excluded closed country with all relative zone changes

                    foreach (var excludeClosedCountryId in excludedClosedCountryIds)
                    {
                        var countryName = countryManager.GetCountryName(excludeClosedCountryId);
                        var reason = String.Format("Closing Country '{0}' will not be reflected to subscriber '{1}'", countryName, subscriberName);
                        AddExcludedCountry(excludedCountries, excludeClosedCountryId, reason, processInstanceId);
                    }
                }
            }

            if (subscriberSoldCountriesByCountryId.Count == 0 && newCountriesToSold.Count() == 0)
            {
                foreach (var countryId in publisherChangesCountryIds)
                {
                    var reason = String.Format("No countries are sold to subscriber '{0}'", _carrierAccountManager.GetCarrierAccountName(subscriberId));
                    AddExcludedCountry(excludedCountries, countryId, reason, processInstanceId);
                }
            }

            foreach (var countryId in publisherChangesCountryIds)
            {
                if (subscriberSoldCountriesByCountryId.GetRecord(countryId) == null && excludedCountries.GetRecord(countryId) == null && !newCountriesToSold.Any(x => x.CountryId == countryId))
                {
                    var countryName = countryManager.GetCountryName(countryId);
                    var reason = String.Format("Country'{0}' is not sold or closed for subscriber '{1}'", countryName, _carrierAccountManager.GetCarrierAccountName(subscriberId));
                    AddExcludedCountry(excludedCountries, countryId, reason, processInstanceId);
                }
            }
        }

        private List<ZoneChanges> FilterZoneChanges(int customerId, DateTime effectiveDate, bool followPublisherRatesBED, bool followPublisherRoutingProduct, int publisherId, Dictionary<int, ExcludedItem> excludedCountries, Dictionary<int, List<ExcludedItem>> excludedZones, Dictionary<int, List<ZoneChanges>> subscriberZoneChangesByCountryId, long processInstanceId, Changes filteredChanges, Changes changes, Dictionary<int, CustomerCountry2> subscriberSoldCountriesByCountryId, List<DraftNewCountry> newCountriesToSold)
        {
            List<int> rateTypeIds = new List<int>();
            IEnumerable<ZoneChanges> zoneChanges = changes.ZoneChanges;
            var filteredZoneChanges = new List<ZoneChanges>();
            int sellingProductId = _carrierAccountManager.GetSellingProductId(customerId);
            int publisherSellingProductId = _carrierAccountManager.GetSellingProductId(publisherId);
            var pricingSettings = _carrierAccountManager.GetCustomerPricingSettings(customerId);


           
                // TOne.WhS.BusinessEntity.Business.ConfigManager configManager = new TOne.WhS.BusinessEntity.Business.ConfigManager();

                var effectiveRateLocator = GetNewRateLocator(customerId, zoneChanges, sellingProductId); ;
            var closedRateLocator = GetClosedRateLocator(customerId, zoneChanges, sellingProductId);
           // var zoneRoutingProductLocator = GetZoneRoutingProductLocator(customerId, zoneChanges);
            //var zoneNewRateRoutingProductLocator = GetZoneNewRateRoutingProductLocator(customerId, publisherId, zoneChanges);

            foreach (var zoneChange in zoneChanges)
            {
                if ((zoneChange.NewRates != null && zoneChange.NewRates.Any()) || (zoneChange.ClosedRates != null && zoneChange.ClosedRates.Any()))
                {
                    rateTypeIds = BusinessEntity.Business.Helper.GetRateTypeIds(customerId, zoneChange.ZoneId, DateTime.Now).ToList();
                }
                    var subscriberZoneChange = subscriberZoneChangesByCountryId.GetOrCreateItem(zoneChange.CountryId);
             
                if (excludedCountries.GetRecord(zoneChange.CountryId) != null)
                {
                    var excludedCountry = excludedCountries.GetRecord(zoneChange.CountryId);
                    string reason = excludedCountry.Reason;
                    AddExcludedZone(excludedZones, zoneChange.ZoneId, reason, zoneChange.CountryId, processInstanceId);
                    continue;
                }
                CustomerCountry2 customerCountry;
                if (!subscriberSoldCountriesByCountryId.TryGetValue(zoneChange.CountryId, out customerCountry))
                {
                    continue;
                }

                if (newCountriesToSold.Any(x=>x.CountryId==zoneChange.CountryId))
                {
                    //Zone Change is added directly because the country is new sold
                    subscriberZoneChange.Add(zoneChange);
                    filteredZoneChanges.Add(zoneChange);
                    continue;
                }

                var filteredZoneChange = new ZoneChanges
                {
                    ZoneId = zoneChange.ZoneId,
                    ZoneName = zoneChange.ZoneName,
                    CountryId = zoneChange.CountryId,
                    NewOtherRateBED = zoneChange.NewOtherRateBED
                };

                // New Rates Filtering
                if (zoneChange.NewRates != null && zoneChange.NewRates.Any())
                {
                    List<DraftRateToChange> filteredRates = new List<DraftRateToChange>();
                    
                        bool isRateValid = FilterZoneNewRates(customerId, sellingProductId, zoneChange.NewRates, customerCountry.BED, effectiveDate, pricingSettings.IncreasedRateDayOffset.Value, pricingSettings.DecreasedRateDayOffset.Value, effectiveRateLocator, changes.CurrencyId.Value, followPublisherRatesBED, excludedZones, zoneChange, processInstanceId, out filteredRates, rateTypeIds);
                        if (!isRateValid)
                            continue;
                    
                   // subscriberZoneChange.Add(filteredZoneChange);
                    filteredZoneChange.NewRates = filteredRates;
                }

                //Closed Rates Filtering
                if (zoneChange.ClosedRates != null && zoneChange.ClosedRates.Any())
                {
                    List<DraftRateToClose> ratesToClose = new List<DraftRateToClose>();
                    bool isRateToCloseValid = FilterZoneClosedRates(customerId, zoneChange.ClosedRates, customerCountry.BED, closedRateLocator, sellingProductId, changes.CurrencyId.Value, excludedZones, zoneChange, processInstanceId, out ratesToClose, rateTypeIds);
                    if (!isRateToCloseValid)
                        continue;
                    filteredZoneChange.ClosedRates = ratesToClose;
                }

                //New Routing Product Filtering

                if (zoneChange.NewRoutingProduct != null)
                {
                    DraftNewSaleZoneRoutingProduct newRoutingProduct = new DraftNewSaleZoneRoutingProduct();
                    bool isNewRoutingProductValid = FilterZoneNewRoutingProduct(zoneChange, customerCountry, followPublisherRatesBED, filteredZoneChange, customerId, publisherId, zoneChanges, sellingProductId, publisherSellingProductId, excludedZones, processInstanceId, out newRoutingProduct);
                    if (!isNewRoutingProductValid)
                        continue;
                    filteredZoneChange.NewRoutingProduct = newRoutingProduct;
                }
                if (zoneChange.RoutingProductChange != null)
                {
                    DraftChangedSaleZoneRoutingProduct routingProductChange = new DraftChangedSaleZoneRoutingProduct();
                    bool isRoutingProductCloseValid = closeZoneRoutingProduct(zoneChange, customerId, sellingProductId, zoneChanges, excludedZones, processInstanceId, out routingProductChange);
                    if (!isRoutingProductCloseValid)
                        continue;
                    filteredZoneChange.RoutingProductChange = routingProductChange;
                }


                //Adding Subscriber filtered changes to a separate dictionary
                subscriberZoneChange.Add(filteredZoneChange);
                filteredZoneChanges.Add(filteredZoneChange);
            }

            return filteredZoneChanges;
        }
        private bool FilterZoneNewRoutingProduct(ZoneChanges zoneChange, CustomerCountry2 customerCountry, bool followPublisherRoutingProduct, ZoneChanges filteredZoneChange, int customerId, int publisherId, IEnumerable<ZoneChanges> zoneChanges, int sellingProductId, int publisherSellingProductId, Dictionary<int, List<ExcludedItem>> excludedZonesByCountryId, long processInstanceId, out DraftNewSaleZoneRoutingProduct newRoutingProduct)
        {
            RoutingProductManager routingProductManager = new RoutingProductManager();
            var zoneNewRateRoutingProductLocator = GetZoneNewRateRoutingProductLocator(customerId, publisherId, zoneChanges);
            newRoutingProduct = new DraftNewSaleZoneRoutingProduct();

            if (customerCountry.BED > zoneChange.NewRoutingProduct.BED)
            {
                string subscriberName = _carrierAccountManager.GetCarrierAccountName(customerId);
                string routingProductName = routingProductManager.GetRoutingProductName(zoneChange.NewRoutingProduct.ZoneRoutingProductId);
                routingProductName.ThrowIfNull("routingProductName");
                string zoneName = _saleZoneManager.GetSaleZoneName(zoneChange.ZoneId);
                string reason = String.Format("Cannot create new routing product '{0}' for subscriber '{1}' because subscriber country BED '{2}' is greater than the new routing product BED '{3}'", routingProductName, subscriberName, customerCountry.BED, zoneChange.NewRoutingProduct.BED);
                AddExcludedZone(excludedZonesByCountryId, zoneChange.ZoneId, reason, null, processInstanceId);
                return false;
            }
            else
                newRoutingProduct = zoneChange.NewRoutingProduct;

            if (followPublisherRoutingProduct && filteredZoneChange.NewRates != null && filteredZoneChange.NewRates.Any(item => item.RateTypeId == null))
            {

                var subscriberRoutingProduct = zoneNewRateRoutingProductLocator.GetCustomerZoneRoutingProduct(customerId, sellingProductId, zoneChange.ZoneId);
                var publisherRoutingProduct = zoneNewRateRoutingProductLocator.GetCustomerZoneRoutingProduct(publisherId, publisherSellingProductId, zoneChange.ZoneId);

                if (subscriberRoutingProduct.RoutingProductId != publisherRoutingProduct.RoutingProductId)
                {
                    var draftNewSaleZoneRoutingProduct = new DraftNewSaleZoneRoutingProduct()
                    {
                        ZoneId = zoneChange.ZoneId,
                        ZoneRoutingProductId = publisherRoutingProduct.RoutingProductId,
                        BED = publisherRoutingProduct.BED,
                        EED = publisherRoutingProduct.EED,
                    };
                    newRoutingProduct = draftNewSaleZoneRoutingProduct;
                }
                else
                {
                    string subscriberName = _carrierAccountManager.GetCarrierAccountName(customerId);
                    string routingProductName = routingProductManager.GetRoutingProductName(publisherRoutingProduct.RoutingProductId);
                    routingProductName.ThrowIfNull("routingProductName");
                    string zoneName = _saleZoneManager.GetSaleZoneName(zoneChange.ZoneId);
                    string reason = String.Format("Routing product '{0}' already exists at subscriber : '{1}'", routingProductName, subscriberName);
                    AddExcludedZone(excludedZonesByCountryId, zoneChange.ZoneId, reason, null, processInstanceId);
                    return false;
                }
            }
            return true;
        }
        private bool closeZoneRoutingProduct(ZoneChanges zoneChange, int customerId, int sellingProductId, IEnumerable<ZoneChanges> zoneChanges, Dictionary<int, List<ExcludedItem>> excludedZonesByCountryId, long processInstanceId, out DraftChangedSaleZoneRoutingProduct routingProductChange)
        {
            RoutingProductManager routingProductManager = new RoutingProductManager();
            var zoneRoutingProductLocator = GetZoneRoutingProductLocator(customerId, zoneChanges);
            routingProductChange = new DraftChangedSaleZoneRoutingProduct();
            var currentRoutingProduct = zoneRoutingProductLocator.GetCustomerZoneRoutingProduct(customerId, sellingProductId, zoneChange.ZoneId);

            if (currentRoutingProduct != null && currentRoutingProduct.Source != SaleEntityZoneRoutingProductSource.CustomerZone)
            {
                string subscriberName = _carrierAccountManager.GetCarrierAccountName(customerId);
                string routingProductName = routingProductManager.GetRoutingProductName(zoneChange.RoutingProductChange.ZoneRoutingProductId);
                routingProductName.ThrowIfNull("routingProductName");
                string zoneName = _saleZoneManager.GetSaleZoneName(zoneChange.ZoneId);
                string reason = String.Format("Cannot close routing product '{0}' for subscriber '{1}' because his routing product source is not Customer Zone ", routingProductName, subscriberName);
                AddExcludedZone(excludedZonesByCountryId, zoneChange.ZoneId, reason, null, processInstanceId);
                return false;
            }
            routingProductChange = zoneChange.RoutingProductChange;
            return true;
        }

        private bool FilterZoneNewRates(int customerId, int sellingProductId, IEnumerable<DraftRateToChange> newRates, DateTime countrySellDate, DateTime effectiveDate, int increasedRateDayOffset, int decreasedRateDayOffset, SaleEntityZoneRateLocator effectiveRateLocator, int currencyId, bool followPublisherRatesBED, Dictionary<int, List<ExcludedItem>> excludedZonesByCountryId, ZoneChanges zoneChange, long processInstanceId, out List<DraftRateToChange> filteredRates,List<int> rateTypeIds)
        {
            long zoneId = zoneChange.ZoneId;
            RateTypeManager rateTypeManager = new RateTypeManager();
            List<DraftRateToChange> excludedOtherRates = new List<DraftRateToChange>();
            filteredRates = new List<DraftRateToChange>();
           
            var rateAtActionDate = effectiveRateLocator.GetCustomerZoneRate(customerId, sellingProductId, zoneId);
             
            if (rateAtActionDate != null && rateAtActionDate.EffectiveCurrencyId != currencyId)
            {
                string zoneName = _saleZoneManager.GetSaleZoneName(zoneId);
                var reason = String.Format("Zone : '{0}' have different currency than publisher", zoneName);
                AddExcludedZone(excludedZonesByCountryId, zoneId, reason, null, processInstanceId);

                return false;
            }
            foreach (var newRate in newRates)
            {
                if (newRate.BED < countrySellDate)
                {
                    string zoneName = _saleZoneManager.GetSaleZoneName(zoneId);
                    string reason;
                    if (newRate.RateTypeId == null)
                        reason = String.Format("New rate for zone '{0}' has BED : '{1}' less than country sell date : '{2}'", zoneName, newRate.BED, countrySellDate);
                    else
                    {
                        var otherRateName = rateTypeManager.GetRateTypeName(newRate.RateTypeId.Value);
                        reason = String.Format("New other rate ({0}) for zone '{1}' has BED : '{2}' less than country sell date : '{3}'", otherRateName, zoneName, newRate.BED, countrySellDate);
                    }
                    AddExcludedZone(excludedZonesByCountryId, newRate.ZoneId, reason, null, processInstanceId);
                    return false;
                }
                if (rateAtActionDate == null)
                {
                    string reason;
                    if (newRate.RateTypeId == null)
                        reason = String.Format("Country is not sold at this zone rate change date:'{0}'", effectiveDate);
                    else
                    {
                        var otherRateName = rateTypeManager.GetRateTypeName(newRate.RateTypeId.Value);
                        reason = String.Format("Country is not sold at this zone other rate change ({0}) date:'{1}'", otherRateName, effectiveDate);
                    }
                    AddExcludedZone(excludedZonesByCountryId, newRate.ZoneId, reason, null, processInstanceId);
                    return false;
                }
                if ((newRate.RateTypeId != null && rateTypeIds != null && !rateTypeIds.Any(item => item == newRate.RateTypeId)) || (newRate.RateTypeId != null && rateTypeIds == null))
                {
                    excludedOtherRates.Add(newRate);
                    continue;
                }



                if (!followPublisherRatesBED)
                {
                    SaleRate matchedRate;
                    if (!newRate.RateTypeId.HasValue)
                    {
                        matchedRate = rateAtActionDate.Rate;
                    }
                    else rateAtActionDate.RatesByRateType.TryGetValue(newRate.RateTypeId.Value, out matchedRate);

                    var saleZone = _saleZoneManager.GetSaleZone(zoneId);
                    if (saleZone == null)
                        throw new DataIntegrityValidationException(string.Format("There is no sale zone with id '{0}'.", zoneId));

                    var saleZoneBED = countrySellDate > saleZone.BED ? countrySellDate : saleZone.BED;

                    if (matchedRate != null)
                    {
                        if (newRate.Rate <= matchedRate.Rate)
                            newRate.BED = saleZoneBED > effectiveDate.AddDays(decreasedRateDayOffset) ? saleZoneBED : effectiveDate.AddDays(increasedRateDayOffset);

                        else
                            newRate.BED = saleZoneBED > effectiveDate.AddDays(increasedRateDayOffset) ? saleZoneBED : effectiveDate.AddDays(increasedRateDayOffset);
                    }
                    else
                        newRate.BED = saleZoneBED > effectiveDate ? saleZoneBED : effectiveDate;

                }
                filteredRates.Add(newRate);
            }
            if (excludedOtherRates.Count() != 0 && (excludedOtherRates.Count() == newRates.Count()))
            {
                string zoneName = _saleZoneManager.GetSaleZoneName(zoneId);
                string reason = String.Format("Other rates changes on zone {0} is not applicable because zone does not contain this other rate type", zoneName);
                AddExcludedZone(excludedZonesByCountryId, zoneId, reason, null, processInstanceId);
                return false;
            }
            return true;
        }

        private bool FilterZoneClosedRates(int customerId, IEnumerable<DraftRateToClose> closedRates, DateTime countrySellDate, SaleEntityZoneRateLocator closedRateLocator, int sellingProductId, int currencyId, Dictionary<int, List<ExcludedItem>> excludedZonesByCountryId, ZoneChanges zoneChange, long processInstanceId, out List<DraftRateToClose> ratesToClose,List<int> rateTypeIds)
        {
            long zoneId = zoneChange.ZoneId;
            RateTypeManager rateTypeManager = new RateTypeManager();
            List<DraftRateToClose> excludedOtherRates = new List<DraftRateToClose>();
            ratesToClose = new List<DraftRateToClose>();
            foreach (var closedRate in closedRates)
            {
                if (closedRate.EED < countrySellDate)
                {
                    string reason;
                    if (closedRate.RateTypeId == null)
                        reason = String.Format("Closed rate EED '{0}' is less than country sell date : '{1}'", closedRate.EED, countrySellDate);
                    else
                    {
                        var otherRateName = rateTypeManager.GetRateTypeName(closedRate.RateTypeId.Value);
                        reason = String.Format("Closed other rate ({0}) EED '{1}' is less than country sell date : '{2}'", otherRateName, closedRate.EED, countrySellDate);
                    }
                    AddExcludedZone(excludedZonesByCountryId, closedRate.ZoneId, reason, null, processInstanceId);
                    return false;
                }
                if ((closedRate.RateTypeId != null && rateTypeIds != null && !rateTypeIds.Any(item => item == closedRate.RateTypeId)) || (closedRate.RateTypeId != null && rateTypeIds == null))
                {
                    if (closedRate.RateTypeId != null)
                    {
                        excludedOtherRates.Add(closedRate);
                        continue;
                    }
                    //string zoneName = _saleZoneManager.GetSaleZoneName(closedRate.ZoneId);
                    //string reason = String.Format("Other rates changes on zone {0} is not applicable because zone does not contain this other rate type", zoneName);
                    //AddExcludedZone(excludedZonesByCountryId, closedRate.ZoneId, reason, null, processInstanceId);
                    //return false;
                }

                var currentRate = closedRateLocator.GetCustomerZoneRate(customerId, sellingProductId, closedRate.ZoneId);
                if (currentRate != null && currentRate.Source == SalePriceListOwnerType.Customer && currentRate.EffectiveCurrencyId == currencyId && (closedRate.RateTypeId == null || currentRate.RatesByRateType.ContainsKey(closedRate.RateTypeId.Value)))
                    ratesToClose.Add(closedRate);
            }
            if (excludedOtherRates.Count() != 0 && (excludedOtherRates.Count() == closedRates.Count()))
            {
                string zoneName = _saleZoneManager.GetSaleZoneName(zoneId);
                string reason = String.Format("Other rates changes on zone {0} is not applicable because zone does not contain changed other rate types ", zoneName);
                AddExcludedZone(excludedZonesByCountryId, zoneId, reason, null, processInstanceId);
                return false;
            }
            return true;
        }

        private Dictionary<int, CustomerCountry2> GetNotEndedCustomerCountriesByCountryId(int customerId, DateTime effectiveDate)
        {
            var customerCountryManager = new CustomerCountryManager();
            var customerCountries = customerCountryManager.GetNotEndedCustomerCountriesEffectiveAfter(customerId, effectiveDate);
            var customerCountriesSoldDateByCountryId = new Dictionary<int, CustomerCountry2>();

            if (customerCountries != null)
            {
                foreach (var customerCountry in customerCountries)
                    customerCountriesSoldDateByCountryId.GetOrCreateItem(customerCountry.CountryId, () =>
                    {
                        return customerCountry;
                    });
            }
            return customerCountriesSoldDateByCountryId;
        }

        private SaleEntityZoneRateLocator GetClosedRateLocator(int customerId, IEnumerable<ZoneChanges> zoneChanges, int sellingProductId)
        {
            var zonesWithClosedRates = zoneChanges.FindAllRecords(item => item.ClosedRates != null && item.ClosedRates.Any());
            if (!zonesWithClosedRates.Any())
                return null;

            DateTime minimumDate = zonesWithClosedRates.Min(item => item.ClosedRates.Min(cr => cr.EED));
            IEnumerable<long> zoneIds = zonesWithClosedRates.Select(item => item.ZoneId);
            Dictionary<long, DateTime> rateActionDateByZoneId = new Dictionary<long, DateTime>();

            foreach (var zoneChange in zonesWithClosedRates)
            {
                rateActionDateByZoneId.GetOrCreateItem(zoneChange.ZoneId, () =>
                {
                    return zoneChange.ClosedRates.Min(item => item.EED);
                });
            }
            return (new SaleEntityZoneRateLocator(new SaleRateReadRPChanges(customerId, sellingProductId, zoneIds, minimumDate, rateActionDateByZoneId)));
        }
        private SaleEntityZoneRateLocator GetNewRateLocator(int customerId, IEnumerable<ZoneChanges> zoneChanges, int sellingProductId)
        {
            var zonesWithNewRates = zoneChanges.FindAllRecords(item => item.NewRates != null && item.NewRates.Any());
            if (!zonesWithNewRates.Any())
                return null;

            DateTime minimumDate = zonesWithNewRates.Min(item => item.NewRates.Min(cr => cr.BED));
            IEnumerable<long> zoneIds = zonesWithNewRates.Select(item => item.ZoneId);
            Dictionary<long, DateTime> rateActionDateByZoneId = new Dictionary<long, DateTime>();

            foreach (var zoneChange in zonesWithNewRates)
            {
                rateActionDateByZoneId.GetOrCreateItem(zoneChange.ZoneId, () =>
                {
                    return zoneChange.NewRates.Min(item => item.BED);
                });
            }
            return (new SaleEntityZoneRateLocator(new SaleRateReadRPChanges(customerId, sellingProductId, zoneIds, minimumDate, rateActionDateByZoneId)));
        }
        private SaleEntityZoneRoutingProductLocator GetZoneRoutingProductLocator(int customerId, IEnumerable<ZoneChanges> zoneChanges)
        {
            var zoneIdsWithActionDate = new Dictionary<long, DateTime>();

            var zonesWithRoutingProductChange = zoneChanges.FindAllRecords(item => item.RoutingProductChange != null);
            if (!zonesWithRoutingProductChange.Any())
                return null;
            foreach (var zoneChange in zonesWithRoutingProductChange)
            {
                zoneIdsWithActionDate.GetOrCreateItem(zoneChange.ZoneId, () =>
                {
                    return zoneChange.RoutingProductChange.EED;
                });
            }
            return (new SaleEntityZoneRoutingProductLocator(new SaleEntityRoutingProductReadByRateBED(new List<int>() { customerId }, zoneIdsWithActionDate)));
        }

        private SaleEntityZoneRoutingProductLocator GetZoneNewRateRoutingProductLocator(int customerId, int publisherId, IEnumerable<ZoneChanges> zoneChanges)
        {
            var zoneIdsWithActionDate = new Dictionary<long, DateTime>();

            var zonesWithNewNormalRateChange = zoneChanges.FindAllRecords(item => item.NewRates != null && item.NewRates.Any(x => x.RateTypeId == null));
            if (!zonesWithNewNormalRateChange.Any())
                return null;
            foreach (var zoneChange in zonesWithNewNormalRateChange)
            {
                zoneIdsWithActionDate.GetOrCreateItem(zoneChange.ZoneId, () =>
                {
                    return zoneChange.NewRates.FindRecord(x => x.RateTypeId == null).BED;
                });
            }
            return (new SaleEntityZoneRoutingProductLocator(new SaleEntityRoutingProductReadByRateBED(new List<int>() { customerId, publisherId }, zoneIdsWithActionDate)));
        }
        private void AddExcludedCountry(Dictionary<int, ExcludedItem> excludedCountries, int excludedNewCountryId, string reason, long processInstanceId)
        {
            CountryManager countryManager = new CountryManager();
            var excludedCountry = excludedCountries.GetRecord(excludedNewCountryId);
            if (excludedCountry == null)
            {

                excludedCountries.Add(excludedNewCountryId, new ExcludedItem()
                {
                    ItemId = excludedNewCountryId.ToString(),
                    ItemName = countryManager.GetCountryName(excludedNewCountryId),
                    ItemType = ExcludedItemTypeEnum.Country,
                    Reason = reason,
                    ProcessInstanceId = processInstanceId
                });
            }
        }

        private void AddExcludedZone(Dictionary<int, List<ExcludedItem>> excludedZones, long zoneId, string reason, int? parentId, long processInstanceId)
        {
            var currentZone = _saleZoneManager.GetSaleZone(zoneId);
            currentZone.ThrowIfNull("currentZone");
            var excludedZoneItem = new ExcludedItem()
            {
                ItemId = zoneId.ToString(),
                ItemName = currentZone.Name,
                ItemType = ExcludedItemTypeEnum.Zone,
                Reason = reason,
                ParentId = parentId,
                ProcessInstanceId = processInstanceId
            };
            var excludedCountryZoneChanges = excludedZones.GetOrCreateItem(currentZone.CountryId);
            excludedCountryZoneChanges.Add(excludedZoneItem);
        }

        private Dictionary<int, CustomerCountry2> GetEffectiveOrFutureCustomerCountriesByCountryId(int customerId, DateTime effectiveDate)
        {
            var customerCountryManager = new CustomerCountryManager();
            Dictionary<int, CustomerCountry2> effectiveOrFutureCustomerCountriesByCountryId = new Dictionary<int, CustomerCountry2>();
            var customerCountries = customerCountryManager.GetEffectiveOrFutureCustomerCountries(customerId, effectiveDate);
            if (customerCountries != null)
            {
                foreach (var customerCountry in customerCountries)
                {
                    effectiveOrFutureCustomerCountriesByCountryId.Add(customerCountry.CountryId, customerCountry);
                }
            }
            return effectiveOrFutureCustomerCountriesByCountryId;
        }
        private Dictionary<int, List<ZoneChanges>> GetPublisherZoneChangesByCountryId(Changes changes)
        {
            Dictionary<int, List<ZoneChanges>> publisherZoneChangesByCountryId = new Dictionary<int, List<ZoneChanges>>();
            if (changes != null && changes.ZoneChanges.Any())
            {
                foreach (ZoneChanges zoneChange in changes.ZoneChanges)
                {
                    var currentZoneChange = publisherZoneChangesByCountryId.GetOrCreateItem(zoneChange.CountryId);
                    currentZoneChange.Add(zoneChange);
                }
            }

            return publisherZoneChangesByCountryId;
        }
    }

}
