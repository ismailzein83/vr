using System;
using System.Linq;
using Vanrise.Common;
using TOne.WhS.Deal.Entities;
using System.Collections.Generic;
using Vanrise.GenericData.Business;

namespace TOne.WhS.Deal.Business
{
    public class WholeDealProgressDataProviderSettings : BusinessObjectDataProviderExtendedSettings
    {
        public override Guid ConfigId => new Guid("3F99CF31-4298-482D-9E1C-BA0F3005F9CC");

        public override bool DoesSupportFilterOnAllFields => false;

        public override void LoadRecords(IBusinessObjectDataProviderLoadRecordsContext context)
        {
            if (!context.FromTime.HasValue)
                throw new NullReferenceException("context.FromTime");

            if (!context.ToTime.HasValue)
                throw new NullReferenceException("context.ToTime");

            var swapDealManager = new SwapDealManager();
            IEnumerable<DealDefinition> deals = swapDealManager.GetSwapDealsBetweenDate(context.FromTime.Value, context.ToTime.Value);

            DateTime minDealBED = DateTime.Now;

            var dealProgressByDealId = new Dictionary<int, DealProgress>();
            foreach (var deal in deals)
            {
                if (!dealProgressByDealId.TryGetValue(deal.DealId, out var dealProgress))
                {
                    dealProgress = new DealProgress
                    {
                        DealID = deal.DealId,
                        DealBED = deal.Settings.RealBED,
                        DealEED = deal.Settings.RealEED
                    };
                    DealGetZoneGroupsContext zoneGroupContext = new DealGetZoneGroupsContext(deal.DealId, DealZoneGroupPart.Both, true);
                    deal.Settings.GetZoneGroups(zoneGroupContext);

                    var dealSetting = (SwapDealSettings)deal.Settings;

                    dealProgress.SaleVolume = dealSetting.Inbounds.Sum(item => item.Volume);
                    dealProgress.CostVolume = dealSetting.Outbounds.Sum(item => item.Volume);
                    dealProgressByDealId.Add(deal.DealId, dealProgress);
                }
                minDealBED = minDealBED < deal.Settings.BeginDate ? minDealBED : deal.Settings.BeginDate;
            }

            var dealDetailedProgressManager = new DealDetailedProgressManager();
            IEnumerable<DealDetailedProgress> dealDetailProgresses = dealDetailedProgressManager.GetDealsDetailedProgress(dealProgressByDealId.Keys.ToList(), minDealBED, context.ToTime.Value);

            foreach (var dealDetailProgress in dealDetailProgresses)
            {
                if (!dealDetailProgress.TierNb.HasValue || dealDetailProgress.TierNb.Value == 2) //tiernumber = null Deal does not have any extra volume rate but there is traffic related to this zone, tiernumber=2 extra volume traffic will not be included
                    continue;

                var dealId = dealDetailProgress.DealId;
                var dealProgress = dealProgressByDealId.GetRecord(dealId);

                if (dealProgress == null)
                    continue;

                if (dealDetailProgress.IsSale)
                    dealProgress.SaleProgressInMinutes += dealDetailProgress.ReachedDurationInSeconds / 60;
                else
                    dealProgress.CostProgressInMinutes += dealDetailProgress.ReachedDurationInSeconds / 60;

                dealProgress.SaleProgressPercentage = dealProgress.SaleProgressInMinutes >= dealProgress.SaleVolume
                                                        ? 100
                                                        : (dealProgress.SaleProgressInMinutes * 100) / dealProgress.SaleVolume;

                dealProgress.CostProgressPercentage = dealProgress.CostProgressInMinutes >= dealProgress.SaleVolume
                                                        ? 100
                                                        : (dealProgress.CostProgressInMinutes * 100) / dealProgress.CostVolume;
                dealProgress.SaleCostProgressDifference = dealProgress.SaleProgressInMinutes - dealProgress.CostProgressInMinutes;

                dealProgress.SaleCostProgressDifferencePercentage = dealProgress.SaleProgressPercentage - dealProgress.CostProgressPercentage;
            }

            foreach (var dataRecord in dealProgressByDealId.Values)
            {
                context.OnRecordLoaded(DataRecordObjectMapper(dataRecord), DateTime.Now);
            }
        }
        private DataRecordObject DataRecordObjectMapper(DealProgress dealProgress)
        {
            var swapDealObject = new Dictionary<string, object>
            {
                {"Deal", dealProgress.DealID},
                {"DealBED", dealProgress.DealBED},
                {"DealEED", dealProgress.DealEED},
                {"SaleProgressInMinutes", dealProgress.SaleProgressInMinutes},
                {"SaleProgressPercentage", dealProgress.SaleProgressPercentage},
                {"CostProgressInMinutes", dealProgress.CostProgressInMinutes},
                {"CostProgressPercentage", dealProgress.CostProgressPercentage},
                { "DealProgress",dealProgress.SaleCostProgressDifference},
                { "DealProgressPercentage",dealProgress.SaleCostProgressDifferencePercentage}
            };
            return new DataRecordObject(new Guid("c87d22db-f35a-42e2-b22d-0d376f0e6898"), swapDealObject);
        }

        private class DealProgress
        {
            public int DealID { get; set; }
            public DateTime DealBED { get; set; }
            public DateTime? DealEED { get; set; }
            public decimal SaleProgressInMinutes { get; set; }
            public decimal SaleProgressPercentage { get; set; }
            public decimal CostProgressInMinutes { get; set; }
            public decimal CostProgressPercentage { get; set; }
            public decimal SaleCostProgressDifference { get; set; }
            public decimal SaleCostProgressDifferencePercentage { get; set; }
            public int SaleVolume { get; set; }
            public int CostVolume { get; set; }
        }
    }
}
