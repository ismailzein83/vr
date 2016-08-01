using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.AccountBalance.Data;
using Vanrise.AccountBalance.Business;

namespace Vanrise.AccountBalance.BP.Activities
{

    public sealed class CreateBalanceClosingPeriod : CodeActivity
    {
       [RequiredArgument]
        public InArgument<DateTime> BalanceClosingPeriod { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            IClosingPeriodDataManager dataManager = AccountBalanceDataManagerFactory.GetDataManager<IClosingPeriodDataManager>();
            ConfigurationManager configurationManager = new ConfigurationManager();
            dataManager.CreateClosingPeriod(BalanceClosingPeriod.Get(context), configurationManager.GetUsageTransactionTypeId());
        }
    }
}
