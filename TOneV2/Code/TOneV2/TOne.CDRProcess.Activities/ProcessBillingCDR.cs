﻿using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Business;
using TOne.Caching;
using TOne.CDR.Business;
using TOne.CDR.Entities;
using TOne.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Caching;
using Vanrise.Queueing;

namespace TOne.CDRProcess.Activities
{
    #region Arguments Classes

    public class ProcessBillingCDRInput
    {
        public BaseQueue<TOne.CDR.Entities.CDRBillingBatch> InputQueue { get; set; }

        public BaseQueue<TOne.CDR.Entities.CDRMainBatch> OutputMainCDRQueue { get; set; }

        public BaseQueue<TOne.CDR.Entities.CDRInvalidBatch> OutputInvalidCDRQueue { get; set; }


    }

    #endregion

    public sealed class ProcessBillingCDR : DependentAsyncActivity<ProcessBillingCDRInput>
    {
        #region Arguments

        [RequiredArgument]
        public InArgument<BaseQueue<TOne.CDR.Entities.CDRBillingBatch>> InputQueue { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<TOne.CDR.Entities.CDRMainBatch>> OutputMainCDRQueue { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<TOne.CDR.Entities.CDRInvalidBatch>> OutputInvalidCDRQueue { get; set; }

        #endregion

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, Vanrise.BusinessProcess.AsyncActivityHandle handle)
        {
            if (this.OutputMainCDRQueue.Get(context) == null)
                this.OutputMainCDRQueue.Set(context, new MemoryQueue<TOne.CDR.Entities.CDRMainBatch>());

            if (this.OutputInvalidCDRQueue.Get(context) == null)
                this.OutputInvalidCDRQueue.Set(context, new MemoryQueue<TOne.CDR.Entities.CDRInvalidBatch>());
            
            var cacheManager = context.GetSharedInstanceData().GetCacheManager<TOneCacheManager>();
            handle.CustomData.Add("CacheManager", cacheManager);
            base.OnBeforeExecute(context, handle);
        }

        protected override void DoWork(ProcessBillingCDRInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            TOneCacheManager cacheManager = handle.CustomData["CacheManager"] as TOneCacheManager;

            PricingGenerator generator;
            
            generator = new PricingGenerator(cacheManager);
            CDRGenerator cdrGenerator = new CDRGenerator();

            ProtCodeMap codeMap = new ProtCodeMap(cacheManager);

            bool hasItem = false;
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue((billingCDR) =>
                    {
                        TOne.CDR.Entities.CDRMainBatch  cdrMains = new TOne.CDR.Entities.CDRMainBatch();
                        TOne.CDR.Entities.CDRInvalidBatch cdrInvalids = new TOne.CDR.Entities.CDRInvalidBatch();

                        cdrMains.MainCDRs = new List<BillingCDRMain>();
                        cdrInvalids.InvalidCDRs = new List<BillingCDRInvalid>();

                        foreach (BillingCDRBase cdr in billingCDR.CDRs)
                        {

                            if (cdr.IsValid)
                            {
                                BillingCDRMain main = new BillingCDRMain(cdr);

                                main.cost = generator.GetRepricing<BillingCDRCost>(main);
                                main.sale = generator.GetRepricing<BillingCDRSale>(main);

                                cdrGenerator.HandlePassThrough(main);

                                if (main != null && main.cost != null && main.SupplierCode != null)
                                    main.cost.Code = main.SupplierCode;

                                if (main != null && main.sale != null && main.OurCode != null)
                                    main.sale.Code = main.OurCode;

                                cdrMains.MainCDRs.Add(main);

                            }
                            else
                                cdrInvalids.InvalidCDRs.Add(new BillingCDRInvalid(cdr));
                        }

                        if (cdrMains.MainCDRs.Count > 0) inputArgument.OutputMainCDRQueue.Enqueue(cdrMains);
                        if (cdrInvalids.InvalidCDRs.Count > 0) inputArgument.OutputInvalidCDRQueue.Enqueue(cdrInvalids);

                    });
                }
                while (!ShouldStop(handle) && hasItem);
            });

        }

        protected override ProcessBillingCDRInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ProcessBillingCDRInput
            {
                InputQueue = this.InputQueue.Get(context),
                OutputMainCDRQueue = this.OutputMainCDRQueue.Get(context),
                OutputInvalidCDRQueue = this.OutputInvalidCDRQueue.Get(context)
            };
        }
    }
}
