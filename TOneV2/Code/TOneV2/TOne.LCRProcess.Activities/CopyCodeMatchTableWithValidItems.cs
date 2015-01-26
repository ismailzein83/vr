using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.LCR.Entities;
using Vanrise.BusinessProcess;
using TOne.LCR.Data;
using TOne.Entities;

namespace TOne.LCRProcess.Activities
{
    #region Arguments Classes

    public class CopyCodeMatchTableWithValidItemsInput
    {
        public bool IsFuture { get; set; }

        public List<SupplierCodeInfo> SupplierCodeInfo { get; set; }

        public CodeList DistinctCodes { get; set; }
    }

    #endregion

    public sealed class CopyCodeMatchTableWithValidItems : BaseAsyncActivity<CopyCodeMatchTableWithValidItemsInput>
    {
        [RequiredArgument]
        public InArgument<bool> IsFuture { get; set; }

        [RequiredArgument]
        public InArgument<List<SupplierCodeInfo>> SupplierCodeInfo { get; set; }

        [RequiredArgument]
        public InArgument<CodeList> DistinctCodes { get; set; }

        protected override CopyCodeMatchTableWithValidItemsInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new CopyCodeMatchTableWithValidItemsInput
            {
                IsFuture = this.IsFuture.Get(context),
                SupplierCodeInfo = this.SupplierCodeInfo.Get(context),
                DistinctCodes = this.DistinctCodes.Get(context)
            };
        }

        protected override void DoWork(CopyCodeMatchTableWithValidItemsInput inputArgument, AsyncActivityHandle handle)
        {
            ICodeMatchDataManager dataManager = LCRDataManagerFactory.GetDataManager<ICodeMatchDataManager>();
            dataManager.CopyCodeMatchTableWithValidItems(inputArgument.IsFuture, inputArgument.DistinctCodes, inputArgument.SupplierCodeInfo);
        }
    }
}
