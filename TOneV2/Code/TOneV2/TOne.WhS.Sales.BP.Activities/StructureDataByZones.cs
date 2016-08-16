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

            foreach (RateToChange rateToChange in ratesToChange)
            {
                if (!dataByZoneName.TryGetValue(rateToChange.ZoneName, out dataByZone))
                    AddEmptyDataByZone(dataByZoneName, rateToChange.ZoneName, out dataByZone);
                
                if (rateToChange.RateTypeId.HasValue)
                    dataByZone.OtherRatesToChange.Add(rateToChange);
                else
                    dataByZone.NormalRateToChange = rateToChange;

                if (dataByZone.CurrentRate == null)
                    dataByZone.CurrentRate = ratePlanManager.GetRate(ownerType, ownerId, rateToChange.ZoneId, DateTime.Now);

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

                if (dataByZone.CurrentRate == null)
                    dataByZone.CurrentRate = ratePlanManager.GetRate(ownerType, ownerId, rateToClose.ZoneId, DateTime.Now);

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

        #endregion
    }
}
