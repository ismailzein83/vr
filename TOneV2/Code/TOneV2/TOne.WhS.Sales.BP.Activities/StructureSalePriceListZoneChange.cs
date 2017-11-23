using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Business.Reader;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.BusinessProcess;
using Vanrise.Entities;

namespace TOne.WhS.Sales.BP.Activities
{
    public class StructureSalePriceListZoneChange : CodeActivity
    {
        public InArgument<int?> RerservedSalePriceListId { get; set; }
        public InArgument<int> CurrencyId { get; set; }
        public InArgument<IEnumerable<SaleZone>> SaleZones { get; set; }
        public InArgument<IEnumerable<SaleRate>> SaleRates { get; set; }
        public InArgument<IEnumerable<DataByZone>> DataByZone { get; set; }
        public InArgument<DateTime> MinimumDateTime { get; set; }
        public InArgument<IEnumerable<CustomerCountryToAdd>> CustomerCountriesToAdd { get; set; }
        public InArgument<IEnumerable<CustomerCountryToChange>> CustomerCountriesToChange { get; set; }
        public InArgument<SalePriceListOwnerType> OwnerType { get; set; }
        public InArgument<int> OwnerId { get; set; }
        public InArgument<Changes> Draft { get; set; }
        public InArgument<DefaultRoutingProductToAdd> DefaultRoutingProductToAdd { get; set; }
        public InArgument<DefaultRoutingProductToClose> DefaultRoutingProductToClose { get; set; }
        public InArgument<DateTime> EffectiveOn { get; set; }
        public OutArgument<IEnumerable<NewCustomerPriceListChange>> CustomerChange { get; set; }
        public OutArgument<IEnumerable<NewPriceList>> NewSalePriceList { get; set; }
        public OutArgument<IEnumerable<SalePricelistRPChange>> AllSalePricelistRPChanges { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            #region Getting Arguments From Context

            IRatePlanContext ratePlanContext = context.GetRatePlanContext();
            IEnumerable<DataByZone> dataByZones = DataByZone.Get(context);
            IEnumerable<CustomerCountryToAdd> customerCountriesToAdd = CustomerCountriesToAdd.Get(context);
            IEnumerable<CustomerCountryToChange> customerCountriesToClose = CustomerCountriesToChange.Get(context);
            IEnumerable<SaleZone> saleZones = SaleZones.Get(context);
            IEnumerable<SaleRate> saleRates = SaleRates.Get(context);
            DateTime minimumDate = MinimumDateTime.Get(context);
            int ownerId = this.OwnerId.Get(context);
            SalePriceListOwnerType ownerType = this.OwnerType.Get(context);
            Changes draft = this.Draft.Get(context);
            DateTime effectiveOn = this.EffectiveOn.Get(context);
            int? priceListId = RerservedSalePriceListId.Get(context);
            int currencyId = CurrencyId.Get(context);

            #endregion

            Dictionary<int, List<DataByZone>> importedZonesByCountryId = this.StructureImportedZonesByCountryId(dataByZones);
            Dictionary<int, List<SaleZone>> existingZonesByCountryId = this.StructureExistingZonesByCountryId(saleZones);
            List<SalePricelistRPChange> outRoutingProductChanges = new List<SalePricelistRPChange>();
            List<int> customerIds;
            IEnumerable<RoutingCustomerInfoDetails> dataByCustomer = GetDataByCustomer(ratePlanContext.OwnerType, ratePlanContext.OwnerId, ratePlanContext.EffectiveDate, out customerIds);
            SaleEntityZoneRoutingProductLocator effectiveRoutingProductLocator = new SaleEntityZoneRoutingProductLocator(new SaleEntityRoutingProductReadWithDraft(ownerType, ownerId, effectiveOn, draft, false));
            SaleEntityZoneRoutingProductLocator currenRoutingProductLocator = new SaleEntityZoneRoutingProductLocator(new SaleEntityRoutingProductReadAllNoCache(customerIds, effectiveOn, false));

            List<CustomerPriceListChange> customerPriceListChanges = new List<CustomerPriceListChange>();
            var lastRateNoCachelocator = new SaleEntityZoneRateLocator(new SaleRateReadLastRateNoCache(dataByCustomer, ratePlanContext.EffectiveDate));

            if (dataByCustomer != null)
            {
                #region Getting Pricelist Changes

                if (ratePlanContext.OwnerType == SalePriceListOwnerType.SellingProduct)
                {
                    #region Selling Product

                    //var futureRateLocator = new SaleEntityZoneRateLocator(new SaleRateReadAllNoCache(dataByCustomer, ratePlanContext.EffectiveDate, true));

                    SellingProductChangesContext sellingProductContext = new SellingProductChangesContext
                    {
                        ImportedZonesByCountryId = importedZonesByCountryId,
                        ExistingZonesByCountryId = existingZonesByCountryId,
                        Customers = dataByCustomer,
                        LastRateNoCachelocator = lastRateNoCachelocator,
                        MinimumDate = minimumDate,
                        EffectiveRoutingProductLocator = effectiveRoutingProductLocator,
                        CurrentRoutingProductLocator = currenRoutingProductLocator
                    };

                    customerPriceListChanges = this.GetChangesForSellingProduct(sellingProductContext, out outRoutingProductChanges);

                    #endregion
                }
                else
                {
                    #region Customer

                    #region Preparing Data for Processing

                    RoutingCustomerInfoDetails customerInfo = dataByCustomer.FirstOrDefault();

                    Dictionary<int, CustomerCountryToAdd> countriesToAddByCountryId = customerCountriesToAdd.ToDictionary(x => x.CountryId);
                    Dictionary<int, CustomerCountryToChange> countriesToCloseByCountryId = customerCountriesToClose.ToDictionary(x => x.CountryId);
                    StructuredZoneActions structuredZoneActions = this.GetZoneActions(importedZonesByCountryId, countriesToAddByCountryId, countriesToCloseByCountryId);

                    ExistingDataInfo existingDataInfo = this.BuildExistingDataInfo(structuredZoneActions, customerCountriesToAdd, customerCountriesToClose, existingZonesByCountryId, saleRates);

                    CustomerPriceListChange changesForThisCustomer = new CustomerPriceListChange
                    {
                        CustomerId = customerInfo.CustomerId
                    };

                    #endregion

                    #region Preparing Rate Change Locator

                    SaleEntityZoneRateLocator rateChangeLocator = null;
                    if (customerCountriesToClose.Any() || structuredZoneActions.RatesToAdd.Any() || structuredZoneActions.RatesToClose.Any())
                    {
                        List<long> zoneIds = new List<long>();
                        zoneIds.AddRange(existingDataInfo.CountriesToCloseByZoneIds.Values.SelectMany(z => z));
                        zoneIds.AddRange(existingDataInfo.RateActionsExistingZoneIds);

                        rateChangeLocator = new SaleEntityZoneRateLocator(new SaleRateReadRPChanges(existingDataInfo.CustomerRates, customerInfo,
                            zoneIds, minimumDate, existingDataInfo.ActionDatesByZoneId));
                    }

                    #endregion

                    #region Processing Countries Actions

                    #region Get Existing Sale Codes for Sold and Close Countries

                    Dictionary<long, List<SaleCode>> saleCodesByZoneId = null;
                    if (customerCountriesToAdd.Any() || customerCountriesToClose.Any())
                    {
                        IEnumerable<SaleCode> existingSaleCodes = this.GetExistingSaleCodes(existingDataInfo, minimumDate);
                        saleCodesByZoneId = this.StructureExistingSaleCodesByZoneId(existingSaleCodes);
                    }

                    #endregion

                    #region Get Changes from New Countries

                    if (customerCountriesToAdd.Any())
                    {
                        CustomerNewCountriesChangesContext customerNewCountriesContext = new CustomerNewCountriesChangesContext()
                        {
                            CountriesToAdd = customerCountriesToAdd,
                            CountriesToAddExistingZoneIdsByCountryId = existingDataInfo.CountriesToAddExistingZoneIdsByCountryId,
                            CustomerInfo = customerInfo,
                            MinimumDate = minimumDate,
                            RatesToAddForNewCountriesbyCountryId = structuredZoneActions.RatesToAddForNewCountriesbyCountryId,
                            SaleCodesByZoneId = saleCodesByZoneId,
                            CurrencyId = ratePlanContext.CurrencyId,
                            RoutingProductEffectiveLocator = effectiveRoutingProductLocator
                        };

                        this.GetChangesForNewCountries(customerNewCountriesContext);
                        outRoutingProductChanges.AddRange(customerNewCountriesContext.RoutingProductChanges);
                        changesForThisCustomer.RoutingProductChanges.AddRange(customerNewCountriesContext.RoutingProductChanges);
                        changesForThisCustomer.RateChanges.AddRange(customerNewCountriesContext.RateChangesOutArgument);
                        changesForThisCustomer.CodeChanges.AddRange(customerNewCountriesContext.CodeChangesOutArgument);
                    }

                    #endregion

                    #region Get Changes from Closed Countries

                    if (customerCountriesToClose.Any())
                    {
                        CustomerCountriesToCloseChangesContext customerCountriesToCloseContext = new CustomerCountriesToCloseChangesContext()
                        {
                            CountriesToClose = customerCountriesToClose,
                            CountriesToCloseByZoneIds = existingDataInfo.CountriesToCloseByZoneIds,
                            CustomerInfo = customerInfo,
                            ProcessEffectiveDate = ratePlanContext.EffectiveDate,
                            RateChangeLocator = rateChangeLocator,
                            SaleCodesByZoneId = saleCodesByZoneId,
                            CurrencyId = ratePlanContext.CurrencyId,
                            ActionDatesByZoneId = existingDataInfo.ActionDatesByZoneId
                        };

                        this.GetChangesForCountriesToClose(customerCountriesToCloseContext);

                        changesForThisCustomer.RateChanges.AddRange(customerCountriesToCloseContext.RateChangesOutArgument);
                        changesForThisCustomer.CodeChanges.AddRange(customerCountriesToCloseContext.CodeChangesOutArgument);
                    }

                    #endregion

                    #endregion

                    #region Processing Rates Actions

                    if (structuredZoneActions.RatesToAdd.Any() || structuredZoneActions.RatesToClose.Any() || structuredZoneActions.RatesToChange.Any())
                    {
                        CustomerRateActionChangesContext customerRateActionContext = new CustomerRateActionChangesContext()
                        {
                            CustomerInfo = customerInfo,
                            RateChangeLocator = rateChangeLocator,
                            StructuredRateActions = structuredZoneActions,
                            CurrencyId = ratePlanContext.CurrencyId
                        };
                        this.GetChangesForRateActions(customerRateActionContext);
                        changesForThisCustomer.RateChanges.AddRange(customerRateActionContext.RateChangesOutArgument);
                    }

                    #endregion

                    #region Processing Routing Product Actions

                    var customerCountryManager = new CustomerCountryManager();
                    var soldCountries = customerCountryManager.GetCustomerCountriesEffectiveAfter(customerInfo.CustomerId, minimumDate);
                    if (soldCountries != null)
                    {
                        List<SalePricelistRPChange> routingProductChanges = GetRoutingProductChanges(customerInfo.CustomerId, customerInfo.SellingProductId,
                            soldCountries.Select(x => x.CountryId), existingZonesByCountryId, effectiveRoutingProductLocator, currenRoutingProductLocator);
                        outRoutingProductChanges.AddRange(routingProductChanges);
                        changesForThisCustomer.RoutingProductChanges.AddRange(routingProductChanges);
                    }

                    #endregion

                    //Add routing productID to SaleRateChange
                    if (changesForThisCustomer.RateChanges.Any())
                    {
                        SetRoutingProductIdOnRateChange(customerInfo.CustomerId, customerInfo.SellingProductId, changesForThisCustomer.RateChanges, changesForThisCustomer.RoutingProductChanges);
                        customerPriceListChanges.Add(changesForThisCustomer);
                    }

                    #endregion
                }

                #endregion
            }

            var salePriceListManager = new SalePriceListManager();
            long processInstanceId = context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID;
            int userId = context.GetSharedInstanceData().InstanceInfo.InitiatorUserId;

            var pricelistByCurrencyId = CreatePriceList(ownerId, ownerType, priceListId, currencyId, processInstanceId, userId);
            var structuredCustomers = salePriceListManager.StructureCustomerPricelistChange(customerPriceListChanges);
            var changes = salePriceListManager.CreateCustomerChanges(structuredCustomers, lastRateNoCachelocator, pricelistByCurrencyId, effectiveOn, processInstanceId, userId);

            GetPricelistDescription(changes, customerCountriesToAdd, customerCountriesToClose);

            NewSalePriceList.Set(context, pricelistByCurrencyId.Values.SelectMany(p => p));
            CustomerChange.Set(context, changes);
            AllSalePricelistRPChanges.Set(context, outRoutingProductChanges);
        }

