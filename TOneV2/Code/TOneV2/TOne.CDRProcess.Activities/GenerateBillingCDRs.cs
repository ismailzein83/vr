using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Business;
using TOne.Caching;
using TOne.Entities;
using Vanrise.Caching;

namespace TOne.CDRProcess.Activities
{
    #region Arguments Classes

    public class GenerateBillingCDRsInput
    {
        public int SwitchID { get; set; }

        public TOneQueue<CDRBatch> InputQueue { get; set; }

        public List<TOneQueue<CDRBase>> OutputQueues { get; set; }


        public Guid CacheManagerId { get; set; }

    }

    #endregion
    public sealed class GenerateBillingCDRs : Vanrise.BusinessProcess.DependentAsyncActivity<GenerateBillingCDRsInput>
    {

        [RequiredArgument]
        public InArgument<Guid> CacheManagerId { get; set; }

        [RequiredArgument]
        public InArgument<TOneQueue<CDRBatch>> InputQueue { get; set; }

        [RequiredArgument]
        public InArgument<int> SwitchID { get; set; }

        [RequiredArgument]
        public InOutArgument<List<TOneQueue<CDRBase>>> OutputQueues { get; set; }


        protected override void OnBeforeExecute(AsyncCodeActivityContext context,Vanrise.BusinessProcess.AsyncActivityHandle handle)
        {
            if (this.OutputQueues.Get(context) == null)
                this.OutputQueues.Set(context, new List<TOneQueue<CDRBase>>());

            base.OnBeforeExecute(context, handle);
        }


        protected override void DoWork(GenerateBillingCDRsInput inputArgument, Vanrise.BusinessProcess.AsyncActivityStatus previousActivityStatus, Vanrise.BusinessProcess.AsyncActivityHandle handle)
        {
            TOneCacheManager cacheManager = CacheManagerFactory.GetCacheManager<TOneCacheManager>(inputArgument.CacheManagerId);
            ProtCodeMap codeMap = new ProtCodeMap(cacheManager);
            CDRManager manager = new CDRManager();

            bool hasItem = false;
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue((cdrBatch) =>
                    {
                        CDRBase CdrBillingGenerated = new CDRBase();

                        foreach (TABS.CDR cdr in cdrBatch.CDRs)
                        {
                            CdrBillingGenerated.CDRs.Add(manager.GenerateBillingCdr(codeMap, cdr));
                        }

                        //CDRBase cdrs = manager.GenerateBillingCdrs(cdrBatch, codeMap);

                        foreach (TOneQueue<CDRBase> outputQueue in inputArgument.OutputQueues)
                            outputQueue.Enqueue(CdrBillingGenerated);


                    });
                }
                while (!ShouldStop(handle) && hasItem);
            });
        }




        protected override GenerateBillingCDRsInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new GenerateBillingCDRsInput
            {
                InputQueue = this.InputQueue.Get(context),
                CacheManagerId = this.CacheManagerId.Get(context),
                SwitchID = this.SwitchID.Get(context),
                OutputQueues = this.OutputQueues.Get(context)
            };
        }
    }
}
