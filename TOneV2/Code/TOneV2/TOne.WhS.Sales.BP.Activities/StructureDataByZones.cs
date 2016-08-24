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

namespace TOne.WhS.Sales.BP.Activities
{
    public class StructureDataByZones : CodeActivity
    {
        #region Input Arguments

        [RequiredArgument]
        public InArgument<SalePriceListOwnerType> OwnerType { get; set; }

        [RequiredArgument]
        public InArgument<int> OwnerId { get; set; }

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

        #endregion

        #region Output Arguments

        [RequiredArgument]
        public OutArgument<IEnumerable<DataByZone>> DataByZone { get; set; }

        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            SalePriceListOwnerType ownerType = OwnerType.Get(context);
            int ownerId = OwnerId.Get(context);
            int currencyId = CurrencyId.Get(context);

            IEnumerable<RateToChange> ratesToChange = this.RatesToChange.Get(context);
            IEnumerable<RateToClose> ratesToClose = this.RatesToClose.Get(context);

            IEnumerable<SaleZoneRoutingProductToAdd> saleZoneRoutingProductsToAdd = this.SaleZoneRoutingProductsToAdd.Get(context);
            IEnumerable<SaleZoneRoutingProductToClose> saleZoneRoutingProductsToClose = this.SaleZoneRoutingProductsToClose.Get(context);

            IEnumerable<CustomerCountry> soldCountries = null;
            if (ownerType == SalePriceListOwnerType.Customer)
                soldCountries = GetSoldCountries(ownerId, DateTime.Now, false);

            var saleZoneManager = new SaleZoneManager();

            Dictionary<string, DataByZone> dataByZoneName = new Dictionary<string, DataByZone>();
            DataByZone dataByZone;

            var ratePlanManager = new RatePlanManager();
            var currencyExchangeManager = new CurrencyExchangeRateManager();
            var saleRateManager = new SaleRateManager();

            foreach (RateToChange rateToChange in ratesToChange)
            {
                if (!dataByZoneName.TryGetValue(rateToChange.ZoneName, out dataByZone))
                    AddEmptyDataByZone(dataByZoneName, rateToChange.ZoneName, out dataByZone);

                if (rateToChange.RateTypeId.HasValue)
                    dataByZone.OtherRatesToChange.Add(rateToChange);
                else
                    dataByZone.NormalRateToChange = rateToChange;

                if (dataByZone.ZoneRateGroup == null)
                    dataByZone.ZoneRateGroup = GetZoneRateGroup(ownerType, ownerId, rateToChange.ZoneId, DateTime.Now, currencyId, ratePlanManager, currencyExchangeManager, saleRateManager);

                if (ownerType == SalePriceListOwnerType.Customer && !dataByZone.SoldOn.HasValue)
                    dataByZone.SoldOn = GetStartEffectiveTime(soldCountries, saleZoneManager, rateToChange.ZoneId);
            }

            foreach (RateToClose rateToClose in ratesToClose)
            {
                if (!dataByZoneName.TryGetValue(rateToClose.ZoneName, out dataByZone))
                    AddEmptyDataByZone(dataByZoneName, rateToClose.ZoneName, out dataByZone);

                if (rateToClose.RateTypeId.HasValue)
                    dataByZone.OtherRatesToClose.Add(rateToClose);
                else
                    dataByZone.NormalRateToClose = rateToClose;

                if (dataByZone.ZoneRateGroup == null)
                    dataByZone.ZoneRateGroup = GetZoneRateGroup(ownerType, ownerId, rateToClose.ZoneId, DateTime.Now, currencyId, ratePlanManager, currencyExchangeManager, saleRateManager);

                if (ownerType == SalePriceListOwnerType.Customer && !dataByZone.SoldOn.HasValue)
                    dataByZone.SoldOn = GetStartEffectiveTime(soldCountries, saleZoneManager, rateToClose.ZoneId);
            }

            foreach (SaleZoneRoutingProductToAdd routingProductToAdd in saleZoneRoutingProductsToAdd)
            {
                if (!dataByZoneName.TryGetValue(routingProductToAdd.ZoneName, out dataByZone))
                    AddEmptyDataByZone(dataByZoneName, routingProductToAdd.ZoneName, out dataByZone);
                dataByZone.SaleZoneRoutingProductToAdd = routingProductToAdd;
            }

            foreach (SaleZoneRoutingProductToClose routingProductToClose in saleZoneRoutingProductsToClose)
            {
                if (!dataByZoneName.TryGetValue(routingProductToClose.ZoneName, out dataByZone))
                    AddEmptyDataByZone(dataByZoneName, routingProductToClose.ZoneName, out dataByZone);
                dataByZone.SaleZoneRoutingProductToClose = routingProductToClose;
            }

            this.DataByZone.Set(context, dataByZoneName.Values);
        }

        #region Private Methods

        private void AddEmptyDataByZone(Dictionary<string, DataByZone> dataByZoneName, string zoneName, out DataByZone dataByZone)
        {
            dataByZone = new DataByZone();
            dataByZone.ZoneName = zoneName;
            dataByZone.OtherRatesToChange = new List<RateToChange>();
            dataByZone.OtherRatesToClose = new List<RateToClose>();
            dataByZoneName.Add(zoneName, dataByZone);
        }

        private IEnumerable<CustomerCountry> GetSoldCountries(int customerId, DateTime effectiveOn, bool isEffectiveInFuture)
        {
            var customerZoneManager = new CustomerZoneManager();
            CustomerZones customerZones = customerZoneManager.GetCustomerZones(customerId, effectiveOn, isEffectiveInFuture);
            if (customerZones == null)
                throw new NullReferenceException("customerZones");
            if (customerZones.Countries == null)
                throw new NullReferenceException("customerZones.Countries");
            return customerZones.Countries;
        }

        private DateTime GetStartEffectiveTime(IEnumerable<CustomerCountry> soldCountries, SaleZoneManager saleZoneManager, long zoneId)
        {
            SaleZone zone = saleZoneManager.GetSaleZone(zoneId);
            if (zone == null)
                throw new NullReferenceException("zone");
            CustomerCountry country = soldCountries.FindRecord(x => x.CountryId == zone.CountryId);
            if (country == null)
                throw new NullReferenceException("country");
            return country.StartEffectiveTime.Date;
        }

        private ZoneRateGroup GetZoneRateGroup
        (
            SalePriceListOwnerType ownerType,
            int ownerId,
            long zoneId,
            DateTime effectiveOn,
            int targetCurrencyId,
            RatePlanManager ratePlanManager,
            CurrencyExchangeRateManager currencyExchangeRateManager,
            SaleRateManager saleRateManager
        )
        {
            ZoneRateGroup zoneRateGroup = null;
            SaleEntityZoneRate currentRate = ratePlanManager.GetRate(ownerType, ownerId, zoneId, effectiveOn);
            if (currentRate != null)
            {
                zoneRateGroup = new ZoneRateGroup();
                if (currentRate.Rate != null)
                {
                    zoneRateGroup.NormalRate = new ZoneRate();

                    decimal convertedNormalRate =
                        currencyExchangeRateManager.ConvertValueToCurrency(currentRate.Rate.NormalRate, saleRateManager.GetCurrencyId(currentRate.Rate), targetCurrencyId, effectiveOn);
                    
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

                            decimal convertedOtherRate =
                                currencyExchangeRateManager.ConvertValueToCurrency(kvp.Value.NormalRate, saleRateManager.GetCurrencyId(kvp.Value), targetCurrencyId, effectiveOn);

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
