using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;
using TOne.Entities;
using System.Collections.Concurrent;
using TOne.LCR.Entities;
using TABS;
using TOne.LCR.Data;

namespace TOne.LCRProcess.Activities
{
    #region Arguments Classes

    public class LoadCodesByUpdatedSuppliersInput
    {
        public byte[] CodeUpdatedAfter { get; set; }

        public bool IsFuture { get; set; }

        public ConcurrentQueue<Tuple<string, List<LCRCode>>> QueueLoadedCodes { get; set; }
    }

    #endregion

    public sealed class LoadCodesByUpdatedSuppliers : BaseAsyncActivity<LoadCodesByUpdatedSuppliersInput>
    {
        [RequiredArgument]
        public InArgument<byte[]> CodeUpdatedAfter { get; set; }

        [RequiredArgument]
        public InArgument<bool> IsFuture { get; set; }

        [RequiredArgument]
        public InArgument<ConcurrentQueue<Tuple<string, List<LCRCode>>>> QueueLoadedCodes { get; set; }

        protected override LoadCodesByUpdatedSuppliersInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new LoadCodesByUpdatedSuppliersInput
            {
                CodeUpdatedAfter = this.CodeUpdatedAfter.Get(context),
                IsFuture = this.IsFuture.Get(context),
                QueueLoadedCodes = this.QueueLoadedCodes.Get(context)
            };
        }
        protected override void DoWork(LoadCodesByUpdatedSuppliersInput inputArgument, AsyncActivityHandle handle)
        {
            DateTime effectiveOn = DateTime.Today;
            if (inputArgument.IsFuture)
            {
                var noticeDays = (double)TABS.SystemConfiguration.KnownParameters[KnownSystemParameter.sys_BeginEffectiveRateDays].NumericValue.Value;
                effectiveOn = DateTime.Today.AddDays(noticeDays);
            }
            byte[] updatedAfter = inputArgument.CodeUpdatedAfter;
            if (updatedAfter == null)
                updatedAfter = new byte[0];
            ICodeDataManager dataManager = LCRDataManagerFactory.GetDataManager<ICodeDataManager>();
            dataManager.LoadCodesByUpdatedSuppliers(updatedAfter, effectiveOn, (supplierId, supplierCodes) =>
                {
                    Console.WriteLine("{0}: {1} codes loaded for supplier {2}", DateTime.Now, supplierCodes.Count, supplierId);
                    inputArgument.QueueLoadedCodes.Enqueue(new Tuple<string, List<LCRCode>>(supplierId, supplierCodes));                    
                });
        }
    }
}
