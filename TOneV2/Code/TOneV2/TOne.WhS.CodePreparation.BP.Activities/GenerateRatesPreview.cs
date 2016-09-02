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

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<ZoneToProcess> zonesToProcess = this.ZonesToProcess.Get(context);

            IEnumerable<NotImportedZone> notImportedZones = this.NotImportedZones.Get(context);

            BaseQueue<IEnumerable<RatePreview>> previewRatesQueue = this.PreviewRatesQueue.Get(context);

            SalePriceListManager salePriceListManager = new SalePriceListManager();

            List<RatePreview> ratesPreview = new List<RatePreview>();

            foreach (ZoneToProcess zoneToProcess in zonesToProcess)
            {
                foreach (RateToAdd rateToAdd in zoneToProcess.RatesToAdd)
                {
                    ratesPreview.Add(new RatePreview()
                    {
                        ZoneName = rateToAdd.ZoneName,
                        OnwerType = rateToAdd.PriceListToAdd.OwnerType,
                        OwnerId = rateToAdd.PriceListToAdd.OwnerId,
                        Rate = rateToAdd.Rate,
                        BED = GetZoneBED(rateToAdd.AddedRates),
                        EED = GetZoneEED(rateToAdd.AddedRates)
                    });
                }

                ExistingRate systemRate = zoneToProcess.SystemRate;
                if (systemRate != null)
                {
                    SalePriceList salePriceList = salePriceListManager.GetPriceList(systemRate.RateEntity.PriceListId);

                    ratesPreview.Add(new RatePreview()
                    {
                        ZoneName = zoneToProcess.ZoneName,
                        OnwerType = salePriceList.OwnerType,
                        OwnerId = salePriceList.OwnerId,
                        Rate = systemRate.RateEntity.NormalRate,
                        BED = systemRate.BED,
                        EED = systemRate.EED
                    });
                }
            }


            foreach (NotImportedZone notImportedZone in notImportedZones)
            {
                if (notImportedZone.ExistingRate != null)
                {
                    SalePriceList salePriceList = salePriceListManager.GetPriceList(notImportedZone.ExistingRate.RateEntity.PriceListId);
                    ratesPreview.Add(new RatePreview()
                    {
                        ZoneName = notImportedZone.ZoneName,
                        OnwerType = salePriceList.OwnerType,
                        OwnerId = salePriceList.OwnerId,
                        Rate = notImportedZone.ExistingRate.RateEntity.NormalRate,
                        BED = notImportedZone.ExistingRate.RateEntity.BED,
                        EED = notImportedZone.ExistingRate.RateEntity.EED
                    });
                }

            }
            
            previewRatesQueue.Enqueue(ratesPreview);

        }

        private DateTime GetZoneBED(IEnumerable<AddedRate> addedRates)
        {
            return addedRates.Select(item => item.BED).Min();
        }

        private DateTime? GetZoneEED(IEnumerable<AddedRate> addedRates)
        {
            return addedRates.Select(item => item.EED).VRMaximumDate();
        }

    }
}
