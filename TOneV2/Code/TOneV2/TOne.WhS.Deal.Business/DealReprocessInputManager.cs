using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Deal.Data;
using TOne.WhS.Deal.Entities;

namespace TOne.WhS.Deal.Business
{
    public class DealReprocessInputManager
    {
        public void InsertDealReprocessInputs(IEnumerable<DealReprocessInput> dealReprocessInputs)
        {
            if (dealReprocessInputs == null || dealReprocessInputs.Count() == 0)
                return;

            IDealReprocessInputDataManager dealReprocessInputDataManager = DealDataManagerFactory.GetDataManager<IDealReprocessInputDataManager>();
            dealReprocessInputDataManager.InsertDealReprocessInputs(dealReprocessInputs.ToList());
        }

        public DealReprocessInput BuildDealReprocessInput(DealBillingSummary dealBillingSummary)
        {
            return new DealReprocessInput()
            {
                DealID = dealBillingSummary.DealId,
                ZoneGroupNb = dealBillingSummary.DealZoneGroupNb,
                IsSale = dealBillingSummary.IsSale,
                TierNb = dealBillingSummary.DealTierNb,
                RateTierNb = dealBillingSummary.DealRateTierNb,
                FromTime = dealBillingSummary.BatchStart,
                ToTime = dealBillingSummary.BatchStart.AddMinutes(30),
                UpToDurationInSec = dealBillingSummary.DurationInSeconds
            };
        }
    }
}
