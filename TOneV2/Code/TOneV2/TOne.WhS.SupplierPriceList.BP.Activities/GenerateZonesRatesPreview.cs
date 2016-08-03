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
                        ZoneEED = importedZone.EED,
                        CurrentRate = GetCurrentRate(importedZone),
                        ImportedRate = decimal.Round(importedRateFirst.NormalRate, 8),
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
                    //If a zone is renamed, do not show it in preview screen as an not imported zone
                    if(zonesRatesPreview.FindRecord(item => item.RecentZoneName != null && item.RecentZoneName.Equals(notImportedZone.Name, StringComparison.InvariantCultureIgnoreCase)) != null)
                        continue;

                    //Get the changed rate, the one that was closed by this action
                    ExistingRate closedExistingRate = notImportedZone.ExistingRates.Where(itm => itm.ChangedRate != null).FirstOrDefault();
                    zonesRatesPreview.Add(new ZoneRatePreview()
                    {
                        CountryId = notImportedZone.CountryId,
                        ZoneName = notImportedZone.Name,
                        ChangeTypeZone = ZoneChangeType.Deleted,
                        ZoneBED = notImportedZone.BED,
                        ZoneEED = notImportedZone.ChangedZone.EED,
                        CurrentRate = closedExistingRate.RateEntity.NormalRate,
                        CurrentRateBED = closedExistingRate.BED,
                        CurrentRateEED = closedExistingRate.EED,
                        ChangeTypeRate = RateChangeType.Deleted
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


        private decimal? GetCurrentRate(ImportedZone importedZone)
        {
            ExistingRate recentExistingRate = GetRecentExistingRate(importedZone);
            return recentExistingRate != null ? (decimal?)recentExistingRate.RateEntity.NormalRate : null;
        }


        private DateTime? GetCurrentRateBED(ImportedZone importedZone)
        {
            ExistingRate recentExistingRate = GetRecentExistingRate(importedZone);
            return recentExistingRate != null ? (DateTime?)recentExistingRate.BED : null;
        }


        private DateTime? GetCurrentRateEED(ImportedZone importedZone)
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
            return GetImportedRate(importedZone).ProcessInfo.RecentExistingRate;
        }

        private ImportedRate GetImportedRate(ImportedZone importedZone)
        {
            //TODO: change this logic when on import multiple and different rates are allowed
            return importedZone.ImportedRates.First();
        }
    }
}
