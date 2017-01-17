using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;
using Vanrise.AccountBalance.Data;
using Vanrise.AccountBalance.Business;

namespace Vanrise.AccountBalance.BP.Activities
{
    public class LoadAccountBalanceUpdateHandler : CodeActivity
    {

        #region Arguments
        [RequiredArgument]
        public InArgument<int> UsageCacheDays { get; set; }

        [RequiredArgument]
        public InArgument<Guid> AccountTypeId { get; set; }

        [RequiredArgument]
        public OutArgument<AccountBalanceUpdateHandler> Handler { get; set; }

        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            Handler.Set(context, AccountBalanceUpdateHandler.GetHandlerByAccountTypeId(AccountTypeId.Get(context), UsageCacheDays.Get(context)));
        }
    }
}
