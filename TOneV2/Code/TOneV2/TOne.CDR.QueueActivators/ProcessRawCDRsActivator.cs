using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TABS;
using TOne.Business;
using TOne.Caching;
using TOne.CDR.Business;
using TOne.CDR.Entities;
using Vanrise.Queueing.Entities;

namespace TOne.CDR.QueueActivators
{
    public class ProcessRawCDRsActivator : QueueActivator
    {
        public override void ProcessItem(PersistentQueueItem item, ItemsToEnqueue outputItems)
        {
            Guid cacheManagerId = Guid.NewGuid();
            TOneCacheManager cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<TOneCacheManager>(cacheManagerId);
            ProtCodeMap codeMap = new ProtCodeMap(cacheManager);
            CDRGenerator cdrGenerator = new CDRGenerator();
            CDRBatch cdrBatch = item as CDRBatch;
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
                Billing_CDR_Base billingCDR = null;

                if (cdr.DurationInSeconds > 0)
                {
                    billingCDR = new Billing_CDR_Main();
                }
                else
                    billingCDR = new Billing_CDR_Invalid();
                billingCDR.Attempt = cdr.AttemptDateTime;
                cdrBillingGenerated.CDRs.Add(cdrGenerator.GetBillingCDRBase(billingCDR));
            }
            outputItems.Add("Process Billing CDRs", cdrBillingGenerated);
            Vanrise.Caching.CacheManagerFactory.RemoveCacheManager(cacheManagerId);
        }

        public override void OnDisposed()
        {
            
        }
    }
}
