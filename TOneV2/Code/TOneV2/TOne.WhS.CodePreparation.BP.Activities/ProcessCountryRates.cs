using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.Common;
using Vanrise.BusinessProcess;
using TOne.WhS.CodePreparation.Business;
using TOne.WhS.CodePreparation.Entities;

namespace TOne.WhS.CodePreparation.BP.Activities
{

    public class ProcessCountryRatesInput
    {
        public Dictionary<long, ExistingZone> ExistingZonesByZoneId { get; set; }
        public IEnumerable<ExistingRate> ExistingRates { get; set; }
        public IEnumerable<ZoneToProcess> ZonesToProcess { get; set; }
        public DateTime EffectiveDate { get; set; }
        public SalePriceListsByOwner SalePriceListsByOwner { get; set; }
        public int SellingNumberPlanId { get; set; }
        public IEnumerable<NotImportedZone> NotImportedZones { get; set; }
    }

    public class ProcessCountryRatesOutput
    {
        public IEnumerable<ChangedRate> ChangedRates { get; set; }
        public IEnumerable<AddedRate> NewRates { get; set; }
        public SalePriceListsByOwner SalePriceListsByOwner { get; set; }

    }

    public sealed class ProcessCountryRates : BaseAsyncActivity<ProcessCountryRatesInput, ProcessCountryRatesOutput>
    {

        [RequiredArgument]
        public InArgument<Dictionary<long, ExistingZone>> ExistingZonesByZoneId { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<ZoneToProcess>> ZonesToProcess { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<ExistingRate>> ExistingRates { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> EffectiveDate { get; set; }

        [RequiredArgument]
        public InArgument<SalePriceListsByOwner> SalePriceListsByOwner { get; set; }

        [RequiredArgument]
        public InArgument<int> SellingNumberPlanId { get; set; }

        [RequiredArgument]

        public InArgument<IEnumerable<NotImportedZone>> NotImportedZones { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<ChangedRate>> ChangedRates { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<AddedRate>> NewRates { get; set; }

        protected override ProcessCountryRatesOutput DoWorkWithResult(ProcessCountryRatesInput inputArgument, AsyncActivityHandle handle)
        {
            IEnumerable<ExistingZone> existingZones = null;
            SalePriceListsByOwner salePriceListsByOwner = inputArgument.SalePriceListsByOwner;

            if (inputArgument.ExistingZonesByZoneId != null)
                existingZones = inputArgument.ExistingZonesByZoneId.Select(item => item.Value);

            ProcessCountryRatesContext processCountryRateContext = new ProcessCountryRatesContext()
            {
                ExistingZones = existingZones,
                ExistingRates = inputArgument.ExistingRates,
                ZonesToProcess = inputArgument.ZonesToProcess,
                EffectiveDate = inputArgument.EffectiveDate,
                SalePriceListsByOwner = inputArgument.SalePriceListsByOwner,
                SellingNumberPlanId = inputArgument.SellingNumberPlanId,
                NotImportedZones = inputArgument.NotImportedZones
            };

            PriceListRateManager plCodeManager = new PriceListRateManager();
            plCodeManager.ProcessCountryRates(processCountryRateContext, salePriceListsByOwner);


            return new ProcessCountryRatesOutput()
            {
                ChangedRates = processCountryRateContext.ChangedRates,
                NewRates = processCountryRateContext.NewRates,
                SalePriceListsByOwner = salePriceListsByOwner
            };
        }

        protected override ProcessCountryRatesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new ProcessCountryRatesInput()
            {
                ExistingRates = this.ExistingRates.Get(context),
                ExistingZonesByZoneId = this.ExistingZonesByZoneId.Get(context),
                ZonesToProcess = this.ZonesToProcess.Get(context),
                EffectiveDate = this.EffectiveDate.Get(context),
                SalePriceListsByOwner = this.SalePriceListsByOwner.Get(context),
                SellingNumberPlanId = this.SellingNumberPlanId.Get(context),
                NotImportedZones = this.NotImportedZones.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, ProcessCountryRatesOutput result)
        {
            ChangedRates.Set(context, result.ChangedRates);
            NewRates.Set(context, result.NewRates);
            SalePriceListsByOwner.Set(context, result.SalePriceListsByOwner);
        }
    }
}
