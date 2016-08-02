using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.AccountBalance.Business;
using Vanrise.AccountBalance.Data;

namespace Vanrise.AccountBalance.BP.Activities
{

    public sealed class GetNextBalanceClosingDate : CodeActivity
    {
        [RequiredArgument]
        public OutArgument<DateTime> NextClosingDate { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            ConfigurationManager configurationManager = new ConfigurationManager();
            IClosingPeriodDataManager dataManager = AccountBalanceDataManagerFactory.GetDataManager<IClosingPeriodDataManager>();
            var lastClosingPeriod =  dataManager.GetLastClosingPeriod();

            BalancePeriodContext balancePeriodContext = new Business.BalancePeriodContext
            {
                LastPeriodDate = lastClosingPeriod != null ? lastClosingPeriod.ClosingTime : default(DateTime?)
            };
            configurationManager.GetBalancePeriod().Execute(balancePeriodContext);
            NextClosingDate.Set(context, balancePeriodContext.NextPeriodDate);
        }
    }
}
