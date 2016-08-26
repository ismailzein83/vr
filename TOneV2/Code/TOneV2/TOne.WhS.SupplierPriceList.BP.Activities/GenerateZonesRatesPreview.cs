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
        public InArgument<IEnumerable<NotImportedZone>> NotImportedZones { get; set; }

        [RequiredArgument]
        public InArgument<BaseQueue<IEnumerable<ZoneRatePreview>>> PreviewZonesRatesQueue { get; set; }
        
        protected override void Execute(CodeActivityContext context)
        {
            BaseQueue<IEnumerable<ZoneRatePreview>> previewZonesRatesQueue = this.PreviewZonesRatesQueue.Get(context);
            IEnumerable<ImportedZone> importedZones = this.ImportedZones.Get(context);
            IEnumerable<NotImportedZone> notImportedZones = this.NotImportedZones.Get(context);

            List<ZoneRatePreview> zonesRatesPreview = new List<ZoneRatePreview>();


            if (importedZones != null)
            {
                foreach (ImportedZone importedZone in importedZones)
                {
                    ImportedRate importedRate = importedZone.NormalRate;
                    zonesRatesPreview.Add(new ZoneRatePreview
                    {
                        CountryId = GetCountryId(importedZone),
                        ZoneName = importedZone.ZoneName,
                        RecentZoneName = importedZone.RecentZoneName,
                        ChangeTypeZone = importedZone.ChangeType,
                        ZoneBED = importedZone.BED,
                        ZoneEED = importedZone.EED,
                        SystemRate = GetSystemRate(importedZone),
                        ImportedRate = decimal.Round(importedRate.NormalRate, 8),
                        ImportedRateBED = importedRate.BED,
                        SystemRateBED = GetSystemRateBED(importedZone),
                        SystemRateEED = GetSystemRateEED(importedZone),
                        ChangeTypeRate = GetRateChangeType(importedZone)
                    });
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

                    if (notImportedZone.NormalSystemRate != null)
                    {
                        zoneRatePreview.SystemRate = notImportedZone.NormalSystemRate.RateEntity.NormalRate;
                        zoneRatePreview.SystemRateBED = notImportedZone.NormalSystemRate.BED;
                        zoneRatePreview.SystemRateEED = notImportedZone.NormalSystemRate.EED;
                    }

                    zonesRatesPreview.Add(zoneRatePreview);
                }
            }

            previewZonesRatesQueue.Enqueue(zonesRatesPreview);
        }

        private int GetCountryId(ImportedZone importedZone)
        {
            ImportedCode importedCode = importedZone.ImportedCodes.First();
            return importedCode.CodeGroup.CountryId;
        }


        private decimal? GetSystemRate(ImportedZone importedZone)
        {
            ExistingRate recentExistingRate = GetRecentExistingRate(importedZone);
            return recentExistingRate != null ? (decimal?)recentExistingRate.RateEntity.NormalRate : null;
        }


        private DateTime? GetSystemRateBED(ImportedZone importedZone)
        {
            ExistingRate recentExistingRate = GetRecentExistingRate(importedZone);
            return recentExistingRate != null ? (DateTime?)recentExistingRate.BED : null;
        }


        private DateTime? GetSystemRateEED(ImportedZone importedZone)
        {
            ExistingRate recentExistingRate = GetRecentExistingRate(importedZone);
            return recentExistingRate != null ? (DateTime?)recentExistingRate.RateEntity.EED : null;
        }

        private RateChangeType GetRateChangeType(ImportedZone importedZone)
        {
            return GetImportedRate(importedZone).ChangeType;
        }

        private ExistingRate GetRecentExistingRate(ImportedZone importedZone)
        {
            ExistingRate recentExistingRate = GetImportedRate(importedZone).ProcessInfo.RecentExistingRate;
            
            if (recentExistingRate == null)
            {
                List<ExistingZone> connectedExistingZones = importedZone.ExistingZones.GetConnectedEntities(DateTime.Today);
                if (connectedExistingZones != null)
                {
                    List<ExistingRate> existingRates = new List<ExistingRate>();

                    existingRates.AddRange(connectedExistingZones.SelectMany(item => item.ExistingRates).OrderBy(itm => itm.BED));
                    recentExistingRate = existingRates.LastOrDefault();
                }
            }

            return recentExistingRate;
        }

        private ImportedRate GetImportedRate(ImportedZone importedZone)
        {
            //TODO: change this logic when on import multiple and different rates are allowed
            return importedZone.NormalRate;
        }
    }
}
