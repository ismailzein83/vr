using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.BusinessProcess;
namespace TOne.WhS.SupplierPriceList.BP.Activities
{
    public class ApplySupplierZonesForDB:CodeActivity
    {
        public InOutArgument<List<SupplierZone>> SupplierZones { get; set; }
        public InArgument<int> SupplierId { get; set; }
        public InArgument<DateTime> EffectiveDate { get; set; }
        public OutArgument<List<long>> OldSupplierZones { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            DateTime startApplying = DateTime.Now;
            SupplierZoneManager manager = new SupplierZoneManager();
            List<SupplierZone> oldSuplierZones = manager.GetSupplierZones(SupplierId.Get(context), EffectiveDate.Get(context));
            List<long> oldSuplierZonesIds = new List<long>();
            foreach (SupplierZone supplierZone in oldSuplierZones)
                oldSuplierZonesIds.Add(supplierZone.SupplierZoneId);

            manager.UpdateSupplierZones(SupplierId.Get(context), EffectiveDate.Get(context));
            manager.InsertSupplierZones(SupplierZones.Get(context));
            List<SupplierZone> suplierZones = manager.GetSupplierZones(SupplierId.Get(context), EffectiveDate.Get(context));
            SupplierZones.Set(context, suplierZones);
            OldSupplierZones.Set(context, oldSuplierZonesIds);
            
            TimeSpan spent = DateTime.Now.Subtract(startApplying);
            context.WriteTrackingMessage(LogEntryType.Information, "Apply Supplier Zones  done and takes:{0}", spent);
        }
    }
}
