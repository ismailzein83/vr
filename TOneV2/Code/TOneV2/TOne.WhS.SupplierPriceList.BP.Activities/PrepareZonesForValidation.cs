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
        public InArgument<IEnumerable<ImportedZone>> ImportedZones { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<ExistingZone>> NotImportedZones { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<ImportedZone> importedZones = ImportedZones.Get(context);
            ZonesByName newAndExistingZones = NewAndExistingZones.Get(context);
            IEnumerable<ExistingZone> existingZones = ExistingZones.Get(context);

            HashSet<string> importedZoneNamesHashSet = ToHashSet(importedZones.Select(item => item.ZoneName));
            IEnumerable<ExistingZone> notImportedZones = PrepareNotImportedZones(existingZones, importedZoneNamesHashSet);
            
            UpdateImportedZonesInfo(importedZones, newAndExistingZones, existingZones);
            
            NotImportedZones.Set(context, notImportedZones);
        }


        private void UpdateImportedZonesInfo(IEnumerable<ImportedZone> importedZones, ZonesByName newAndExistingZones, IEnumerable<ExistingZone> existingZones)
        {
            foreach (ImportedZone importedZone in importedZones)
            {
                List<IZone> newZones;
                newAndExistingZones.TryGetValue(importedZone.ZoneName, out newZones);

                importedZone.NewZones =newZones != null ? newZones.Select(itm => itm as NewZone).ToList() : new List<NewZone>();
                importedZone.ExistingZones = existingZones.FindAllRecords(item => item.ZoneEntity.Name.Equals(importedZone.ZoneName, StringComparison.InvariantCultureIgnoreCase)).ToList();
                importedZone.BED = importedZone.ImportedCodes.Min(item => item.BED);
                FillZoneChangeTypeAndRecentZoneName(importedZone, existingZones);
            }
        }

        private IEnumerable<ExistingZone> PrepareNotImportedZones(IEnumerable<ExistingZone> existingZones, HashSet<string> importedZoneNames)
        {
            List<ExistingZone> notImportedZones = new List<ExistingZone>();

            foreach (ExistingZone existingZone in existingZones)
            {
                if (existingZone.ChangedZone != null && !importedZoneNames.Contains(existingZone.ZoneEntity.Name))
                    notImportedZones.Add(existingZone);
            }

            return notImportedZones;
        }


        private void FillZoneChangeTypeAndRecentZoneName(ImportedZone importedZone, IEnumerable<ExistingZone> existingZones)
        {

            if (importedZone.ImportedCodes.All(item => item.ChangeType == CodeChangeType.Moved))
            {
                string recentZoneName = importedZone.ImportedCodes.First().ProcessInfo.RecentZoneName;
                ExistingZone existingClosedZone = existingZones.FindRecord(item => item.Name.Equals(recentZoneName, StringComparison.InvariantCultureIgnoreCase));
                
                //check if all importedCodes are moved from same zone and if all codes in old zone are closed and there's no newCodes and importedZone must be newZone
                if(importedZone.ImportedCodes.All(item => item.ProcessInfo.RecentZoneName.Equals(recentZoneName, StringComparison.InvariantCultureIgnoreCase))
                    && existingClosedZone.ExistingCodes.All(item => item.EED.HasValue) && existingClosedZone.NewCodes.Count() == 0
                    && existingZones.FindRecord(item => item.ZoneEntity.Name.Equals(importedZone.ZoneName, StringComparison.InvariantCultureIgnoreCase)) == null)
                {
                        importedZone.ChangeType = ZoneChangeType.Renamed;
                        importedZone.RecentZoneName = recentZoneName;
                }

                else if (importedZone.NewZones != null && importedZone.NewZones.Count() > 0 && importedZone.ExistingZones.All(item => item.ExistingCodes.All(itm => itm.EED.HasValue) && item.ExistingCodes.Any(itm => itm.ChangedCode == null)))
                    importedZone.ChangeType = ZoneChangeType.New;
            }

            else if (importedZone.NewZones != null && importedZone.NewZones.Count() > 0 && importedZone.ExistingZones.All(item => item.ExistingCodes.All(itm => itm.EED.HasValue) && item.ExistingCodes.Any(itm=> itm.ChangedCode == null)))
                importedZone.ChangeType= ZoneChangeType.New;

            else if (importedZone.ExistingZones != null && importedZone.ExistingZones.Any(item => item.ChangedZone != null))
                importedZone.ChangeType = ZoneChangeType.Closed;

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
