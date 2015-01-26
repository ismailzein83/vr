using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;
using TOne.LCR.Entities;
using TOne.Entities;
using TOne.LCR.Data;

namespace TOne.LCRProcess.Activities
{
    #region Argument Classes

    public class BuildCodeMatchFromDistinctCodesInput
    {
        public CodeList DistinctCodes { get; set; }

        public DateTime EffectiveOn { get; set; }

        public List<SupplierCodeInfo> SupplierCodeInfo { get; set; }

        public TOneQueue<List<CodeMatch>> OutputQueue { get; set; }
    }

    #endregion

    public sealed class BuildCodeMatchFromDistinctCodes : BaseAsyncActivity<BuildCodeMatchFromDistinctCodesInput>
    {
        [RequiredArgument]
        public InArgument<CodeList> DistinctCodes { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> EffectiveOn { get; set; }

        [RequiredArgument]
        public InArgument<List<SupplierCodeInfo>> SupplierCodeInfo { get; set; }

        [RequiredArgument]
        public InOutArgument<TOneQueue<List<CodeMatch>>> OutputQueue { get; set; }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new TOneQueue<List<CodeMatch>>());
            base.OnBeforeExecute(context, handle);
        }

        protected override BuildCodeMatchFromDistinctCodesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new BuildCodeMatchFromDistinctCodesInput
            {
                DistinctCodes = this.DistinctCodes.Get(context),
                EffectiveOn = this.EffectiveOn.Get(context),
                SupplierCodeInfo = this.SupplierCodeInfo.Get(context),
                OutputQueue = this.OutputQueue.Get(context)
            };
        }

        protected override void DoWork(BuildCodeMatchFromDistinctCodesInput inputArgument, AsyncActivityHandle handle)
        {
            ICodeDataManager dataManager = LCRDataManagerFactory.GetDataManager<ICodeDataManager>();
            List<CodeMatch> codeMatches = new List<CodeMatch>();
            dataManager.LoadCodeMatchesFromDistinctCodes(inputArgument.DistinctCodes, inputArgument.EffectiveOn, inputArgument.SupplierCodeInfo, (codeMatch) =>
                {
                    codeMatches.Add(codeMatch);
                    if (codeMatches.Count > 50000)
                    {
                        inputArgument.OutputQueue.Enqueue(codeMatches);
                        codeMatches = new List<CodeMatch>();
                    }
                });
        }
    }
}
