using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Common;
using TOne.WhS.CodePreparation.Entities.Processing;
using TOne.WhS.CodePreparation.Entities;
using Vanrise.BusinessProcess;
using TOne.WhS.BusinessEntity.Business;

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
        public InArgument<Dictionary<string,List<ExistingZone>>> ClosedExistingZones { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<NotImportedZone>> NotImportedZones { get; set; }

        protected override ProcessCountryZonesOutput DoWorkWithResult(ProcessCountryZonesInput inputArgument, AsyncActivityHandle handle)
        {
            Dictionary<string, Dictionary<string, List<ExistingCode>>> existingCodeByZoneName = StructureExistingCodesByZonesNames(inputArgument.ExistingCodes);

            HashSet<string> zonesToProcessNamesHashSet = new HashSet<string>(inputArgument.ZonesToProcess.Select(item => item.ZoneName), StringComparer.InvariantCultureIgnoreCase);

            UpdateImportedZonesInfo(inputArgument.ZonesToProcess, inputArgument.NewAndExistingZones, inputArgument.ExistingZones, zonesToProcessNamesHashSet, existingCodeByZoneName, inputArgument.ClosedExistingZones);

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

                zoneToProcess.ChangeType = GetZoneChangeType(zoneToProcess, closedExistingZones);


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
            notImportedZone.ExistingRate = firstElementInTheList.BED <= DateTime.Today.Date ? existingRates.GetSystemRate<ExistingRate>(DateTime.Today) : existingRates.FirstOrDefault();
            notImportedZone.HasChanged = linkedExistingZones.Any(x => x.ChangedZone != null);

            return notImportedZone;
        }

        private List<ExistingRate> GetExistingRatesByLinkedExistingZones(List<ExistingZone> linkedExistingZones)
        {
            List<ExistingRate> existingRates = new List<ExistingRate>();

            existingRates.AddRange(linkedExistingZones.SelectMany(item => item.ExistingRates).OrderBy(itm => itm.BED));

            return existingRates;
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

        private ZoneChangeType GetZoneChangeType(ZoneToProcess zoneToProcess, Dictionary<string, List<ExistingZone>> closedExistingZones)
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
                 return ZoneChangeType.Deleted; 

            if (zoneToProcess.AddedZones.Count() > 0 && zoneToProcess.ExistingZones.Count() == 0)
                return ZoneChangeType.New;

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
