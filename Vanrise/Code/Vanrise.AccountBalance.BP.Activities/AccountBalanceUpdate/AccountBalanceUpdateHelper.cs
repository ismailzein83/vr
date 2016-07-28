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
    public class AccountBalanceUpdateHelper : CodeActivity
    {
        #region Arguments

        [RequiredArgument]
        public OutArgument<AcountBalanceUpdateHandler> AcountBalanceUpdateHandler { get; set; }

        #endregion
        protected override void Execute(CodeActivityContext context)
        {
            AcountBalanceUpdateHandler handler = new AcountBalanceUpdateHandler();
            AcountBalanceUpdateHandler.Set(context,handler);
        }
    }
}
