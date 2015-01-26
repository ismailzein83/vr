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

        public TOneQueue<CDRBase> OutputStatisticsQueue { get; set; }

        public TOneQueue<CDRBase> OutputPricingQueue { get; set; }

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
        public InOutArgument<TOneQueue<CDRBase>> OutputStatisticsQueue { get; set; }

        [RequiredArgument]
        public InOutArgument<TOneQueue<CDRBase>> OutputPricingQueue { get; set; }
        protected override void DoWork(GenerateBillingCDRsInput inputArgument, Vanrise.BusinessProcess.AsyncActivityHandle handle)
        {
            TOneCacheManager cacheManager = CacheManagerFactory.GetCacheManager<TOneCacheManager>(inputArgument.CacheManagerId);
            ProtCodeMap codeMap = new ProtCodeMap(cacheManager);
            CDRManager manager = new CDRManager();
            CDRBase cdrs = manager.GenerateBillingCdrs(inputArgument.CDRs, codeMap);
            inputArgument.OutputStatisticsQueue.Enqueue(cdrs);
            inputArgument.OutputPricingQueue.Enqueue(cdrs);
        }

        protected override GenerateBillingCDRsInput GetInputArgument(System.Activities.AsyncCodeActivityContext context)
        {
            return new GenerateBillingCDRsInput
            {
                CDRs = this.CDRs.Get(context),
                CacheManagerId = this.CacheManagerId.Get(context),
                SwitchID = this.SwitchID.Get(context),
                OutputStatisticsQueue = this.OutputStatisticsQueue.Get(context),
                OutputPricingQueue = this.OutputPricingQueue.Get(context),
            };
        }
    }
}
