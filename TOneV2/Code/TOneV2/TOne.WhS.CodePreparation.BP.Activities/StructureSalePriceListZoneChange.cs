﻿using System;
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
        public InArgument<ClosedExistingZones> ClosedExistingZones { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<CountryToProcess> countriesToProcess = CountriesToProcess.Get(context);
            SalePriceListsByOwner salePriceListByOwner = SalePriceListsByOwner.Get(context);
            int sellingNumberPlanId = SellingNumberPlanId.Get(context);
            DateTime processEffectiveDate = EffectiveDate.Get(context);
            int userId = context.GetSharedInstanceData().InstanceInfo.InitiatorUserId;
            ClosedExistingZones closedExistingZones = ClosedExistingZones.Get(context);

            long processInstanceId = context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID;
            List<StructuredCustomerPricelistChange> allCustomersPricelistChanges = new List<StructuredCustomerPricelistChange>();

            var carrierAccountManager = new CarrierAccountManager();
            var salePriceListManager = new SalePriceListManager();
            IEnumerable<CarrierAccountInfo> customersForThisSellingNumberPlan = carrierAccountManager.GetCustomersBySellingNumberPlanId(sellingNumberPlanId, true);

            if (customersForThisSellingNumberPlan != null && customersForThisSellingNumberPlan.Any())
            {
                IEnumerable<StructuredCountryActions> allCountryActions = this.GetCountryActions(countriesToProcess, closedExistingZones, processEffectiveDate);

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

        private ZoneToClose GetExistingZoneAtClosureTime(List<ExistingZone> existingZones, String zoneName, DateTime closureDate, int? countryId)
        {
            ExistingZone existingZoneAtClosureTime = new ExistingZone();

            foreach (var existingZone in existingZones)
            {
                if (existingZone.BED <= closureDate && existingZone.OriginalEED.VRGreaterThan(closureDate) && (countryId == null || existingZone.CountryId == countryId))
                {
                    existingZoneAtClosureTime = existingZone;
                    break;
                }
            }

            if (existingZoneAtClosureTime == null)
                throw new DataIntegrityValidationException(string.Format("Could not find existing zone at closure time for Zone {0}", zoneName));

            return (new ZoneToClose { ZoneId = existingZoneAtClosureTime.ZoneId, ZoneName = zoneName, EED = closureDate });
        }

        private IEnumerable<StructuredCountryActions> GetCountryActions(IEnumerable<CountryToProcess> countriesToProcess, ClosedExistingZones closedExistingZones, DateTime processEffectiveDate)
        {
            List<StructuredCountryActions> allCountryActions = new List<StructuredCountryActions>();
            Dictionary<string, List<ExistingZone>> closedExistingZonesDictionary = (closedExistingZones != null) ? closedExistingZones.GetClosedExistingZones() : new Dictionary<string, List<ExistingZone>>();

            foreach (CountryToProcess countryData in countriesToProcess)
            {
                StructuredCountryActions actionsForThisCountry = new StructuredCountryActions();

                foreach (var closedExistingZone in closedExistingZonesDictionary)
                {
                    actionsForThisCountry.ZonesToClose.Add(GetExistingZoneAtClosureTime(closedExistingZone.Value, closedExistingZone.Key, processEffectiveDate, countryData.CountryId));
                }

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

                    if (zoneData.ChangeType == ZoneChangeType.Deleted || zoneData.ChangeType == ZoneChangeType.PendingClosed)
                    {
                        if (!zoneData.EED.HasValue)
                            throw new Exception(string.Format("Closing zone {0} without EED", zoneData.ZoneName));
                        DateTime closureDate = zoneData.EED.Value;

                        actionsForThisCountry.ZonesToClose.Add(GetExistingZoneAtClosureTime(zoneData.ExistingZones, zoneData.ZoneName, closureDate, null));
                    }
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
                    this.GetRateChangesFromZonesToAdd(countryAction.NewZonesToAdd, ratesToAddLocator, countryAction.CountryId, customerId, sellingProductId, zonesRoutingProductToAddLocator);

                    routingProductChanges.AddRange(GetRPChangesFromNewRateChange(newRateChanges, customerId, sellingProductId, zonesRoutingProductToAddLocator));
                    rateChanges.AddRange(newRateChanges);

                    IEnumerable<SalePricelistRateChange> zonesToCloseRateChanges = this.GetRateChangesFromClosedZone(countryAction.ZonesToClose, lastRateNoCacheLocator,
                            countryAction.CountryId, customerId, sellingProductId, countrySellDate);
                    rateChanges.AddRange(zonesToCloseRateChanges);
                    routingProductChanges.AddRange(GetRPChangesFromZonesToClose(customerId, sellingProductId, zonesToCloseRateChanges, routingProductLocator));

                    codeChanges.AddRange(this.GetCodeChangesFromCodeToAdd(countryAction.CodesToAdd, countryAction.CountryId));
                    codeChanges.AddRange(this.GetCodeChangesFromCodeToMove(countryAction.CodesToMove, countryAction.CountryId));
                    codeChanges.AddRange(this.GetCodeChangesFromCodeToClose(countryAction.CodesToClose, countryAction.CountryId, customerId, processEffectiveDate));

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
            int customerId, int sellingProductId, SaleEntityZoneRoutingProductLocator zonesRoutingProductToAddLocator)
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
                    BED = rateToSend.Rate.BED,
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
                    BED = zoneRoutingProduct.BED,
                    EED =zoneRoutingProduct.EED,
                    RoutingProductId =zoneRoutingProduct.RoutingProductId,
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
                    BED = routingPRoduct.BED,//TODO the BED should be the assigned date of routing product on this zone
                    EED = zoneToCloseRateChange.EED,
                    RoutingProductId = routingPRoduct.RoutingProductId
                });
            }
            return routingProductchanges;
        }
        private IEnumerable<SalePricelistRateChange> GetRateChangesFromClosedZone(IEnumerable<ZoneToClose> zonesToClose, SaleEntityZoneRateLocator lastRateNoCacheLocator, int countryId, int customerId, int sellingProductId, DateTime countrySellDate)
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

                rateChanges.Add(new SalePricelistRateChange()
                {
                    CountryId = countryId,
                    ZoneName = zoneToClose.ZoneName,
                    ZoneId = zoneToClose.ZoneId,
                    Rate = closedRate.Rate.Rate,
                    ChangeType = RateChangeType.Deleted,
                    BED = (closedRate.Rate.BED > countrySellDate) ? closedRate.Rate.BED : countrySellDate, //TODO: The same gap in Rate Plan when it is a selling product rate the BED is not correclt the same as the one on customer side
                    EED = zoneToClose.EED,
                    CurrencyId = saleRateManager.GetCurrencyId(closedRate.Rate)
                });
                zoneIdsWithRateBED.Add(zoneToClose.ZoneId, closedRate.Rate.BED);
            }
            //assing routing product id
            SetRoutingProductIdOnRateChange(customerId, sellingProductId, rateChanges, zoneIdsWithRateBED);
            return rateChanges;
        }
        private void SetRoutingProductIdOnRateChange(int customerId, int sellingProductId, List<SalePricelistRateChange> rateChanges, Dictionary<long, DateTime> zoneIdsWithRateBED)
        {
            SaleEntityZoneRoutingProductLocator routingProductLocatorByRateBED = new SaleEntityZoneRoutingProductLocator(new SaleEntityRoutingProductReadByRateBED(new List<int> { customerId }, zoneIdsWithRateBED));

            foreach (var rateChange in rateChanges)
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

                long? ExistingZoneId = GetZoneIdofLastExistingCode(codeToMove.ChangedExistingCodes);
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
                codeChanges.Add(new SalePricelistCodeChange
                {
                    CountryId = countryId,
                    ZoneName = codeToMove.OldZoneName,
                    ZoneId = ExistingZoneId,
                    Code = codeToMove.Code,
                    ChangeType = CodeChange.Closed,
                    EED = codeToMove.BED,
                    BED = codeToMove.OldCodeBED
                });
            }
            return codeChanges;
        }

        private IEnumerable<SalePricelistCodeChange> GetCodeChangesFromCodeToClose(IEnumerable<CodeToClose> codesToClose, int countryId, int customerId, DateTime effectiveDate)
        {
            List<SalePricelistCodeChange> codeChanges = new List<SalePricelistCodeChange>();

            CustomerCountryManager customerCountryManager = new CustomerCountryManager();
            CustomerCountry2 soldCountry = customerCountryManager.GetEffectiveOrFutureCustomerCountry(customerId, countryId, effectiveDate);

            soldCountry.ThrowIfNull("soldCountry");

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
                    BED = firstExistingCode.BED > soldCountry.BED ? firstExistingCode.BED : soldCountry.BED,
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
