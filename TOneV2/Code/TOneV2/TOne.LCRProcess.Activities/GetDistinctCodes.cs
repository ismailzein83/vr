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
using TOne.LCR.Entities;

namespace TOne.LCRProcess.Activities
{
    #region Argument Classes

    public class GetDistinctCodesInput
    {
        public DateTime CodeEffectiveOn { get; set; }

        public List<SupplierCodeInfo> SuppliersCodeInfo { get; set; }
    }

    public class GetDistinctCodesOutput
    {
        public CodeList DistinctCodes { get; set; }
    }

    #endregion

    public sealed class GetDistinctCodes : BaseAsyncActivity<GetDistinctCodesInput, GetDistinctCodesOutput>
    {
        [RequiredArgument]
        public InArgument<DateTime> CodeEffectiveOn { get; set; }

        [RequiredArgument]
        public InArgument<List<SupplierCodeInfo>> SuppliersCodeInfo { get; set; }

        public OutArgument<CodeList> DistinctCodes { get; set; }

        protected override GetDistinctCodesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new GetDistinctCodesInput
            {
                CodeEffectiveOn = this.CodeEffectiveOn.Get(context),
                SuppliersCodeInfo = this.SuppliersCodeInfo.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, GetDistinctCodesOutput result)
        {
            this.DistinctCodes.Set(context, result.DistinctCodes);
        }

        protected override GetDistinctCodesOutput DoWorkWithResult(GetDistinctCodesInput inputArgument, AsyncActivityHandle handle)
        {
            ICodeDataManager dataManager = LCRDataManagerFactory.GetDataManager<ICodeDataManager>();
            var distinctCodes = new CodeList(dataManager.GetDistinctCodes(inputArgument.SuppliersCodeInfo, inputArgument.CodeEffectiveOn));
            return new GetDistinctCodesOutput
            {
                DistinctCodes = distinctCodes
            };
        }
    }
}
