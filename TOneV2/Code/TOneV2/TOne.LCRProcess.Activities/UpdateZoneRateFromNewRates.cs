using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.Data;

namespace TOne.LCRProcess.Activities
{

    public sealed class UpdateZoneRateFromNewRates : CodeActivity
    {
        [RequiredArgument]
        public InArgument<byte[]> RatesUpdatedAfter { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            byte[] ratesUpdatedAfter = this.RatesUpdatedAfter.Get(context);
            if (ratesUpdatedAfter == null)
                ratesUpdatedAfter = new byte[0];
            IZoneRateDataManager dataManager = DataManagerFactory.GetDataManager<IZoneRateDataManager>();
            dataManager.UpdateFromNewRates(ratesUpdatedAfter);
        }
    }
}
