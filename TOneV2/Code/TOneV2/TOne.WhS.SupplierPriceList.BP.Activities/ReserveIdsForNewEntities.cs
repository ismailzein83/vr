using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using TOne.WhS.BusinessEntity.Business;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{

    public sealed class ReserveIdsForNewEntities : CodeActivity
    {
        [RequiredArgument]
        public InArgument<IEnumerable<NewZone>> NewZones { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<NewCode>> NewCodes { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<NewRate>> NewRates { get; set; }

        [RequiredArgument]
        public OutArgument<int> PriceListId { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<NewZone> zoneList = NewZones.Get(context);
            IEnumerable<NewCode> codeList = NewCodes.Get(context);
            IEnumerable<NewRate> rateList = NewRates.Get(context);

            TOne.WhS.SupplierPriceList.Business.SupplierPriceListManager priceListManager = new TOne.WhS.SupplierPriceList.Business.SupplierPriceListManager();
            int priceListId = (int)priceListManager.ReserveIDRange(1);

            PriceListId.Set(context, priceListId);

            SupplierZoneManager zoneManager = new SupplierZoneManager();
            long zoneStartingId = zoneManager.ReserveIDRange(zoneList.Count());

            foreach (NewZone zone in zoneList)
            {
                zone.ZoneId = zoneStartingId++;
            }

            SupplierCodeManager codeManager = new SupplierCodeManager();
            long codeStartingId = codeManager.ReserveIDRange(codeList.Count());

            foreach (NewCode code in codeList)
            {
                code.CodeId = codeStartingId++;
            }

            SupplierRateManager rateManager = new SupplierRateManager();
            long rateStartingId = rateManager.ReserveIDRange(rateList.Count());

            foreach (NewRate rate in rateList)
            {
                rate.RateId = rateStartingId++;
            }
        }
    }
}
