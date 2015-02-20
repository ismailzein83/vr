using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Business;
using TOne.Caching;
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

        public Guid CacheManagerId { get; set; }

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

        [RequiredArgument]
        public InArgument<Guid> CacheManagerId { get; set; }

        #endregion

        #region ProcessBillingCDR Controls

        private void HandlePassThrough(BillingCDRMain cdr)
        {
            TABS.CarrierAccount Customer = TABS.CarrierAccount.All.ContainsKey(cdr.CustomerID) ? TABS.CarrierAccount.All[cdr.CustomerID] : null;
            TABS.CarrierAccount Supplier = TABS.CarrierAccount.All.ContainsKey(cdr.SupplierID) ? TABS.CarrierAccount.All[cdr.SupplierID] : null;

            if (Customer == null || Supplier == null) return;

            if (Customer.IsPassThroughCustomer && cdr.cost != null)
            {
                var sale = new BillingCDRSale();
                cdr.sale = sale;
                sale.Copy(cdr.cost);
                sale.ZoneID = cdr.OurZoneID;
            }
            if (Supplier.IsPassThroughSupplier && cdr.sale != null)
            {
                var cost = new BillingCDRCost();
                cdr.cost = cost;
                cost.Copy(cdr.sale);
                cost.ZoneID = cdr.SupplierZoneID;
            }
        }

        #endregion

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, Vanrise.BusinessProcess.AsyncActivityHandle handle)
        {
            if (this.OutputMainCDRQueue.Get(context) == null)
                this.OutputMainCDRQueue.Set(context, new MemoryQueue<TOne.CDR.Entities.CDRMainBatch>());

            if (this.OutputInvalidCDRQueue.Get(context) == null)
                this.OutputInvalidCDRQueue.Set(context, new MemoryQueue<TOne.CDR.Entities.CDRInvalidBatch>());

            base.OnBeforeExecute(context, handle);
        }

        protected override void DoWork(ProcessBillingCDRInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            TOneCacheManager cacheManager = CacheManagerFactory.GetCacheManager<TOneCacheManager>(inputArgument.CacheManagerId);

            TOne.Business.ProtPricingGenerator generator;
            
            generator = new TOne.Business.ProtPricingGenerator(cacheManager);


            ProtCodeMap codeMap = new ProtCodeMap(cacheManager);

            bool hasItem = false;
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue((billingCDR) =>
                    {
                        TOne.CDR.Entities.CDRMainBatch CDRMains = new TOne.CDR.Entities.CDRMainBatch();
                        TOne.CDR.Entities.CDRInvalidBatch CDRInvalids = new TOne.CDR.Entities.CDRInvalidBatch();

                        foreach (BillingCDRBase CDR in billingCDR.CDRs)
                        {

                            if (CDR.IsValid)
                            {
                                BillingCDRMain main = (BillingCDRMain)CDR;

                                main.cost = generator.GetRepricing<BillingCDRCost>(main);
                                main.sale = generator.GetRepricing<BillingCDRSale>(main);

                                HandlePassThrough(main);

                                if (main != null && main.cost != null && main.SupplierCode != null)
                                    main.cost.Code = main.SupplierCode;

                                if (main != null && main.sale != null && main.OurCode != null)
                                    main.sale.Code = main.OurCode;

                                CDRMains.MainCDRs.Add(main);

                            }
                            else
                                CDRInvalids.InvalidCDRs.Add((BillingCDRInvalid)(CDR));
                        }

                        inputArgument.OutputMainCDRQueue.Enqueue(CDRMains);
                        inputArgument.OutputInvalidCDRQueue.Enqueue(CDRInvalids);

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
                OutputInvalidCDRQueue = this.OutputInvalidCDRQueue.Get(context),
                CacheManagerId = this.CacheManagerId.Get(context)
            };
        }
    }
}
