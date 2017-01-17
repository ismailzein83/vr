using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Business;
using Vanrise.AccountBalance.Data;
using Vanrise.BusinessProcess;
namespace Vanrise.AccountBalance.BP.Activities
{
    public class InsertBillingTransactionFromAccountUsageAndUpdate : CodeActivity
    {
        #region Arguments
        [RequiredArgument]
        public InArgument<Guid> AccountTypeId { get; set; }

        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            var accountTypeId = this.AccountTypeId.Get(context);
            context.GetSharedInstanceData().WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Insert Billing Transactions From AccountUsage");
            var timeOffset = new AccountTypeManager().GetTimeOffset(accountTypeId);
            IBillingTransactionDataManager dataManager = AccountBalanceDataManagerFactory.GetDataManager<IBillingTransactionDataManager>();
            dataManager.InsertBillingTransactionFromAccountUsageAndUpdate(this.AccountTypeId.Get(context), timeOffset);
        }
    }
}