        #region Get Pricelist Description
        private void GetPricelistDescription(List<NewCustomerPriceListChange> customerChanges, IEnumerable<CustomerCountryToAdd> customerCountriesToAdd, IEnumerable<CustomerCountryToChange> customerCountriesToClose)
        {
            foreach (NewCustomerPriceListChange customerChange in customerChanges)
            {
                foreach (PriceListChange pricelist in customerChange.PriceLists)
                {
                    int newCountriesCounter = 0;
                    int closedCountriesCounter = 0;
                    int newRatesCounter = 0;
                    int increasedRatesCounter = 0;
                    int decreasedRatesCounter = 0;

                    foreach (CountryChange countryChange in pricelist.CountryChanges)
                    {
                        if (customerCountriesToAdd.Where(item => item.CountryId == countryChange.CountryId && item.CustomerId == customerChange.CustomerId).Count() > 0)
                            newCountriesCounter++;
                        else if (customerCountriesToClose.Where(item => item.CountryId == countryChange.CountryId).Count() > 0)
                            closedCountriesCounter++;

                        foreach (SalePricelistZoneChange zoneChange in countryChange.ZoneChanges)
                        {
                            if (zoneChange.RateChange == null)
                                continue;

                            if (zoneChange.RateChange.ChangeType == RateChangeType.New)
                                newRatesCounter++;
                            else if (zoneChange.RateChange.ChangeType == RateChangeType.Increase)
                                increasedRatesCounter++;
                            else if (zoneChange.RateChange.ChangeType == RateChangeType.Decrease)
                                decreasedRatesCounter++;
                        }
                    }
                    pricelist.PriceList.Description = GetPricelistDescriptionString(newCountriesCounter, closedCountriesCounter, newRatesCounter, increasedRatesCounter, decreasedRatesCounter);
                }
            }
        }

