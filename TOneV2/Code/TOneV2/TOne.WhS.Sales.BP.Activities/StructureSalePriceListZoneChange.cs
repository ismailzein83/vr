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
using Vanrise.Entities;

namespace TOne.WhS.Sales.BP.Activities
{
    public class StructureSalePriceListZoneChange : CodeActivity
    {
        public InArgument<IEnumerable<SaleZone>> SaleZones { get; set; }
        public InArgument<IEnumerable<SaleRate>> SaleRates { get; set; }
        public InArgument<IEnumerable<DataByZone>> DataByZone { get; set; }
        public InArgument<DateTime> MinimumDateTime { get; set; }
        public InArgument<IEnumerable<CustomerCountryToAdd>> CustomerCountriesToAdd { get; set; }
        public InArgument<IEnumerable<CustomerCountryToChange>> CustomerCountriesToChange { get; set; }
        public OutArgument<IEnumerable<CustomerPriceListChange>> CustomerChange { get; set; }

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

            #endregion

            Dictionary<int, List<DataByZone>> importedZonesByCountryId = this.StructureImportedZonesByCountryId(dataByZones);

            List<CustomerPriceListChange> customerPriceListChanges = new List<CustomerPriceListChange>();
            IEnumerable<RoutingCustomerInfoDetails> dataByCustomer = GetDataByCustomer(ratePlanContext.OwnerType, ratePlanContext.OwnerId, ratePlanContext.EffectiveDate);

            if(dataByCustomer != null)
            {
                #region Getting Pricelist Changes

                if (ratePlanContext.OwnerType == SalePriceListOwnerType.SellingProduct)
                {
                    #region Selling Product

                    var futureRateLocator = new SaleEntityZoneRateLocator(new SaleRateReadAllNoCache(dataByCustomer, ratePlanContext.EffectiveDate, true));
                    SellingProductChangesContext sellingProductContext = new SellingProductChangesContext()
                    {
                        ImportedZonesByCountryId = importedZonesByCountryId,
                        Customers = dataByCustomer,
                        FutureLocator = futureRateLocator,
                        MinimumDate = minimumDate
                    };

                    customerPriceListChanges = this.GetChangesForSellingProduct(sellingProductContext);

                    #endregion
                }
                else
                {
                    #region Customer

                    #region Preparing Data for Processing

                    RoutingCustomerInfoDetails customerInfo = dataByCustomer.FirstOrDefault();

                    Dictionary<int, CustomerCountryToAdd> countriesToAddByCountryId = customerCountriesToAdd.ToDictionary(x => x.CountryId);
                    Dictionary<int, CustomerCountryToChange> countriesToCloseByCountryId = customerCountriesToClose.ToDictionary(x => x.CountryId);
                    StructuredRateActions structuredRateActions = this.GetRateActions(importedZonesByCountryId, countriesToAddByCountryId, countriesToCloseByCountryId);

                    Dictionary<int, List<SaleZone>> existingZonesByCountryId = this.StructureExistingZonesByCountryId(saleZones);
                    ExistingDataInfo existingDataInfo = this.BuildExistingDataInfo(structuredRateActions, customerCountriesToAdd, customerCountriesToClose, existingZonesByCountryId, saleRates);

                    CustomerPriceListChange changesForThisCustomer = new CustomerPriceListChange();
                    changesForThisCustomer.CustomerId = customerInfo.CustomerId;

                    #endregion

                    #region Preparing Rate Change Locator

                    SaleEntityZoneRateLocator rateChangeLocator = null;
                    if (customerCountriesToClose.Any() || structuredRateActions.RatesToAdd.Any() || structuredRateActions.RatesToClose.Any())
                    {
                        List<long> zoneIds = new List<long>();
                        zoneIds.AddRange(existingDataInfo.CountriesToCloseExistingZoneIds);
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
                            RatesToAddForNewCountriesbyCountryId = structuredRateActions.RatesToAddForNewCountriesbyCountryId,
                            SaleCodesByZoneId = saleCodesByZoneId
                        };

                        this.GetChangesForNewCountries(customerNewCountriesContext);

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
                            CountriesToCloseExistingZoneIds = existingDataInfo.CountriesToCloseExistingZoneIds,
                            CustomerInfo = customerInfo,
                            ProcessEffectiveDate = ratePlanContext.EffectiveDate,
                            RateChangeLocator = rateChangeLocator,
                            SaleCodesByZoneId = saleCodesByZoneId
                        };

                        this.GetChangesForCountriesToClose(customerCountriesToCloseContext);

                        changesForThisCustomer.RateChanges.AddRange(customerCountriesToCloseContext.RateChangesOutArgument);
                        changesForThisCustomer.CodeChanges.AddRange(customerCountriesToCloseContext.CodeChangesOutArgument);
                    }

