using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.AccountBalance.Data;
using Vanrise.AccountBalance.Business;
using Vanrise.BusinessProcess;

namespace Vanrise.AccountBalance.BP.Activities
{

    public sealed class CreateBalanceClosingPeriod : CodeActivity
    {

        #region Arguments
         [RequiredArgument]
        public InArgument<Guid> AccountTypeId { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> BalanceClosingPeriod { get; set; }
        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            context.GetSharedInstanceData().WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Create Balance Closing Period for {0}", this.BalanceClosingPeriod.Get(context));

            IClosingPeriodDataManager dataManager = AccountBalanceDataManagerFactory.GetDataManager<IClosingPeriodDataManager>();
            ConfigurationManager configurationManager = new ConfigurationManager();
            dataManager.CreateClosingPeriod(BalanceClosingPeriod.Get(context), AccountTypeId.Get(context), configurationManager.GetUsageTransactionTypeId());
        }
    }
}
