﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Common;
using Vanrise.BusinessProcess;
using TOne.WhS.SupplierPriceList.BP.Activities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.BusinessEntity.Business;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{

    public class ProcessCountryZonesInput
    {
        public IEnumerable<ExistingZone> ExistingZones { get; set; }

        public ZonesByName NewAndExistingZones { get; set; }

        public IEnumerable<ImportedZone> ImportedZones { get; set; }

        public Dictionary<string, List<ExistingZone>> ClosedExistingZones { get; set; }

    }
    public class ProcessCountryZonesOutput
    {
        public IEnumerable<NotImportedZone> NotImportedZones { get; set; }
    }

    public sealed class ProcessCountryZones : BaseAsyncActivity<ProcessCountryZonesInput, ProcessCountryZonesOutput>
    {
        [RequiredArgument]
        public InArgument<IEnumerable<ExistingZone>> ExistingZones { get; set; }

        [RequiredArgument]
        public InArgument<ZonesByName> NewAndExistingZones { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<ImportedZone>> ImportedZones { get; set; }


        [RequiredArgument]
        public OutArgument<IEnumerable<NotImportedZone>> NotImportedZones { get; set; }

        protected override ProcessCountryZonesOutput DoWorkWithResult(ProcessCountryZonesInput inputArgument, AsyncActivityHandle handle)
        {

            HashSet<string> importedZoneNamesHashSet = new HashSet<string>(inputArgument.ImportedZones.Select(item => item.ZoneName), StringComparer.InvariantCultureIgnoreCase);

            UpdateImportedZonesInfo(inputArgument.ImportedZones, inputArgument.NewAndExistingZones, inputArgument.ExistingZones, importedZoneNamesHashSet);

            IEnumerable<NotImportedZone> notImportedZones = PrepareNotImportedZones(inputArgument.ExistingZones, importedZoneNamesHashSet);

            return new ProcessCountryZonesOutput()
            {
                NotImportedZones = notImportedZones
            };
        }

        protected override ProcessCountryZonesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new ProcessCountryZonesInput()
            {
                ExistingZones = this.ExistingZones.Get(context),
                NewAndExistingZones = this.NewAndExistingZones.Get(context),
                ImportedZones = this.ImportedZones.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, ProcessCountryZonesOutput result)
        {
            this.NotImportedZones.Set(context, result.NotImportedZones);
        }

        #region Private Methods

        private void UpdateImportedZonesInfo(IEnumerable<ImportedZone> importedZones, ZonesByName newAndExistingZones, IEnumerable<ExistingZone> existingZones, HashSet<string> importedZoneNamesHashSet)
        {
           
            foreach (ImportedZone importedZone in importedZones)
            {
                List<IZone> matchedZones;
                newAndExistingZones.TryGetValue(importedZone.ZoneName, out matchedZones);

                if (matchedZones != null)
                    importedZone.NewZones.AddRange(matchedZones.Where(itm => itm is NewZone).Select(itm => itm as NewZone).ToList());

                IEnumerable<ExistingZone> matchedExistingZones = existingZones.FindAllRecords(item => item.ZoneEntity.Name.Equals(importedZone.ZoneName, StringComparison.InvariantCultureIgnoreCase));
                importedZone.ExistingZones.AddRange(matchedExistingZones);

                importedZone.ChangeType = GetZoneChangeType(importedZone, existingZones, importedZoneNamesHashSet);
                importedZone.BED = GetZoneBED(importedZone);
                importedZone.EED = (importedZone.ChangeType == ZoneChangeType.NotChanged) ? importedZone.ExistingZones.Select(x => x.EED).VRMaximumDate() : null;
            }
        }

        private IEnumerable<NotImportedZone> PrepareNotImportedZones(IEnumerable<ExistingZone> existingZones, HashSet<string> importedZoneNamesHashSet)
        {
            Dictionary<string, List<ExistingZone>> notImportedZonesByZoneName = new Dictionary<string, List<ExistingZone>>();

            foreach (ExistingZone existingZone in existingZones)
            {
                string zoneName = existingZone.ZoneEntity.Name;
                if (!importedZoneNamesHashSet.Contains(zoneName, StringComparer.InvariantCultureIgnoreCase))
                {
                    List<ExistingZone> existingZonesList = null;
                    if (!notImportedZonesByZoneName.TryGetValue(zoneName, out existingZonesList))
                    {
                        existingZonesList = new List<ExistingZone>();
                        notImportedZonesByZoneName.Add(zoneName, existingZonesList);
                    }
                    existingZonesList.Add(existingZone);
                }
            }

            return notImportedZonesByZoneName.MapRecords(NotImportedZoneInfoMapper);
        }

        private NotImportedZone NotImportedZoneInfoMapper(List<ExistingZone> existingZones)
        {
            List<ExistingZone> linkedExistingZones = existingZones.GetConnectedEntities(DateTime.Today);

            NotImportedZone notImportedZone = new NotImportedZone();
            ExistingZone firstElementInTheList = linkedExistingZones.First();
            ExistingZone lastElementInTheList = linkedExistingZones.Last();

            List<ExistingRate> existingRates = GetExistingRatesByLinkedExistingZones(linkedExistingZones);

            notImportedZone.ZoneName = firstElementInTheList.Name;
            //TODO: get it from foreach activity in the process
            notImportedZone.CountryId = firstElementInTheList.CountryId;
            notImportedZone.BED = firstElementInTheList.BED;
            notImportedZone.EED = lastElementInTheList.EED;
            notImportedZone.HasChanged = linkedExistingZones.Any(x => x.ChangedZone != null);

            return notImportedZone;
        }

        private List<ExistingRate> GetExistingRatesByLinkedExistingZones(List<ExistingZone> linkedExistingZones)
        {
            List<ExistingRate> existingRates = new List<ExistingRate>();

            existingRates.AddRange(linkedExistingZones.SelectMany(item => item.ExistingRates).OrderBy(itm => itm.BED));

            return existingRates;
        }

        private DateTime GetZoneBED(ImportedZone importedZone)
        {
            if (importedZone.NewZones.Count() > 0)
                return importedZone.NewZones.Min(item => item.BED);

            List<ExistingZone> connectedExistingZones = importedZone.ExistingZones.GetConnectedEntities(DateTime.Today);
            return connectedExistingZones.First().BED;
        }

        private ZoneChangeType GetZoneChangeType(ImportedZone importedZone, IEnumerable<ExistingZone> existingZones, HashSet<string> importedZoneNamesHashSet)
        {
            if (importedZone.ExistingZones.Count == 0 && importedZone.ImportedCodes.Any(itm => itm.ChangeType == CodeChangeType.Moved))
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

        #endregion
    }
}