                    #endregion

                    #endregion

                    #region Processing Rates Actions

                    if (structuredRateActions.RatesToAdd.Any() || structuredRateActions.RatesToClose.Any() || structuredRateActions.RatesToChange.Any())
                    {
                        CustomerRateActionChangesContext customerRateActionContext = new CustomerRateActionChangesContext()
                        {
                            CustomerInfo = customerInfo,
                            RateChangeLocator = rateChangeLocator,
                            StructuredRateActions = structuredRateActions
                        };

                        this.GetChangesForRateActions(customerRateActionContext);
                        changesForThisCustomer.RateChanges.AddRange(customerRateActionContext.RateChangesOutArgument);
                    }

                    #endregion

                    customerPriceListChanges.Add(changesForThisCustomer);

                    #endregion
                }

                #endregion
            }

            CustomerChange.Set(context, customerPriceListChanges);
        }

        #region Get Pricelist Changes from Selling Product Methods

        private List<CustomerPriceListChange> GetChangesForSellingProduct(SellingProductChangesContext context)
        {
            var customerCountryManager = new CustomerCountryManager();
            List<CustomerPriceListChange> customerPriceListChanges = new List<CustomerPriceListChange>();

            foreach (var customer in context.Customers)
            {
                var soldCountries = customerCountryManager.GetCustomerCountriesEffectiveAfter(customer.CustomerId, context.MinimumDate);
                if (soldCountries == null)
                    continue;

                List<SalePricelistRateChange> rateChanges = new List<SalePricelistRateChange>();

                foreach (var country in soldCountries)
                {
                    IEnumerable<DataByZone> zones = context.ImportedZonesByCountryId.GetRecord(country.CountryId);

                    if (zones == null)
                        continue;

                    foreach (var zone in zones)
                    {
                        SaleEntityZoneRate zoneRate = context.FutureLocator.GetCustomerZoneRate(customer.CustomerId, customer.SellingProductId, zone.ZoneId);

                        if (zoneRate != null && zoneRate.Source == SalePriceListOwnerType.Customer)
                            continue; // customer has explicit rate and no need to notify him with this change

                        if (zone.NormalRateToChange != null && zone.NormalRateToChange.RateTypeId == null)
                        {
                            SalePricelistRateChange rateChange = new SalePricelistRateChange
                            {
                                CountryId = zone.CountryId,
                                ZoneName = zone.ZoneName,
                                Rate = zone.NormalRateToChange.NormalRate,
                                ChangeType = zone.NormalRateToChange.ChangeType,
                                BED = zone.NormalRateToChange.BED,
                                EED = zone.NormalRateToChange.EED
                            };

                            if (zone.NormalRateToChange.RecentExistingRate != null)
                                rateChange.RecentRate = zone.NormalRateToChange.RecentExistingRate.ConvertedRate;

                            rateChanges.Add(rateChange);
                        }
                    }
                }

                if (rateChanges.Any())
                {
                    CustomerPriceListChange changesForThisCustomer = new CustomerPriceListChange();
                    changesForThisCustomer.CustomerId = customer.CustomerId;
                    changesForThisCustomer.RateChanges.AddRange(rateChanges);

                    customerPriceListChanges.Add(changesForThisCustomer);
                }
            }

            return customerPriceListChanges;
        }

        #endregion

        #region Get Pricelist Changes from Customer Methods

        private void GetChangesForNewCountries(CustomerNewCountriesChangesContext context)
        {
            context.RateChangesOutArgument = new List<SalePricelistRateChange>();
            context.CodeChangesOutArgument = new List<SalePricelistCodeChange>();

            var futureRateLocator = new SaleEntityZoneRateLocator(new SaleRateReadAllNoCache(new List<RoutingCustomerInfoDetails> { context.CustomerInfo }, context.MinimumDate, true));
            SaleZoneManager saleZoneManager = new SaleZoneManager();

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
                            ZoneName = rate.ZoneName,
                            Rate = rate.NormalRate,
                            ChangeType = RateChangeType.New,
                            BED = rate.BED,
                            EED = rate.EED
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

                    if (!zoneIdsWithExplicitRates.Contains(zoneId))
                    {
                        //Ignore zones that have explicit rates
                        var zoneRate = futureRateLocator.GetSellingProductZoneRate(context.CustomerInfo.SellingProductId, zoneId);
                        if (zoneRate == null)
                            throw new VRBusinessException(string.Format("Zone {0} has no rates set neither for customer nor for selling product", zoneName));

                        context.RateChangesOutArgument.Add(new SalePricelistRateChange
                        {
                            CountryId = countryToAdd.CountryId,
                            ZoneName = zoneName,
                            Rate = zoneRate.Rate.Rate,
                            ChangeType = RateChangeType.New,
                            BED = countryToAdd.BED
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
                            Code = existingCode.Code,
                            ChangeType = CodeChange.New,
                            BED = countryToAdd.BED
                        });
                    }
                }

                #endregion
            }
        }

        private IEnumerable<SaleCode> GetExistingSaleCodes(ExistingDataInfo existingDataInfo, DateTime minimumDate)
        {
            var zoneIds = new List<long>();

            zoneIds.AddRange(existingDataInfo.CountriesToAddExistingZoneIdsByCountryId.Values.SelectMany(z => z));
            zoneIds.AddRange(existingDataInfo.CountriesToCloseExistingZoneIds);

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

            foreach (var countryToClose in context.CountriesToClose)
            {
                CustomerCountry2 soldCountry = customerCountryManager.GetCustomerCountry(context.CustomerInfo.CustomerId, countryToClose.CountryId, context.ProcessEffectiveDate, true);

                foreach (var zoneId in context.CountriesToCloseExistingZoneIds)
                {
                    string zoneName = saleZoneManager.GetSaleZoneName(zoneId);

                    #region Get Rate Changes

                    var zoneRate = context.RateChangeLocator.GetCustomerZoneRate(context.CustomerInfo.CustomerId, context.CustomerInfo.SellingProductId, zoneId);
                    if (zoneRate == null)
                        throw new VRBusinessException(string.Format("Zone {0} does neither have an explicit rate nor a default rate set for selling product", zoneName));

                    context.RateChangesOutArgument.Add(new SalePricelistRateChange
                    {
                        CountryId = countryToClose.CountryId,
                        ZoneName = zoneName,
                        Rate = zoneRate.Rate.Rate,
                        ChangeType = RateChangeType.Deleted,
                        BED = zoneRate.Rate.BED, //TODO: There is a gap here that we need to fix, if the rate is got from selling product BED is not exaclty the one we sent to customer
                        EED = countryToClose.CloseEffectiveDate
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

            #region Processing Rate To Change Increase and Decrease

            foreach (var rateToChange in context.StructuredRateActions.RatesToChange)
            {
                int? countryId = saleZoneManager.GetSaleZoneCountryId(rateToChange.ZoneId);
                if (countryId == null)
                    throw new DataIntegrityValidationException(string.Format("Zone with Id {0} is not assigned to any country", rateToChange.ZoneId));

                context.RateChangesOutArgument.Add(new SalePricelistRateChange
                {
                    CountryId = countryId.Value,
                    ZoneName = rateToChange.ZoneName,
                    Rate = rateToChange.NormalRate,
                    RecentRate = rateToChange.RecentExistingRate.ConvertedRate,
                    ChangeType = rateToChange.ChangeType,
                    BED = rateToChange.BED,
                    EED = rateToChange.EED
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

                context.RateChangesOutArgument.Add(new SalePricelistRateChange
                {
                    CountryId = countryId.Value,
                    ZoneName = rateToAdd.ZoneName,
                    Rate = rateToAdd.NormalRate,
                    RecentRate = recentRate.Rate.Rate,
                    ChangeType = recentRate.Rate.Rate > rateToAdd.NormalRate ? RateChangeType.Decrease : RateChangeType.Increase,
                    BED = rateToAdd.BED,
                    EED = null
                });
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

                context.RateChangesOutArgument.Add(new SalePricelistRateChange
                {
                    CountryId = countryId.Value,
                    ZoneName = rateToClose.ZoneName,
                    Rate = newRate.Rate.Rate,
                    RecentRate = recentRate.Rate.Rate,
                    ChangeType = recentRate.Rate.Rate > newRate.Rate.Rate ? RateChangeType.Decrease : RateChangeType.Increase,
                    BED = rateToClose.CloseEffectiveDate,
                    EED = null
                });
            }

            #endregion
        }

        #endregion

        #region Private Methods

        private StructuredRateActions GetRateActions(Dictionary<int, List<DataByZone>> importedZonesByCountryId, Dictionary<int, CustomerCountryToAdd> countriesToAdd, Dictionary<int, CustomerCountryToChange> countriesToClose)
        {
            StructuredRateActions ratesActions = new StructuredRateActions();

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
                    ratesActions.RatesToAddForNewCountriesbyCountryId.Add(countryId, this.GetRatesToAddFromImportedZones(importedZones));
                }
                else
                {
                    //Get all rates changes and closed rates for this country
                    ratesActions.RatesToAdd.AddRange(this.GetRatesToAddFromImportedZones(importedZones));
                    ratesActions.RatesToChange.AddRange(this.GetRatesToChangeFromImportedZones(importedZones));
                    ratesActions.RatesToClose.AddRange(this.GetRatestoCloseFromImportedZones(importedZones));
                }
            }

            return ratesActions;
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

        private ExistingDataInfo BuildExistingDataInfo(StructuredRateActions structuredRateActions, IEnumerable<CustomerCountryToAdd> countriesToAdd,
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
                    if (zone.IsInTimeRange(countryToAdd.BED))
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
                        info.CountriesToCloseExistingZoneIds.Add(zone.SaleZoneId);
                        info.ActionDatesByZoneId.Add(zone.SaleZoneId, countryToClose.CloseEffectiveDate);

                        //Get the customer rate at the time of closure. These rates will be used by rate plan locator when getting rates for each zone related to a closed country
                        IEnumerable<SaleRate> zoneCustomerRates = existingRatesByZoneId.GetRecord(zone.SaleZoneId);
                        SaleRate customerRateatClosingDate = zoneCustomerRates.FindRecord(x => x.IsInTimeRange(countryToClose.CloseEffectiveDate));
                        if (customerRateatClosingDate != null)
                            info.CustomerRates.Add(customerRateatClosingDate);
                    }
                }
            }

            #endregion

            return info;
        }

        private IEnumerable<RoutingCustomerInfoDetails> GetDataByCustomer(SalePriceListOwnerType ownerType, int ownerId, DateTime effectiveDate)
        {
            var customerIds = new List<int>();
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

            private List<long> _countriesToCloseExistingZoneIds = new List<long>();
            public List<long> CountriesToCloseExistingZoneIds { get { return this._countriesToCloseExistingZoneIds; } }

            private List<long> _rateActionsExistingZoneIds = new List<long>();
            public List<long> RateActionsExistingZoneIds { get { return this._rateActionsExistingZoneIds; } }

            private Dictionary<long, DateTime> _actionDatesByZoneId = new Dictionary<long, DateTime>();
            public Dictionary<long, DateTime> ActionDatesByZoneId { get { return this._actionDatesByZoneId; } }

            private List<SaleRate> _customerRates = new List<SaleRate>();
            public List<SaleRate> CustomerRates { get { return this._customerRates; } }
        }

        private class StructuredRateActions
        {
            private Dictionary<int, List<RateToChange>> _ratesToAddForNewCountriesbyCountryId = new Dictionary<int, List<RateToChange>>();
            public Dictionary<int, List<RateToChange>> RatesToAddForNewCountriesbyCountryId { get { return this._ratesToAddForNewCountriesbyCountryId; } }

            private List<RateToChange> _ratesToAdd = new List<RateToChange>();
            public List<RateToChange> RatesToAdd { get { return this._ratesToAdd; } }

            private List<RateToChange> _ratesToChange = new List<RateToChange>();
            public List<RateToChange> RatesToChange { get { return this._ratesToChange; } }

            private List<RateToClose> _ratesToClose = new List<RateToClose>();
            public List<RateToClose> RatesToClose { get { return this._ratesToClose; } }
        }

        private class SellingProductChangesContext
        {
            public Dictionary<int, List<DataByZone>> ImportedZonesByCountryId { get; set; }

            public IEnumerable<RoutingCustomerInfoDetails> Customers { get; set; }

            public SaleEntityZoneRateLocator FutureLocator { get; set; }

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

            #endregion

            #region Output Arguments

            public List<SalePricelistRateChange> RateChangesOutArgument { get; set; }

            public List<SalePricelistCodeChange> CodeChangesOutArgument { get; set; }

            #endregion
        }

        private class CustomerCountriesToCloseChangesContext
        {
            #region Input Arguments

            public IEnumerable<CustomerCountryToChange> CountriesToClose { get; set; }

            public List<long> CountriesToCloseExistingZoneIds { get; set; }

            public SaleEntityZoneRateLocator RateChangeLocator { get; set; }

            public RoutingCustomerInfoDetails CustomerInfo { get; set; }

            public Dictionary<long, List<SaleCode>> SaleCodesByZoneId { get; set; }

            public DateTime ProcessEffectiveDate { get; set; }

            #endregion

            #region Output Arguments

            public List<SalePricelistRateChange> RateChangesOutArgument { get; set; }

            public List<SalePricelistCodeChange> CodeChangesOutArgument { get; set; }

            #endregion
        }

        private class CustomerRateActionChangesContext
        {
            #region Input Arguments

            public StructuredRateActions StructuredRateActions { get; set; }

            public SaleEntityZoneRateLocator RateChangeLocator { get; set; }

            public RoutingCustomerInfoDetails CustomerInfo { get; set; }

            #endregion

            #region Output Arguments

            public List<SalePricelistRateChange> RateChangesOutArgument { get; set; }

            #endregion
        }

        #endregion
    }
}
