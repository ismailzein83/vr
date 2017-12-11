using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.CodePreparation.Business;
using TOne.WhS.CodePreparation.Entities;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.BusinessProcess;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.CodePreparation.BP.Activities
{
    public sealed class StructureSalePriceListZoneChange : CodeActivity
    {
        [RequiredArgument]
        public InArgument<int> SellingNumberPlanId { get; set; }
        public InArgument<IEnumerable<CountryToProcess>> CountriesToProcess { get; set; }
        public InArgument<DateTime> EffectiveDate { get; set; }
        public OutArgument<IEnumerable<NewCustomerPriceListChange>> CustomerPriceListChange { get; set; }
        public OutArgument<IEnumerable<NewPriceList>> SalePriceList { get; set; }
        public InArgument<SalePriceListsByOwner> SalePriceListsByOwner { get; set; }
        [RequiredArgument]
        public InArgument<ClosedExistingZonesByCountry> ClosedExistingZonesByCountry { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<CountryToProcess> countriesToProcess = CountriesToProcess.Get(context);
            SalePriceListsByOwner salePriceListByOwner = SalePriceListsByOwner.Get(context);
            int sellingNumberPlanId = SellingNumberPlanId.Get(context);
            DateTime processEffectiveDate = EffectiveDate.Get(context);
            int userId = context.GetSharedInstanceData().InstanceInfo.InitiatorUserId;
            ClosedExistingZonesByCountry closedExistingZonesByCountry = ClosedExistingZonesByCountry.Get(context);

            long processInstanceId = context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID;
            List<StructuredCustomerPricelistChange> allCustomersPricelistChanges = new List<StructuredCustomerPricelistChange>();

            var carrierAccountManager = new CarrierAccountManager();
            var salePriceListManager = new SalePriceListManager();
            IEnumerable<CarrierAccountInfo> customersForThisSellingNumberPlan = carrierAccountManager.GetCustomersBySellingNumberPlanId(sellingNumberPlanId, true);

            if (customersForThisSellingNumberPlan != null && customersForThisSellingNumberPlan.Any())
            {
                IEnumerable<StructuredCountryActions> allCountryActions = this.GetCountryActions(countriesToProcess, closedExistingZonesByCountry, processEffectiveDate);

                IEnumerable<RateToAdd> allRatesToAdd = allCountryActions.SelectMany(x => x.RatesToAdd);
                IEnumerable<ZoneRoutingProductToAdd> allZonesRoutingProductsToAdd = allCountryActions.SelectMany(x => x.ZonesRoutingProductsToAdd);

                SaleEntityZoneRateLocator ratesToAddLocator = null;
                if (allRatesToAdd.Any())
                {
                    ratesToAddLocator = new SaleEntityZoneRateLocator(new ReadRatesToAddChanges(allRatesToAdd));
                }

                SaleEntityZoneRoutingProductLocator zonesRoutingProductToAddLocator = null;
                if (allZonesRoutingProductsToAdd.Any())
                {
                    zonesRoutingProductToAddLocator = new SaleEntityZoneRoutingProductLocator(new ReadZonesRoutingProductsToAddChanges(allZonesRoutingProductsToAdd));
                }

                IEnumerable<RoutingCustomerInfoDetails> customersInfoDetails = GetCustomersInfoDetails(customersForThisSellingNumberPlan, processEffectiveDate);
                Dictionary<int, RoutingCustomerInfoDetails> infoDetailsByCustomerId = customersInfoDetails.ToDictionary(x => x.CustomerId);

                SaleEntityZoneRateLocator lastRateNoCacheLocator = new SaleEntityZoneRateLocator(new SaleRateReadLastRateNoCache(customersInfoDetails, processEffectiveDate));

                allCustomersPricelistChanges = GetCustomerPriceListChanges(allCountryActions, customersForThisSellingNumberPlan, ratesToAddLocator,
                     lastRateNoCacheLocator, infoDetailsByCustomerId, processEffectiveDate, zonesRoutingProductToAddLocator);
                Dictionary<int, List<NewPriceList>> salePriceListsByCurrencyId = StructurePriceListByCurrencyId(salePriceListByOwner, processInstanceId, userId);
                var customerChanges = salePriceListManager.CreateCustomerChanges(allCustomersPricelistChanges, lastRateNoCacheLocator, salePriceListsByCurrencyId, processEffectiveDate, processInstanceId, userId);

                GetPricelistDescription(customerChanges, closedExistingZonesByCountry, allCountryActions);
                CustomerPriceListChange.Set(context, customerChanges);
                SalePriceList.Set(context, salePriceListsByCurrencyId.Values.SelectMany(p => p));
            }
        }

        #region Pre-requisites Methods

        private IEnumerable<RoutingCustomerInfoDetails> GetCustomersInfoDetails(IEnumerable<CarrierAccountInfo> customers, DateTime processEffectiveDate)
        {
            List<RoutingCustomerInfoDetails> routingCustomerInfoDetails = new List<RoutingCustomerInfoDetails>();
            CustomerSellingProductManager customerSellingProductManager = new CustomerSellingProductManager();

            foreach (var customer in customers)
            {
                int customerId = customer.CarrierAccountId;

                int? effectiveSellingProductId = customerSellingProductManager.GetEffectiveSellingProductId(customerId, processEffectiveDate, false);
                if (effectiveSellingProductId.HasValue)
                {
                    routingCustomerInfoDetails.Add(new RoutingCustomerInfoDetails()
                    {
                        CustomerId = customerId,
                        SellingProductId = effectiveSellingProductId.Value
                    });
                }
            }

            return routingCustomerInfoDetails;
        }

        #endregion

        #region Private Methods

        private void GetPricelistDescription(List<NewCustomerPriceListChange> customerChanges, ClosedExistingZonesByCountry closedExistingZonesByCountry, IEnumerable<StructuredCountryActions> structuredCountryActions)
        {
            Dictionary<string, NewZoneToAdd> zoneToAddByZoneName = new Dictionary<string, NewZoneToAdd>();

            NewZoneToAdd newZoneToAdd = new NewZoneToAdd();
            foreach (var structuredCountryAction in structuredCountryActions)
            {
                foreach (var zoneToAdd in structuredCountryAction.NewZonesToAdd)
                {
                    if (!zoneToAddByZoneName.TryGetValue(zoneToAdd.ZoneName, out newZoneToAdd))
                    {
                        zoneToAddByZoneName.Add(zoneToAdd.ZoneName, zoneToAdd);
                    }
                }
            }

            List<ExistingZone> existingZone;
            foreach (NewCustomerPriceListChange customerChange in customerChanges)
            {
                foreach (PriceListChange pricelist in customerChange.PriceLists)
                {
                    int newZonesCounter = 0;
                    int closedZonesCounter = 0;
                    int newCodesCounter = 0;
                    int closedCodesCounter = 0;
                    int movedCodesCounter = 0;
                    int newRatesCounter = 0;
                    int closedRatesCounter = 0;

                    foreach (CountryChange countryChange in pricelist.CountryChanges)
                    {
                        Dictionary<string, List<ExistingZone>> countryClosedExistingZones;
                        closedExistingZonesByCountry.TryGetValue(countryChange.CountryId, out countryClosedExistingZones);

                        foreach (SalePricelistZoneChange zoneChange in countryChange.ZoneChanges)
                        {
                            if (countryClosedExistingZones != null && countryClosedExistingZones.TryGetValue(zoneChange.ZoneName, out existingZone))
                                closedZonesCounter++;
                            else if (zoneToAddByZoneName.TryGetValue(zoneChange.ZoneName, out newZoneToAdd))
                                newZonesCounter++;

                            foreach (SalePricelistCodeChange codeChange in zoneChange.CodeChanges)
                            {
                                if (codeChange.ChangeType == CodeChange.New)
                                    newCodesCounter++;
                                else if (codeChange.ChangeType == CodeChange.Closed)
                                    closedCodesCounter++;
                                else if (codeChange.ChangeType == CodeChange.Moved)
                                    movedCodesCounter++;
                            }
                            if (zoneChange.RateChange != null)
                            {
                                if (zoneChange.RateChange.ChangeType == RateChangeType.New)
                                    newRatesCounter++;
                                else if (zoneChange.RateChange.ChangeType == RateChangeType.Deleted)
                                    closedRatesCounter++;
                            }
                        }
                    }
                    pricelist.PriceList.Description = GetPricelistDescriptionString(newZonesCounter, closedZonesCounter, newCodesCounter, closedCodesCounter, movedCodesCounter, newRatesCounter, closedRatesCounter);
                }
            }
        }

        private string GetPricelistDescriptionString(int newZonesCounter, int closedZonesCounter, int newCodesCounter, int closedCodesCounter, int movedCodesCounter, int newRatesCounter, int closedRatesCounter)
        {
            string result = "";
            if (newZonesCounter != 0)
                result = string.Format(result + "New Zones: {0} ", newZonesCounter);
            if (closedZonesCounter != 0)
                result = string.Format(result + "Closed Zones: {0} ", closedZonesCounter);
            if (newCodesCounter != 0)
                result = string.Format(result + "New Codes: {0} ", newCodesCounter);
            if (movedCodesCounter != 0)
                result = string.Format(result + "Moved Codes: {0} ", movedCodesCounter);
            if (closedCodesCounter != 0)
                result = string.Format(result + "Closed Codes: {0} ", closedCodesCounter);
            //if (newRatesCounter != 0)
            //    result = string.Format(result + "New Rates: {0} ", newRatesCounter);
            //if (closedRatesCounter != 0)
            //    result = string.Format(result + "Closed Rates: {0} ", closedRatesCounter);
            return result;
        }



        private Dictionary<int, List<NewPriceList>> StructurePriceListByCurrencyId(SalePriceListsByOwner salePriceListByOnwer, long processInstanceId, int userId)
        {
            List<NewPriceList> priceLists = ConvertSalePriceListByOwner(salePriceListByOnwer, processInstanceId, userId);
            var priceListByCurrencyId = new Dictionary<int, List<NewPriceList>>();
            foreach (var priceList in priceLists)
            {
                List<NewPriceList> grouppedPriceLists = priceListByCurrencyId.GetOrCreateItem(priceList.CurrencyId);
                grouppedPriceLists.Add(priceList);
            }
            return priceListByCurrencyId;
        }
        private List<NewPriceList> ConvertSalePriceListByOwner(SalePriceListsByOwner salePriceLists, long processInstanceId, int userId)
        {
            return salePriceLists.GetSalePriceLists().Select(priceListItem =>
                new NewPriceList
                {
                    OwnerId = priceListItem.OwnerId,
                    PriceListId = priceListItem.PriceListId,
                    CurrencyId = priceListItem.CurrencyId,
                    OwnerType = priceListItem.OwnerType,
                    PriceListType = SalePriceListType.Country,
                    EffectiveOn = priceListItem.EffectiveOn,
                    ProcessInstanceId = processInstanceId,
                    UserId = userId
                }).ToList();
        }

        private List<ZoneToClose> GetZoneToCloseFromClosedExistingZones(List<ExistingZone> closedExistingZones, DateTime closureDate)
        {
            List<ZoneToClose> zonesToClose = new List<ZoneToClose>();

            foreach (var existingZone in closedExistingZones)
            {
                if (existingZone.BED <= closureDate && existingZone.OriginalEED.VRGreaterThan(closureDate))
                {
                    zonesToClose.Add(new ZoneToClose { ZoneId = existingZone.ZoneId, ZoneName = existingZone.ZoneEntity.Name, EED = closureDate });
                }
            }

            return zonesToClose;
        }

        private IEnumerable<StructuredCountryActions> GetCountryActions(IEnumerable<CountryToProcess> countriesToProcess, ClosedExistingZonesByCountry closedExistingZonesByCountry, DateTime processEffectiveDate)
        {
            List<StructuredCountryActions> allCountryActions = new List<StructuredCountryActions>();


            foreach (CountryToProcess countryData in countriesToProcess)
            {
                StructuredCountryActions actionsForThisCountry = new StructuredCountryActions();

                Dictionary<string, List<ExistingZone>> countryClosedExistingZones;
                List<ExistingZone> closedExistingZonesForThisCountry = new List<ExistingZone>();

                if (closedExistingZonesByCountry.TryGetValue(countryData.CountryId, out countryClosedExistingZones))
                    closedExistingZonesForThisCountry.AddRange(countryClosedExistingZones.SelectMany(item => item.Value));

                if (closedExistingZonesForThisCountry != null)
                    actionsForThisCountry.ZonesToClose.AddRange(GetZoneToCloseFromClosedExistingZones(closedExistingZonesForThisCountry, processEffectiveDate));

                actionsForThisCountry.CountryId = countryData.CountryId;
                actionsForThisCountry.CodesToAdd.AddRange(countryData.CodesToAdd);
                actionsForThisCountry.CodesToMove.AddRange(countryData.CodesToMove);
                actionsForThisCountry.CodesToClose.AddRange(countryData.CodesToClose);

                foreach (ZoneToProcess zoneData in countryData.ZonesToProcess)
                {
                    if (zoneData.ChangeType == ZoneChangeType.New || zoneData.ChangeType == ZoneChangeType.Renamed)
                    {
                        if (!zoneData.AddedZones.Any())
                            throw new Exception(string.Format("New Zone with name {0} to be created without Added Zones", zoneData.ZoneName));

                        actionsForThisCountry.NewZonesToAdd.Add(
                            new NewZoneToAdd
                            {
                                ZoneName = zoneData.ZoneName,
                                ZoneId = zoneData.AddedZones.Last().ZoneId
                            });
                    }

                    actionsForThisCountry.RatesToAdd.AddRange(zoneData.RatesToAdd);
                    actionsForThisCountry.ZonesRoutingProductsToAdd.AddRange(zoneData.ZonesRoutingProductsToAdd);

                    //if (zoneData.ChangeType == ZoneChangeType.Deleted || zoneData.ChangeType == ZoneChangeType.PendingClosed)
                    //{
                    //    if (!zoneData.EED.HasValue)
                    //        throw new Exception(string.Format("Closing zone {0} without EED", zoneData.ZoneName));
                    //    DateTime closureDate = zoneData.EED.Value;

                    //    actionsForThisCountry.ZonesToClose.Add(GetExistingZoneAtClosureTime(zoneData.ExistingZones, zoneData.ZoneName, closureDate, null));
                    //}
                }

                allCountryActions.Add(actionsForThisCountry);
            }

            return allCountryActions;
        }

        private List<StructuredCustomerPricelistChange> GetCustomerPriceListChanges(IEnumerable<StructuredCountryActions> allCountryActions, IEnumerable<CarrierAccountInfo> customers,
            SaleEntityZoneRateLocator ratesToAddLocator, SaleEntityZoneRateLocator lastRateNoCacheLocator, Dictionary<int, RoutingCustomerInfoDetails> infoDetailsByCustomerId,
            DateTime processEffectiveDate, SaleEntityZoneRoutingProductLocator zonesRoutingProductToAddLocator)
        {
            var structuredCustomerPricelistChange = new List<StructuredCustomerPricelistChange>();
            var customerCountryManager = new CustomerCountryManager();

            foreach (CarrierAccountInfo customer in customers)
            {
                int customerId = customer.CarrierAccountId;

                IEnumerable<CustomerCountry2> soldCountries = customerCountryManager.GetNotClosedCustomerCountries(customerId);
                if (soldCountries == null)
                    continue;

                var actionsForSoldCountryOfThisCustomer = allCountryActions.FindAllRecords(
                    countryAction => soldCountries.Any(soldCountry => soldCountry.CountryId == countryAction.CountryId));

                if (!actionsForSoldCountryOfThisCustomer.Any())
                    continue;

                int sellingProductId = infoDetailsByCustomerId.GetRecord(customer.CarrierAccountId).SellingProductId;
                //CustomerPriceListChange changesForThisCustomer = new CustomerPriceListChange { CustomerId = customerId };

                SaleEntityZoneRoutingProductLocator routingProductLocator = new SaleEntityZoneRoutingProductLocator(
                        new SaleEntityRoutingProductReadAllNoCache(new List<int> { customerId }, processEffectiveDate, false));

                var defaultRoutingProduct = routingProductLocator.GetCustomerDefaultRoutingProduct(customerId, sellingProductId);

                if (defaultRoutingProduct == null)
                    throw new DataIntegrityValidationException(string.Format("No default selling product for customer {0}", customer.Name));

                List<CountryGroup> countryGroups = new List<CountryGroup>();
                foreach (var countryAction in actionsForSoldCountryOfThisCustomer)
                {
                    var rateChanges = new List<SalePricelistRateChange>();
                    var routingProductChanges = new List<SalePricelistRPChange>();
                    var codeChanges = new List<SalePricelistCodeChange>();

                    CustomerCountry2 customerCountry = customerCountryManager.GetEffectiveOrFutureCustomerCountry(customerId, countryAction.CountryId, processEffectiveDate);

                    if (customerCountry == null)
                    {
                        CountryManager countryManager = new CountryManager();
                        string countryName = countryManager.GetCountryName(countryAction.CountryId);
                        throw new DataIntegrityValidationException(string.Format("Country {0} is sold to customer {1} but dont have a record in the database", countryName, customer.Name));
                    }
                    DateTime countrySellDate = customerCountry.BED;

                    IEnumerable<SalePricelistRateChange> newRateChanges =
                    this.GetRateChangesFromZonesToAdd(countryAction.NewZonesToAdd, ratesToAddLocator, countryAction.CountryId, customerId, sellingProductId, zonesRoutingProductToAddLocator, countrySellDate);

                    routingProductChanges.AddRange(GetRPChangesFromNewRateChange(newRateChanges, customerId, sellingProductId, zonesRoutingProductToAddLocator));
                    rateChanges.AddRange(newRateChanges);

                    IEnumerable<SalePricelistRateChange> zonesToCloseRateChanges = this.GetRateChangesFromClosedZone(countryAction.ZonesToClose, lastRateNoCacheLocator,
                            countryAction.CountryId, customerId, sellingProductId, countrySellDate, routingProductLocator);
                    rateChanges.AddRange(zonesToCloseRateChanges);
                    routingProductChanges.AddRange(GetRPChangesFromZonesToClose(customerId, sellingProductId, zonesToCloseRateChanges, routingProductLocator));

                    codeChanges.AddRange(this.GetCodeChangesFromCodeToAdd(countryAction.CodesToAdd, countryAction.CountryId));
                    codeChanges.AddRange(this.GetCodeChangesFromCodeToMove(countryAction.CodesToMove, countryAction.CountryId));
                    codeChanges.AddRange(this.GetCodeChangesFromCodeToClose(countryAction.CodesToClose, countryAction.CountryId, customerId));

                    countryGroups.Add(new CountryGroup
                    {
                        CountryId = countryAction.CountryId,
                        RateChanges = rateChanges,
                        CodeChanges = codeChanges,
                        RPChanges = routingProductChanges
                    });
                }
                structuredCustomerPricelistChange.Add(new StructuredCustomerPricelistChange
                {
                    CustomerId = customerId,
                    CountryGroups = countryGroups
                });
            }
            return structuredCustomerPricelistChange;
        }

        private IEnumerable<SalePricelistRateChange> GetRateChangesFromZonesToAdd(IEnumerable<NewZoneToAdd> newZonesToAdd, SaleEntityZoneRateLocator ratesToAddLocator, int countryId,
            int customerId, int sellingProductId, SaleEntityZoneRoutingProductLocator zonesRoutingProductToAddLocator, DateTime countrySellDate)
        {
            List<SalePricelistRateChange> rateChanges = new List<SalePricelistRateChange>();

            foreach (var zoneToAdd in newZonesToAdd)
            {
                var rateToSend = ratesToAddLocator.GetCustomerZoneRate(customerId, sellingProductId, zoneToAdd.ZoneId);
                if (rateToSend == null)
                    throw new VRBusinessException(string.Format("Zone {0} has no rates set neither for customer nor for selling product", zoneToAdd.ZoneName));

                SaleEntityZoneRoutingProduct zoneRoutingProduct = zonesRoutingProductToAddLocator.GetCustomerZoneRoutingProduct(customerId, sellingProductId, zoneToAdd.ZoneId);
                if (zoneRoutingProduct == null)
                    throw new VRBusinessException(string.Format("Zone {0} has no routing produce set", zoneToAdd.ZoneName));

                rateChanges.Add(new SalePricelistRateChange
                {
                    CountryId = countryId,
                    ZoneName = zoneToAdd.ZoneName,
                    ZoneId = zoneToAdd.ZoneId,
                    Rate = rateToSend.Rate.Rate,
                    ChangeType = RateChangeType.New,
                    BED = (rateToSend.Rate.BED > countrySellDate) ? rateToSend.Rate.BED : countrySellDate,
                    RoutingProductId = zoneRoutingProduct.RoutingProductId,
                    CurrencyId = rateToSend.Rate.CurrencyId
                });
            }

            return rateChanges;
        }

        private List<SalePricelistRPChange> GetRPChangesFromNewRateChange(IEnumerable<SalePricelistRateChange> rateChanges, int customerId, int sellingProductId, SaleEntityZoneRoutingProductLocator zonesRoutingProductToAddLocator)
        {
            List<SalePricelistRPChange> routingProductchanges = new List<SalePricelistRPChange>();
            foreach (var rateChange in rateChanges)
            {
                SaleEntityZoneRoutingProduct zoneRoutingProduct = zonesRoutingProductToAddLocator.GetCustomerZoneRoutingProduct(customerId, sellingProductId, rateChange.ZoneId.Value);
                if (zoneRoutingProduct == null)
                    throw new VRBusinessException(string.Format("Zone {0} has no routing produce set", rateChange.ZoneName));
                routingProductchanges.Add(new SalePricelistRPChange
                {
                    CountryId = rateChange.CountryId,
                    ZoneName = rateChange.ZoneName,
                    ZoneId = rateChange.ZoneId,
                    BED = rateChange.BED,
                    EED = rateChange.EED,
                    RoutingProductId = zoneRoutingProduct.RoutingProductId,
                });
            }
            return routingProductchanges;
        }
        private List<SalePricelistRPChange> GetRPChangesFromZonesToClose(int customerId, int sellingProductId, IEnumerable<SalePricelistRateChange> zonesToCloseRateChanges, SaleEntityZoneRoutingProductLocator routingProductLocator)
        {
            List<SalePricelistRPChange> routingProductchanges = new List<SalePricelistRPChange>();
            foreach (var zoneToCloseRateChange in zonesToCloseRateChanges)
            {
                var routingPRoduct = routingProductLocator.GetCustomerZoneRoutingProduct(customerId, sellingProductId,
                    zoneToCloseRateChange.ZoneId.Value);
                routingProductchanges.Add(new SalePricelistRPChange
                {
                    CountryId = zoneToCloseRateChange.CountryId,
                    ZoneName = zoneToCloseRateChange.ZoneName,
                    ZoneId = zoneToCloseRateChange.ZoneId,
                    BED = zoneToCloseRateChange.BED,//TODO the BED should be the assigned date of routing product on this zone
                    EED = zoneToCloseRateChange.EED,
                    RoutingProductId = routingPRoduct.RoutingProductId
                });
            }
            return routingProductchanges;
        }
        private IEnumerable<SalePricelistRateChange> GetRateChangesFromClosedZone(IEnumerable<ZoneToClose> zonesToClose, SaleEntityZoneRateLocator lastRateNoCacheLocator, int countryId, int customerId, int sellingProductId, DateTime countrySellDate, SaleEntityZoneRoutingProductLocator routingProductLocator)
        {
            List<SalePricelistRateChange> rateChanges = new List<SalePricelistRateChange>();
            SaleRateManager saleRateManager = new SaleRateManager();
            if (!zonesToClose.Any()) return rateChanges;

            var zoneIdsWithRateBED = new Dictionary<long, DateTime>();

            foreach (var zoneToClose in zonesToClose)
            {
                var closedRate = lastRateNoCacheLocator.GetCustomerZoneRate(customerId, sellingProductId, zoneToClose.ZoneId);
                if (closedRate == null)
                    throw new VRBusinessException(string.Format("Zone {0} has no rates set neither for customer nor for selling product", zoneToClose.ZoneName));

                DateTime rateBED = (closedRate.Rate.BED > countrySellDate) ? closedRate.Rate.BED : countrySellDate;
                DateTime rateEED = (zoneToClose.EED > rateBED) ? zoneToClose.EED : rateBED;

                rateChanges.Add(new SalePricelistRateChange()
                {
                    CountryId = countryId,
                    ZoneName = zoneToClose.ZoneName,
                    ZoneId = zoneToClose.ZoneId,
                    Rate = closedRate.Rate.Rate,
                    ChangeType = RateChangeType.Deleted,
                    BED = rateBED, //TODO: The same gap in Rate Plan when it is a selling product rate the BED is not correclt the same as the one on customer side
                    EED = rateEED,
                    CurrencyId = saleRateManager.GetCurrencyId(closedRate.Rate)
                });
                zoneIdsWithRateBED.Add(zoneToClose.ZoneId, closedRate.Rate.BED);
            }
            //assing routing product id
            SetRoutingProductIdOnRateChange(customerId, sellingProductId, rateChanges, zoneIdsWithRateBED, routingProductLocator);
            return rateChanges;
        }
        private void SetRoutingProductIdOnRateChange(int customerId, int sellingProductId, List<SalePricelistRateChange> rateChanges, Dictionary<long, DateTime> zoneIdsWithRateBED, SaleEntityZoneRoutingProductLocator routingProductLocator)
        {
            foreach (var rateChange in rateChanges)
            {
                var saleEntityZoneRoutingProduct = routingProductLocator.GetCustomerZoneRoutingProduct(customerId, sellingProductId, rateChange.ZoneId.Value);
                if (saleEntityZoneRoutingProduct == null)
                    throw new Exception(string.Format("No routing product assigned for customer {0}", customerId));

                rateChange.RoutingProductId = saleEntityZoneRoutingProduct.RoutingProductId;
            }
        }
        private IEnumerable<SalePricelistCodeChange> GetCodeChangesFromCodeToAdd(IEnumerable<CodeToAdd> codesToAdd, int countryId)
        {
            List<SalePricelistCodeChange> codeChanges = new List<SalePricelistCodeChange>();

            foreach (var codeToAdd in codesToAdd)
            {
                //in case of retroactive we will have multiple added codes with each zoneid
                //we need to take the first zoneid
                //else take the code zoneid

                AddedCode firstCode = codeToAdd.AddedCodes.OrderBy(x => x.BED).FirstOrDefault();
                long? zoneId;
                if (firstCode == null)
                    throw new Exception(string.Format("Added codes is null for code to Add {0}", codeToAdd.Code));

                zoneId = firstCode.Zone.ZoneId;

                codeChanges.Add(new SalePricelistCodeChange
                {
                    CountryId = countryId,
                    ZoneName = codeToAdd.ZoneName,
                    Code = codeToAdd.Code,
                    ChangeType = CodeChange.New,
                    BED = codeToAdd.BED,
                    ZoneId = zoneId
                });
            }
            return codeChanges;
        }

        private long? GetZoneIdofLastExistingCode(List<ExistingCode> existingCodes)
        {
            long? zoneId = null;
            foreach (var changedExistingCode in existingCodes)
            {
                if (changedExistingCode.OriginalEED == null)
                    zoneId = changedExistingCode.ParentZone.ZoneId;
            }
            return zoneId;
        }
        private long? GetZoneIdofLastAddedCode(List<AddedCode> addedCodes)
        {
            long? zoneId = null;
            foreach (var addedCode in addedCodes)
            {
                if (addedCode.Zone.EED == null)
                    zoneId = addedCode.Zone.ZoneId;
            }
            return zoneId;
        }
        private IEnumerable<SalePricelistCodeChange> GetCodeChangesFromCodeToMove(IEnumerable<CodeToMove> codesToMove, int countryId)
        {
            List<SalePricelistCodeChange> codeChanges = new List<SalePricelistCodeChange>();

            foreach (var codeToMove in codesToMove)
            {

                //long? ExistingZoneId = GetZoneIdofLastExistingCode(codeToMove.ChangedExistingCodes);
                long? NewZoneId = GetZoneIdofLastAddedCode(codeToMove.AddedCodes);

                codeToMove.OldCodeBED = codeToMove.ChangedExistingCodes.Select(item => item.CodeEntity.BED).Min();
                codeChanges.Add(new SalePricelistCodeChange
                {
                    CountryId = countryId,
                    ZoneName = codeToMove.ZoneName,
                    ZoneId = NewZoneId,
                    Code = codeToMove.Code,
                    ChangeType = CodeChange.Moved,
                    RecentZoneName = codeToMove.OldZoneName,
                    BED = codeToMove.BED
                });

                //We need to mention the closed code also in the sheet
                //codeChanges.Add(new SalePricelistCodeChange
                //{
                //    CountryId = countryId,
                //    ZoneName = codeToMove.OldZoneName,
                //    ZoneId = ExistingZoneId,
                //    Code = codeToMove.Code,
                //    ChangeType = CodeChange.Closed,
                //    EED = codeToMove.BED,
                //    BED = codeToMove.OldCodeBED
                //});
            }
            return codeChanges;
        }

        private IEnumerable<SalePricelistCodeChange> GetCodeChangesFromCodeToClose(IEnumerable<CodeToClose> codesToClose, int countryId, int customerId)
        {
            List<SalePricelistCodeChange> codeChanges = new List<SalePricelistCodeChange>();

            foreach (var codeToClose in codesToClose)
            {
                ExistingCode firstExistingCode = codeToClose.ChangedExistingCodes.FirstOrDefault();
                if (firstExistingCode == null)
                    throw new Exception(string.Format("Trying to close code {0} on zone {1}, this code does not have existing data", codeToClose.Code, codeToClose.ZoneName));
                long? zoneId = GetZoneIdofLastExistingCode(codeToClose.ChangedExistingCodes);

                codeChanges.Add(new SalePricelistCodeChange
                {
                    CountryId = countryId,
                    ZoneName = codeToClose.ZoneName,
                    Code = codeToClose.Code,
                    ChangeType = CodeChange.Closed,
                    BED = firstExistingCode.BED,
                    EED = codeToClose.CloseEffectiveDate,
                    ZoneId = zoneId
                });
            }

            return codeChanges;
        }

        #endregion

        #region Private Classes

        private class StructuredCountryActions
        {
            public int CountryId { get; set; }

            private List<RateToAdd> _ratesToAdd = new List<RateToAdd>();
            public List<RateToAdd> RatesToAdd { get { return this._ratesToAdd; } }

            private List<ZoneRoutingProductToAdd> _zonesRoutingProductsToAdd = new List<ZoneRoutingProductToAdd>();
            public List<ZoneRoutingProductToAdd> ZonesRoutingProductsToAdd { get { return this._zonesRoutingProductsToAdd; } }

            private List<CodeToAdd> _codesToAdd = new List<CodeToAdd>();
            public List<CodeToAdd> CodesToAdd { get { return this._codesToAdd; } }

            private List<CodeToMove> _codesToMove = new List<CodeToMove>();
            public List<CodeToMove> CodesToMove { get { return this._codesToMove; } }

            private List<CodeToClose> _codesToClose = new List<CodeToClose>();
            public List<CodeToClose> CodesToClose { get { return this._codesToClose; } }

            private List<NewZoneToAdd> _newZonesToAdd = new List<NewZoneToAdd>();
            public List<NewZoneToAdd> NewZonesToAdd { get { return this._newZonesToAdd; } }

            private List<ZoneToClose> _zonesToClose = new List<ZoneToClose>();
            public List<ZoneToClose> ZonesToClose { get { return this._zonesToClose; } }
        }

        private class NewZoneToAdd
        {
            public long ZoneId { get; set; }
            public string ZoneName { get; set; }
        }

        private class ZoneToClose
        {
            public long ZoneId { get; set; }
            public string ZoneName { get; set; }
            public DateTime EED { get; set; }
        }
        #endregion
    }
}