        private string GetPricelistDescriptionString(int newCountriesCounter, int closedCountriesCounter, int newRatesCounter, int increasedRatesCounter, int decreasedRatesCounter)
        {
            string result = "";
            if (newCountriesCounter != 0)
                result = string.Format(result + "New Countries : {0} ", newCountriesCounter);
            if (closedCountriesCounter != 0)
                result = string.Format(result + "Closed Countries : {0} ", closedCountriesCounter);
            if (newRatesCounter != 0)
                result = string.Format(result + "New Rates : {0} ", newRatesCounter);
            if (increasedRatesCounter != 0)
                result = string.Format(result + "Increased Rates : {0} ", increasedRatesCounter);
            if (decreasedRatesCounter != 0)
                result = string.Format(result + "Decreased Rates : {0} ", decreasedRatesCounter);
            return result;
        }
        #endregion

        #region Get Pricelist Changes from Selling Product Methods

        private List<CustomerPriceListChange> GetChangesForSellingProduct(SellingProductChangesContext context, out List<SalePricelistRPChange> outRoutingProductChanges)
        {
            outRoutingProductChanges = new List<SalePricelistRPChange>();
            var customerCountryManager = new CustomerCountryManager();
            List<CustomerPriceListChange> customerPriceListChanges = new List<CustomerPriceListChange>();

            foreach (var customer in context.Customers)
            {
                var soldCountries = customerCountryManager.GetNotEndedCustomerCountriesEffectiveAfter(customer.CustomerId, context.MinimumDate);
                if (soldCountries == null)
                    continue;

                List<SalePricelistRateChange> rateChanges = this.GetRateChangesForCustomer(customer.CustomerId, customer.SellingProductId, soldCountries,
                    context.ImportedZonesByCountryId, context.LastRateNoCachelocator);

                IEnumerable<SalePricelistRPChange> routingProductChanges = GetRoutingProductChanges(customer.CustomerId, customer.SellingProductId,
                    soldCountries.Select(itm => itm.CountryId), context.ExistingZonesByCountryId, context.EffectiveRoutingProductLocator, context.CurrentRoutingProductLocator);
                outRoutingProductChanges.AddRange(routingProductChanges);
                if (rateChanges.Any())
                {
                    CustomerPriceListChange changesForThisCustomer = new CustomerPriceListChange
                    {
                        CustomerId = customer.CustomerId
                    };
                    if (routingProductChanges.Any())
                    {
                        SetRoutingProductIdOnRateChange(customer.CustomerId, customer.SellingProductId, rateChanges, routingProductChanges);
                        changesForThisCustomer.RoutingProductChanges.AddRange(routingProductChanges);
                    }
                    changesForThisCustomer.RateChanges.AddRange(rateChanges);
                    customerPriceListChanges.Add(changesForThisCustomer);
                }
            }

            return customerPriceListChanges;
        }

        private List<SalePricelistRateChange> GetRateChangesForCustomer(int customerId, int sellingProductId, IEnumerable<CustomerCountry2> soldCountries,
            Dictionary<int, List<DataByZone>> importedZonesByCountryId, SaleEntityZoneRateLocator lastRateNoCachelocator)
        {
            List<SalePricelistRateChange> rateChanges = new List<SalePricelistRateChange>();
            var saleRateManager = new SaleRateManager();

            foreach (var country in soldCountries)
            {
                IEnumerable<DataByZone> zones = importedZonesByCountryId.GetRecord(country.CountryId);

                if (zones == null)
                    continue;

                foreach (var zone in zones)
                {
                    SaleEntityZoneRate zoneRate = lastRateNoCachelocator.GetCustomerZoneRate(customerId, sellingProductId, zone.ZoneId);

                    if (zoneRate == null)
                        throw new DataIntegrityValidationException(string.Format("Zone {0} does neither have an explicit rate nor a default rate set for selling product. Additional info: customer with id {1}", zone.ZoneName, customerId));

                    if (zoneRate.Source == SalePriceListOwnerType.Customer)
                        continue; // customer has explicit rate and no need to notify him with this change

                    if (zone.NormalRateToChange != null && zone.NormalRateToChange.RateTypeId == null)
                    {
                        SalePricelistRateChange rateChange = new SalePricelistRateChange
                        {
                            CountryId = zone.CountryId,
                            ZoneId = zone.ZoneId,
                            ZoneName = zone.ZoneName,
                            Rate = zone.NormalRateToChange.NormalRate,
                            ChangeType = zone.NormalRateToChange.ChangeType,
                            //TODO: make sure if the explicit rate has EED, if we need to make the BED of the new rate starts from EED of explicit rate
                            BED = (country.BED > zone.NormalRateToChange.BED) ? country.BED : zone.NormalRateToChange.BED,
                            EED = zone.NormalRateToChange.EED,
                            CurrencyId = saleRateManager.GetCurrencyId(zoneRate.Rate)
                        };

                        if (zone.NormalRateToChange.RecentExistingRate != null)
                            rateChange.RecentRate = zone.NormalRateToChange.RecentExistingRate.ConvertedRate;

                        rateChanges.Add(rateChange);
                    }
                }
            }

            return rateChanges;
        }

        #endregion

        #region Get Pricelist Changes from Customer Methods

        private Dictionary<int, List<NewPriceList>> CreatePriceList(int ownerId, SalePriceListOwnerType ownerType, int? reservedId, int currencyId, long processInstanceId
            , int userId)
        {
            Dictionary<int, List<NewPriceList>> priceListByCurrencyId = new Dictionary<int, List<NewPriceList>>();
            if (reservedId.HasValue)
            {
                NewPriceList newPricelist = new NewPriceList
                 {
                     OwnerId = ownerId,
                     PriceListId = reservedId.Value,
                     CurrencyId = currencyId,
                     OwnerType = ownerType,
                     //PriceListType = salePriceListType,
                     EffectiveOn = DateTime.Now,
                     ProcessInstanceId = processInstanceId,
                     UserId = userId
                 };
                priceListByCurrencyId.Add(currencyId, new List<NewPriceList> { newPricelist });
            }
            return priceListByCurrencyId;
        }

