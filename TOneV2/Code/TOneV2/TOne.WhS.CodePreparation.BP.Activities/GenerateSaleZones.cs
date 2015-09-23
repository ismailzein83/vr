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
        public InArgument<int> SaleZonePackageId { get; set; }
        public InArgument<Dictionary<string, List<SaleCode>>> NewZonesOrCodes { get; set; }
        public InArgument<Dictionary<string, List<SaleCode>>> DeletedZonesOrCodes { get; set; }

        public OutArgument<List<SaleZone>> ZonesToAdd { get; set; }
        public OutArgument<List<SaleZone>> ZonesToDelete { get; set; }
        public OutArgument<List<SaleZone>> SaleZones { get; set; }
        public OutArgument<Dictionary<string, List<SaleCode>>> ZonesToAddDictionary { get; set; }
        public OutArgument<Dictionary<string, List<SaleCode>>> ZonesToDeleteDictionary { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            DateTime startReading = DateTime.Now;
            DateTime effectiveDate=EffectiveDate.Get(context);
            int saleZonePackageId = SaleZonePackageId.Get(context);

            SaleZoneManager saleZoneManager = new SaleZoneManager();
            Dictionary<string, List<SaleCode>> saleZonesWithCodes = saleZoneManager.GetSaleZonesWithCodes(saleZonePackageId, effectiveDate);
            List<SaleZone> saleZones = new List<SaleZone>();

            saleZones = saleZoneManager.GetSaleZones(saleZonePackageId, effectiveDate);
            Dictionary<string, List<SaleCode>> zonesToAddDictionary = new Dictionary<string, List<SaleCode>>();
            List<SaleZone> zonesToAdd = GetSaleZonesList(saleZonePackageId, effectiveDate, NewZonesOrCodes.Get(context), zonesToAddDictionary, ImportType.New, saleZonesWithCodes, saleZones);
            Dictionary<string, List<SaleCode>> zonesToDeleteDictionary = new Dictionary<string, List<SaleCode>>();
            List<SaleZone> zonesToDelete = GetSaleZonesList(saleZonePackageId, effectiveDate, DeletedZonesOrCodes.Get(context), zonesToDeleteDictionary, ImportType.Delete, saleZonesWithCodes, saleZones);
            ZonesToAdd.Set(context, zonesToAdd);
            ZonesToDelete.Set(context, zonesToDelete);
            ZonesToAddDictionary.Set(context,zonesToAddDictionary);
            ZonesToDeleteDictionary.Set(context, zonesToDeleteDictionary);
            TimeSpan spent = DateTime.Now.Subtract(startReading);
            saleZones = new List<SaleZone>();
            foreach (SaleZone salezone in zonesToDelete)
            {
                saleZones.Add(salezone);
            }
            SaleZones.Set(context, saleZones);
            context.WriteTrackingMessage(LogEntryType.Information, "Generating SaleZones done and Takes: {0}", spent);
        }
        private List<SaleZone> GetSaleZonesList(int saleZonePackageId, DateTime effectiveDate, Dictionary<string, List<SaleCode>> zoneByCodesDictionary, Dictionary<string,
            List<SaleCode>> importedList, ImportType type, Dictionary<string, List<SaleCode>> saleZonesWithCodes, List<SaleZone> saleZones)
        {
            List<SaleZone> saleZonesList = new List<SaleZone>();
            foreach (var obj in zoneByCodesDictionary)
            {
                switch (type)
                {
                    case ImportType.New:
                        if (!importedList.ContainsKey(obj.Key) && !saleZonesWithCodes.ContainsKey(obj.Key))
                        {
                            saleZonesList.Add(new SaleZone
                            {
                                Name = obj.Key,
                                SaleZonePackageId = saleZonePackageId,
                                BeginEffectiveDate = effectiveDate,
                                EndEffectiveDate = null
                            });
                            importedList.Add(obj.Key, obj.Value);
                        }
                        break;
                    case ImportType.Delete:
                        List<SaleCode> saleCodesList = null;
                        if (saleZonesWithCodes.TryGetValue(obj.Key, out saleCodesList))
                        {
                            SaleZone saleZone = saleZones.Find(x => x.Name == obj.Key);
                            saleZonesList.Add(new SaleZone
                            {
                                SaleZoneId = saleZone.SaleZoneId,
                                Name = obj.Key,
                                SaleZonePackageId = saleZonePackageId,
                                EndEffectiveDate = effectiveDate
                            });
                            foreach (SaleCode deletedCode in saleCodesList)
                            {
                                deletedCode.EndEffectiveDate = effectiveDate;
                            }
                            importedList.Add(obj.Key, saleCodesList);
                        }
                        break;
                }


            }
            return saleZonesList;
        }
    }
}
