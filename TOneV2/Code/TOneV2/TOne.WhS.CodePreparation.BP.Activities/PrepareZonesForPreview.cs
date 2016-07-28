﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Common;
using TOne.WhS.CodePreparation.Entities.Processing;
using TOne.WhS.CodePreparation.Entities;
using Vanrise.BusinessProcess;

namespace TOne.WhS.CodePreparation.BP.Activities
{

    public class PrepareZoneForPreviewInput
    {
        public IEnumerable<ExistingZone> ExistingZones { get; set; }

        public ZonesByName NewAndExistingZones { get; set; }

        public IEnumerable<ZoneToProcess> ZonesToProcess { get; set; }

        public IEnumerable<ExistingCode> ExistingCodes { get; set; }

        public Dictionary<string, List<ExistingZone>> ClosedExistingZones { get; set; }

    }
    public class PrepareZoneForPreviewOutput
    {
        public IEnumerable<ExistingZone> NotChangedZones { get; set; }
    }

    public sealed class PrepareZonesForPreview : BaseAsyncActivity<PrepareZoneForPreviewInput, PrepareZoneForPreviewOutput>
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
        public OutArgument<IEnumerable<ExistingZone>> NotChangedZones { get; set; }

        protected override PrepareZoneForPreviewOutput DoWorkWithResult(PrepareZoneForPreviewInput inputArgument, AsyncActivityHandle handle)
        {
            Dictionary<string, Dictionary<string, List<ExistingCode>>> existingCodeByZoneName = StructureExistingCodesByZonesNames(inputArgument.ExistingCodes);

            HashSet<string> zonesToProcessNamesHashSet = new HashSet<string>(inputArgument.ZonesToProcess.Select(item => item.ZoneName), StringComparer.InvariantCultureIgnoreCase);

            UpdateImportedZonesInfo(inputArgument.ZonesToProcess, inputArgument.NewAndExistingZones, inputArgument.ExistingZones, zonesToProcessNamesHashSet, existingCodeByZoneName, inputArgument.ClosedExistingZones);

            IEnumerable<ExistingZone> notChangedZones = PrepareNotChangedZones(inputArgument.ExistingZones, zonesToProcessNamesHashSet);

            return new PrepareZoneForPreviewOutput()
            {
                NotChangedZones = notChangedZones
            };
        }

        protected override PrepareZoneForPreviewInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new PrepareZoneForPreviewInput()
            {
                ClosedExistingZones = this.ClosedExistingZones.Get(context),
                ExistingCodes = this.ExistingCodes.Get(context),
                ExistingZones = this.ExistingZones.Get(context),
                NewAndExistingZones = this.NewAndExistingZones.Get(context),
                ZonesToProcess = this.ZonesToProcess.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, PrepareZoneForPreviewOutput result)
        {
            this.NotChangedZones.Set(context, result.NotChangedZones);
        }

        #region Private Methods

        private void UpdateImportedZonesInfo(IEnumerable<ZoneToProcess> zonesToProcess, ZonesByName newAndExistingZones, IEnumerable<ExistingZone> existingZones, HashSet<string> zonesToProcessNamesHashSet, Dictionary<string, Dictionary<string, List<ExistingCode>>> existingCodesByZoneNames, Dictionary<string, List<ExistingZone>> closedExistingZones)
        {
            foreach (ZoneToProcess zoneToProcess in zonesToProcess)
            {
                List<IZone> matchedZones;
                newAndExistingZones.TryGetValue(zoneToProcess.ZoneName, out matchedZones);

                if (matchedZones != null)
                    zoneToProcess.AddedZones.AddRange(matchedZones.Where(itm => itm is AddedZone).Select(item => item as AddedZone));

                if (existingZones != null)
                    zoneToProcess.ExistingZones.AddRange(existingZones.FindAllRecords(item => item.Name.Equals(zoneToProcess.ZoneName, StringComparison.InvariantCultureIgnoreCase)));


                zoneToProcess.BED = GetZoneBED(zoneToProcess);
                zoneToProcess.EED = zoneToProcess.ExistingZones.Count() > 0 ? zoneToProcess.ExistingZones.Select(x => x.EED).VRMinimumDate() : null;

                zoneToProcess.ChangeType = GetZoneChangeType(zoneToProcess, existingZones, zonesToProcessNamesHashSet, existingCodesByZoneNames, closedExistingZones);


            }
        }

