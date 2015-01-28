using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TABS;
using TOne.Business;
using TOne.Caching;
using TOne.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Caching;

namespace TOne.CDRProcess.Activities
{
    #region Arguments Classes

    public class ProcessBillingCDRsInput
    {
        public TOneQueue<CDRBillingBatch> InputQueue { get; set; }

        public TOneQueue<CDRMainBatch> OutputMainCDRQueue { get; set; }

        public TOneQueue<CDRInvalidBatch> OutputInvalidCDRQueue { get; set; }

        public Guid CacheManagerId { get; set; }

    }

    #endregion

    public sealed class ProcessBillingCDRs : DependentAsyncActivity<ProcessBillingCDRsInput>
    {
        #region Arguments

        [RequiredArgument]
        public InArgument<TOneQueue<CDRBillingBatch>> InputQueue { get; set; }

        [RequiredArgument]
        public InOutArgument<TOneQueue<CDRMainBatch>> OutputMainCDRQueue { get; set; }

        [RequiredArgument]
        public InOutArgument<TOneQueue<CDRInvalidBatch>> OutputInvalidCDRQueue { get; set; }

        [RequiredArgument]
        public InArgument<Guid> CacheManagerId { get; set; }

        #endregion

        #region ProcessBillingCDRs Controls

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
                this.OutputMainCDRQueue.Set(context, new TOneQueue<CDRBillingBatch>());

            if (this.OutputInvalidCDRQueue.Get(context) == null)
                this.OutputInvalidCDRQueue.Set(context, new TOneQueue<CDRBillingBatch>());

            base.OnBeforeExecute(context, handle);
        }

        protected override void DoWork(ProcessBillingCDRsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
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
                        CDRMainBatch CDRMains = new CDRMainBatch();
                        CDRInvalidBatch CDRInvalids = new CDRInvalidBatch();

                        foreach (TABS.Billing_CDR_Base CDR in billingCDR.CDRs)
                        {
                            if (CDR.IsValid)
                            {
                                TABS.Billing_CDR_Main main = (TABS.Billing_CDR_Main)CDR;
                                main.Billing_CDR_Cost = generator.GetRepricing<TABS.Billing_CDR_Cost>(main);
                                main.Billing_CDR_Sale = generator.GetRepricing<TABS.Billing_CDR_Sale>(main);
                                if (main.Billing_CDR_Sale == null)
                                    ((ProtPricingGenerator)generator).FixParentCodeSale(main, null, null, codeMap);
                                HandlePassThrough(main);

                                if (main != null && main.Billing_CDR_Cost != null && main.SupplierCode != null)
                                    main.Billing_CDR_Cost.Code = main.SupplierCode;

                                if (main != null && main.Billing_CDR_Sale != null && main.OurCode != null)
                                    main.Billing_CDR_Sale.Code = main.OurCode;

                                CDRMains.mainCDRs.Add(main);

                            }
                            else
                                CDRInvalids.InvalidCDRs.Add(new Billing_CDR_Invalid(CDR));
                        }
                        inputArgument.OutputMainCDRQueue.Enqueue(CDRMains);
                        inputArgument.OutputInvalidCDRQueue.Enqueue(CDRInvalids);
                    });
                }
                while (!ShouldStop(handle) && hasItem);
            });

        }

        protected override ProcessBillingCDRsInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ProcessBillingCDRsInput
            {
                InputQueue = this.InputQueue.Get(context),
                OutputMainCDRQueue = this.OutputMainCDRQueue.Get(context),
                OutputInvalidCDRQueue = this.OutputInvalidCDRQueue.Get(context),
                CacheManagerId = this.CacheManagerId.Get(context)
            };
        }
    }
}
