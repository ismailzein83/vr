using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Caching;
using TOne.Caching;
using TABS;
using TOne.Entities;
using Vanrise.BusinessProcess;
using TOne.LCR.Data;

namespace TOne.LCRProcess.Activities
{
    #region Argument Classes

    public class GetDistinctCodesInput
    {
        public bool IsFuture { get; set; }
        public char FirstDigit { get; set; }
        public DateTime EffectiveOn { get; set; }
        public bool GetChangedGroupsOnly { get; set; }
    }

    public class GetDistinctCodesOutput
    {
        public CodeTree DistinctCodes { get; set; }
    }

    #endregion

    public sealed class GetDistinctCodes : BaseAsyncActivity<GetDistinctCodesInput, GetDistinctCodesOutput>
    {
        [RequiredArgument]
        public InArgument<bool> IsFuture { get; set; }

        [RequiredArgument]
        public InArgument<char> FirstDigit { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> EffectiveOn { get; set; }

        [RequiredArgument]
        public InArgument<bool> GetChangedGroupsOnly { get; set; }

        public OutArgument<CodeTree> DistinctCodes { get; set; }

        protected override GetDistinctCodesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new GetDistinctCodesInput
            {
                IsFuture = this.IsFuture.Get(context),
                FirstDigit = this.FirstDigit.Get(context),
                EffectiveOn = this.EffectiveOn.Get(context),
                GetChangedGroupsOnly = this.GetChangedGroupsOnly.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, GetDistinctCodesOutput result)
        {
            this.DistinctCodes.Set(context, result.DistinctCodes);
        }

        protected override GetDistinctCodesOutput DoWorkWithResult(GetDistinctCodesInput inputArgument, AsyncActivityHandle handle)
        {
            ICodeDataManager dataManager = LCRDataManagerFactory.GetDataManager<ICodeDataManager>();
            var distinctCodes = new CodeTree(dataManager.GetDistinctCodes(inputArgument.IsFuture));//, inputArgument.EffectiveOn, inputArgument.GetChangedGroupsOnly));
            return new GetDistinctCodesOutput
            {
                DistinctCodes = distinctCodes
            };
        }
    }
}
