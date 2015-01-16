using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.Data;

namespace TOne.LCRProcess.Activities
{

    public sealed class GetRateLastTimestamp : CodeActivity
    {
        public OutArgument<byte[]> LastTimestamp { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IRateDataManager dataManager = DataManagerFactory.GetDataManager<IRateDataManager>();
            this.LastTimestamp.Set(context, dataManager.GetRateLastTimestamp());
        }
    }
}
