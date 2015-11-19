using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.CodePreparation.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Common;
namespace TOne.WhS.CodePreparation.BP.Activities
{
    public class GenerateSaleZones : CodeActivity
    {
        public InArgument<DateTime> EffectiveDate { get; set; }
        public InArgument<int> SellingNumberPlanId { get; set; }

        public InArgument<Dictionary<string, SaleZone>> NewZonesOrCodes { get; set; }
        public InArgument<Dictionary<string, SaleZone>> DeletedZonesOrCodes { get; set; }
        public OutArgument<Dictionary<string, SaleZone>> ZonesToAddDictionary { get; set; }
        public OutArgument<Dictionary<string, SaleZone>> ZonesToDeleteDictionary { get; set; }

        public OutArgument<Dictionary<string, SaleZone>> AllZonesDictionary { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            DateTime startReading = DateTime.Now;
            DateTime effectiveDate=EffectiveDate.Get(context);
            int sellingNumberPlanId = SellingNumberPlanId.Get(context);

            SaleZoneManager saleZoneManager = new SaleZoneManager();

          

            Dictionary<string, SaleZone> saleZonesWithCodes = saleZoneManager.GetSaleZonesWithCodes(sellingNumberPlanId, effectiveDate);
                        
            List<SaleZone> saleZones = new List<SaleZone>();

            saleZones = saleZoneManager.GetSaleZones(sellingNumberPlanId, effectiveDate).ToList();
            Dictionary<string, SaleZone> allZonesDictionary = new Dictionary<string, SaleZone>();


            Dictionary<string, SaleZone> zonesToAddDictionary = GetSaleZonesList(sellingNumberPlanId, effectiveDate, NewZonesOrCodes.Get(context), ImportType.New, saleZonesWithCodes, saleZones);
            Dictionary<string, SaleZone> zonesToDeleteDictionary = GetSaleZonesList(sellingNumberPlanId, effectiveDate, DeletedZonesOrCodes.Get(context), ImportType.Delete, saleZonesWithCodes, saleZones);
            ZonesToAddDictionary.Set(context,zonesToAddDictionary);
            ZonesToDeleteDictionary.Set(context, zonesToDeleteDictionary);

            long lastTakenId = saleZoneManager.ReserveIDRange(zonesToAddDictionary.Count());

            foreach (var zone in zonesToAddDictionary)
            {
               
                if (!allZonesDictionary.ContainsKey(zone.Key))
                {
                    SaleZone salezone = new SaleZone()
                    {
                        Status=zone.Value.Status,
                        SaleZoneId = lastTakenId++,
                        SellingNumberPlanId=zone.Value.SellingNumberPlanId,
                        BeginEffectiveDate=zone.Value.BeginEffectiveDate,
                        EndEffectiveDate=zone.Value.EndEffectiveDate,
                        Name=zone.Value.Name
                    };

                    allZonesDictionary.Add(zone.Key, salezone);
                }
                    
            }
            foreach (var zone in zonesToDeleteDictionary)
            {
                if (!allZonesDictionary.ContainsKey(zone.Key))
                {
                    SaleZone salezone = new SaleZone()
                    {
                        Status = zone.Value.Status,
                        SaleZoneId = zone.Value.SaleZoneId,
                        SellingNumberPlanId = zone.Value.SellingNumberPlanId,
                        BeginEffectiveDate = zone.Value.BeginEffectiveDate,
                        EndEffectiveDate = zone.Value.EndEffectiveDate,
                        Name = zone.Value.Name
                    };
                    allZonesDictionary.Add(zone.Key, salezone);
                }
                   
            }
            AllZonesDictionary.Set(context, allZonesDictionary);
            TimeSpan spent = DateTime.Now.Subtract(startReading);

            context.WriteTrackingMessage(LogEntryType.Information, "Generating SaleZones done and Takes: {0}", spent);
        }
        private  Dictionary<string, SaleZone> GetSaleZonesList(int sellingNumberPlanId, DateTime effectiveDate, Dictionary<string, SaleZone> zoneByCodesDictionary, ImportType type, Dictionary<string, SaleZone> saleZonesWithCodes, List<SaleZone> saleZones)
        {
            Dictionary<string, SaleZone> importedList= new Dictionary<string,SaleZone>();
            foreach (var obj in zoneByCodesDictionary)
            {
                switch (type)
                {
                    case ImportType.New:
                        if (!importedList.ContainsKey(obj.Key) && !saleZonesWithCodes.ContainsKey(obj.Key))
                        {
                            obj.Value.SellingNumberPlanId = sellingNumberPlanId;
                            importedList.Add(obj.Key, obj.Value);
                        }
                        break;
                    case ImportType.Delete:
                        SaleZone saleCodesList = null;
                        if (saleZonesWithCodes.TryGetValue(obj.Key, out saleCodesList))
                        {
                            SaleZone saleZone = saleZones.Find(x => x.Name == obj.Key);
                            obj.Value.SellingNumberPlanId = sellingNumberPlanId;
                            obj.Value.SaleZoneId = saleZone.SaleZoneId;
                            saleCodesList.Status = Status.Deleted;
                            saleCodesList.EndEffectiveDate = effectiveDate;
                            importedList.Add(obj.Key, saleCodesList);
                        }
                        break;
                }


            }
            return importedList;
        }
    }
}
