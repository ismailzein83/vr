using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Common;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{
    public class ApplySupplierRateInput
    {
        public List<SupplierRate> SupplierRates { get; set; }
        public DateTime EffectiveDate { get; set; }
        public List<long> OldSupplierZones { get; set; }
    }

    public class ApplySupplierRatesForDB : BaseAsyncActivity<ApplySupplierRateInput>
    {
        public InArgument<List<SupplierRate>> SupplierRates { get; set; }
        public InArgument<DateTime> EffectiveDate { get; set; }
        public InArgument<List<long>> OldSupplierZones { get; set; }

       protected override void DoWork(ApplySupplierRateInput inputArgument, AsyncActivityHandle handle)
       {
           DateTime startApplying = DateTime.Now;
           SupplierRateManager manager = new SupplierRateManager();
           manager.UpdateSupplierRates(inputArgument.OldSupplierZones, inputArgument.EffectiveDate);
           manager.InsertSupplierRates(inputArgument.SupplierRates);
           TimeSpan spent = DateTime.Now.Subtract(startApplying);
           handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Apply Supplier Rates  done and takes:{0}", spent);
       }

       protected override ApplySupplierRateInput GetInputArgument(System.Activities.AsyncCodeActivityContext context)
       {
           return new ApplySupplierRateInput
           {
               SupplierRates = this.SupplierRates.Get(context),
               EffectiveDate = this.EffectiveDate.Get(context),
               OldSupplierZones = this.OldSupplierZones.Get(context),
           };
       }
    }
}