        private void GetChangesForNewCountries(CustomerNewCountriesChangesContext context)
        {
            context.RateChangesOutArgument = new List<SalePricelistRateChange>();
            context.CodeChangesOutArgument = new List<SalePricelistCodeChange>();
            context.RoutingProductChanges = new List<SalePricelistRPChange>();
            var lastRateLocator = new SaleEntityZoneRateLocator(new SaleRateReadLastRateNoCache(new List<RoutingCustomerInfoDetails> { context.CustomerInfo }, context.MinimumDate));
            var saleZoneManager = new SaleZoneManager();
            var saleRateManager = new SaleRateManager();

            foreach (var countryToAdd in context.CountriesToAdd)
            {
                #region Get Customer Rate Changes

                List<RateToChange> explicitRates = context.RatesToAddForNewCountriesbyCountryId.GetRecord(countryToAdd.CountryId);
                List<long> zoneIdsWithExplicitRates = new List<long>();
                if (explicitRates != null)
                {
                    //These are the rates that are added explicitly for this customer after selling the country
                    foreach (var rate in explicitRates)
                    {
                        zoneIdsWithExplicitRates.Add(rate.ZoneId);
                        context.RateChangesOutArgument.Add(new SalePricelistRateChange
                        {
                            CountryId = countryToAdd.CountryId,
                            ZoneId = rate.ZoneId,
                            ZoneName = rate.ZoneName,
                            Rate = rate.NormalRate,
                            ChangeType = RateChangeType.New,
                            BED = rate.BED,
                            EED = rate.EED,
                            CurrencyId = rate.CurrencyId
                        });
                    }
                }

                #endregion

                #region Get Selling Product Rate and Code Changes

                List<long> zoneIdsForThisCountry =
                    context.CountriesToAddExistingZoneIdsByCountryId.GetRecord(countryToAdd.CountryId);

                foreach (var zoneId in zoneIdsForThisCountry)
                {
                    string zoneName = saleZoneManager.GetSaleZoneName(zoneId);
                    var zoneEntity = saleZoneManager.GetSaleZone(zoneId);
                    if (!zoneIdsWithExplicitRates.Contains(zoneId))
                    {
                        //Ignore zones that have explicit rates
                        var zoneRate = lastRateLocator.GetSellingProductZoneRate(context.CustomerInfo.SellingProductId, zoneId);
                        if (zoneRate == null)
                            throw new VRBusinessException(string.Format("Zone {0} has no rates set neither for customer nor for selling product", zoneName));

                        context.RateChangesOutArgument.Add(new SalePricelistRateChange
                        {
                            CountryId = countryToAdd.CountryId,
                            ZoneId = zoneId,
                            ZoneName = zoneName,
                            Rate = zoneRate.Rate.Rate,
                            ChangeType = RateChangeType.New,
                            BED = countryToAdd.BED > zoneRate.Rate.BED ? countryToAdd.BED : zoneRate.Rate.BED,
                            CurrencyId = saleRateManager.GetCurrencyId(zoneRate.Rate)
                        });

                    }

                    IEnumerable<SaleCode> zoneCodes = context.SaleCodesByZoneId.GetRecord(zoneId);
                    if (zoneCodes == null)
                        throw new DataIntegrityValidationException(string.Format("Zone {0} has no existing codes.", zoneName));

                    foreach (var existingCode in zoneCodes)
                    {
                        context.CodeChangesOutArgument.Add(new SalePricelistCodeChange
                        {
                            CountryId = countryToAdd.CountryId,
                            ZoneName = zoneName,
                            ZoneId = existingCode.ZoneId,
                            Code = existingCode.Code,
                            ChangeType = CodeChange.New,
                            BED = existingCode.BED > countryToAdd.BED ? existingCode.BED : countryToAdd.BED
                        });
                    }
                    SaleEntityZoneRoutingProduct effectiveRoutingProduct = context.RoutingProductEffectiveLocator.GetCustomerZoneRoutingProduct(context.CustomerInfo.CustomerId, context.CustomerInfo.SellingProductId, zoneId);

                    if (effectiveRoutingProduct == null)
                        throw new VRBusinessException(string.Format("No routing product assigned for zone {0}", zoneName));
                    var BEDs = new List<DateTime?>();
                    BEDs.Add(countryToAdd.BED);
                    BEDs.Add(effectiveRoutingProduct.BED);
                    BEDs.Add(zoneEntity.BED);
                    var routingProduct = new SalePricelistRPChange
                    {
                        CountryId = countryToAdd.CountryId,
                        ZoneName = zoneName,
                        ZoneId = zoneId,
                        RoutingProductId = effectiveRoutingProduct.RoutingProductId,
                        BED = UtilitiesManager.GetMaxDate(BEDs).Value,
                        EED = null, //TODO: this is not reflecting the correct value now, if the def customer ro for example is closed in the future
                        CustomerId = context.CustomerInfo.CustomerId
                    };
                    context.RoutingProductChanges.Add(routingProduct);
                }

                #endregion
            }
        }

        private IEnumerable<SaleCode> GetExistingSaleCodes(ExistingDataInfo existingDataInfo, DateTime minimumDate)
        {
            var zoneIds = new List<long>();

            zoneIds.AddRange(existingDataInfo.CountriesToAddExistingZoneIdsByCountryId.Values.SelectMany(z => z));
            zoneIds.AddRange(existingDataInfo.CountriesToCloseByZoneIds.Values.SelectMany(z => z));

            SaleCodeManager saleCodeManager = new SaleCodeManager();
            var saleCodes = saleCodeManager.GetSaleCodesByZoneIDs(zoneIds, minimumDate);
            return saleCodes;
        }

        private void GetChangesForCountriesToClose(CustomerCountriesToCloseChangesContext context)
        {
            context.RateChangesOutArgument = new List<SalePricelistRateChange>();
            context.CodeChangesOutArgument = new List<SalePricelistCodeChange>();

            SaleZoneManager saleZoneManager = new SaleZoneManager();
            CustomerCountryManager customerCountryManager = new CustomerCountryManager();
            var saleRateManager = new SaleRateManager();

            foreach (var countryToClose in context.CountriesToClose)
            {
                CustomerCountry2 soldCountry = customerCountryManager.GetCustomerCountry(context.CustomerInfo.CustomerId, countryToClose.CountryId, context.ProcessEffectiveDate, true);
                
                List<long> zonesForThisCountry;
                if (!context.CountriesToCloseByZoneIds.TryGetValue(countryToClose.CountryId, out zonesForThisCountry))
                    continue;

                foreach (var zoneId in zonesForThisCountry)
                {
                    string zoneName = saleZoneManager.GetSaleZoneName(zoneId);

                    #region Get Rate Changes

                    var zoneRate = context.RateChangeLocator.GetCustomerZoneRate(context.CustomerInfo.CustomerId, context.CustomerInfo.SellingProductId, zoneId);
                    if (zoneRate == null)
                        throw new VRBusinessException(string.Format("Zone '{0}' neither has an explicit rate nor has selling product rate. Country is sold to customer with id {1}", zoneName, context.CustomerInfo.CustomerId));

                    context.RateChangesOutArgument.Add(new SalePricelistRateChange
                    {
                        CountryId = countryToClose.CountryId,
                        ZoneId = zoneId,
                        ZoneName = zoneName,
                        Rate = zoneRate.Rate.Rate,
                        ChangeType = RateChangeType.Deleted,
                        BED = (soldCountry.BED > zoneRate.Rate.BED) ? soldCountry.BED : zoneRate.Rate.BED, //TODO: The only gap found here if there was a rate explicit closed at a time and another inherited. This way the BED of the sent rate will be sell date or SP rate BED and not the closure date of the explicit rate
                        EED = context.ActionDatesByZoneId[zoneId],// countryToClose.CloseEffectiveDate,
                        CurrencyId = saleRateManager.GetCurrencyId(zoneRate.Rate)
                    });

                    #endregion

                    #region Get Code Changes

                    IEnumerable<SaleCode> zoneCodes = context.SaleCodesByZoneId.GetRecord(zoneId);
                    if (zoneCodes == null)
                        throw new DataIntegrityValidationException(string.Format("Zone {0} has no existing codes.", zoneName));

                    foreach (var existingCode in zoneCodes)
                    {
                        context.CodeChangesOutArgument.Add(new SalePricelistCodeChange
                        {
                            CountryId = countryToClose.CountryId,
                            ZoneName = zoneName,
                            ZoneId = existingCode.ZoneId,
                            Code = existingCode.Code,
                            ChangeType = CodeChange.Closed,
                            BED = existingCode.BED > soldCountry.BED ? existingCode.BED : soldCountry.BED,
                            EED = countryToClose.CloseEffectiveDate
                        });
                    }

                    #endregion
                }
            }
        }

