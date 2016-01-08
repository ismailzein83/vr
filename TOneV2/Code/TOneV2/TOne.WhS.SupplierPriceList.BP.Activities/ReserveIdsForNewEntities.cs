using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using TOne.WhS.BusinessEntity.Business;
using Vanrise.BusinessProcess;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{
    public class ReserveIdsForNewEntitiesInput
    {
        public IEnumerable<NewZone> NewZones { get; set; }

        public IEnumerable<NewCode> NewCodes { get; set; }

        public IEnumerable<NewRate> NewRates { get; set; }
    }

    public sealed class ReserveIdsForNewEntities : BaseAsyncActivity<ReserveIdsForNewEntitiesInput>
    {
        [RequiredArgument]
        public InArgument<IEnumerable<NewZone>> NewZones { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<NewCode>> NewCodes { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<NewRate>> NewRates { get; set; }

        protected override void DoWork(ReserveIdsForNewEntitiesInput inputArgument, AsyncActivityHandle handle)
        {
            SupplierZoneManager zoneManager = new SupplierZoneManager();
            long zoneStartingId = zoneManager.ReserveIDRange(inputArgument.NewZones.Count());

            foreach (NewZone zone in inputArgument.NewZones)
            {
                zone.ZoneId = zoneStartingId++;
            }

            SupplierCodeManager codeManager = new SupplierCodeManager();
            long codeStartingId = codeManager.ReserveIDRange(inputArgument.NewCodes.Count());

            foreach (NewCode code in inputArgument.NewCodes)
            {
                code.CodeId = codeStartingId++;
            }

            SupplierRateManager rateManager = new SupplierRateManager();
            long rateStartingId = rateManager.ReserveIDRange(inputArgument.NewRates.Count());

            foreach (NewRate rate in inputArgument.NewRates)
            {
                rate.RateId = rateStartingId++;
            }
        }

        protected override ReserveIdsForNewEntitiesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new ReserveIdsForNewEntitiesInput()
            {
                NewCodes = this.NewCodes.Get(context),
                NewRates = this.NewRates.Get(context),
                NewZones = this.NewZones.Get(context)
            };
        }
    }
}