        private IEnumerable<ExistingZone> PrepareNotChangedZones(IEnumerable<ExistingZone> existingZones, HashSet<string> zonesToProcessNamesHashSet)
        {
            List<ExistingZone> notImportedZones = new List<ExistingZone>();

            foreach (ExistingZone existingZone in existingZones)
            {
                string zoneName = existingZone.ZoneEntity.Name;
                if (!zonesToProcessNamesHashSet.Contains(zoneName, StringComparer.InvariantCultureIgnoreCase))
                    notImportedZones.Add(existingZone);
            }

            return notImportedZones;
        }


        private DateTime GetZoneBED(ZoneToProcess zoneToProcess)
        {
            List<DateTime?> minDates = new List<DateTime?>();

            if (zoneToProcess.AddedZones.Count() > 0)
                minDates.Add(zoneToProcess.AddedZones.Min(item => item.BED));

            if (zoneToProcess.ExistingZones.Count() > 0)
                minDates.Add(zoneToProcess.ExistingZones.Min(item => item.BED));

            return minDates.VRMinimumDate().Value;
        }

        private ZoneChangeType GetZoneChangeType(ZoneToProcess zoneToProcess, IEnumerable<ExistingZone> existingZones, HashSet<string> zonesToProcessNamesHashSet, Dictionary<string, Dictionary<string, List<ExistingCode>>> existingCodesByZoneNames, Dictionary<string, List<ExistingZone>> closedExistingZones)
        {


            if (zoneToProcess.CodesToMove.Count() > 0 && zoneToProcess.ExistingZones.Count() == 0)
            {
                string originalZoneName = zoneToProcess.CodesToMove.First().OldZoneName;
                if (zoneToProcess.CodesToMove.All(itm => itm.OldZoneName == originalZoneName))
                {
                    List<ExistingZone> matchedRenamedExistingZones;
                    if (closedExistingZones != null && closedExistingZones.TryGetValue(originalZoneName, out matchedRenamedExistingZones))
                    {
                        zoneToProcess.RecentZoneName = originalZoneName;
                        return ZoneChangeType.Renamed;
                    }
                        
                }
            }

            List<ExistingZone> matchedExistingZones;
            if (closedExistingZones != null && closedExistingZones.TryGetValue(zoneToProcess.ZoneName, out matchedExistingZones))
                 return zoneToProcess.EED > DateTime.Now.Date ? ZoneChangeType.PendingClosed : ZoneChangeType.Deleted; 

            if (zoneToProcess.AddedZones.Count() > 0 && zoneToProcess.ExistingZones.Count() == 0)
                return zoneToProcess.BED > DateTime.Now.Date ? ZoneChangeType.PendingEffective : ZoneChangeType.New;

            return ZoneChangeType.NotChanged;
        }


        private Dictionary<string, Dictionary<string, List<ExistingCode>>> StructureExistingCodesByZonesNames(IEnumerable<ExistingCode> existingCodes)
        {
            Dictionary<string, Dictionary<string, List<ExistingCode>>> existingCodeByZoneName = new Dictionary<string, Dictionary<string, List<ExistingCode>>>();
            Dictionary<string, List<ExistingCode>> existingCodesByCode;

            foreach (ExistingCode existingCode in existingCodes)
            {
                if (!existingCodeByZoneName.TryGetValue(existingCode.ParentZone.Name, out existingCodesByCode))
                {
                    List<ExistingCode> existingCodesList = new List<ExistingCode>();
                    existingCodesList.Add(existingCode);
                    existingCodesByCode = new Dictionary<string, List<ExistingCode>>();
                    existingCodesByCode.Add(existingCode.CodeEntity.Code, existingCodesList);
                    existingCodeByZoneName.Add(existingCode.ParentZone.Name, existingCodesByCode);
                }
                else
                {
                    List<ExistingCode> matchedCodes;
                    if (!existingCodesByCode.TryGetValue(existingCode.CodeEntity.Code, out matchedCodes))
                    {
                        matchedCodes = new List<ExistingCode>();
                        matchedCodes.Add(existingCode);
                        existingCodesByCode.Add(existingCode.CodeEntity.Code, matchedCodes);
                    }
                    else
                        matchedCodes.Add(existingCode);
                }
            }

            return existingCodeByZoneName;
        }

        #endregion
    }
}
