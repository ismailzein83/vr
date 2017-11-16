using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.Activities.Expressions;
using Vanrise.Common;
using TOne.WhS.CodePreparation.Entities.Processing;
using TOne.WhS.CodePreparation.Entities;
using Vanrise.BusinessProcess;
using TOne.WhS.BusinessEntity.Business;
using Vanrise.Entities;
using TOne.WhS.CodePreparation.Business;

namespace TOne.WhS.CodePreparation.BP.Activities
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
        public InArgument<Dictionary<string, List<ExistingZone>>> ClosedExistingZones { get; set; }

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
            //CheckForRenamedOrSplitZones(zonesToProcess.Where(z => z.ChangeType == ZoneChangeType.New), closedExistingZones);
            DefineSplitOrMergedZones(zonesToProcess.Where(z => z.ChangeType == ZoneChangeType.New), closedExistingZones);
        }

        private void DefineSplitOrMergedZones(IEnumerable<ZoneToProcess> newZones, Dictionary<string, List<ExistingZone>> closedExistingZones)
        {
            foreach (var newZone in newZones)
            {
                if (!newZone.CodesToMove.Any() || newZone.ExistingZones.Any() || newZone.CodesToAdd.Any())
                    continue;

                Dictionary<string, List<string>> codesToMoveByZoneName = GroupCodesByZoneName(newZone.CodesToMove);

                //split zone will be moved from only one origin zone
                if (codesToMoveByZoneName.Count == 1)
                    CheckSplitZone(newZone);
                else //check merge zone and get source zone names
                    newZone.SourceZoneNames = GetSourceZoneNames(codesToMoveByZoneName, closedExistingZones);
            }
        }

        private Dictionary<string, List<string>> GroupCodesByZoneName(List<CodeToMove> codesToMove)
        {
            // Group moved code by original zone name
            var codeToMovebyZoneName = new Dictionary<string, List<string>>();
            foreach (var codeToMove in codesToMove)
            {
                List<string> codes;
                if (!codeToMovebyZoneName.TryGetValue(codeToMove.OldZoneName, out codes))
                {
                    codes = new List<string>();
                    codeToMovebyZoneName.Add(codeToMove.OldZoneName, codes);
                }
                codes.Add(codeToMove.Code);
            }
            return codeToMovebyZoneName;
        }

        private void CheckSplitZone(ZoneToProcess zone)
        {
            string originalZoneName = zone.CodesToMove.First().OldZoneName;
            if (zone.CodesToMove.All(itm => itm.OldZoneName.Equals(originalZoneName, StringComparison.InvariantCultureIgnoreCase)))
                zone.SplitFromZoneName = originalZoneName;
        }

        private IEnumerable<string> GetSourceZoneNames(Dictionary<string, List<string>> codesToMoveByZoneName, Dictionary<string, List<ExistingZone>> closedExistingZones)
        {
            // all source zones must be closed. all codes from source zone must be moved to the new merged one.

            if (closedExistingZones == null) return null; // merged zone will result in closing all source zones, so if no closed zones return.

            foreach (var zone in codesToMoveByZoneName)
            {
                List<ExistingZone> existingZones;
                if (!closedExistingZones.TryGetValue(zone.Key, out existingZones)) // if at least one zone from origin zones is openned, the zone is not considered as merged.
                    return null;

                // Since the dictionary is groupped by origin zone name with their moved codes
                // We will check the existing codes if all codes were moved from origin to merged.
                HashSet<string> codesFromOriginalZone = NumberingPlanHelper.GetExistingCodes(existingZones);
                if (zone.Value.Count != codesFromOriginalZone.Count)
                    return null;
            }
            return codesToMoveByZoneName.Keys;
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