        private void GetChangesForRateActions(CustomerRateActionChangesContext context)
        {
            context.RateChangesOutArgument = new List<SalePricelistRateChange>();
            SaleZoneManager saleZoneManager = new SaleZoneManager();
            var currencyExchangeRateManager = new CurrencyExchangeRateManager();
            var saleRateManager = new SaleRateManager();
            int longPrecision = new Vanrise.Common.Business.GeneralSettingsManager().GetLongPrecisionValue();


            #region Processing Rate To Change Increase and Decrease

            foreach (var rateToChange in context.StructuredRateActions.RatesToChange)
            {
                int? countryId = saleZoneManager.GetSaleZoneCountryId(rateToChange.ZoneId);
                if (countryId == null)
                    throw new DataIntegrityValidationException(string.Format("Zone with Id {0} is not assigned to any country", rateToChange.ZoneId));


                context.RateChangesOutArgument.Add(new SalePricelistRateChange
                {
                    CountryId = countryId.Value,
                    ZoneId = rateToChange.ZoneId,
                    ZoneName = rateToChange.ZoneName,
                    Rate = rateToChange.NormalRate,
                    RecentRate = rateToChange.RecentExistingRate.ConvertedRate,
                    ChangeType = rateToChange.ChangeType,
                    BED = rateToChange.BED,
                    EED = rateToChange.EED,
                    CurrencyId = context.CurrencyId
                });
            }

            #endregion

            #region Processing Rate To Add

            foreach (var rateToAdd in context.StructuredRateActions.RatesToAdd)
            {
                int? countryId = saleZoneManager.GetSaleZoneCountryId(rateToAdd.ZoneId);
                if (countryId == null)
                    throw new DataIntegrityValidationException(string.Format("Zone with Id {0} is not assigned to any country", rateToAdd.ZoneId));

                var recentRate = context.RateChangeLocator.GetSellingProductZoneRate(context.CustomerInfo.SellingProductId, rateToAdd.ZoneId);
                if (recentRate == null)
                    throw new VRBusinessException(string.Format("Zone {0} does neither have an explicit rate nor a default rate set for selling product", rateToAdd.ZoneName));

                var salePricelistRateChange = new SalePricelistRateChange
                {
                    CountryId = countryId.Value,
                    ZoneId = rateToAdd.ZoneId,
                    ZoneName = rateToAdd.ZoneName,
                    Rate = rateToAdd.NormalRate,
                    RecentRate = recentRate.Rate.Rate,
                    BED = rateToAdd.BED,
                    EED = null,
                    CurrencyId = context.CurrencyId
                };

                Decimal convertedRate = UtilitiesManager.ConvertToCurrencyAndRound(recentRate.Rate.Rate, saleRateManager.GetCurrencyId(recentRate.Rate), context.CurrencyId, DateTime.Now, longPrecision, currencyExchangeRateManager);

                if (rateToAdd.NormalRate > convertedRate)
                    salePricelistRateChange.ChangeType = RateChangeType.Increase;
                else if (rateToAdd.NormalRate < convertedRate)
                    salePricelistRateChange.ChangeType = RateChangeType.Decrease;

                context.RateChangesOutArgument.Add(salePricelistRateChange);
            }

            #endregion

            #region Processing Rate To Close

            foreach (var rateToClose in context.StructuredRateActions.RatesToClose)
            {
                int? countryId = saleZoneManager.GetSaleZoneCountryId(rateToClose.ZoneId);
                if (countryId == null)
                    throw new DataIntegrityValidationException(string.Format("Zone with Id {0} is not assigned to any country", rateToClose.ZoneId));

                var newRate = context.RateChangeLocator.GetSellingProductZoneRate(context.CustomerInfo.SellingProductId, rateToClose.ZoneId);
                if (newRate == null)
                    throw new VRBusinessException(string.Format("Zone {0} does neither have an explicit rate nor a default rate set for selling product", rateToClose.ZoneName));

                var recentRate = context.RateChangeLocator.GetCustomerZoneRate(context.CustomerInfo.CustomerId, context.CustomerInfo.SellingProductId, rateToClose.ZoneId);

                var salePriceListRateChange = new SalePricelistRateChange
                {
                    CountryId = countryId.Value,
                    ZoneId = rateToClose.ZoneId,
                    ZoneName = rateToClose.ZoneName,
                    Rate = newRate.Rate.Rate,
                    RecentRate = UtilitiesManager.ConvertToCurrencyAndRound(recentRate.Rate.Rate,
                        saleRateManager.GetCurrencyId(recentRate.Rate), context.CurrencyId, DateTime.Now, longPrecision,
                        currencyExchangeRateManager),
                    BED = rateToClose.CloseEffectiveDate,
                    EED = null,
                    CurrencyId = saleRateManager.GetCurrencyId(newRate.Rate)
                };

                Decimal convertedRate = UtilitiesManager.ConvertToCurrencyAndRound(newRate.Rate.Rate, saleRateManager.GetCurrencyId(newRate.Rate), context.CurrencyId, DateTime.Now, longPrecision, currencyExchangeRateManager);

                if (convertedRate > salePriceListRateChange.RecentRate)
                    salePriceListRateChange.ChangeType = RateChangeType.Increase;
                else if (convertedRate < salePriceListRateChange.RecentRate)
                    salePriceListRateChange.ChangeType = RateChangeType.Decrease;

                context.RateChangesOutArgument.Add(salePriceListRateChange);
            }

            #endregion
        }

        #endregion

        #region Private Methods

