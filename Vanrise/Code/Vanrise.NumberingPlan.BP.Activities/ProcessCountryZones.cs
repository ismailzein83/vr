using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Common;
using Vanrise.BusinessProcess;
using Vanrise.Entities;
using Vanrise.NumberingPlan.Entities;
using Vanrise.NumberingPlan.Business;

namespace Vanrise.NumberingPlan.BP.Activities
{

    public class ProcessCountryZonesInput
    {
        public IEnumerable<ExistingZone> ExistingZones { get; set; }

        public ZonesByName NewAndExistingZones { get; set; }

        public IEnumerable<ZoneToProcess> ZonesToProcess { get; set; }

        public IEnumerable<ExistingCode> ExistingCodes { get; set; }

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
        public InArgument<IEnumerable<ZoneToProcess>> ZonesToProcess { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<ExistingCode>> ExistingCodes { get; set; }

        [RequiredArgument]
        public InArgument<Dictionary<string,List<ExistingZone>>> ClosedExistingZones { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<NotImportedZone>> NotImportedZones { get; set; }

        protected override ProcessCountryZonesOutput DoWorkWithResult(ProcessCountryZonesInput inputArgument, AsyncActivityHandle handle)
        {
            HashSet<string> zonesToProcessNamesHashSet = new HashSet<string>(inputArgument.ZonesToProcess.Select(item => item.ZoneName), StringComparer.InvariantCultureIgnoreCase);

            UpdateImportedZonesInfo(inputArgument.ZonesToProcess, inputArgument.NewAndExistingZones, inputArgument.ExistingZones, zonesToProcessNamesHashSet, inputArgument.ClosedExistingZones);

            IEnumerable<NotImportedZone> notImportedZones = PrepareNotImportedZones(inputArgument.ExistingZones, zonesToProcessNamesHashSet);

            return new ProcessCountryZonesOutput()
            {
                NotImportedZones = notImportedZones
            };
        }

        protected override ProcessCountryZonesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new ProcessCountryZonesInput()
            {
                ClosedExistingZones = this.ClosedExistingZones.Get(context),
                ExistingCodes = this.ExistingCodes.Get(context),
                ExistingZones = this.ExistingZones.Get(context),
                NewAndExistingZones = this.NewAndExistingZones.Get(context),
                ZonesToProcess = this.ZonesToProcess.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, ProcessCountryZonesOutput result)
        {
            this.NotImportedZones.Set(context, result.NotImportedZones);
        }

        #region Private Methods

        private void UpdateImportedZonesInfo(IEnumerable<ZoneToProcess> zonesToProcess, ZonesByName newAndExistingZones, IEnumerable<ExistingZone> existingZones,
            HashSet<string> zonesToProcessNamesHashSet, Dictionary<string, List<ExistingZone>> closedExistingZones)
        {
            foreach (ZoneToProcess zoneToProcess in zonesToProcess)
            {
                List<IZone> matchedZones;
                newAndExistingZones.TryGetValue(zoneToProcess.ZoneName, out matchedZones);

                if (matchedZones != null)
                    zoneToProcess.AddedZones.AddRange(matchedZones.Where(itm => itm is AddedZone).Select(item => item as AddedZone));

                if (existingZones != null)
                    zoneToProcess.ExistingZones.AddRange(existingZones.FindAllRecords(item => item.Name.Equals(zoneToProcess.ZoneName, StringComparison.InvariantCultureIgnoreCase)));

                zoneToProcess.ChangeType = GetZoneChangeType(zoneToProcess, closedExistingZones);

                zoneToProcess.BED = GetZoneBED(zoneToProcess);
                zoneToProcess.EED = GetZoneEED(zoneToProcess);
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
            IEnumerable<ExistingZone> connectedEntities = this.GetConnectedExistingZones(existingZones);

            NotImportedZone notImportedZone = new NotImportedZone();
            ExistingZone firstElementInTheList = connectedEntities.First();
            ExistingZone lastElementInTheList = connectedEntities.Last();

            notImportedZone.ZoneName = firstElementInTheList.Name;
            //TODO: get it from foreach activity in the process
            notImportedZone.CountryId = firstElementInTheList.CountryId;
            notImportedZone.BED = firstElementInTheList.BED;
            notImportedZone.EED = lastElementInTheList.EED;
            notImportedZone.HasChanged = connectedEntities.Any(x => x.ChangedZone != null);

            return notImportedZone;
        }

        private DateTime GetZoneBED(ZoneToProcess zoneToProcess)
        {
            if (zoneToProcess.ChangeType == ZoneChangeType.New)
                return zoneToProcess.AddedZones.Min(item => item.BED);

            IEnumerable<ExistingZone> connectedExistingZones = this.GetConnectedExistingZones(zoneToProcess.ExistingZones, zoneToProcess.ZoneName);
            return connectedExistingZones.First().BED;
        }

        private DateTime? GetZoneEED(ZoneToProcess zoneToProcess)
        {
            if (zoneToProcess.ChangeType == ZoneChangeType.NotChanged || zoneToProcess.ChangeType == ZoneChangeType.PendingClosed || zoneToProcess.ChangeType == ZoneChangeType.Deleted)
            {
                IEnumerable<ExistingZone> connectedEntites = this.GetConnectedExistingZones(zoneToProcess.ExistingZones, zoneToProcess.ZoneName);
                return connectedEntites.Last().EED;
            }

            return null;
        }

        private ZoneChangeType GetZoneChangeType(ZoneToProcess zoneToProcess, Dictionary<string, List<ExistingZone>> closedExistingZones)
        {
            /*if (zoneToProcess.CodesToMove.Count() > 0 && zoneToProcess.ExistingZones.Count() == 0)
            {
                //Check if all codes are coming from the same original zone; otherwise we cannot consider it as renamed
                string originalZoneName = zoneToProcess.CodesToMove.First().OldZoneName;
                if (zoneToProcess.CodesToMove.All(itm => itm.OldZoneName == originalZoneName))
                {
                    //Check if the original zone has been close, otherwise the original still exists change type of this zone cannot be considered as renamed
                    List<ExistingZone> matchedRenamedExistingZones;
                    if (closedExistingZones != null && closedExistingZones.TryGetValue(originalZoneName, out matchedRenamedExistingZones))
                    {
                        //The last check is on the count of zones between original and new zone, in case they are not equal we cannot consider this zone as renamed
                        HashSet<string> codesFromOriginalZone = NumberingPlanHelper.GetExistingCodes(matchedRenamedExistingZones);
                        if(zoneToProcess.CodesToMove.Count == codesFromOriginalZone.Count)
                        {
                            zoneToProcess.RecentZoneName = originalZoneName;
                            return ZoneChangeType.Renamed;
                        }
                    }
                }
            }*/

            List<ExistingZone> matchedExistingZones;
            if (closedExistingZones != null && closedExistingZones.TryGetValue(zoneToProcess.ZoneName, out matchedExistingZones))
                 return ZoneChangeType.Deleted; 

            if (zoneToProcess.AddedZones.Count() > 0 && zoneToProcess.ExistingZones.Count() == 0)
                return ZoneChangeType.New;

            return ZoneChangeType.NotChanged;
        }

        private IEnumerable<ExistingZone> GetConnectedExistingZones(List<ExistingZone> existingZones)
        {
            return this.GetConnectedExistingZones(existingZones, null);
        }

        private IEnumerable<ExistingZone> GetConnectedExistingZones(List<ExistingZone> existingZones, string zoneName)
        {
            IEnumerable<ExistingZone> connectedEntites = existingZones.GetConnectedEntities(DateTime.Now);
            if (connectedEntites == null)
            {
                string message = "Not Imported Zone has missing existing zones";
                if (zoneName != null)
                    message = string.Format("Zone {0} is missing existing zones", zoneName);

                throw new DataIntegrityValidationException(message);
            }

            return connectedEntites;
        }

        #endregion
    }
}
