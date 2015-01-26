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

        public CDRBatch CDRs { get; set; }

        public List<TOneQueue<CDRBase>> OutputQueues { get; set; }


        public Guid CacheManagerId { get; set; }

    }

    #endregion
    public sealed class GenerateBillingCDRs : Vanrise.BusinessProcess.BaseAsyncActivity<GenerateBillingCDRsInput>
    {

        [RequiredArgument]
        public InArgument<Guid> CacheManagerId { get; set; }

        [RequiredArgument]
        public InArgument<CDRBatch> CDRs { get; set; }

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

        protected override void DoWork(GenerateBillingCDRsInput inputArgument, Vanrise.BusinessProcess.AsyncActivityHandle handle)
        {
            TOneCacheManager cacheManager = CacheManagerFactory.GetCacheManager<TOneCacheManager>(inputArgument.CacheManagerId);
            ProtCodeMap codeMap = new ProtCodeMap(cacheManager);
            CDRManager manager = new CDRManager();
            CDRBase cdrs = manager.GenerateBillingCdrs(inputArgument.CDRs, codeMap);
            inputArgument.OutputQueues[0].Enqueue(cdrs);
            inputArgument.OutputQueues[1].Enqueue(cdrs);
        }

        protected override GenerateBillingCDRsInput GetInputArgument(System.Activities.AsyncCodeActivityContext context)
        {
            return new GenerateBillingCDRsInput
            {
                CDRs = this.CDRs.Get(context),
                CacheManagerId = this.CacheManagerId.Get(context),
                SwitchID = this.SwitchID.Get(context),
                OutputQueues = this.OutputQueues.Get(context),
            };
        }
    }
}
