using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Common;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{
    #region Arguments Classes

    public class PrepareSupplierRateInput
    {
        public List<SupplierZone> SupplierZones { get; set; }

        public DateTime EffectiveDate { get; set; }
        public Dictionary<String, List<TOne.WhS.SupplierPriceList.Entities.SupplierPriceList>> PriceListDictionary { get; set; }
        public List<SupplierRate> SupplierRates { get; set; }
        public int SupplierPriceListId { get; set; }
    }

    #endregion
    public class PrepareSupplierRate : BaseAsyncActivity<PrepareSupplierRateInput>
    {
        public InArgument<List<SupplierZone>> SupplierZones { get; set; }
        public InArgument<DateTime> EffectiveDate { get; set; }
        public InArgument<Dictionary<String, List<TOne.WhS.SupplierPriceList.Entities.SupplierPriceList>>> PriceListDictionary { get; set; }
        public OutArgument<List<SupplierRate>> SupplierRates { get; set; }

        public InArgument<int> SupplierPriceListId { get; set; }
        protected override void DoWork(PrepareSupplierRateInput inputArgument, AsyncActivityHandle handle)
        {
            DateTime startPreparing = DateTime.Now;
            foreach (SupplierZone supplierZone in inputArgument.SupplierZones)
            {
                List<TOne.WhS.SupplierPriceList.Entities.SupplierPriceList> supplierPriceList = null;
                if (inputArgument.PriceListDictionary.TryGetValue(supplierZone.Name, out supplierPriceList))
                {
                        inputArgument.SupplierRates.Add(new SupplierRate
                        {
                            NormalRate = supplierPriceList[0].Rate,
                            PriceListId = inputArgument.SupplierPriceListId,
                            ZoneId = supplierZone.SupplierZoneId,
                            BeginEffectiveDate = supplierPriceList[0].BED,
                            EndEffectiveDate = null
                        });
                }
            }
            TimeSpan spent = DateTime.Now.Subtract(startPreparing);
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Preparing Supplier Rate  done and takes:{0}", spent);
        }

        protected override PrepareSupplierRateInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new PrepareSupplierRateInput
            {
                SupplierZones = this.SupplierZones.Get(context),
                EffectiveDate = this.EffectiveDate.Get(context),
                PriceListDictionary = this.PriceListDictionary.Get(context),
                SupplierRates = this.SupplierRates.Get(context),
                SupplierPriceListId = this.SupplierPriceListId.Get(context),
            };
        }
        protected override void OnBeforeExecute(AsyncCodeActivityContext context, Vanrise.BusinessProcess.AsyncActivityHandle handle)
        {
            if (this.SupplierRates.Get(context) == null)
                this.SupplierRates.Set(context, new List<SupplierRate>());

            base.OnBeforeExecute(context, handle);
        }
    }
}
