using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.SupplierPriceList.Business;
using Vanrise.BusinessProcess;
using Vanrise.Common;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{
    #region Arguments Classes

    public class ApplySupplierCodeInput
    {
        public List<SupplierCode> SupplierCodes { get; set; }
        public DateTime EffectiveDate { get; set; }
        public List<long> OldSupplierZones { get; set; }
    }

    #endregion
    public class ApplySupplierCodesForDB : BaseAsyncActivity<ApplySupplierCodeInput>
    {
        public InArgument<List<SupplierCode>> SupplierCodes { get; set; }
        public InArgument<DateTime> EffectiveDate { get; set; }
        public InArgument<List<long>> OldSupplierZones { get; set; }
        protected override void DoWork(ApplySupplierCodeInput inputArgument, AsyncActivityHandle handle)
        {
            DateTime startApplying = DateTime.Now;
            SupplierCodeManager manager = new SupplierCodeManager();
           // manager.UpdateSupplierCodes(inputArgument.OldSupplierZones, inputArgument.EffectiveDate);
         //   manager.InsertSupplierCodes(inputArgument.SupplierCodes);
            TimeSpan spent = DateTime.Now.Subtract(startApplying);
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Apply Supplier Codes  done and takes:{0}", spent);
        }

        protected override ApplySupplierCodeInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new ApplySupplierCodeInput
            {
                SupplierCodes = this.SupplierCodes.Get(context),
                EffectiveDate = this.EffectiveDate.Get(context),
                OldSupplierZones = this.OldSupplierZones.Get(context),
            };
        }
    }
}
