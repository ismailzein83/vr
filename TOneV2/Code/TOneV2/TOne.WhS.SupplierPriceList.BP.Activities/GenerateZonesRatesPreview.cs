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

namespace TOne.WhS.SupplierPriceList.BP.Activities
{

    public sealed class GenerateZonesRatesPreview : CodeActivity
    {

        [RequiredArgument]
        public InArgument<IEnumerable<ImportedZone>> ImportedZones { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<ExistingZone>> NotImportedZones { get; set; }

        [RequiredArgument]
        public InArgument<BaseQueue<IEnumerable<ZoneRatePreview>>> PreviewZonesRatesQueue { get; set; }
        
        protected override void Execute(CodeActivityContext context)
        {
            BaseQueue<IEnumerable<ZoneRatePreview>> previewZonesRatesQueue = this.PreviewZonesRatesQueue.Get(context);
            IEnumerable<ImportedZone> importedZones = this.ImportedZones.Get(context);
            IEnumerable<ExistingZone> notImportedZones = this.NotImportedZones.Get(context);

            List<ZoneRatePreview> zonesRatesPreview = new List<ZoneRatePreview>();


            if (importedZones != null)
            {
                foreach (ImportedZone importedZone in importedZones)
                {
                    ImportedRate importedRateFirst = importedZone.ImportedRates.First();
                    zonesRatesPreview.Add(new ZoneRatePreview
                    {
                        CountryId = GetCountryId(importedZone),
                        ZoneName = importedZone.ZoneName,
                        RecentZoneName = importedZone.RecentZoneName,
                        ChangeTypeZone = importedZone.ChangeType,
                        ZoneBED = importedZone.BED,
                        ZoneEED = importedZone.ChangeType == ZoneChangeType.NotChanged ? importedZone.ExistingZones.First().EED : null,
                        CurrentRate = GetCurrentRate(importedZone),
                        ImportedRate = importedRateFirst.NormalRate,
                        ImportedRateBED = importedRateFirst.BED,
                        CurrentRateBED = GetCurrentRateBED(importedZone),
                        CurrentRateEED = GetCurrentRateEED(importedZone),
                        ChangeTypeRate = GetRateChangeType(importedZone)
                    });
                }
            }


            if (notImportedZones != null)
            {

                foreach (ExistingZone notImportedZone in notImportedZones)
                {
                    if(zonesRatesPreview.FindRecord(item => item.RecentZoneName != null && item.RecentZoneName.Equals(notImportedZone.Name, StringComparison.InvariantCultureIgnoreCase)) != null)
                        continue;

                    ExistingRate existingRateFirst = notImportedZone.ExistingRates.First();
                    zonesRatesPreview.Add(new ZoneRatePreview()
                    {
                        CountryId = notImportedZone.CountryId,
                        ZoneName = notImportedZone.Name,
                        ChangeTypeZone = ZoneChangeType.Closed,
                        ZoneBED = notImportedZones.Max(item => item.BED),
                        ZoneEED = notImportedZone.ChangedZone.EED,
                        CurrentRate = existingRateFirst.RateEntity.NormalRate,
                        CurrentRateBED = notImportedZone.ExistingRates.Min(item => item.BED),
                        CurrentRateEED = notImportedZone.ExistingRates.Max(item => item.EED.Value),
                        ChangeTypeRate = RateChangeType.NotChanged
                    });
                }
            }

            previewZonesRatesQueue.Enqueue(zonesRatesPreview);
        }


       

        private int GetCountryId(ImportedZone importedZone)
        {
            ImportedCode importedCode = importedZone.ImportedCodes.First();
            return importedCode.CodeGroup.CountryId;
        }


        private decimal GetCurrentRate(ImportedZone importedZone)
        {
            ImportedRate importedRateFirst = importedZone.ImportedRates.First();
            decimal? changedExistingRate = null;

            if (importedRateFirst.ChangedExistingRates != null && importedRateFirst.ChangedExistingRates.Count() > 0)
                changedExistingRate = importedRateFirst.ChangedExistingRates.First().RateEntity.NormalRate;

            return changedExistingRate.HasValue ? changedExistingRate.Value : importedRateFirst.NormalRate;
        }


        private DateTime GetCurrentRateBED(ImportedZone importedZone)
        {
            ImportedRate importedRateFirst = importedZone.ImportedRates.First();
            DateTime? changedExistingRateBED = null;

            if (importedRateFirst.ChangedExistingRates != null && importedRateFirst.ChangedExistingRates.Count() > 0)
                changedExistingRateBED = importedRateFirst.ChangedExistingRates.First().BED;

            return changedExistingRateBED.HasValue ? changedExistingRateBED.Value : importedRateFirst.BED;
        }


        private DateTime? GetCurrentRateEED(ImportedZone importedZone)
        {
            ImportedRate importedRateFirst = importedZone.ImportedRates.First();

            DateTime? changedExistingRateEED = null;

            if (importedRateFirst.ChangedExistingRates != null && importedRateFirst.ChangedExistingRates.Count() > 0)
                changedExistingRateEED = importedRateFirst.ChangedExistingRates.First().EED;

            return changedExistingRateEED.HasValue ? changedExistingRateEED.Value : importedRateFirst.EED;
        }


        private RateChangeType GetRateChangeType(ImportedZone importedZone)
        {
            if (importedZone.ChangeType == ZoneChangeType.New)
                return RateChangeType.New;

            ImportedRate importedRate = importedZone.ImportedRates.First();

            if (importedRate.ChangedExistingRates != null && importedRate.ChangedExistingRates.Count() > 0)
            {
                decimal existingRate = importedRate.ChangedExistingRates.First().RateEntity.NormalRate;
                if (existingRate > importedRate.NormalRate)
                    return RateChangeType.Decrease;
                else if (existingRate < importedRate.NormalRate)
                    return RateChangeType.Increase;
            }

            return RateChangeType.NotChanged;
        }

    }
}
