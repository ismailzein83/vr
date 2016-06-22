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

            HashSet<string> importedZoneNamesHashSet = new HashSet<string>(importedZones.Select(item => item.ZoneName), StringComparer.InvariantCultureIgnoreCase);

            UpdateImportedZonesInfo(importedZones, newAndExistingZones, existingZones, importedZoneNamesHashSet);

            IEnumerable<ExistingZone> notImportedZones = PrepareNotImportedZones(existingZones, importedZoneNamesHashSet);
            NotImportedZones.Set(context, notImportedZones);
        }


        private void UpdateImportedZonesInfo(IEnumerable<ImportedZone> importedZones, ZonesByName newAndExistingZones, IEnumerable<ExistingZone> existingZones, HashSet<string> importedZoneNamesHashSet)
        {
            foreach (ImportedZone importedZone in importedZones)
            {
                List<IZone> matchedZones;
                newAndExistingZones.TryGetValue(importedZone.ZoneName, out matchedZones);

                if(matchedZones != null)
                    importedZone.NewZones.AddRange(matchedZones.Where(itm => itm is NewZone).Select(itm => itm as NewZone).ToList());

                IEnumerable<ExistingZone> matchedExistingZones = existingZones.FindAllRecords(item => item.ZoneEntity.Name.Equals(importedZone.ZoneName, StringComparison.InvariantCultureIgnoreCase));
                importedZone.ExistingZones.AddRange(matchedExistingZones);
                
                importedZone.ChangeType = GetZoneChangeType(importedZone, existingZones, importedZoneNamesHashSet);
                importedZone.BED = GetZoneBED(importedZone);
                importedZone.EED = (importedZone.ChangeType == ZoneChangeType.NotChanged) ? importedZone.ExistingZones.Select(x => x.EED).VRMinimumDate() : null;
            }
        }

        private IEnumerable<ExistingZone> PrepareNotImportedZones(IEnumerable<ExistingZone> existingZones, HashSet<string> importedZoneNamesHashSet)
        {
            List<ExistingZone> notImportedZones = new List<ExistingZone>();

            foreach (ExistingZone existingZone in existingZones)
            {
                string zoneName = existingZone.ZoneEntity.Name;
                if (existingZone.ChangedZone != null && !importedZoneNamesHashSet.Contains(zoneName, StringComparer.InvariantCultureIgnoreCase))
                    notImportedZones.Add(existingZone);
            }

            return notImportedZones;
        }


        private DateTime GetZoneBED(ImportedZone importedZone)
        {
            List<DateTime?> minDates = new List<DateTime?>();
            
            if (importedZone.NewZones.Count() > 0)
                minDates.Add(importedZone.NewZones.Min(item => item.BED));

            if (importedZone.ExistingZones.Count() > 0)
                minDates.Add(importedZone.ExistingZones.Min(item => item.BED));

            return minDates.VRMinimumDate().Value;          
        }

        private ZoneChangeType GetZoneChangeType(ImportedZone importedZone, IEnumerable<ExistingZone> existingZones, HashSet<string> importedZoneNamesHashSet)
        {
            if(importedZone.ExistingZones.Count == 0 && importedZone.ImportedCodes.Any(itm => itm.ChangeType == CodeChangeType.Moved))
            {
                //This zone can be a considered as renamed but with specific conditions, first let us take the zone name of the first moved code as original zone name
                IEnumerable<ImportedCode> allMovedCodes = importedZone.ImportedCodes.Where(itm => itm.ChangeType == CodeChangeType.Moved);
                string originalZoneName = allMovedCodes.First().ProcessInfo.RecentZoneName;
                
                //if all moved codes are coming from the same origine & this original zone name does not exist anymore in imported zones list
                //then consider this as a renamed zone, otherwise the zone is considered as a new zone
                if (allMovedCodes.All(itm => itm.ProcessInfo.RecentZoneName == originalZoneName) && !importedZoneNamesHashSet.Contains(originalZoneName, StringComparer.InvariantCultureIgnoreCase))
                {
                    importedZone.RecentZoneName = originalZoneName;
                    return ZoneChangeType.Renamed;
                }
            }

            if (importedZone.NewZones.Count() > 0 && importedZone.ExistingZones.Count == 0)
                return ZoneChangeType.New;

            if (importedZone.NewZones.Count() > 0 && importedZone.ExistingZones.Count > 0)
                return ZoneChangeType.ReOpened;

            return ZoneChangeType.NotChanged;
        }
    }
}
