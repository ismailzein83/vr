using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.AccountBalance.Business;
using Vanrise.AccountBalance.Data;
using Vanrise.BusinessProcess;
namespace Vanrise.AccountBalance.BP.Activities
{

    public sealed class GetNextBalanceClosingDate : CodeActivity
    {
       
        #region Arguments
       
        [RequiredArgument]
        public OutArgument<DateTime> NextClosingDate { get; set; }
     
        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            context.GetSharedInstanceData().WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Load Next Balance Closing Date.");
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
