using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.CodePreparation.Business;
using TOne.WhS.CodePreparation.Entities;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.Common;
using Vanrise.Entities;

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

            List<CustomerPriceListChange> allCustomersPricelistChanges = new List<CustomerPriceListChange>();

            var carrierAccountManager = new CarrierAccountManager();
            IEnumerable<CarrierAccountInfo> customersForThisSellingNumberPlan = carrierAccountManager.GetCustomersBySellingNumberPlanId(sellingNumberPlanId, true);

            if (customersForThisSellingNumberPlan != null && customersForThisSellingNumberPlan.Any())
            {
                IEnumerable<StructuredCountryActions> allCountryActions = this.GetCountryActions(countriesToProcess);

                IEnumerable<RateToAdd> allRatesToAdd = allCountryActions.SelectMany(x => x.RatesToAdd);
                SaleEntityZoneRateLocator ratesToAddLocator = null;
                if (allRatesToAdd.Any())
                {
                    ratesToAddLocator = new SaleEntityZoneRateLocator(new ReadRatesToAddChanges(allRatesToAdd));
                }

                IEnumerable<RoutingCustomerInfoDetails> customersInfoDetails = GetCustomersInfoDetails(customersForThisSellingNumberPlan, effectiveDate);
                Dictionary<int, RoutingCustomerInfoDetails> infoDetailsByCustomerId = customersInfoDetails.ToDictionary(x => x.CustomerId);

                SaleEntityZoneRateLocator lastRateNoCacheLocator = null;
                IEnumerable<ZoneToProcess> allZonesToClose = allCountryActions.SelectMany(x => x.ZonesToClose);
                if (allZonesToClose.Any())
                {
                    lastRateNoCacheLocator = new SaleEntityZoneRateLocator(new SaleRateReadLastRateNoCache(customersInfoDetails, effectiveDate));
                }

                allCustomersPricelistChanges = GetCustomerPriceListChanges(allCountryActions, customersForThisSellingNumberPlan, ratesToAddLocator,
                     lastRateNoCacheLocator, infoDetailsByCustomerId, effectiveDate);
            }

            CustomerChange.Set(context, allCustomersPricelistChanges);
        }

        #region Pre-requisites Methods

        private IEnumerable<RoutingCustomerInfoDetails> GetCustomersInfoDetails(IEnumerable<CarrierAccountInfo> customers, DateTime effectiveDate)
        {
            List<RoutingCustomerInfoDetails> routingCustomerInfoDetails = new List<RoutingCustomerInfoDetails>();
            CustomerSellingProductManager customerSellingProductManager = new CustomerSellingProductManager();

            foreach (var customer in customers)
            {
                int customerId = customer.CarrierAccountId;

                int? effectiveSellingProductId = customerSellingProductManager.GetEffectiveSellingProductId(customerId, effectiveDate, false);
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

        private IEnumerable<StructuredCountryActions> GetCountryActions(IEnumerable<CountryToProcess> countriesToProcess)
        {
            List<StructuredCountryActions> allCountryActions = new List<StructuredCountryActions>();

            foreach (CountryToProcess countryData in countriesToProcess)
            {
                StructuredCountryActions actionsForThisCountry = new StructuredCountryActions();
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

                    if (zoneData.ChangeType == ZoneChangeType.Deleted || zoneData.ChangeType == ZoneChangeType.PendingClosed)
                        actionsForThisCountry.ZonesToClose.Add(zoneData);
                }

                allCountryActions.Add(actionsForThisCountry);
            }

            return allCountryActions;
        }

        private List<CustomerPriceListChange> GetCustomerPriceListChanges(IEnumerable<StructuredCountryActions> allCountryActions, IEnumerable<CarrierAccountInfo> customers,
            SaleEntityZoneRateLocator ratesToAddLocator, SaleEntityZoneRateLocator lastRateNoCacheLocator, Dictionary<int, RoutingCustomerInfoDetails> infoDetailsByCustomerId,
            DateTime effectiveDate)
        {
            var customerPriceListChanges = new List<CustomerPriceListChange>();
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
                CustomerPriceListChange changesForThisCustomer = new CustomerPriceListChange { CustomerId = customerId };

                SaleEntityZoneRoutingProductLocator routingProductLocator = new SaleEntityZoneRoutingProductLocator(
                        new SaleEntityRoutingProductReadAllNoCache(new List<int> { customerId }, effectiveDate, false));

                var defaultRoutingProduct = routingProductLocator.GetCustomerDefaultRoutingProduct(customerId, sellingProductId);

                if (defaultRoutingProduct == null)
                    throw new DataIntegrityValidationException(string.Format("No default selling product for customer {0}", customer.Name));

                foreach (var countryAction in actionsForSoldCountryOfThisCustomer)
                {
                    IEnumerable<SalePricelistRateChange> newRateChanges =
                    this.GetRateChangesFromZonesToAdd(countryAction.NewZonesToAdd, ratesToAddLocator, countryAction.CountryId, customerId, sellingProductId, defaultRoutingProduct.RoutingProductId);

                    changesForThisCustomer.RoutingProductChanges.AddRange(GetRPChangesFromNewRateChange(newRateChanges, defaultRoutingProduct));
                    changesForThisCustomer.RateChanges.AddRange(newRateChanges);

                    IEnumerable<SalePricelistRateChange> zonesToCloseRateChanges = this.GetRateChangesFromClosedZone(countryAction.ZonesToClose, lastRateNoCacheLocator,
                            countryAction.CountryId, customerId, sellingProductId);
                    changesForThisCustomer.RateChanges.AddRange(zonesToCloseRateChanges);
                    changesForThisCustomer.RoutingProductChanges.AddRange(GetRPChangesFromZonesToClose(customerId, sellingProductId, zonesToCloseRateChanges, routingProductLocator));

                    changesForThisCustomer.CodeChanges.AddRange(this.GetCodeChangesFromCodeToAdd(countryAction.CodesToAdd, countryAction.CountryId));
                    changesForThisCustomer.CodeChanges.AddRange(this.GetCodeChangesFromCodeToMove(countryAction.CodesToMove, countryAction.CountryId));
                    changesForThisCustomer.CodeChanges.AddRange(this.GetCodeChangesFromCodeToClose(countryAction.CodesToClose, countryAction.CountryId, customerId, effectiveDate));

                }

                customerPriceListChanges.Add(changesForThisCustomer);
            }
            return customerPriceListChanges;
        }

        private IEnumerable<SalePricelistRateChange> GetRateChangesFromZonesToAdd(IEnumerable<NewZoneToAdd> newZonesToAdd, SaleEntityZoneRateLocator ratesToAddLocator, int countryId,
            int customerId, int sellingProductId, int defaultRoutingProductId)
        {
            List<SalePricelistRateChange> rateChanges = new List<SalePricelistRateChange>();

            foreach (var zoneToAdd in newZonesToAdd)
            {
                var rateToSend = ratesToAddLocator.GetCustomerZoneRate(customerId, sellingProductId, zoneToAdd.ZoneId);
                if (rateToSend == null)
                    throw new VRBusinessException(string.Format("Zone {0} has no rates set neither for customer nor for selling product", zoneToAdd.ZoneName));

                rateChanges.Add(new SalePricelistRateChange
                {
                    CountryId = countryId,
                    ZoneName = zoneToAdd.ZoneName,
                    Rate = rateToSend.Rate.Rate,
                    ChangeType = RateChangeType.New,
                    BED = rateToSend.Rate.BED,
                    RoutingProductId = defaultRoutingProductId,
                    CurrencyId = rateToSend.Rate.CurrencyId
                });
            }

            return rateChanges;
        }

        private List<SalePricelistRPChange> GetRPChangesFromNewRateChange(IEnumerable<SalePricelistRateChange> rateChanges, SaleEntityZoneRoutingProduct defaultRoutingProduct)
        {
            List<SalePricelistRPChange> routingProductchanges =
                rateChanges.Select(rateChange => new SalePricelistRPChange
                {
                    CountryId = rateChange.CountryId,
                    ZoneName = rateChange.ZoneName,
                    BED = rateChange.BED,
                    EED = defaultRoutingProduct.EED,
                    RoutingProductId = defaultRoutingProduct.RoutingProductId
                }).ToList();
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
                    BED = routingPRoduct.BED,//TODO the BED should be the assigned date of routing product on this zone
                    EED = zoneToCloseRateChange.EED,
                    RoutingProductId = routingPRoduct.RoutingProductId
                });
            }
            return routingProductchanges;
        }
        private IEnumerable<SalePricelistRateChange> GetRateChangesFromClosedZone(IEnumerable<ZoneToProcess> zonesToClose, SaleEntityZoneRateLocator lastRateNoCacheLocator, int countryId, int customerId, int sellingProductId)
        {
            List<SalePricelistRateChange> rateChanges = new List<SalePricelistRateChange>();
            SaleRateManager saleRateManager = new SaleRateManager();
            if (!zonesToClose.Any()) return rateChanges;

            var zoneIdsWithRateBED = new Dictionary<long, DateTime>();


            foreach (var zoneToClose in zonesToClose)
            {
                if (!zoneToClose.EED.HasValue)
                    throw new Exception(string.Format("Closing zone {0} without EED", zoneToClose.ZoneName));

                ExistingZone existingZoneAtClosureTime = null;
                DateTime closureDate = zoneToClose.EED.Value;

                foreach (var existingZone in zoneToClose.ExistingZones)
                {
                    //Comparing with Original EED instead of EED because existing zones EEDs will be updated to the changed EED.
                    //This way no existing zones will be considered as effective at closure time. Comparing with Original EED that should be null will get us results.
                    if (existingZone.BED <= closureDate && existingZone.OriginalEED.VRGreaterThan(closureDate))
                    {
                        existingZoneAtClosureTime = existingZone;
                        break;
                    }
                }

                if (existingZoneAtClosureTime == null)
                    throw new DataIntegrityValidationException(string.Format("Could not find existing zone at closure time for Zone {0}", zoneToClose.ZoneName));

                var closedRate = lastRateNoCacheLocator.GetCustomerZoneRate(customerId, sellingProductId, existingZoneAtClosureTime.ZoneId);
                if (closedRate == null)
                    throw new VRBusinessException(string.Format("Zone {0} has no rates set neither for customer nor for selling product", zoneToClose.ZoneName));

                rateChanges.Add(new SalePricelistRateChange()
                {
                    CountryId = countryId,
                    ZoneName = zoneToClose.ZoneName,
                    ZoneId = existingZoneAtClosureTime.ZoneId,
                    Rate = closedRate.Rate.Rate,
                    ChangeType = RateChangeType.Deleted,
                    BED = closedRate.Rate.BED, //TODO: The same gap in Rate Plan when it is a selling product rate the BED is not correclt the same as the one on customer side
                    EED = zoneToClose.EED,
                    CurrencyId = saleRateManager.GetCurrencyId(closedRate.Rate)
                });
                zoneIdsWithRateBED.Add(existingZoneAtClosureTime.ZoneId, closedRate.Rate.BED);
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
                codeChanges.Add(new SalePricelistCodeChange()
                {
                    CountryId = countryId,
                    ZoneName = codeToAdd.ZoneName,
                    Code = codeToAdd.Code,
                    ChangeType = CodeChange.New,
                    BED = codeToAdd.BED
                });
            }

            return codeChanges;
        }

        private IEnumerable<SalePricelistCodeChange> GetCodeChangesFromCodeToMove(IEnumerable<CodeToMove> codesToMove, int countryId)
        {
            List<SalePricelistCodeChange> codeChanges = new List<SalePricelistCodeChange>();

            foreach (var codeToMove in codesToMove)
            {
                codeChanges.Add(new SalePricelistCodeChange
                {
                    CountryId = countryId,
                    ZoneName = codeToMove.ZoneName,
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

                codeChanges.Add(new SalePricelistCodeChange
                {
                    CountryId = countryId,
                    ZoneName = codeToClose.ZoneName,
                    Code = codeToClose.Code,
                    ChangeType = CodeChange.Closed,
                    BED = firstExistingCode.BED > soldCountry.BED ? firstExistingCode.BED : soldCountry.BED,
                    EED = codeToClose.CloseEffectiveDate
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

            private List<CodeToAdd> _codesToAdd = new List<CodeToAdd>();
            public List<CodeToAdd> CodesToAdd { get { return this._codesToAdd; } }

            private List<CodeToMove> _codesToMove = new List<CodeToMove>();
            public List<CodeToMove> CodesToMove { get { return this._codesToMove; } }

            private List<CodeToClose> _codesToClose = new List<CodeToClose>();
            public List<CodeToClose> CodesToClose { get { return this._codesToClose; } }

            private List<NewZoneToAdd> _newZonesToAdd = new List<NewZoneToAdd>();
            public List<NewZoneToAdd> NewZonesToAdd { get { return this._newZonesToAdd; } }

            private List<ZoneToProcess> _zonesToClose = new List<ZoneToProcess>();
            public List<ZoneToProcess> ZonesToClose { get { return this._zonesToClose; } }
        }

        private class NewZoneToAdd
        {
            public long ZoneId { get; set; }
            public string ZoneName { get; set; }
        }
        #endregion
    }
}
