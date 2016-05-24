using System.Collections.Generic;
using System.Activities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using System.Linq;
using System.Collections.Generic;
using Vanrise.Common;
using System;
using TOne.WhS.SupplierPriceList.Entities;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{

    public sealed class PrepareZonesForValidation : CodeActivity
    {

        [RequiredArgument]
        public InArgument<IEnumerable<ExistingZone>> ExistingZones { get; set; }

        [RequiredArgument]
        public InArgument<ZonesByName> NewAndExistingZones { get; set; }

        [RequiredArgument]
        public InOutArgument<IEnumerable<ImportedZone>> ImportedZones { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<ExistingZone>> NotImportedZones { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<ImportedZone> importedZones = ImportedZones.Get(context);
            ZonesByName newAndExistingZones = NewAndExistingZones.Get(context);

            HashSet<string> importedZonesHashSet = ToHashSet(importedZones.Select(item => item.ZoneName));

            IEnumerable<ExistingZone> existingZones = ExistingZones.Get(context);
            
            IEnumerable<ExistingZone> notImportedZones = PrepareNotImportedZones(existingZones, importedZonesHashSet);
            
            UpdateImportedZonesInfo(importedZones, newAndExistingZones, existingZones);
            
            ImportedZones.Set(context, importedZones);
            NotImportedZones.Set(context, notImportedZones);
        }


        private void UpdateImportedZonesInfo(IEnumerable<ImportedZone> importedZones, ZonesByName newAndExistingZones, IEnumerable<ExistingZone> existingZones)
        {
            foreach (ImportedZone importedZone in importedZones)
            {
                importedZone.NewZones = newAndExistingZones.SelectMany(itm => itm.Value.Where(izone => izone is NewZone && izone.Name.Equals(importedZone.ZoneName, StringComparison.InvariantCultureIgnoreCase))).Select(itm => itm as NewZone).ToList();
                importedZone.ExistingZones = existingZones.FindAllRecords(item => item.ZoneEntity.Name.Equals(importedZone.ZoneName, StringComparison.InvariantCultureIgnoreCase)).ToList();
                importedZone.BED = importedZone.ImportedCodes.Min(item => item.BED);
                FillZoneChangeTypeAndRecentZoneName(importedZone, existingZones);
            }
        }

        private IEnumerable<ExistingZone> PrepareNotImportedZones(IEnumerable<ExistingZone> existingZones, HashSet<string> importedZones)
        {
            List<ExistingZone> notImportedZones = new List<ExistingZone>();

            foreach (ExistingZone existingZone in existingZones)
            {
                if (existingZone.ChangedZone != null && !importedZones.Contains(existingZone.ZoneEntity.Name))
                    notImportedZones.Add(existingZone);
            }

            return notImportedZones;
        }


        private void FillZoneChangeTypeAndRecentZoneName(ImportedZone importedZone, IEnumerable<ExistingZone> existingZones)
        {

            if (importedZone.NewZones != null && importedZone.NewZones.Count() > 0)
                importedZone.ChangeType= ZoneChangeType.New;

            else if (importedZone.ExistingZones != null && importedZone.ExistingZones.Any(item => item.ChangedZone != null))
                importedZone.ChangeType = ZoneChangeType.Closed;

            else if (importedZone.ImportedCodes != null && importedZone.ImportedCodes.Count() > 0)
            {
                var firstZoneName = importedZone.ImportedCodes.FirstOrDefault().ZoneName;

                ExistingZone existingZone = existingZones.FindRecord(item => item.ZoneEntity.Name.Equals(firstZoneName, StringComparison.InvariantCultureIgnoreCase));

                if (existingZone != null && existingZone.ExistingCodes.Count() == 0 
                    && importedZone.ImportedCodes.All(item => item.ZoneName.Equals(firstZoneName, StringComparison.InvariantCultureIgnoreCase)))
                {
                    importedZone.RecentZoneName = firstZoneName;
                    importedZone.ChangeType = ZoneChangeType.Renamed;
                }

                else
                    importedZone.ChangeType = ZoneChangeType.NotChanged;
            }

            else
                importedZone.ChangeType =  ZoneChangeType.NotChanged;

        }


        private HashSet<T> ToHashSet<T>(IEnumerable<T> list)
        {
            HashSet<T> result = new HashSet<T>();
            result.UnionWith(list);
            return result;
        }
    }
}
