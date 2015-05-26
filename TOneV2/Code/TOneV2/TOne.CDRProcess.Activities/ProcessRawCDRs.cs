using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TABS;
using TOne.Business;
using TOne.Caching;
using TOne.CDR.Entities;
using Vanrise.Caching;
using Vanrise.Queueing;
using Vanrise.BusinessProcess;
using TOne.CDR.Business;

namespace TOne.CDRProcess.Activities
{

    #region Arguments Classes

    public class ProcessRawCDRsInput
    {
        public BaseQueue<TOne.CDR.Entities.CDRBatch> InputQueue { get; set; }

        public BaseQueue<TOne.CDR.Entities.CDRBillingBatch> OutputQueue { get; set; }

    }

    #endregion


    public sealed class ProcessRawCDRs : Vanrise.BusinessProcess.DependentAsyncActivity<ProcessRawCDRsInput>
    {

        [RequiredArgument]
        public InArgument<BaseQueue<TOne.CDR.Entities.CDRBatch>> InputQueue { get; set; }
        
        [RequiredArgument]
        public InOutArgument<BaseQueue<TOne.CDR.Entities.CDRBillingBatch>> OutputQueue { get; set; }


        protected override void OnBeforeExecute(AsyncCodeActivityContext context, Vanrise.BusinessProcess.AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<TOne.CDR.Entities.CDRBillingBatch>());
            var cacheManager = context.GetSharedInstanceData().GetCacheManager<TOneCacheManager>();
            handle.CustomData.Add("CacheManager", cacheManager);
            base.OnBeforeExecute(context, handle);
        }

        protected override void DoWork(ProcessRawCDRsInput inputArgument, Vanrise.BusinessProcess.AsyncActivityStatus previousActivityStatus, Vanrise.BusinessProcess.AsyncActivityHandle handle)
        {
            TOneCacheManager cacheManager = handle.CustomData["CacheManager"] as TOneCacheManager;
            ProtCodeMap codeMap = new ProtCodeMap(cacheManager);
            CDRGenerator cdrGenerator = new CDRGenerator();

            bool hasItem = false;
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue((cdrBatch) =>
                    {
                        TOne.CDR.Entities.CDRBillingBatch cdrBillingGenerated = new TOne.CDR.Entities.CDRBillingBatch();
                        cdrBillingGenerated.CDRs = new List<BillingCDRBase>();
                        TABS.Switch cdrSwitch;
                        if (!TABS.Switch.All.TryGetValue(cdrBatch.SwitchId, out cdrSwitch))
                        {
                            throw new Exception("Switch Not Exist");
                        }
                        foreach (TABS.CDR cdr in cdrBatch.CDRs)
                        {
                            cdr.Switch = cdrSwitch;
                            Billing_CDR_Base cdrBase = cdrGenerator.GenerateBillingCdr(codeMap, cdr);
                            cdrBillingGenerated.CDRs.Add(cdrGenerator.GetBillingCDRBase(cdrBase));
                        }
                        inputArgument.OutputQueue.Enqueue(cdrBillingGenerated);
                    });
                }
                while (!ShouldStop(handle) && hasItem);
            });
        }

        protected override ProcessRawCDRsInput GetInputArgument2(System.Activities.AsyncCodeActivityContext context)
        {
            return new ProcessRawCDRsInput
            {
                InputQueue = this.InputQueue.Get(context),
                OutputQueue = this.OutputQueue.Get(context)
            };
        }

    }
}
