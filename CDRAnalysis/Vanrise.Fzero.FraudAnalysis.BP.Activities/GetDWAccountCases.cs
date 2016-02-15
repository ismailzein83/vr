using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using Vanrise.BusinessProcess;
using Vanrise.Common;
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
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Started comparing account cases");
            DWAccountCaseManager accountCaseManager = new DWAccountCaseManager();
            IEnumerable<DWAccountCase> accountCases = accountCaseManager.GetDWAccountCases(inputArgument.FromDate, inputArgument.ToDate);
            DWAccountCaseDictionary AccountCases = new DWAccountCaseDictionary();

            foreach (var accountCase in accountCases)
            {
                List<DWAccountCase> values = AccountCases.GetOrCreateItem(accountCase.AccountNumber, () =>
                {
                    return new List<DWAccountCase>();
                });
                values.Add(accountCase);
            }


          
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Finished comparing {0} account cases", accountCases.Count());

            return new GetDWAccountCasesOutput
            {
                AccountCases = AccountCases
            };
        }

        List<DWAccountCase> DWAccountCaseInstatnce()
        {
            return new List<DWAccountCase>();
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, GetDWAccountCasesOutput result)
        {
            this.AccountCases.Set(context, result.AccountCases);
        }
    }
}
