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

    public sealed class GenerateOtherRatesPreview : CodeActivity
    {

        [RequiredArgument]
        public InArgument<IEnumerable<ImportedZone>> ImportedZones { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<NotImportedZone>> NotImportedZones { get; set; }

        [RequiredArgument]
        public InArgument<BaseQueue<IEnumerable<OtherRatePreview>>> PreviewOtherRatesQueue { get; set; }
        
        protected override void Execute(CodeActivityContext context)
        {
            BaseQueue<IEnumerable<OtherRatePreview>> previewZonesRatesQueue = this.PreviewOtherRatesQueue.Get(context);
            IEnumerable<ImportedZone> importedZones = this.ImportedZones.Get(context);
            IEnumerable<NotImportedZone> notImportedZones = this.NotImportedZones.Get(context);

            List<OtherRatePreview> otherRatesPreview = new List<OtherRatePreview>();


            if (importedZones != null)
            {
                foreach (ImportedZone importedZone in importedZones)
                {
                    foreach (ImportedRate importedOtherRate in importedZone.OtherRates.Values)
                    {
                        otherRatesPreview.Add(new OtherRatePreview
                        {
                            ZoneName = importedZone.ZoneName,
                            ImportedRate = decimal.Round(importedOtherRate.NormalRate, 8),
                            ImportedRateBED = importedOtherRate.BED,
                            SystemRate = GetSystemRate(importedZone, importedOtherRate),
                            SystemRateBED = GetSystemRateBED(importedZone, importedOtherRate),
                            SystemRateEED = GetSystemRateEED(importedZone, importedOtherRate),
                            RateTypeId = importedOtherRate.RateTypeId.Value,
                            ChangeTypeRate = GetRateChangeType(importedOtherRate)
                        });
                    }

                    foreach (NotImportedRate notImportedOtherRate in importedZone.NotImportedRates)
                    {
                        otherRatesPreview.Add(new OtherRatePreview
                        {
                            ZoneName = importedZone.ZoneName,
                            SystemRate = notImportedOtherRate.SystemRate,
                            SystemRateBED = notImportedOtherRate.BED,
                            SystemRateEED = notImportedOtherRate.EED,
                            RateTypeId = notImportedOtherRate.RateTypeId.Value,
                            ChangeTypeRate = RateChangeType.Deleted
                        });
                    }
                }
            }


            if (notImportedZones != null)
            {
                foreach (NotImportedZone notImportedZone in notImportedZones)
                {
                    foreach (NotImportedRate notImportedOtherRate in notImportedZone.OtherSystemRates)
                    {
                        OtherRatePreview zoneRatePreview = new OtherRatePreview()
                        {
                            ZoneName = notImportedZone.ZoneName,
                            ChangeTypeRate = notImportedZone.HasChanged ? RateChangeType.Deleted : RateChangeType.NotChanged,
                            SystemRate = notImportedOtherRate.SystemRate,
                            SystemRateBED = notImportedOtherRate.BED,
                            SystemRateEED = notImportedOtherRate.EED,
                            RateTypeId = notImportedOtherRate.RateTypeId.Value
                        };
                        otherRatesPreview.Add(zoneRatePreview);
                    }
                }
            }

            previewZonesRatesQueue.Enqueue(otherRatesPreview);
        }

        private decimal? GetSystemRate(ImportedZone importedZone, ImportedRate importedRate)
        {
            ExistingRate recentExistingRate = GetRecentExistingRate(importedZone, importedRate);
            return recentExistingRate != null ? (decimal?)recentExistingRate.RateEntity.NormalRate : null;
        }

        private DateTime? GetSystemRateBED(ImportedZone importedZone, ImportedRate importedRate)
        {
            ExistingRate recentExistingRate = GetRecentExistingRate(importedZone, importedRate);
            return recentExistingRate != null ? (DateTime?)recentExistingRate.BED : null;
        }

        private DateTime? GetSystemRateEED(ImportedZone importedZone, ImportedRate importedRate)
        {
            ExistingRate recentExistingRate = GetRecentExistingRate(importedZone, importedRate);
            return recentExistingRate != null ? (DateTime?)recentExistingRate.RateEntity.EED : null;
        }

        private RateChangeType GetRateChangeType(ImportedRate importedRate)
        {
            return importedRate.ChangeType;
        }

        private ExistingRate GetRecentExistingRate(ImportedZone importedZone, ImportedRate importedRate)
        {
            ExistingRate recentExistingRate = importedRate.ProcessInfo.RecentExistingRate;
            
            if (recentExistingRate == null)
            {
                List<ExistingZone> connectedExistingZones = importedZone.ExistingZones.GetConnectedEntities(DateTime.Today);
                if (connectedExistingZones != null)
                {
                    List<ExistingRate> existingRates = new List<ExistingRate>();

                    existingRates.AddRange(connectedExistingZones.SelectMany(item => item.ExistingRates.Where(existingRate => existingRate.RateEntity.RateTypeId.HasValue && existingRate.RateEntity.RateTypeId == importedRate.RateTypeId)).OrderBy(itm => itm.BED));
                    recentExistingRate = existingRates.LastOrDefault();
                }
            }

            return recentExistingRate;
        }
    }
}
