using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.BP.Activities
{
    public class ProcessRates : CodeActivity
    {
        #region Arguments

        [RequiredArgument]
        public InArgument<IEnumerable<RateToChange>> RatesToChange { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<RateToClose>> RatesToClose { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<ExistingZone>> ExistingZones { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<ExistingRate>> ExistingRates { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<NewRate>> NewRates { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<ChangedRate>> ChangedRates { get; set; }

        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<RateToChange> ratesToChange = this.RatesToChange.Get(context);
            IEnumerable<RateToClose> ratesToClose = this.RatesToClose.Get(context);
            IEnumerable<ExistingZone> existingZones = this.ExistingZones.Get(context);
            IEnumerable<ExistingRate> existingRates = this.ExistingRates.Get(context);

            var priceListRateManager = new PriceListRateManager2();

            var processRatesContext = new ProcessRatesContext()
            {
                RatesToChange = ratesToChange,
                RatesToClose = ratesToClose,
                ExistingZones = existingZones,
                ExistingRates = existingRates
            };
            
            priceListRateManager.ProcessCountryRates(processRatesContext);

            IEnumerable<NewRate> newRates = (processRatesContext.NewRates != null && processRatesContext.NewRates.Count() > 0) ? processRatesContext.NewRates : null;
            IEnumerable<ChangedRate> changedRates = (processRatesContext.ChangedRates != null && processRatesContext.ChangedRates.Count() > 0) ? processRatesContext.ChangedRates : null;

            this.NewRates.Set(context, newRates);
            this.ChangedRates.Set(context, changedRates);
        }
    }
}
