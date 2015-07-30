using System;
using System.Collections.Generic;
using TOne.Business;
using TOne.Caching;
using TOne.CDR.Business;
using TOne.CDR.Entities;
using Vanrise.Queueing.Entities;

namespace TOne.CDR.QueueActivators
{
    public class ProcessBillingCDRsActivator : QueueActivator
    {
        public override void OnDisposed()
        {
            
        }

        public override void ProcessItem(PersistentQueueItem item, ItemsToEnqueue outputItems)
        {
            Guid cacheManagerId = Guid.NewGuid();
            TOneCacheManager cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<TOneCacheManager>(cacheManagerId);
            PricingGenerator generator = new PricingGenerator(cacheManager);
            CDRGenerator cdrGenerator = new CDRGenerator();
            ProtCodeMap codeMap = new ProtCodeMap(cacheManager);

            
            CDRBillingBatch billingCdr = item as CDRBillingBatch;

            CDRMainBatch cdrMains = new CDRMainBatch
            {
                MainCDRs = new List<BillingCDRMain>()
            };
            
            CDRInvalidBatch cdrInvalids = new CDRInvalidBatch
            {
                InvalidCDRs = new List<BillingCDRInvalid>()
            };

            if (billingCdr != null)
            {
                foreach (BillingCDRBase cdr in billingCdr.CDRs)
                {
                    if (cdr.IsValid)
                    {
                        BillingCDRMain main = new BillingCDRMain(cdr);

                        main.cost = generator.GetRepricing<BillingCDRCost>(main);
                        main.sale = generator.GetRepricing<BillingCDRSale>(main);

                        cdrGenerator.HandlePassThrough(main);

                        if (main.cost != null && main.SupplierCode != null)
                            main.cost.Code = main.SupplierCode;

                        if (main.sale != null && main.OurCode != null)
                            main.sale.Code = main.OurCode;

                        cdrMains.MainCDRs.Add(main);

                    }
                    else
                        cdrInvalids.InvalidCDRs.Add(new BillingCDRInvalid(cdr));
                }

                if (cdrMains.MainCDRs.Count > 0) outputItems.Add("Store CDR Main", cdrMains);
                if (cdrInvalids.InvalidCDRs.Count > 0) outputItems.Add("Store CDR Invalid", cdrInvalids);
            }
            
            Vanrise.Caching.CacheManagerFactory.RemoveCacheManager(cacheManagerId);
        }
    }
}