        private void SetRoutingProductIdOnRateChange(int customerId, int sellingProductId, List<SalePricelistRateChange> rateChanges, IEnumerable<SalePricelistRPChange> routingProductChanges)
        {
            Dictionary<long, DateTime> zoneIdsWithRateBED = StructureRateChangeByzoneIdWithRateBED(rateChanges);
            SaleEntityZoneRoutingProductLocator routingProductLocatorByRateBED = new SaleEntityZoneRoutingProductLocator(new SaleEntityRoutingProductReadByRateBED(new List<int> { customerId }, zoneIdsWithRateBED));

            var routingProductChangesByZoneName = StructureCustomerSaleRpChangesByZoneName(routingProductChanges);
            foreach (var rateChange in rateChanges)
            {
                SalePricelistRPChange routinProductChange = routingProductChangesByZoneName.GetRecord(rateChange.ZoneName);
                if (routinProductChange != null)
                    rateChange.RoutingProductId = routinProductChange.RoutingProductId;
                else
                {
                    var saleEntityZoneRoutingProduct = routingProductLocatorByRateBED.GetCustomerZoneRoutingProduct(customerId, sellingProductId, rateChange.ZoneId.Value);
                    if (saleEntityZoneRoutingProduct != null)
                        rateChange.RoutingProductId = saleEntityZoneRoutingProduct.RoutingProductId;
                    else
                    {
                        throw new Exception(string.Format("No routing product assigned for customer {0}", customerId));
                    }
                }

            }
        }
        private Dictionary<string, SalePricelistRPChange> StructureCustomerSaleRpChangesByZoneName(IEnumerable<SalePricelistRPChange> routingProductChanges)
        {
            Dictionary<string, SalePricelistRPChange> routingProductChangesByZoneName = new Dictionary<string, SalePricelistRPChange>();
            foreach (var rpChange in routingProductChanges)
            {
                if (!routingProductChangesByZoneName.ContainsKey(rpChange.ZoneName))
                    routingProductChangesByZoneName.Add(rpChange.ZoneName, rpChange);
            }
            return routingProductChangesByZoneName;
        }

        private Dictionary<long, DateTime> StructureRateChangeByzoneIdWithRateBED(IEnumerable<SalePricelistRateChange> rateChanges)
        {
            Dictionary<long, DateTime> zoneIdByRateBED = new Dictionary<long, DateTime>();
            foreach (var rateChange in rateChanges)
            {
                if (!rateChange.ZoneId.HasValue) continue;
                DateTime rateBED;
                if (!zoneIdByRateBED.TryGetValue(rateChange.ZoneId.Value, out rateBED))
                    zoneIdByRateBED.Add(rateChange.ZoneId.Value, rateChange.BED);

            }
            return zoneIdByRateBED;
        }
        private StructuredZoneActions GetZoneActions(Dictionary<int, List<DataByZone>> importedZonesByCountryId, Dictionary<int, CustomerCountryToAdd> countriesToAdd, Dictionary<int, CustomerCountryToChange> countriesToClose)
        {
            StructuredZoneActions zoneActions = new StructuredZoneActions();

            foreach (KeyValuePair<int, List<DataByZone>> kvp in importedZonesByCountryId)
            {
                int countryId = kvp.Key;
                IEnumerable<DataByZone> importedZones = kvp.Value;

                if (countriesToClose.ContainsKey(countryId))
                {
                    //If country is closed ignore all rate actions
                    continue;
                }
                else if (countriesToAdd.ContainsKey(countryId))
                {
                    //It is a new country, get only new rates added for this country
                    zoneActions.RatesToAddForNewCountriesbyCountryId.Add(countryId, this.GetRatesToAddFromImportedZones(importedZones));
                }
                else
                {
                    //Get all rates changes and closed rates for this country
                    zoneActions.RatesToAdd.AddRange(this.GetRatesToAddFromImportedZones(importedZones));
                    zoneActions.RatesToChange.AddRange(this.GetRatesToChangeFromImportedZones(importedZones));
                    zoneActions.RatesToClose.AddRange(this.GetRatestoCloseFromImportedZones(importedZones));
                }

                zoneActions.ZoneRoutingProductsToAdd.AddRange(importedZones.Where(x => x.SaleZoneRoutingProductToAdd != null).Select(x => x.SaleZoneRoutingProductToAdd));
                zoneActions.ZoneRoutinProductsToClose.AddRange(importedZones.Where(x => x.SaleZoneRoutingProductToClose != null).Select(x => x.SaleZoneRoutingProductToClose));
            }

            return zoneActions;
        }

        private List<RateToChange> GetRatesToAddFromImportedZones(IEnumerable<DataByZone> importedZones)
        {
            List<RateToChange> ratesToChange = new List<RateToChange>();

            foreach (var zone in importedZones)
            {
                if (zone.NormalRateToChange != null && zone.NormalRateToChange.RateTypeId == null && zone.NormalRateToChange.ChangeType == RateChangeType.New)
                    ratesToChange.Add(zone.NormalRateToChange);
            }

            return ratesToChange;
        }

        private List<RateToChange> GetRatesToChangeFromImportedZones(IEnumerable<DataByZone> importedZones)
        {
            List<RateToChange> ratesToChange = new List<RateToChange>();

            foreach (var zone in importedZones)
            {
                if (zone.NormalRateToChange != null && zone.NormalRateToChange.RateTypeId == null &&
                    (zone.NormalRateToChange.ChangeType == RateChangeType.Increase || zone.NormalRateToChange.ChangeType == RateChangeType.Decrease))
                {
                    ratesToChange.Add(zone.NormalRateToChange);
                }
            }

            return ratesToChange;
        }

        private IEnumerable<RateToClose> GetRatestoCloseFromImportedZones(IEnumerable<DataByZone> importedZones)
        {
            List<RateToClose> ratesToClose = new List<RateToClose>();

            foreach (var zone in importedZones)
            {
                if (zone.NormalRateToClose != null && zone.NormalRateToClose.RateTypeId == null)
                    ratesToClose.Add(zone.NormalRateToClose);
            }

            return ratesToClose;
        }

