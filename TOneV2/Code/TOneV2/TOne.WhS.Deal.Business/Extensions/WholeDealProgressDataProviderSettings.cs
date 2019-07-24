using System;
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
            List<int> dealIds = new List<int>();
            foreach (var deal in deals)
            {
                dealIds.Add(deal.DealId);
                minDealBED = minDealBED < deal.Settings.BeginDate ? minDealBED : deal.Settings.BeginDate;
            }

            var dealDetailedProgressManager = new DealDetailedProgressManager();
            IEnumerable<DealDetailedProgress> dealDetailProgresses = dealDetailedProgressManager.GetDealsDetailedProgress(dealIds, minDealBED, context.ToTime.Value);

            var dealDefinitionManager = new DealDefinitionManager();
            var dealProgressByDealIds = new Dictionary<int, DealProgress>();
            foreach (var dealDetailProgress in dealDetailProgresses)
            {
                var dealId = dealDetailProgress.DealId;
                if (!dealProgressByDealIds.TryGetValue(dealId, out var dealProgress))
                {
                    DealDefinition dealDefinition = dealDefinitionManager.GetDeal(dealId);
                    dealProgress = new DealProgress
                    {
                        DealID = dealId,
                        DealBED = dealDefinition.Settings.RealBED,
                        DealEED = dealDefinition.Settings.RealEED
                    };
                    dealProgressByDealIds.Add(dealId, dealProgress);
                }

                if (dealDetailProgress.IsSale)
                    dealProgress.SaleProgressInMinutes += dealDetailProgress.ReachedDurationInSeconds / 60;
                else
                    dealProgress.CostProgressInMinutes += dealDetailProgress.ReachedDurationInSeconds / 60;
                 
                dealProgress.SaleCostProgressDifference = dealProgress.SaleProgressInMinutes - dealProgress.CostProgressInMinutes;
            }

            foreach (var dataRecord in dealProgressByDealIds.Values)
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
                {"CostProgressInMinutes", dealProgress.CostProgressInMinutes},
                { "DealProgress",dealProgress.SaleCostProgressDifference}
            };
            return new DataRecordObject(new Guid("c87d22db-f35a-42e2-b22d-0d376f0e6898"), swapDealObject);
        }

        private class DealProgress
        {
            public int DealID { get; set; }
            public DateTime DealBED { get; set; }
            public DateTime? DealEED { get; set; }
            public decimal SaleProgressInMinutes { get; set; }
            public decimal CostProgressInMinutes { get; set; }
            public decimal SaleCostProgressDifference { get; set; }
        }
    }
}
