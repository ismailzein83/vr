using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Business;
using Vanrise.Queueing;
using Vanrise.Common;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.BusinessEntity.Business;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{

    public sealed class GenerateZonesRatesPreview : CodeActivity
    {

        [RequiredArgument]
        public InArgument<IEnumerable<ImportedZone>> ImportedZones { get; set; }

        [RequiredArgument]
        public InArgument<int> CountryId { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<NotImportedZone>> NotImportedZones { get; set; }

        [RequiredArgument]
        public InArgument<BaseQueue<IEnumerable<ZoneRatePreview>>> PreviewZonesRatesQueue { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            BaseQueue<IEnumerable<ZoneRatePreview>> previewZonesRatesQueue = this.PreviewZonesRatesQueue.Get(context);
            IEnumerable<ImportedZone> importedZones = this.ImportedZones.Get(context);
            IEnumerable<NotImportedZone> notImportedZones = this.NotImportedZones.Get(context);
            List<ZoneRatePreview> zonesRatesPreview = new List<ZoneRatePreview>();
            int countryId = CountryId.Get(context);

            if (importedZones != null)
            {
                foreach (ImportedZone importedZone in importedZones)
                {
                    ImportedRate importedNormalRate = importedZone.ImportedNormalRate;

                    ZoneRatePreview zoneRatePreview = new ZoneRatePreview
                    {
                        CountryId = countryId,
                        ZoneName = importedZone.ZoneName,
                        RecentZoneName = importedZone.RecentZoneName,
                        ChangeTypeZone = importedZone.ChangeType,
                        ZoneBED = importedZone.BED,
                        ZoneEED = importedZone.EED,
                        ImportedRate = decimal.Round(importedNormalRate.Rate, 8),
                        ImportedRateBED = importedNormalRate.BED,
                        ChangeTypeRate = importedZone.ImportedNormalRate.ChangeType,
                        IsExcluded = importedZone.IsExcluded
                    };

                    if (importedNormalRate.SystemRate != null)
                    {
                        zoneRatePreview.SystemRate = importedNormalRate.SystemRate.Rate;
                        zoneRatePreview.SystemRateBED = importedNormalRate.SystemRate.BED;
                        zoneRatePreview.SystemRateEED = importedNormalRate.SystemRate.EED;
                    }

                    ImportedZoneServiceGroup importedZoneServiceGroup = importedZone.ImportedZoneServiceGroup;

                    if (importedZoneServiceGroup != null)
                    {
                        zoneRatePreview.ImportedServiceIds = importedZoneServiceGroup.ServiceIds;
                        zoneRatePreview.ImportedServicesBED = importedZoneServiceGroup.BED;
                        zoneRatePreview.ZoneServicesChangeType = importedZoneServiceGroup.ChangeType;

                        if (importedZoneServiceGroup.SystemZoneServiceGroup != null)
                        {
                            zoneRatePreview.SystemServiceIds = importedZoneServiceGroup.SystemZoneServiceGroup.ZoneServicesIds;
                            zoneRatePreview.SystemServicesBED = importedZoneServiceGroup.SystemZoneServiceGroup.BED;
                            zoneRatePreview.SystemServicesEED = zoneRatePreview.ImportedServicesBED <= zoneRatePreview.SystemServicesBED
                                                                ? zoneRatePreview.SystemServicesBED
                                                                : zoneRatePreview.ImportedServicesBED;
                        }
                    }

                    NotImportedZoneServiceGroup notImportedZoneService = importedZone.NotImportedZoneServiceGroup;
                    if (notImportedZoneService != null)
                    {
                        zoneRatePreview.SystemServiceIds = notImportedZoneService.ZoneServicesIds;
                        zoneRatePreview.SystemServicesBED = notImportedZoneService.BED;
                        zoneRatePreview.SystemServicesEED = notImportedZoneService.EED;
                        zoneRatePreview.ZoneServicesChangeType = notImportedZoneService.HasChanged ? ZoneServiceChangeType.Deleted : ZoneServiceChangeType.NotChanged;
                    }

                    zonesRatesPreview.Add(zoneRatePreview);
                }
            }

            if (notImportedZones != null)
            {
                foreach (NotImportedZone notImportedZone in notImportedZones)
                {
                    //If a zone is renamed, do not show it in preview screen as an not imported zone
                    if (zonesRatesPreview.FindRecord(item => item.RecentZoneName != null && item.RecentZoneName.Equals(notImportedZone.ZoneName, StringComparison.InvariantCultureIgnoreCase)) != null)
                        continue;

                    ZoneRatePreview zoneRatePreview = new ZoneRatePreview()
                    {
                        CountryId = notImportedZone.CountryId,
                        ZoneName = notImportedZone.ZoneName,
                        ChangeTypeZone = notImportedZone.HasChanged ? ZoneChangeType.Deleted : ZoneChangeType.NotChanged,
                        ZoneBED = notImportedZone.BED,
                        ZoneEED = notImportedZone.EED,
                        ChangeTypeRate = notImportedZone.HasChanged ? RateChangeType.Deleted : RateChangeType.NotChanged
                    };

                    if (notImportedZone.NormalRate != null)
                    {
                        zoneRatePreview.SystemRate = notImportedZone.NormalRate.Rate;
                        zoneRatePreview.SystemRateBED = notImportedZone.NormalRate.BED;
                        zoneRatePreview.SystemRateEED = notImportedZone.NormalRate.EED;
                    }

                    if (notImportedZone.NotImportedZoneServiceGroup != null)
                    {
                        NotImportedZoneServiceGroup notImportedZoneService = notImportedZone.NotImportedZoneServiceGroup;
                        zoneRatePreview.ZoneServicesChangeType = notImportedZone.HasChanged ? ZoneServiceChangeType.Deleted : ZoneServiceChangeType.NotChanged;
                        zoneRatePreview.SystemServicesBED = notImportedZoneService.BED;
                        zoneRatePreview.SystemServicesEED = notImportedZoneService.EED;
                        zoneRatePreview.SystemServiceIds = notImportedZoneService.ZoneServicesIds;
                    }

                    zonesRatesPreview.Add(zoneRatePreview);
                }
            }

            previewZonesRatesQueue.Enqueue(zonesRatesPreview);
        }
    }
}