        private ExistingDataInfo BuildExistingDataInfo(StructuredZoneActions structuredRateActions, IEnumerable<CustomerCountryToAdd> countriesToAdd,
            IEnumerable<CustomerCountryToChange> countriesToClose, Dictionary<int, List<SaleZone>> existingZonesByCountryId, IEnumerable<SaleRate> saleRates)
        {
            ExistingDataInfo info = new ExistingDataInfo();
            Dictionary<long, List<SaleRate>> existingRatesByZoneId = this.StructureExistingRatesByZoneId(saleRates);

            #region Fill Info from Rates to Add

            foreach (var rateToAdd in structuredRateActions.RatesToAdd)
            {
                info.RateActionsExistingZoneIds.Add(rateToAdd.ZoneId);
                info.ActionDatesByZoneId.Add(rateToAdd.ZoneId, rateToAdd.BED);
            }

            #endregion

            #region Fill Info from Rates to Close

            foreach (var rateToClose in structuredRateActions.RatesToClose)
            {
                info.RateActionsExistingZoneIds.Add(rateToClose.ZoneId);
                info.ActionDatesByZoneId.Add(rateToClose.ZoneId, rateToClose.CloseEffectiveDate);

                IEnumerable<SaleRate> zoneCustomerRates = existingRatesByZoneId.GetRecord(rateToClose.ZoneId);

                SaleRate customerRateatClosingDate = zoneCustomerRates.FindRecord(x => x.IsInTimeRange(rateToClose.CloseEffectiveDate));
                if (customerRateatClosingDate == null)
                    throw new DataIntegrityValidationException(string.Format("Trying to close a rate for zone {0} that has no existing rate", rateToClose.ZoneName));

                info.CustomerRates.Add(customerRateatClosingDate);
            }

            #endregion

            #region Fill Info from New Countries


            foreach (var countryToAdd in countriesToAdd)
            {
                List<SaleZone> countryExistingZones = existingZonesByCountryId.GetRecord(countryToAdd.CountryId);

                if (countryExistingZones == null)
                    throw new DataIntegrityValidationException(string.Format("Trying to sell a new country with no zones. Country Id {0}", countryToAdd.CountryId));

                foreach (var zone in countryExistingZones)
                {
                    if (zone.IsInTimeRange(countryToAdd.BED) || zone.BED > countryToAdd.BED)
                    {
                        List<long> zoneIds = info.CountriesToAddExistingZoneIdsByCountryId.GetOrCreateItem(countryToAdd.CountryId);
                        zoneIds.Add(zone.SaleZoneId);
                    }
                }
            }


            #endregion

            #region Fill Info from Close Countries

            foreach (var countryToClose in countriesToClose)
            {
                List<SaleZone> countryExistingZones = existingZonesByCountryId.GetRecord(countryToClose.CountryId);

                if (countryExistingZones == null)
                    throw new DataIntegrityValidationException(string.Format("Trying to stop selling a country with no zones. Country Id {0}", countryToClose.CountryId));

                foreach (var zone in countryExistingZones)
                {
                    if (zone.IsInTimeRange(countryToClose.CloseEffectiveDate))
                    {
                        List<long> zoneIds;
                        if (!info.CountriesToCloseByZoneIds.TryGetValue(countryToClose.CountryId, out zoneIds))
                        {
                            zoneIds = new List<long>();
                            info.CountriesToCloseByZoneIds.Add(countryToClose.CountryId, zoneIds);
                        }
                        zoneIds.Add(zone.SaleZoneId);
                        info.ActionDatesByZoneId.Add(zone.SaleZoneId, countryToClose.CloseEffectiveDate);

                        //Get the customer rate at the time of closure. These rates will be used by rate plan locator when getting rates for each zone related to a closed country
                        IEnumerable<SaleRate> zoneCustomerRates = existingRatesByZoneId.GetRecord(zone.SaleZoneId);
                        SaleRate customerRateatClosingDate = zoneCustomerRates.FindRecord(x => x.IsInTimeRange(countryToClose.CloseEffectiveDate));
                        if (customerRateatClosingDate != null)
                            info.CustomerRates.Add(customerRateatClosingDate);
                    }
                    if (zone.BED > countryToClose.CloseEffectiveDate)
                    {
                        List<long> zoneIds;
                        if (!info.CountriesToCloseByZoneIds.TryGetValue(countryToClose.CountryId, out zoneIds))
                        {
                            zoneIds = new List<long>();
                            info.CountriesToCloseByZoneIds.Add(countryToClose.CountryId, zoneIds);
                        }
                        zoneIds.Add(zone.SaleZoneId);
                        info.ActionDatesByZoneId.Add(zone.SaleZoneId, zone.BED);

                        IEnumerable<SaleRate> zoneCustomerRates = existingRatesByZoneId.GetRecord(zone.SaleZoneId);
                        SaleRate customerRateatClosingDate = zoneCustomerRates.FindRecord(x => x.IsInTimeRange(zone.BED));
                        if (customerRateatClosingDate != null)
                            info.CustomerRates.Add(customerRateatClosingDate);
                    }
                }
            }

            #endregion

            return info;
        }

        private IEnumerable<RoutingCustomerInfoDetails> GetDataByCustomer(SalePriceListOwnerType ownerType, int ownerId, DateTime effectiveDate, out List<int> customerIds)
        {
            customerIds = new List<int>();
            int sellingProductId;
            if (ownerType == SalePriceListOwnerType.SellingProduct)
            {
                IEnumerable<int> customerIdsAssignedToSellingProduct =
                    new CustomerSellingProductManager().GetCustomerIdsAssignedToSellingProduct(ownerId, effectiveDate);

                if (customerIdsAssignedToSellingProduct == null || !customerIdsAssignedToSellingProduct.Any())
                    return new List<RoutingCustomerInfoDetails>();

                customerIds.AddRange(customerIdsAssignedToSellingProduct);
                sellingProductId = ownerId;
            }
            else
            {
                customerIds.Add(ownerId);
                int? effectiveSellingProductId = new CustomerSellingProductManager().GetEffectiveSellingProductId(ownerId, effectiveDate, false);
                if (!effectiveSellingProductId.HasValue)
                    throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("Customer '{0}' is not assigned to a Selling Product", ownerId));
                sellingProductId = effectiveSellingProductId.Value;
            }
            return customerIds.MapRecords(customerId => new RoutingCustomerInfoDetails
            {
                CustomerId = customerId,
                SellingProductId = sellingProductId
            });
        }

        private List<SalePricelistRPChange> GetRoutingProductChanges(int customerId, int sellingProductId, IEnumerable<int> soldCountriesIds, Dictionary<int, List<SaleZone>> existingZonesByCountryId,
            SaleEntityZoneRoutingProductLocator routingProductEffectiveLocator, SaleEntityZoneRoutingProductLocator routingProductCurrentLocator)
        {
            List<SalePricelistRPChange> routingProductChanges = new List<SalePricelistRPChange>();
           
            foreach (var countryId in soldCountriesIds)
            {
                IEnumerable<SaleZone> zones = existingZonesByCountryId.GetRecord(countryId);
             
                if (zones == null)
                    continue;

                foreach (var saleZone in zones)
                {
                    SaleEntityZoneRoutingProduct effectiveRoutingProduct = routingProductEffectiveLocator.GetCustomerZoneRoutingProduct(customerId, sellingProductId, saleZone.SaleZoneId);
                    SaleEntityZoneRoutingProduct currentRoutingProduct = routingProductCurrentLocator.GetCustomerZoneRoutingProduct(customerId, sellingProductId, saleZone.SaleZoneId);
                   
                    if (currentRoutingProduct == null || effectiveRoutingProduct.RoutingProductId != currentRoutingProduct.RoutingProductId)
                    {
                        var routingProduct = new SalePricelistRPChange
                        {
                            CountryId = saleZone.CountryId,
                            ZoneName = saleZone.Name,
                            ZoneId = saleZone.SaleZoneId,
                            RoutingProductId = effectiveRoutingProduct.RoutingProductId,
                            BED = effectiveRoutingProduct.BED,
                            EED = effectiveRoutingProduct.EED,
                            CustomerId = customerId
                        };
                        if (currentRoutingProduct != null)
                            routingProduct.RecentRoutingProductId = currentRoutingProduct.RoutingProductId;
                        routingProductChanges.Add(routingProduct);

                    }
                }
            }
            return routingProductChanges;
        }


        #endregion

        #region Structuring Methods

        private Dictionary<int, List<DataByZone>> StructureImportedZonesByCountryId(IEnumerable<DataByZone> dataByZones)
        {
            Dictionary<int, List<DataByZone>> importedZonesByCountryId = new Dictionary<int, List<DataByZone>>();
            foreach (var zone in dataByZones)
            {
                List<DataByZone> zones = importedZonesByCountryId.GetOrCreateItem(zone.CountryId);
                zones.Add(zone);
            }

            return importedZonesByCountryId;
        }

