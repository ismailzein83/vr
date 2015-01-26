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

    public class GetDistinctCodesFromCodeMatchInput
    {
        public bool IsFuture { get; set; }

        public List<SupplierCodeInfo> SuppliersCodeInfo { get; set; }
    }

    public class GetDistinctCodesFromCodeMatchOutput
    {
        public CodeList DistinctCodes { get; set; }
    }

    #endregion

    public sealed class GetDistinctCodesFromCodeMatch : BaseAsyncActivity<GetDistinctCodesFromCodeMatchInput, GetDistinctCodesFromCodeMatchOutput>
    {
        [RequiredArgument]
        public InArgument<bool> IsFuture { get; set; }

        public InArgument<List<SupplierCodeInfo>> SuppliersCodeInfo { get; set; }

        public OutArgument<CodeList> DistinctCodes { get; set; }

        protected override GetDistinctCodesFromCodeMatchInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new GetDistinctCodesFromCodeMatchInput
            {
                IsFuture = this.IsFuture.Get(context),
                SuppliersCodeInfo = this.SuppliersCodeInfo.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, GetDistinctCodesFromCodeMatchOutput result)
        {
            this.DistinctCodes.Set(context, result.DistinctCodes);
        }

        protected override GetDistinctCodesFromCodeMatchOutput DoWorkWithResult(GetDistinctCodesFromCodeMatchInput inputArgument, AsyncActivityHandle handle)
        {
            ICodeMatchDataManager dataManager = LCRDataManagerFactory.GetDataManager<ICodeMatchDataManager>();
            CodeList distinctCodes = new CodeList(dataManager.GetDistinctCodes(inputArgument.IsFuture, inputArgument.SuppliersCodeInfo));
            return new GetDistinctCodesFromCodeMatchOutput
            {
                DistinctCodes = distinctCodes
            };
        }
    }
}
