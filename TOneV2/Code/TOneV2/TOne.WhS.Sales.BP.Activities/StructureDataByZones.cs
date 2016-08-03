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

            Dictionary<string, DataByZone> dataGroupedByZoneName = new Dictionary<string, DataByZone>();
            DataByZone dataByZone;

            foreach (RateToChange rateToChange in ratesToChange)
            {
                if (!dataGroupedByZoneName.TryGetValue(rateToChange.ZoneName, out dataByZone))
                {
                    dataByZone = new DataByZone();
                    dataByZone.ZoneName = rateToChange.ZoneName;
                    dataGroupedByZoneName.Add(rateToChange.ZoneName, dataByZone);
                }
                dataByZone.RateToChange = rateToChange;
            }

            foreach (RateToClose rateToClose in ratesToClose)
            {
                if (!dataGroupedByZoneName.TryGetValue(rateToClose.ZoneName, out dataByZone))
                {
                    dataByZone = new DataByZone();
                    dataByZone.ZoneName = rateToClose.ZoneName;
                    dataGroupedByZoneName.Add(rateToClose.ZoneName, dataByZone);
                }
                dataByZone.RateToClose = rateToClose;
            }

            foreach (SaleZoneRoutingProductToAdd routingProductToAdd in saleZoneRoutingProductsToAdd)
            {
                if (!dataGroupedByZoneName.TryGetValue(routingProductToAdd.ZoneName, out dataByZone))
                {
                    dataByZone = new DataByZone();
                    dataByZone.ZoneName = routingProductToAdd.ZoneName;
                    dataGroupedByZoneName.Add(routingProductToAdd.ZoneName, dataByZone);
                }
                dataByZone.SaleZoneRoutingProductToAdd = routingProductToAdd;
            }

            foreach (SaleZoneRoutingProductToClose routingProductToClose in saleZoneRoutingProductsToClose)
            {
                if (!dataGroupedByZoneName.TryGetValue(routingProductToClose.ZoneName, out dataByZone))
                {
                    dataByZone = new DataByZone();
                    dataByZone.ZoneName = routingProductToClose.ZoneName;
                    dataGroupedByZoneName.Add(routingProductToClose.ZoneName, dataByZone);
                }
                dataByZone.SaleZoneRoutingProductToClose = routingProductToClose;
            }

            this.DataByZone.Set(context, dataGroupedByZoneName.Values);
        }
    }
}
