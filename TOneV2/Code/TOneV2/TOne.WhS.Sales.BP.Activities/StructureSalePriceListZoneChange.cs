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
            IRatePlanContext ratePlanContext = context.GetRatePlanContext();
            IEnumerable<DataByZone> dataByZones = DataByZone.Get(context);
            IEnumerable<CustomerCountryToAdd> customerCountriesToAdd = CustomerCountriesToAdd.Get(context);
            IEnumerable<CustomerCountryToChange> customerCountriesToChange = CustomerCountriesToChange.Get(context);
            IEnumerable<SaleZone> saleZones = SaleZones.Get(context);
            IEnumerable<SaleRate> saleRates = SaleRates.Get(context);
            DateTime minimumDateTime = MinimumDateTime.Get(context);
            IEnumerable<CustomerPriceListChange> customerPriceListChanges;

            IEnumerable<RoutingCustomerInfoDetails> dataByCustomer = GetDataByCustomer(ratePlanContext.OwnerType, ratePlanContext.OwnerId, ratePlanContext.EffectiveDate);

            if (ratePlanContext.OwnerType == SalePriceListOwnerType.SellingProduct)
            {
                var futureRateLocator = new SaleEntityZoneRateLocator(new SaleRateReadAllNoCache(dataByCustomer, ratePlanContext.EffectiveDate, true));
                customerPriceListChanges = ManageSellingProductRateChange(dataByZones, dataByCustomer, futureRateLocator, minimumDateTime);
            }
            else
            {
                Dictionary<long, List<SaleCode>> saleCodesByZoneId;
                SaleEntityZoneRateLocator rateChangeLocator = null;

                LocatorContext locatorContext = PrepareContext(saleRates, saleZones, customerCountriesToAdd, customerCountriesToChange, dataByZones);
                RoutingCustomerInfoDetails customerInfo = dataByCustomer.FirstOrDefault();

                if (locatorContext.ZoneIds != null && locatorContext.ZoneIds.Any())
                    rateChangeLocator = new SaleEntityZoneRateLocator(new SaleRateReadRPChanges(locatorContext.SaleRates, customerInfo, locatorContext.ZoneIds, minimumDateTime, locatorContext.SaleZonesWithBED));
                saleCodesByZoneId = GetSaleCodes(customerCountriesToAdd, customerCountriesToChange, locatorContext.SaleZonesByCountryId, minimumDateTime);

                InputContext inputContext = new InputContext
                {
                    CustomerCountriesToAdd = customerCountriesToAdd,
                    CustomerCountriesToChange = customerCountriesToChange,
                    CountryToAddByCountryId = locatorContext.CountryToAddByCountryId,
                    CustomerInfo = customerInfo,
                    DataByZones = dataByZones,
                    ZonesByCountryId = locatorContext.SaleZonesByCountryId,
                    SalecodesByZoneId = saleCodesByZoneId,
                    RateChangeLocator = rateChangeLocator,
                    MinimumDateTime = minimumDateTime
                };
                customerPriceListChanges = ManageCustomerRateChange(inputContext);
            }
            CustomerChange.Set(context, customerPriceListChanges);
        }

        #region Selling Product Changes
        private List<CustomerPriceListChange> ManageSellingProductRateChange(IEnumerable<DataByZone> dataByZones, IEnumerable<RoutingCustomerInfoDetails> customers, SaleEntityZoneRateLocator locator, DateTime effectiveDate)
        {
            List<CustomerPriceListChange> customerPriceListChanges = new List<CustomerPriceListChange>();
            foreach (var customer in customers)
            {
                List<SalePricelistRateChange> rateChanges = new List<SalePricelistRateChange>();
                Dictionary<int, int> soldcountryIds = GroupByCountryId(customer.CustomerId, effectiveDate);
                if (soldcountryIds == null) continue;
                foreach (var zone in dataByZones)
                {
                    if (!soldcountryIds.ContainsKey(zone.CountryId)) continue; // make sure that the country is sold to this customer
                    SaleEntityZoneRate zoneRate = locator.GetCustomerZoneRate(customer.CustomerId, customer.SellingProductId, zone.ZoneId);
                    if (zoneRate != null) continue; // customer has explicit rate
                    AddSalePricelistRateChange(zone, rateChanges);
                }
                if (rateChanges.Any())
                    customerPriceListChanges.Add(new CustomerPriceListChange
                    {
                        CustomerId = customer.CustomerId,
                        RateChanges = rateChanges,
                        CodeChanges = new List<SalePricelistCodeChange>()
                    });
            }
            return customerPriceListChanges;
        }
        private Dictionary<int, int> GroupByCountryId(int customerId, DateTime effectiveDatetTme)
        {
            var customerCountryManager = new CustomerCountryManager();
            var countryIds = new Dictionary<int, int>();
            var soldCountries = customerCountryManager.GetCustomerCountriesEffectiveAfter(customerId, effectiveDatetTme);
            if (soldCountries == null || !soldCountries.Any()) return null;
            foreach (var country in soldCountries)
            {
                if (!countryIds.ContainsKey(country.CountryId))
                    countryIds.Add(country.CountryId, country.CountryId);
            }
            return countryIds;
        }
        private void AddSalePricelistRateChange(DataByZone zone, List<SalePricelistRateChange> rateChanges)
        {
            if (zone.NormalRateToChange != null)
            {
                SalePricelistRateChange rateChange = new SalePricelistRateChange
                {
                    CountryId = zone.CountryId,
                    ZoneName = zone.ZoneName,
                    Rate = zone.NormalRateToChange.NormalRate,
                    ChangeType = zone.NormalRateToChange.ChangeType,
                    BED = zone.BED,
                    EED = zone.EED
                };
                if (zone.NormalRateToChange.RecentExistingRate != null)
                    rateChange.RecentRate = zone.NormalRateToChange.RecentExistingRate.ConvertedRate;
                rateChanges.Add(rateChange);
            }
        }
        #endregion

        #region Customer Change
        private List<CustomerPriceListChange> ManageCustomerRateChange(InputContext context)
        {
            var customerInfo = context.CustomerInfo;
            var customerPriceListChange = new CustomerPriceListChange
            {
                CustomerId = customerInfo.CustomerId,
                CodeChanges = new List<SalePricelistCodeChange>(),
                RateChanges = new List<SalePricelistRateChange>()
            };
            var newRatesForNewCountryToAdd = new Dictionary<string, DataByZone>();
            SetZoneChanges(context, newRatesForNewCountryToAdd, customerInfo, customerPriceListChange);
            SetCountryToCloseChanges(context, customerInfo, customerPriceListChange, context.SalecodesByZoneId);
            SetCountryToAddChanges(context, newRatesForNewCountryToAdd, customerPriceListChange, customerInfo, context.SalecodesByZoneId);
            return new List<CustomerPriceListChange> { customerPriceListChange };
        }
        private void SetZoneChanges(InputContext context, Dictionary<string, DataByZone> newRatesForNewCountryToAdd, RoutingCustomerInfoDetails customerInfo,
            CustomerPriceListChange customerPriceListChange)
        {
            foreach (var zoneToChange in context.DataByZones)
            {
                SalePricelistRateChange salePricelistRate = new SalePricelistRateChange
                {
                    CountryId = zoneToChange.CountryId,
                    ZoneName = zoneToChange.ZoneName,
                    BED = zoneToChange.BED,
                    EED = zoneToChange.EED
                };
                if (zoneToChange.NormalRateToChange != null)
                {
                    var rate = zoneToChange.NormalRateToChange;
                    salePricelistRate.Rate = rate.NormalRate;
                    switch (rate.ChangeType)
                    {
                        case RateChangeType.Decrease:
                        case RateChangeType.Increase:
                            salePricelistRate.RecentRate = rate.RecentExistingRate.ConvertedRate;
                            salePricelistRate.ChangeType = rate.ChangeType;
                            customerPriceListChange.RateChanges.Add(salePricelistRate);
                            break;
                        case RateChangeType.New:
                            if (context.CountryToAddByCountryId.ContainsKey(zoneToChange.CountryId))
                            {
                                newRatesForNewCountryToAdd.Add(zoneToChange.ZoneName, zoneToChange);
                                break;
                            }
                            var zoneRate =
                                context.RateChangeLocator.GetSellingProductZoneRate(customerInfo.SellingProductId,
                                    zoneToChange.ZoneId);
                            if (zoneRate != null)
                            {
                                salePricelistRate.ChangeType = salePricelistRate.Rate > zoneRate.Rate.Rate
                                    ? RateChangeType.Increase
                                    : RateChangeType.Decrease;
                                salePricelistRate.RecentRate = zoneRate.Rate.Rate;
                            }
                            else
                                salePricelistRate.ChangeType = RateChangeType.New;
                            customerPriceListChange.RateChanges.Add(salePricelistRate);
                            break;
                        //case RateChangeType.Deleted:
                        //    var sellingProductRate =
                        //        context.RateChangeLocator.GetSellingProductZoneRate(customerInfo.SellingProductId,
                        //            zoneToChange.ZoneId);
                        //    salePricelistRate.ChangeType = sellingProductRate.Rate.Rate > salePricelistRate.Rate
                        //        ? RateChangeType.Increase
                        //        : RateChangeType.Decrease;
                        //    salePricelistRate.RecentRate = salePricelistRate.Rate;
                        //    salePricelistRate.Rate = sellingProductRate.Rate.Rate;
                        //    customerPriceListChange.RateChanges.Add(salePricelistRate);
                        //    break;
                    }
                }
                if (zoneToChange.NormalRateToClose != null)
                {
                    var sellingProductRate =
                        context.RateChangeLocator.GetSellingProductZoneRate(customerInfo.SellingProductId,
                            zoneToChange.ZoneId);
                    salePricelistRate.ChangeType = sellingProductRate.Rate.Rate > salePricelistRate.Rate
                        ? RateChangeType.Increase
                        : RateChangeType.Decrease;
                    salePricelistRate.BED = zoneToChange.NormalRateToClose.CloseEffectiveDate;
                    salePricelistRate.EED = null;
                    salePricelistRate.RecentRate = salePricelistRate.Rate;
                    salePricelistRate.Rate = sellingProductRate.Rate.Rate;
                    customerPriceListChange.RateChanges.Add(salePricelistRate);
                    break;
                }
            }
        }
        private void SetCountryToCloseChanges(InputContext context, RoutingCustomerInfoDetails customerInfo, CustomerPriceListChange customerPriceListChange, Dictionary<long, List<SaleCode>> saleCodesByZoneId)
        {
            foreach (var countryToClose in context.CustomerCountriesToChange)
            {
                List<SaleZone> zonesToClose;
                if (!context.ZonesByCountryId.TryGetValue(countryToClose.CountryId, out zonesToClose)) continue;

                foreach (var zone in zonesToClose)
                {
                    var customerRate = context.RateChangeLocator.GetCustomerZoneRate(context.CustomerInfo.CustomerId,
                        customerInfo.SellingProductId, zone.SaleZoneId);
                    if (customerRate == null) continue;
                    customerPriceListChange.RateChanges.Add(new SalePricelistRateChange
                    {
                        CountryId = zone.CountryId,
                        ZoneName = zone.Name,
                        Rate = customerRate.Rate.Rate,
                        ChangeType = RateChangeType.Deleted,
                        BED = customerRate.Rate.BED,
                        EED = countryToClose.CloseEffectiveDate
                    });
                    customerPriceListChange.CodeChanges.AddRange(GetSalePricelistCodeChange(saleCodesByZoneId,
                         zone.SaleZoneId, countryToClose.CountryId, zone.Name, CodeChange.Closed,
                         zone.BED, countryToClose.CloseEffectiveDate));
                }
            }
        }
        private void SetCountryToAddChanges(InputContext context, Dictionary<string, DataByZone> newRatesForNewCountryToAdd, CustomerPriceListChange customerPriceListChange, RoutingCustomerInfoDetails customerInfo,
            Dictionary<long, List<SaleCode>> saleCodesByZoneId)
        {
            if (!context.CustomerCountriesToAdd.Any()) return;
            var futureRateLocator = new SaleEntityZoneRateLocator(new SaleRateReadAllNoCache(new List<RoutingCustomerInfoDetails> { context.CustomerInfo }, context.MinimumDateTime, true));
            foreach (var countryToAdd in context.CustomerCountriesToAdd)
            {
                List<SaleZone> zonesToAdd;
                if (!context.ZonesByCountryId.TryGetValue(countryToAdd.CountryId, out zonesToAdd)) continue;

                foreach (var zone in zonesToAdd)
                {
                    DateTime bed = new DateTime();
                    DataByZone dataByZone;
                    if (newRatesForNewCountryToAdd.TryGetValue(zone.Name, out dataByZone))
                    {
                        customerPriceListChange.RateChanges.Add(
                            new SalePricelistRateChange
                            {
                                CountryId = countryToAdd.CountryId,
                                ZoneName = zone.Name,
                                BED = dataByZone.NormalRateToChange.BED,
                                Rate = dataByZone.NormalRateToChange.NormalRate,
                                ChangeType = RateChangeType.New
                            });
                        bed = dataByZone.NormalRateToChange.BED;
                    }
                    else
                    {
                        var zoneRate = futureRateLocator.GetCustomerZoneRate(customerInfo.CustomerId, customerInfo.SellingProductId, zone.SaleZoneId);
                        if (zoneRate != null)
                        {
                            customerPriceListChange.RateChanges.Add(
                                new SalePricelistRateChange
                                {
                                    CountryId = countryToAdd.CountryId,
                                    ZoneName = zone.Name,
                                    BED = countryToAdd.BED,
                                    ChangeType = RateChangeType.New,
                                    Rate = zoneRate.Rate.Rate
                                }
                                );
                            bed = countryToAdd.BED;
                        }
                    }
                    customerPriceListChange.CodeChanges.AddRange(GetSalePricelistCodeChange(saleCodesByZoneId,
                        zone.SaleZoneId, countryToAdd.CountryId, zone.Name, CodeChange.New,
                        bed, null));
                }
            }
        }
        private List<SalePricelistCodeChange> GetSalePricelistCodeChange(Dictionary<long, List<SaleCode>> saleCodesByZoneId, long zoneId, int countryId, string name, CodeChange codeChange, DateTime bed, DateTime? eed)
        {
            List<SalePricelistCodeChange> codeChanges = new List<SalePricelistCodeChange>();
            List<SaleCode> saleCodes;

            if (!saleCodesByZoneId.TryGetValue(zoneId, out saleCodes))
                return codeChanges;

            codeChanges.AddRange(
               saleCodes.Select(c => new SalePricelistCodeChange
               {
                   CountryId = countryId,
                   ZoneName = name,
                   ChangeType = codeChange,
                   Code = c.Code,
                   BED = bed,
                   EED = eed
               }));
            return codeChanges;
        }
        private LocatorContext PrepareContext(IEnumerable<SaleRate> saleRates, IEnumerable<SaleZone> saleZones, IEnumerable<CustomerCountryToAdd> customerCountriesToAdd, IEnumerable<CustomerCountryToChange> customerCountriesToChange,
           IEnumerable<DataByZone> dataByZones)
        {
            var zoneIdsWithBED = new Dictionary<long, DateTime>();
            var zoneIds = new List<long>();
            var customerRates = new List<SaleRate>();
            var newCountryByCountryId = customerCountriesToAdd.ToDictionary(c => c.CountryId, c => c.CountryId);

            var ratesByZoneId = StructureSaleRateByZoneId(saleRates);
            foreach (var dataByZone in dataByZones)
            {
                if (dataByZone.NormalRateToChange != null)
                {
                    switch (dataByZone.NormalRateToChange.ChangeType)
                    {
                        case RateChangeType.New:
                            if (!newCountryByCountryId.ContainsKey(dataByZone.CountryId))
                            {
                                zoneIds.Add(dataByZone.ZoneId);
                                customerRates.Add(SaleRateMapper(dataByZone.NormalRateToChange));
                                zoneIdsWithBED.Add(dataByZone.ZoneId, dataByZone.BED);
                            }
                            break;
                        case RateChangeType.Deleted:
                            List<SaleRate> zoneRates;
                            if (!ratesByZoneId.TryGetValue(dataByZone.ZoneId, out zoneRates))
                            {
                                //data integirty exception
                            }
                            else
                            {
                                customerRates.AddRange(zoneRates);
                                zoneIds.Add(dataByZone.ZoneId);
                                zoneIdsWithBED.Add(dataByZone.ZoneId, dataByZone.BED);
                            }
                            break;
                    }
                }
                if (dataByZone.NormalRateToClose != null)
                {
                    zoneIds.Add(dataByZone.ZoneId);
                    zoneIdsWithBED.Add(dataByZone.ZoneId, dataByZone.BED);
                }
            }
            var zonesByCountryId = StructureSaleZoneByCountryId(saleZones);
            foreach (var countryToClose in customerCountriesToChange)
            {
                List<SaleZone> zonesToClose;
                if (!zonesByCountryId.TryGetValue(countryToClose.CountryId, out zonesToClose))
                    //data integrity exception
                    continue;
                foreach (var zone in zonesToClose)
                {
                    zoneIds.Add(zone.SaleZoneId);
                    zoneIdsWithBED.Add(zone.SaleZoneId, zone.BED);
                    List<SaleRate> zoneRates;
                    if (!ratesByZoneId.TryGetValue(zone.SaleZoneId, out zoneRates)) continue;
                    customerRates.AddRange(zoneRates);
                }
            }
            return new LocatorContext
            {
                ZoneIds = zoneIds,
                SaleZonesWithBED = zoneIdsWithBED,
                SaleRates = customerRates,
                SaleZonesByCountryId = zonesByCountryId,
                CountryToAddByCountryId = newCountryByCountryId
            };
        }
        private Dictionary<long, List<SaleCode>> GetSaleCodes(IEnumerable<CustomerCountryToAdd> customerCountriesToAdd, IEnumerable<CustomerCountryToChange> customerCountriesToChange, Dictionary<int, List<SaleZone>> saleZonesByCountryId, DateTime minimumDateTime)
        {
            var zoneIds = new List<long>();
            foreach (var countryToClose in customerCountriesToChange)
            {
                List<SaleZone> zonesToClose;
                if (saleZonesByCountryId.TryGetValue(countryToClose.CountryId, out zonesToClose))
                    zoneIds.AddRange(zonesToClose.Select(zone => zone.SaleZoneId));
            }
            foreach (var countryToAdd in customerCountriesToAdd)
            {
                List<SaleZone> zonesToAdd;
                if (saleZonesByCountryId.TryGetValue(countryToAdd.CountryId, out zonesToAdd))
                    zoneIds.AddRange(zonesToAdd.Select(zone => zone.SaleZoneId));
            }
            SaleCodeManager saleCodeManager = new SaleCodeManager();
            var saleCodes = saleCodeManager.GetSaleCodesByZoneIDs(zoneIds, minimumDateTime);
            return StructureSaleCodeByZoneId(saleCodes);
        }

        #endregion

        #region structuring methodes

        private Dictionary<int, List<SaleZone>> StructureSaleZoneByCountryId(IEnumerable<SaleZone> saleZones)
        {
            var salezonesByCountryId = new Dictionary<int, List<SaleZone>>();
            foreach (var saleZone in saleZones)
            {
                List<SaleZone> zonesToAdd;
                if (!salezonesByCountryId.TryGetValue(saleZone.CountryId, out zonesToAdd))
                {
                    zonesToAdd = new List<SaleZone>();
                    salezonesByCountryId.Add(saleZone.CountryId, zonesToAdd);
                }
                zonesToAdd.Add(saleZone);
            }
            return salezonesByCountryId;
        }

        private Dictionary<long, List<SaleRate>> StructureSaleRateByZoneId(IEnumerable<SaleRate> saleRates)
        {
            var saleRatesByZoneId = new Dictionary<long, List<SaleRate>>();
            foreach (var rate in saleRates)
            {
                List<SaleRate> saleRate;
                if (!saleRatesByZoneId.TryGetValue(rate.ZoneId, out saleRate))
                {
                    saleRate = new List<SaleRate>();
                    saleRatesByZoneId.Add(rate.ZoneId, saleRate);
                }
                saleRate.Add(rate);
            }
            return saleRatesByZoneId;
        }

        private Dictionary<long, List<SaleCode>> StructureSaleCodeByZoneId(IEnumerable<SaleCode> saleCodes)
        {
            Dictionary<long, List<SaleCode>> saleCodeByZoneId = new Dictionary<long, List<SaleCode>>();
            foreach (var saleCode in saleCodes)
            {
                List<SaleCode> tempSaleCodes;
                if (!saleCodeByZoneId.TryGetValue(saleCode.ZoneId, out tempSaleCodes))
                {
                    tempSaleCodes = new List<SaleCode>();
                    saleCodeByZoneId.Add(saleCode.ZoneId, tempSaleCodes);
                }
                tempSaleCodes.Add(saleCode);
            }
            return saleCodeByZoneId;
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
                    return null;

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

        #region Mapper
        private SaleRate SaleRateMapper(RateToChange rate)
        {
            return new SaleRate
            {
                Rate = rate.NormalRate,
                CurrencyId = rate.CurrencyId,
                SaleRateId = 0,
                ZoneId = rate.ZoneId,
                EED = rate.EED,
                BED = rate.BED,
                RateChange = rate.ChangeType
            };
        }
        #endregion

        #region private class
        public class InputContext
        {
            public IEnumerable<DataByZone> DataByZones { get; set; }
            public IEnumerable<CustomerCountryToAdd> CustomerCountriesToAdd { get; set; }
            public IEnumerable<CustomerCountryToChange> CustomerCountriesToChange { get; set; }
            public Dictionary<int, List<SaleZone>> ZonesByCountryId { get; set; }
            public Dictionary<long, List<SaleCode>> SalecodesByZoneId { get; set; }
            public RoutingCustomerInfoDetails CustomerInfo { get; set; }
            public SaleEntityZoneRateLocator RateChangeLocator { get; set; }
            public Dictionary<int, int> CountryToAddByCountryId { get; set; }
            public DateTime MinimumDateTime { get; set; }
        }

        public class LocatorContext
        {
            public List<long> ZoneIds { get; set; }
            public Dictionary<long, DateTime> SaleZonesWithBED { get; set; }
            public List<SaleRate> SaleRates { get; set; }
            public Dictionary<int, List<SaleZone>> SaleZonesByCountryId { get; set; }
            public Dictionary<int, int> CountryToAddByCountryId { get; set; }
        }
        #endregion
    }
}