        private Dictionary<int, List<SaleZone>> StructureExistingZonesByCountryId(IEnumerable<SaleZone> saleZones)
        {
            Dictionary<int, List<SaleZone>> existingZonesByCountryId = new Dictionary<int, List<SaleZone>>();
            foreach (var zone in saleZones)
            {
                List<SaleZone> zones = existingZonesByCountryId.GetOrCreateItem(zone.CountryId);
                zones.Add(zone);
            }

            return existingZonesByCountryId;
        }

        private Dictionary<long, List<SaleRate>> StructureExistingRatesByZoneId(IEnumerable<SaleRate> saleRates)
        {
            var saleRatesByZoneId = new Dictionary<long, List<SaleRate>>();
            foreach (var rate in saleRates)
            {
                List<SaleRate> rates = saleRatesByZoneId.GetOrCreateItem(rate.ZoneId);
                rates.Add(rate);
            }
            return saleRatesByZoneId;
        }

        private Dictionary<long, List<SaleCode>> StructureExistingSaleCodesByZoneId(IEnumerable<SaleCode> saleCodes)
        {
            var saleCodesByZoneId = new Dictionary<long, List<SaleCode>>();
            foreach (var code in saleCodes)
            {
                List<SaleCode> codes = saleCodesByZoneId.GetOrCreateItem(code.ZoneId);
                codes.Add(code);
            }
            return saleCodesByZoneId;
        }


        #endregion

        #region Private Classes

        public class ExistingDataInfo
        {
            private Dictionary<int, List<long>> _countriesToAddExistingZoneIdsByCountryId = new Dictionary<int, List<long>>();
            public Dictionary<int, List<long>> CountriesToAddExistingZoneIdsByCountryId { get { return this._countriesToAddExistingZoneIdsByCountryId; } }
            private Dictionary<long, List<long>> _countriesToCloseByZoneIds = new Dictionary<long, List<long>>();
            public Dictionary<long, List<long>> CountriesToCloseByZoneIds { get { return _countriesToCloseByZoneIds; } }

            private List<long> _rateActionsExistingZoneIds = new List<long>();
            public List<long> RateActionsExistingZoneIds { get { return this._rateActionsExistingZoneIds; } }

            private Dictionary<long, DateTime> _actionDatesByZoneId = new Dictionary<long, DateTime>();
            public Dictionary<long, DateTime> ActionDatesByZoneId { get { return this._actionDatesByZoneId; } }

            private List<SaleRate> _customerRates = new List<SaleRate>();
            public List<SaleRate> CustomerRates { get { return this._customerRates; } }
        }

        private class StructuredZoneActions
        {
            private Dictionary<int, List<RateToChange>> _ratesToAddForNewCountriesbyCountryId = new Dictionary<int, List<RateToChange>>();
            public Dictionary<int, List<RateToChange>> RatesToAddForNewCountriesbyCountryId { get { return this._ratesToAddForNewCountriesbyCountryId; } }

            private List<RateToChange> _ratesToAdd = new List<RateToChange>();
            public List<RateToChange> RatesToAdd { get { return this._ratesToAdd; } }

            private List<RateToChange> _ratesToChange = new List<RateToChange>();
            public List<RateToChange> RatesToChange { get { return this._ratesToChange; } }

            private List<RateToClose> _ratesToClose = new List<RateToClose>();
            public List<RateToClose> RatesToClose { get { return this._ratesToClose; } }

            private List<SaleZoneRoutingProductToAdd> _zoneRoutingProductsToAdd = new List<SaleZoneRoutingProductToAdd>();
            public List<SaleZoneRoutingProductToAdd> ZoneRoutingProductsToAdd { get { return this._zoneRoutingProductsToAdd; } }

            private List<SaleZoneRoutingProductToClose> _zoneRoutinProductsToClose = new List<SaleZoneRoutingProductToClose>();
            public List<SaleZoneRoutingProductToClose> ZoneRoutinProductsToClose { get { return this._zoneRoutinProductsToClose; } }
        }

        private class SellingProductChangesContext
        {
            public Dictionary<int, List<DataByZone>> ImportedZonesByCountryId { get; set; }

            public Dictionary<int, List<SaleZone>> ExistingZonesByCountryId { get; set; }

            public IEnumerable<RoutingCustomerInfoDetails> Customers { get; set; }
            public SaleEntityZoneRateLocator LastRateNoCachelocator { get; set; }
            public SaleEntityZoneRoutingProductLocator EffectiveRoutingProductLocator { get; set; }
            public SaleEntityZoneRoutingProductLocator CurrentRoutingProductLocator { get; set; }
            public DateTime MinimumDate { get; set; }
        }

        private class CustomerNewCountriesChangesContext
        {
            #region Input Arguments

            public IEnumerable<CustomerCountryToAdd> CountriesToAdd { get; set; }

            public Dictionary<long, List<SaleCode>> SaleCodesByZoneId { get; set; }

            public RoutingCustomerInfoDetails CustomerInfo { get; set; }

            public Dictionary<int, List<long>> CountriesToAddExistingZoneIdsByCountryId { get; set; }

            public Dictionary<int, List<RateToChange>> RatesToAddForNewCountriesbyCountryId { get; set; }

            public DateTime MinimumDate { get; set; }
            public int CurrencyId { get; set; }
            public SaleEntityZoneRoutingProductLocator RoutingProductEffectiveLocator { get; set; }
            #endregion

            #region Output Arguments

            public List<SalePricelistRateChange> RateChangesOutArgument { get; set; }

            public List<SalePricelistCodeChange> CodeChangesOutArgument { get; set; }
            public List<SalePricelistRPChange> RoutingProductChanges { get; set; }
            #endregion
        }

        private class CustomerCountriesToCloseChangesContext
        {
            #region Input Arguments

            public IEnumerable<CustomerCountryToChange> CountriesToClose { get; set; }

            public Dictionary<long, List<long>> CountriesToCloseByZoneIds { get; set; }

            public SaleEntityZoneRateLocator RateChangeLocator { get; set; }

            public RoutingCustomerInfoDetails CustomerInfo { get; set; }

            public Dictionary<long, List<SaleCode>> SaleCodesByZoneId { get; set; }
            public Dictionary<long, DateTime> ActionDatesByZoneId { get; set; }

            public DateTime ProcessEffectiveDate { get; set; }
            public int CurrencyId { get; set; }

            #endregion

            #region Output Arguments

            public List<SalePricelistRateChange> RateChangesOutArgument { get; set; }

            public List<SalePricelistCodeChange> CodeChangesOutArgument { get; set; }


            #endregion
        }

        private class CustomerRateActionChangesContext
        {
            #region Input Arguments

            public StructuredZoneActions StructuredRateActions { get; set; }

            public SaleEntityZoneRateLocator RateChangeLocator { get; set; }

            public RoutingCustomerInfoDetails CustomerInfo { get; set; }
            public int CurrencyId { get; set; }

            #endregion

            #region Output Arguments

            public List<SalePricelistRateChange> RateChangesOutArgument { get; set; }

            #endregion
        }

        #endregion
    }
}
