﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TABS;
using TOne.Data;

namespace TOne.CDRProcess.Activities
{

    public sealed class UpdateDailyPostpaidAmounts : CodeActivity
    {
        [RequiredArgument]
        public InArgument<DateTime> Day { get; set; }

        // If your activity returns a value, derive from CodeActivity<TResult>
        // and return the value from the Execute method.
        protected override void Execute(CodeActivityContext context)
        {
            ICDRTargetDataManager dataManager = DataManagerFactory.GetDataManager<ICDRTargetDataManager>();
            dataManager.UpdateDailyPostpaid(this.Day.Get(context));
            Console.WriteLine("{0}: Finished Updating Daily Pospaid Amounts", DateTime.Now);
        }
    }
}
