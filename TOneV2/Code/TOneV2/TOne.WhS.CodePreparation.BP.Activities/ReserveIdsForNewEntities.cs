using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.CodePreparation.Business;
using TOne.WhS.CodePreparation.Entities;
using TOne.WhS.CodePreparation.Entities.Processing;

namespace TOne.WhS.CodePreparation.BP.Activities
{
    public class ReserveIdsForNewEntitiesInput
    {
        public IEnumerable<AddedZone> NewZones { get; set; }

        public IEnumerable<AddedCode> NewCodes { get; set; }

        public IEnumerable<AddedRate> NewRates { get; set; }

        public SalePriceListsByOwner SalePriceListsByOwner { get; set; }

    }
    public sealed class ReserveIdsForNewEntities : Vanrise.BusinessProcess.BaseAsyncActivity<ReserveIdsForNewEntitiesInput>
    {
        [RequiredArgument]
        public InArgument<IEnumerable<AddedZone>> NewZones { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<AddedCode>> NewCodes { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<AddedRate>> NewRates { get; set; }
       
        [RequiredArgument]
        public InArgument<SalePriceListsByOwner> SalePriceListsByOwner { get; set; }

        protected override void DoWork(ReserveIdsForNewEntitiesInput inputArgument, Vanrise.BusinessProcess.AsyncActivityHandle handle)
        {
            IEnumerable<AddedZone> zoneList = inputArgument.NewZones;
            IEnumerable<AddedCode> codeList = inputArgument.NewCodes;
            IEnumerable<AddedRate> rateList = inputArgument.NewRates;
            SalePriceListsByOwner salePriceListsByOwner = inputArgument.SalePriceListsByOwner;

            SaleZoneManager zoneManager = new SaleZoneManager();
            long zoneStartingId = zoneManager.ReserveIDRange(zoneList.Count());

            foreach (AddedZone zone in zoneList)
            {
                zone.ZoneId = zoneStartingId++;
            }

            SaleCodeManager codeManager = new SaleCodeManager();
            long codeStartingId = codeManager.ReserveIDRange(codeList.Count());

            foreach (AddedCode code in codeList)
            {
                code.CodeId = codeStartingId++;
            }

            SaleRateManager rateManager = new SaleRateManager();
            long rateStartingId = rateManager.ReserveIdRange(rateList.Count());

            foreach (AddedRate addedRate in rateList)
            {
                addedRate.RateId = rateStartingId++;
            }

            salePriceListsByOwner.ReserveIds();

        }

        protected override ReserveIdsForNewEntitiesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new ReserveIdsForNewEntitiesInput()
            {
                NewCodes = this.NewCodes.Get(context),
                NewZones = this.NewZones.Get(context),
                NewRates = this.NewRates.Get(context),
                SalePriceListsByOwner = this.SalePriceListsByOwner.Get(context),
            };
        }
    }
}
