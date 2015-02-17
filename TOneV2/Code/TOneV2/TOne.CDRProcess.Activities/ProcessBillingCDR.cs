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

        private void HandlePassThrough(TABS.Billing_CDR_Main cdr)
        {
            if (cdr.Customer.IsPassThroughCustomer && cdr.Billing_CDR_Cost != null)
            {
                var sale = new TABS.Billing_CDR_Sale();
                cdr.Billing_CDR_Sale = sale;
                sale.Copy(cdr.Billing_CDR_Cost);
                sale.Zone = cdr.OurZone;
            }
            if (cdr.Supplier.IsPassThroughSupplier && cdr.Billing_CDR_Sale != null)
            {
                var cost = new TABS.Billing_CDR_Cost();
                cdr.Billing_CDR_Cost = cost;
                cost.Copy(cdr.Billing_CDR_Sale);
                cost.Zone = cdr.SupplierZone;
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
            using (NHibernate.ISession session = TABS.DataConfiguration.OpenSession())
            {
                generator = new TOne.Business.ProtPricingGenerator(cacheManager, session);
                session.Flush();
                session.Close();
            }

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


                                //if (main.sale == null)
                                //    ((ProtPricingGenerator)generator).FixParentCodeSale(main, null, null, codeMap);


                                //HandlePassThrough(main);

                                //if (main != null && main.Billing_CDR_Cost != null && main.SupplierCode != null)
                                //  main.Billing_CDR_Cost.Code = main.SupplierCode;

                                //if (main != null && main.Billing_CDR_Sale != null && main.OurCode != null)
                                //  main.Billing_CDR_Sale.Code = main.OurCode;

                                //CDRMains.mainCDRs.Add(main);

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
