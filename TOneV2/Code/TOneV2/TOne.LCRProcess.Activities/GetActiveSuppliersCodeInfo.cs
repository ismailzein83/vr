using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;
using TOne.LCR.Data;
using TOne.LCR.Entities;

namespace TOne.LCRProcess.Activities
{
    #region Arguments Classes

    public class GetActiveSuppliersCodeInfoInput
    {
        public DateTime CodeEffectiveAfter { get; set; }

        public DateTime CodeEffectiveOn { get; set; }
    }

    public class GetActiveSuppliersCodeInfoOutput
    {
        public List<SupplierCodeInfo> SuppliersCodeInfo { get; set; }
    }

    #endregion

    public sealed class GetActiveSuppliersCodeInfo : BaseAsyncActivity<GetActiveSuppliersCodeInfoInput, GetActiveSuppliersCodeInfoOutput>
    {
        [RequiredArgument]
        public InArgument<DateTime> CodeEffectiveAfter { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> CodeEffectiveOn { get; set; }

        public OutArgument<List<SupplierCodeInfo>> SuppliersCodeInfo { get; set; }

        protected override GetActiveSuppliersCodeInfoInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new GetActiveSuppliersCodeInfoInput
            {
                CodeEffectiveAfter = this.CodeEffectiveAfter.Get(context),
                CodeEffectiveOn = this.CodeEffectiveOn.Get(context)
            };
        }

        protected override GetActiveSuppliersCodeInfoOutput DoWorkWithResult(GetActiveSuppliersCodeInfoInput inputArgument, AsyncActivityHandle handle)
        {
            ICodeDataManager dataManager = LCRDataManagerFactory.GetDataManager<ICodeDataManager>();
            List<SupplierCodeInfo> supplierCodeInfos = dataManager.GetActiveSupplierCodeInfo(inputArgument.CodeEffectiveAfter, inputArgument.CodeEffectiveOn);
            return new GetActiveSuppliersCodeInfoOutput
            {
                SuppliersCodeInfo = supplierCodeInfos
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, GetActiveSuppliersCodeInfoOutput result)
        {
            this.SuppliersCodeInfo.Set(context, result.SuppliersCodeInfo);
        }
    }
}
