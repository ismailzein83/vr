using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using Vanrise.BusinessProcess;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{
    #region Arguments Classes

    public class GetDWAccountCasesInput
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }
    }


    public class GetDWAccountCasesOutput
    {
        public DWAccountCaseDictionary AccountCases { get; set; }
    }

    #endregion

    public sealed class GetDWAccountCases : BaseAsyncActivity<GetDWAccountCasesInput, GetDWAccountCasesOutput>
    {
        #region Arguments

        [RequiredArgument]
        public InOutArgument<DWAccountCaseDictionary> AccountCases { get; set; }


        [RequiredArgument]
        public InArgument<DateTime> FromDate { get; set; }


        [RequiredArgument]
        public InArgument<DateTime> ToDate { get; set; }


        #endregion



        protected override GetDWAccountCasesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new GetDWAccountCasesInput
            {
                FromDate = this.FromDate.Get(context),
                ToDate = this.ToDate.Get(context),
            };
        }

        protected override GetDWAccountCasesOutput DoWorkWithResult(GetDWAccountCasesInput inputArgument, AsyncActivityHandle handle)
        {
            DWAccountCaseManager accountCaseManager = new DWAccountCaseManager();
            IEnumerable<DWAccountCase> accountCases = accountCaseManager.GetDWAccountCases(inputArgument.FromDate, inputArgument.ToDate);
            DWAccountCaseDictionary AccountCases = new DWAccountCaseDictionary();
            if (accountCases.Count() > 0)
                foreach (var i in accountCases)
                    AccountCases.Add(i.CaseID, i);

            return new GetDWAccountCasesOutput
            {
                AccountCases = AccountCases
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, GetDWAccountCasesOutput result)
        {
            this.AccountCases.Set(context, result.AccountCases);
        }
    }
}
