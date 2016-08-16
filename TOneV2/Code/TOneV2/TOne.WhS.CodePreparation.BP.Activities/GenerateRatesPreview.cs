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
        public InArgument<IEnumerable<ExistingZone>> NotChangedZones { get; set; }

        [RequiredArgument]
        public InArgument<BaseQueue<IEnumerable<RatePreview>>> PreviewRatesQueue { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<ZoneToProcess> zonesToProcess = this.ZonesToProcess.Get(context);

            IEnumerable<ExistingZone> NotChangedZones = this.NotChangedZones.Get(context);

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

                foreach (ExistingZone existingZone in zoneToProcess.ExistingZones)
                {
                    foreach (ExistingRate existingRate in existingZone.ExistingRates)
                    {
                        
                        SalePriceList salePriceList = salePriceListManager.GetPriceList(existingRate.RateEntity.PriceListId);

                        ratesPreview.Add(new RatePreview()
                        {
                            ZoneName = zoneToProcess.ZoneName,
                            OnwerType = salePriceList.OwnerType,
                            OwnerId = salePriceList.OwnerId,
                            Rate = existingRate.RateEntity.NormalRate,
                            BED = existingRate.BED,
                            EED = existingRate.EED
                        });
                    }
                }
            }


            foreach (ExistingZone notChangedZone in NotChangedZones)
            {
                foreach (ExistingRate existingRate in notChangedZone.ExistingRates)
                {
                    SalePriceList salePriceList = salePriceListManager.GetPriceList(existingRate.RateEntity.PriceListId);

                    ratesPreview.Add(new RatePreview()
                    {
                        ZoneName = notChangedZone.Name,
                        OnwerType = salePriceList.OwnerType,
                        OwnerId = salePriceList.OwnerId,
                        Rate = existingRate.RateEntity.NormalRate,
                        BED = existingRate.BED,
                        EED = existingRate.EED
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
