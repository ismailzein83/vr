using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.CodePreparation.Business;
using TOne.WhS.CodePreparation.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Common;
namespace TOne.WhS.CodePreparation.BP.Activities
{
    public class GenerateSaleZones : CodeActivity
    {
        public InArgument<DateTime> EffectiveDate { get; set; }
        public InArgument<int> SellingNumberPlanId { get; set; }

        public InArgument<Dictionary<string, Zone>> ImportedZonesWithCodes { get; set; }
        public OutArgument<Dictionary<string, Zone>> AffectedZonesWithCodes { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            DateTime startReading = DateTime.Now;
            DateTime effectiveDate=EffectiveDate.Get(context); 
            int sellingNumberPlanId = SellingNumberPlanId.Get(context);

            CodePreparationManager codePreparationManager = new CodePreparationManager();
            SaleZoneManager saleZoneManager = new SaleZoneManager();


            Dictionary<string, Zone> saleZonesWithCodes = codePreparationManager.GetSaleZonesWithCodes(sellingNumberPlanId, effectiveDate);

            List<SaleZone> saleZones = new List<SaleZone>();

            saleZones = saleZoneManager.GetSaleZones(sellingNumberPlanId, effectiveDate).ToList();
            Dictionary<string, Zone> allZonesDictionary = new Dictionary<string, Zone>();


            Dictionary<string, Zone> affectedZones = GetSaleZonesList(sellingNumberPlanId, effectiveDate, ImportedZonesWithCodes.Get(context), saleZonesWithCodes, saleZones);
            long lastTakenId = saleZoneManager.ReserveIDRange(affectedZones.Count());

            foreach (var zone in affectedZones)
            {
                zone.Value.SaleZoneId = lastTakenId++;
            }

            AffectedZonesWithCodes.Set(context, affectedZones);
            TimeSpan spent = DateTime.Now.Subtract(startReading);

            context.WriteTrackingMessage(LogEntryType.Information, "Generating SaleZones done and Takes: {0}", spent);
        }
        private Dictionary<string, Zone> GetSaleZonesList(int sellingNumberPlanId, DateTime effectiveDate, Dictionary<string, Zone> importedZonesWithCodes, Dictionary<string, Zone> saleZonesWithCodes, List<SaleZone> saleZones)
        {
            Dictionary<string, Zone> importedList = new Dictionary<string, Zone>();
            foreach (var obj in importedZonesWithCodes)
            {
                switch (obj.Value.Status)
                {
                    case Status.New:
                        if (!importedList.ContainsKey(obj.Key) && !saleZonesWithCodes.ContainsKey(obj.Key))
                        {
                            obj.Value.SellingNumberPlanId = sellingNumberPlanId;
                            importedList.Add(obj.Key,new Zone
                            {
                                SaleZoneId = obj.Value.SaleZoneId,
                                SellingNumberPlanId = sellingNumberPlanId,
                                Status = Status.New,
                                BeginEffectiveDate = obj.Value.BeginEffectiveDate,
                                EndEffectiveDate = obj.Value.EndEffectiveDate,
                                Name = obj.Value.Name,
                            } );
                        }
                        break;
                    case Status.Changed:
                        Zone saleCodesList = null;
                        if (saleZonesWithCodes.TryGetValue(obj.Key, out saleCodesList))
                        {
                            SaleZone saleZone = saleZones.Find(x => x.Name == obj.Key);
                            saleCodesList.EndEffectiveDate = effectiveDate;
                            importedList.Add(obj.Key, new Zone
                            {
                                SaleZoneId= saleZone.SaleZoneId,
                                SellingNumberPlanId = sellingNumberPlanId,
                                Status = Status.Changed,
                                BeginEffectiveDate=saleCodesList.BeginEffectiveDate,
                                EndEffectiveDate = effectiveDate,
                                Name = saleCodesList.Name,
                            });
                        }
                        break;
                }


            }
            return importedList;
        }
    }
}
