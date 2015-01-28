using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.LCR.Data;

namespace TOne.LCRProcess.Activities
{

    public sealed class SwapZoneRateTableWithTemp : CodeActivity
    {
        [RequiredArgument]
        public InArgument<bool> IsFuture { get; set; }

        [RequiredArgument]
        public InArgument<bool> ForSupplier { get; set; }
        
        protected override void Execute(CodeActivityContext context)
        {
            IZoneRateDataManager dataManager = LCRDataManagerFactory.GetDataManager<IZoneRateDataManager>();
            dataManager.SwapTableWithTemp(this.IsFuture.Get(context), this.ForSupplier.Get(context));
        }
    }
}
