using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.Data;

namespace TOne.LCRProcess.Activities
{

    public sealed class ApplyEffectiveRatesToZoneRate : CodeActivity
    {        
        protected override void Execute(CodeActivityContext context)
        {
            IZoneRateDataManager dataManager = DataManagerFactory.GetDataManager<IZoneRateDataManager>();
            dataManager.ApplyEffectiveRates();
        }
    }
}
