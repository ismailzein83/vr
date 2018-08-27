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
                    foreach (ImportedRate importedOtherRate in importedZone.ImportedOtherRates.Values)
                    {
                        OtherRatePreview otherRatePreview = new OtherRatePreview()
                        {
                            ZoneName = importedZone.ZoneName,
                            ImportedRate = decimal.Round(importedOtherRate.Rate, 8),
                            ImportedRateBED = importedOtherRate.BED,
                            RateTypeId = importedOtherRate.RateTypeId.Value,
                            ChangeTypeRate = importedOtherRate.ChangeType,
                            IsExcluded = importedOtherRate.IsExcluded
                        };

                        if (importedOtherRate.SystemRate != null)
                        {
                            otherRatePreview.SystemRate = importedOtherRate.SystemRate.Rate;
                            otherRatePreview.SystemRateBED = importedOtherRate.SystemRate.BED;
                            otherRatePreview.SystemRateEED = importedOtherRate.SystemRate.EED;
                        }

                        otherRatesPreview.Add(otherRatePreview);
                    }

                    foreach (NotImportedRate notImportedOtherRate in importedZone.NotImportedOtherRates)
                    {
                        otherRatesPreview.Add(new OtherRatePreview
                        {
                            ZoneName = importedZone.ZoneName,
                            SystemRate = notImportedOtherRate.Rate,
                            SystemRateBED = notImportedOtherRate.BED,
                            SystemRateEED = notImportedOtherRate.EED,
                            RateTypeId = notImportedOtherRate.RateTypeId.Value,
                            ChangeTypeRate = notImportedOtherRate.HasChanged ? RateChangeType.Deleted : RateChangeType.NotChanged
                        });
                    }
                }
            }


            if (notImportedZones != null)
            {
                foreach (NotImportedZone notImportedZone in notImportedZones)
                {
                    foreach (NotImportedRate notImportedOtherRate in notImportedZone.OtherRates)
                    {
                        OtherRatePreview zoneRatePreview = new OtherRatePreview()
                        {
                            ZoneName = notImportedZone.ZoneName,
                            ChangeTypeRate = notImportedZone.HasChanged ? RateChangeType.Deleted : RateChangeType.NotChanged,
                            SystemRate = notImportedOtherRate.Rate,
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
    }
}
