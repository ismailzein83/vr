using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;
using TOne.Entities;
using System.Collections.Concurrent;
using TABS;
using TOne.LCR.Entities;
using TOne.LCR.Data;

namespace TOne.LCRProcess.Activities
{
    #region Arguments Classes

    public class LoadCodesForActiveSuppliersInput
    {
        public DateTime EffectiveOn { get; set; }

        public List<SupplierCodeInfo> SuppliersCodeInfo { get; set; }

        public bool OnlySuppliersWithUpdatedCodes { get; set; }

        public TOneQueue<Tuple<string, List<LCRCode>>> OutputQueue { get; set; }
    }

    #endregion

    public sealed class LoadCodesForActiveSuppliers : BaseAsyncActivity<LoadCodesForActiveSuppliersInput>
    {       
        [RequiredArgument]
        public InArgument<DateTime> EffectiveOn { get; set; }

        [RequiredArgument]
        public InArgument<List<SupplierCodeInfo>> SuppliersCodeInfo { get; set; }

        [RequiredArgument]
        public InArgument<bool> OnlySuppliersWithUpdatedCodes { get; set; }

        [RequiredArgument]
        public InOutArgument<TOneQueue<Tuple<string, List<LCRCode>>>> OutputQueue { get; set; }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new TOneQueue<Tuple<string, List<LCRCode>>>());
            base.OnBeforeExecute(context, handle);
        }

        protected override LoadCodesForActiveSuppliersInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new LoadCodesForActiveSuppliersInput
            {
                EffectiveOn = this.EffectiveOn.Get(context),
                SuppliersCodeInfo = this.SuppliersCodeInfo.Get(context),
                OnlySuppliersWithUpdatedCodes = this.OnlySuppliersWithUpdatedCodes.Get(context),
                OutputQueue = this.OutputQueue.Get(context)
            };
        }
        protected override void DoWork(LoadCodesForActiveSuppliersInput inputArgument, AsyncActivityHandle handle)
        {
            ICodeDataManager dataManager = LCRDataManagerFactory.GetDataManager<ICodeDataManager>();
            DateTime start = DateTime.Now;
            dataManager.LoadCodesForActiveSuppliers(inputArgument.EffectiveOn, inputArgument.SuppliersCodeInfo, inputArgument.OnlySuppliersWithUpdatedCodes,
                (supplierId, supplierCodes) =>
                {
                    //Console.WriteLine("{0}: {1} codes loaded for supplier {2}", DateTime.Now, supplierCodes.Count, supplierId);
                    inputArgument.OutputQueue.Enqueue(new Tuple<string, List<LCRCode>>(supplierId, supplierCodes));
                });

            Console.WriteLine("{0}: LoadCodesForActiveSuppliers is done in {1}", DateTime.Now, (DateTime.Now - start));
        }
    }
}
