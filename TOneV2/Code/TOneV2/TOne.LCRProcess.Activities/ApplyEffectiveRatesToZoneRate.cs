using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.LCR.Data;

namespace TOne.LCRProcess.Activities
{

    public sealed class ApplyEffectiveRatesToZoneRate : CodeActivity
    {        
        protected override void Execute(CodeActivityContext context)
        {
            IZoneRateDataManager dataManager = LCRDataManagerFactory.GetDataManager<IZoneRateDataManager>();
            dataManager.ApplyEffectiveRates();
        }
    }
}
