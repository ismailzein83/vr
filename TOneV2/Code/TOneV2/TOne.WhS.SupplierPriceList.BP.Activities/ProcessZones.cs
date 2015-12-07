using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.SupplierPriceList.Entities;
using Vanrise.Common;
using Vanrise.BusinessProcess;
namespace TOne.WhS.SupplierPriceList.BP.Activities
{
    public class ProcessZones:CodeActivity
    {
        public InArgument<int> SupplierId { get; set; }
        public InArgument<DateTime?> EffectiveDate { get; set; }
        public InArgument<PriceListByZone> PriceListByZone { get; set; }
        public OutArgument<List<Zone>> Zones { get; set; }
        public InArgument<DateTime?> MinimumDate { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            DateTime startPreparing = DateTime.Now;
            int supplierId = SupplierId.Get(context);
            DateTime? effectiveDate = EffectiveDate.Get(context);
            PriceListByZone priceListByZone = PriceListByZone.Get(context);
            SupplierZoneManager manager = new SupplierZoneManager();
            List<SupplierZone> existingZones = manager.GetSupplierZones(supplierId, (DateTime)MinimumDate.Get(context));
            Dictionary<string,SupplierZone> existingZonesDictionary=new Dictionary<string,SupplierZone>(); 
            foreach(SupplierZone supplierZone in existingZones)
                if(!existingZonesDictionary.ContainsKey(supplierZone.Name))
                 existingZonesDictionary.Add(supplierZone.Name,supplierZone);

            List<Zone> zones = new List<Zone>();

            foreach (var zone in priceListByZone)
            {
                
                Zone addZone = new Zone
                {
                    SupplierId = supplierId,
                    NewRate = zone.Value.Rate,
                    NewCodes = zone.Value.Codes,
                };
                SupplierZone supplierZone;
                if (existingZonesDictionary.TryGetValue(zone.Key, out supplierZone))
                {
                        addZone.Status = TOne.WhS.SupplierPriceList.Entities.Status.NotChanged;
                        addZone.Name = supplierZone.Name;
                        addZone.SupplierZoneId = supplierZone.SupplierZoneId;
                        addZone.BeginEffectiveDate = supplierZone.BED;
                        addZone.EndEffectiveDate = supplierZone.EED;
                }
                else
                {
                    addZone.Status = TOne.WhS.SupplierPriceList.Entities.Status.New;
                    addZone.Name = zone.Key;
                    addZone.BeginEffectiveDate = zone.Value.BED;
                    addZone.EndEffectiveDate = zone.Value.EED;
                }
                zones.Add(addZone);

            }
            long lastTakenId = manager.ReserveIDRange(zones.Where(x => x.Status == TOne.WhS.SupplierPriceList.Entities.Status.New).Count());

            foreach (Zone zone in zones)
            {
                if (zone.Status == TOne.WhS.SupplierPriceList.Entities.Status.New)
                    zone.SupplierZoneId = lastTakenId++;
            }
            foreach (var zone in existingZonesDictionary)
            {
                PriceListZoneItem priceListZoneItem=null;
                if (priceListByZone.TryGetValue(zone.Key, out priceListZoneItem))
                {
                    zones.Add(new Zone
                    {
                        SupplierId = supplierId,
                        Status = TOne.WhS.SupplierPriceList.Entities.Status.Updated,
                        Name = zone.Key,
                        SupplierZoneId=zone.Value.SupplierZoneId,
                        BeginEffectiveDate = zone.Value.BED,
                        EndEffectiveDate = zone.Value.EED,
                        NewRate = priceListZoneItem.Rate,
                        NewCodes = priceListZoneItem.Codes
                    });
                }
            }
            Zones.Set(context, zones);
            TimeSpan spent = DateTime.Now.Subtract(startPreparing);
            context.WriteTrackingMessage(LogEntryType.Information, "ProcessZones done and takes:{0}", spent);
        }
    }
}
