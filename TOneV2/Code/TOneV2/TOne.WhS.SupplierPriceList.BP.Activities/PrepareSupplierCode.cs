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

    public class PrepareSupplierCodeInput
    {
        public List<SupplierZone> SupplierZones { get; set; }

        public DateTime EffectiveDate { get; set; }
        public Dictionary<String, List<TOne.WhS.SupplierPriceList.Entities.SupplierPriceList>> PriceListDictionary { get; set; }
        public List<SupplierCode> SupplierCodes { get; set; }
    }

    #endregion
    public class PrepareSupplierCode : BaseAsyncActivity<PrepareSupplierCodeInput>
    {
        public InArgument<List<SupplierZone>> SupplierZones { get; set; }
        public InArgument<DateTime> EffectiveDate { get; set; }
        public InArgument<Dictionary<String, List<TOne.WhS.SupplierPriceList.Entities.SupplierPriceList>>> PriceListDictionary { get; set; }

        public OutArgument<List<SupplierCode>> SupplierCodes { get; set; }
       

        protected override void DoWork(PrepareSupplierCodeInput inputArgument, AsyncActivityHandle handle)
        {
            DateTime startPreparing = DateTime.Now;
            foreach (SupplierZone supplierZone in inputArgument.SupplierZones)
            {
                List<TOne.WhS.SupplierPriceList.Entities.SupplierPriceList> supplierPriceList = null;
                if (inputArgument.PriceListDictionary.TryGetValue(supplierZone.Name, out supplierPriceList))
                {
                    foreach (TOne.WhS.SupplierPriceList.Entities.SupplierPriceList obj in supplierPriceList)
                        inputArgument.SupplierCodes.Add(new SupplierCode
                        {
                            Code = obj.Code,
                            ZoneId = supplierZone.SupplierZoneId,
                            BeginEffectiveDate = obj.BED,
                            EndEffectiveDate = null
                        });
                }
            }
            TimeSpan spent = DateTime.Now.Subtract(startPreparing);
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Preparing Supplier Codes  done and takes:{0}", spent);
        }

        protected override PrepareSupplierCodeInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new PrepareSupplierCodeInput
            {
                SupplierZones = this.SupplierZones.Get(context),
                EffectiveDate = this.EffectiveDate.Get(context),
                PriceListDictionary = this.PriceListDictionary.Get(context),
                SupplierCodes = this.SupplierCodes.Get(context),
            };
        }
        protected override void OnBeforeExecute(AsyncCodeActivityContext context, Vanrise.BusinessProcess.AsyncActivityHandle handle)
        {
            if (this.SupplierCodes.Get(context) == null)
                this.SupplierCodes.Set(context, new List<SupplierCode>());

            base.OnBeforeExecute(context, handle);
        }

    }
}
