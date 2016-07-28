﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Queueing;
using Vanrise.Common;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.CodePreparation.Entities.Processing;
using TOne.WhS.CodePreparation.Entities;

namespace TOne.WhS.CodePreparation.BP.Activities
{

    public sealed class GenerateZonesPreview : CodeActivity
    {

        [RequiredArgument]
        public InArgument<IEnumerable<ZoneToProcess>> ZonesToProcess { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<ExistingZone>> NotChangedZones { get; set; }

        [RequiredArgument]
        public InArgument<Dictionary<string, List<ExistingZone>>> ClosedExistingZones { get; set; }

        [RequiredArgument]
        public InArgument<BaseQueue<IEnumerable<ZonePreview>>> PreviewZonesQueue { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            BaseQueue<IEnumerable<ZonePreview>> previewZonesQueue = this.PreviewZonesQueue.Get(context);
            IEnumerable<ZoneToProcess> zonesToProcess = this.ZonesToProcess.Get(context);
            IEnumerable<ExistingZone> notChangedZones = this.NotChangedZones.Get(context);
            Dictionary<string, List<ExistingZone>> closedExistingZones = ClosedExistingZones.Get(context);

            List<ZonePreview> zonesPreview = new List<ZonePreview>();


            if (zonesToProcess != null)
            {
                foreach (ZoneToProcess zoneToProcess in zonesToProcess)
                {
                    zonesPreview.Add(new ZonePreview
                    {
                        CountryId = GetCountryId(zoneToProcess),
                        ZoneName = zoneToProcess.ZoneName,
                        RecentZoneName = zoneToProcess.RecentZoneName,
                        ChangeTypeZone = zoneToProcess.ChangeType,
                        ZoneBED = zoneToProcess.BED,
                        ZoneEED = zoneToProcess.EED
                    });
                }
            }


            if (notChangedZones != null)
            {

                foreach (ExistingZone notImportedZone in notChangedZones)
                {
                    //If a zone is renamed, do not show it in preview screen as an not imported zone
                    if (zonesPreview.FindRecord(item => item.RecentZoneName != null && item.RecentZoneName.Equals(notImportedZone.Name, StringComparison.InvariantCultureIgnoreCase)) != null)
                        continue;

                    //If a zone is deleted by moving all its codes, do not show it as not changed
                    List<ExistingZone> matchedClosedZones;
                    if (closedExistingZones.TryGetValue(notImportedZone.ZoneEntity.Name, out matchedClosedZones))
                        continue;

                    zonesPreview.Add(new ZonePreview()
                    {
                        CountryId = notImportedZone.CountryId,
                        ZoneName = notImportedZone.Name,
                        ChangeTypeZone = ZoneChangeType.NotChanged,
                        ZoneBED = notImportedZone.BED,
                        ZoneEED = notImportedZone.EED
                    });
                }
            }


            if (closedExistingZones != null)
            {
                foreach (KeyValuePair<string, List<ExistingZone>> closedExistingZone in closedExistingZones)
                {
                    //If a zone is renamed, do not show it in preview screen as an not imported zone
                    if (zonesPreview.FindRecord(item => item.RecentZoneName != null && item.RecentZoneName.Equals(closedExistingZone.Key, StringComparison.InvariantCultureIgnoreCase)) != null)
                        continue;

                    //If a zone is deleted by deleting its codes, do not show it as not changed
                    if (!zonesToProcess.Any(item => item.ZoneName.Equals(closedExistingZone.Key, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        DateTime? ClosedZoneEED =  Vanrise.Common.ExtensionMethods.VRMaximumDate(closedExistingZone.Value.Select(item => item.EED));
                        zonesPreview.Add(new ZonePreview()
                        {
                            CountryId = closedExistingZone.Value.First().CountryId,
                            ZoneName = closedExistingZone.Key,
                            ChangeTypeZone = ClosedZoneEED > DateTime.Today.Date ? ZoneChangeType.PendingClosed : ZoneChangeType.Deleted,
                            ZoneBED = closedExistingZone.Value.Min(item => item.BED),
                            ZoneEED = ClosedZoneEED
                        });
                    }
                }
            }

            previewZonesQueue.Enqueue(zonesPreview);
        }

        private int GetCountryId(ZoneToProcess zoneToProcess)
        {
            if (zoneToProcess.CodesToAdd.Count() > 0)
                return zoneToProcess.CodesToAdd.First().CodeGroup.CountryId;
            else if (zoneToProcess.CodesToMove.Count() > 0)
                return zoneToProcess.CodesToMove.First().CodeGroup.CountryId;
            else
                return zoneToProcess.CodesToClose.First().CodeGroup.CountryId;
        }

    }
}
