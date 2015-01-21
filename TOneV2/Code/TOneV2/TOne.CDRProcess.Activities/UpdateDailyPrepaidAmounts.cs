using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TABS;
using TOne.CDR.Data;

namespace TOne.CDRProcess.Activities
{

    public sealed class UpdateDailyPrepaidAmounts : CodeActivity
    {
        [RequiredArgument]
        public InArgument<DateTime> Day { get; set; }

        // If your activity returns a value, derive from CodeActivity<TResult>
        // and return the value from the Execute method.
        protected override void Execute(CodeActivityContext context)
        {
            ICDRTargetDataManager dataManager = CDRDataManagerFactory.GetDataManager<ICDRTargetDataManager>();
            dataManager.UpdateDailyPrepaid(this.Day.Get(context));
            Console.WriteLine("{0}: Finished Updating Daily Prepaid Amounts", DateTime.Now);
        }
    }
}
