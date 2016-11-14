using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Queueing;
using Vanrise.Common;
using TOne.WhS.CodePreparation.Entities.Processing;
using TOne.WhS.CodePreparation.Entities;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.BusinessEntity.Business;

namespace TOne.WhS.CodePreparation.BP.Activities
{

    public sealed class GenerateRatesPreview : CodeActivity
    {
        [RequiredArgument]
        public InArgument<IEnumerable<ZoneToProcess>> ZonesToProcess { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<NotImportedZone>> NotImportedZones { get; set; }

        [RequiredArgument]
        public InArgument<BaseQueue<IEnumerable<RatePreview>>> PreviewRatesQueue { get; set; }

        [RequiredArgument]
        public InOutArgument<List<RatePreview>> PreviewRates { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<ZoneToProcess> zonesToProcess = this.ZonesToProcess.Get(context);
            IEnumerable<NotImportedZone> notImportedZones = this.NotImportedZones.Get(context);
            BaseQueue<IEnumerable<RatePreview>> previewRatesQueue = this.PreviewRatesQueue.Get(context);
            List<RatePreview> previewRates = this.PreviewRates.Get(context);

            List<RatePreview> ratesPreview = new List<RatePreview>();

            foreach (ZoneToProcess zoneToProcess in zonesToProcess)
            {
                ratesPreview.AddRange(zoneToProcess.RatesToAdd.MapRecords(RateToAddPreviewMapper));
                ratesPreview.AddRange(zoneToProcess.NotImportedNormalRates.MapRecords(NotImportedRatePreviewMapper));
            }

            foreach (NotImportedZone notImportedZone in notImportedZones)
            {
                ratesPreview.AddRange(notImportedZone.NotImportedNormalRates.MapRecords(NotImportedRatePreviewMapper));
            }
            
            previewRatesQueue.Enqueue(ratesPreview);
            previewRates.AddRange(ratesPreview);
        }

        private DateTime GetRateToAddBED(IEnumerable<AddedRate> addedRates)
        {
            return addedRates.Select(item => item.BED).Min();
        }

        private DateTime? GetRateToAddEED(IEnumerable<AddedRate> addedRates)
        {
            return addedRates.Select(item => item.EED).VRMaximumDate();
        }

        private RatePreview RateToAddPreviewMapper(RateToAdd rateToAdd)
        {
            return new RatePreview()
            {
                ZoneName = rateToAdd.ZoneName,
                OnwerType = rateToAdd.PriceListToAdd.OwnerType,
                ChangeType = RateChangeType.New,
                OwnerId = rateToAdd.PriceListToAdd.OwnerId,
                Rate = rateToAdd.Rate,
                BED = GetRateToAddBED(rateToAdd.AddedRates),
                EED = GetRateToAddEED(rateToAdd.AddedRates)
            };
        }

        private RatePreview NotImportedRatePreviewMapper(NotImportedRate notImportedRate)
        {
            return new RatePreview()
            {
                ZoneName = notImportedRate.ZoneName,
                OnwerType = notImportedRate.OwnerType,
                ChangeType = RateChangeType.NotChanged,
                OwnerId = notImportedRate.OwnerId,
                Rate = notImportedRate.Rate,
                BED = notImportedRate.BED,
                EED = notImportedRate.EED
            };
        }

    }
}
