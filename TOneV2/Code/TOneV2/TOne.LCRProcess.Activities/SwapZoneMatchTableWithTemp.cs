using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.LCR.Data;

namespace TOne.LCRProcess.Activities
{

    public sealed class SwapZoneMatchTableWithTemp : CodeActivity
    {
        [RequiredArgument]
        public InArgument<bool> IsFuture { get; set; }
        
        protected override void Execute(CodeActivityContext context)
        {
            IZoneMatchDataManager dataManager = LCRDataManagerFactory.GetDataManager<IZoneMatchDataManager>();
            dataManager.SwapTableWithTemp(this.IsFuture.Get(context));
        }
    }
}
