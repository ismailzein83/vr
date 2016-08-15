using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.BP.Activities
{
    public class StructureDataByZones : CodeActivity
    {
        #region Input Arguments

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
            IEnumerable<RateToChange> ratesToChange = this.RatesToChange.Get(context);
            IEnumerable<RateToClose> ratesToClose = this.RatesToClose.Get(context);

            IEnumerable<SaleZoneRoutingProductToAdd> saleZoneRoutingProductsToAdd = this.SaleZoneRoutingProductsToAdd.Get(context);
            IEnumerable<SaleZoneRoutingProductToClose> saleZoneRoutingProductsToClose = this.SaleZoneRoutingProductsToClose.Get(context);

            Dictionary<string, DataByZone> dataByZoneName = new Dictionary<string, DataByZone>();
            DataByZone dataByZone;

            foreach (RateToChange rateToChange in ratesToChange)
            {
                if (!dataByZoneName.TryGetValue(rateToChange.ZoneName, out dataByZone))
                    AddEmptyDataByZone(dataByZoneName, rateToChange.ZoneName, out dataByZone);
                dataByZone.OtherRatesToChange.Add(rateToChange);
            }

            foreach (RateToClose rateToClose in ratesToClose)
            {
                if (!dataByZoneName.TryGetValue(rateToClose.ZoneName, out dataByZone))
                    AddEmptyDataByZone(dataByZoneName, rateToClose.ZoneName, out dataByZone);
                dataByZone.OtherRatesToClose.Add(rateToClose);
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
        
        #endregion
    }
}
