using System;
using System.Activities;
using Retail.BusinessEntity.Business;
using System.Collections.Generic;

namespace Retail.BusinessEntity.BP.Activities
{
    public sealed class EvaluateChargingDatesPeriod : CodeActivity
    {
        [RequiredArgument]
        public InArgument<DateTime> EffectiveDate { get; set; }

        [RequiredArgument]
        public InArgument<DateTime?> MaximumEndChargePeriod { get; set; }

        [RequiredArgument]
        public OutArgument<List<DateTime>> ChargingDatesPeriod { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            DateTime effectiveDate = this.EffectiveDate.Get(context);

            DateTime? maximumEndChargePeriod = this.MaximumEndChargePeriod.Get(context);
            DateTime? maximumRecurringChargeDate = maximumEndChargePeriod;

            AccountPackageRecurChargeManager accountPackageRecurChargeManager = new AccountPackageRecurChargeManager();
            DateTime? maximumExistingRecurringChargeDay = accountPackageRecurChargeManager.GetMaximumChargeDay();

            RecurChargeBalanceUpdateSummaryManager recurChargeBalanceUpdateSummaryManager = new Business.RecurChargeBalanceUpdateSummaryManager();
            DateTime? maximumRecurChargeBalanceUpdateSummaryChargeDay = recurChargeBalanceUpdateSummaryManager.GetMaximumChargeDay();

            if (maximumExistingRecurringChargeDay.HasValue)
                maximumRecurringChargeDate = maximumRecurringChargeDate.HasValue && maximumRecurringChargeDate.Value > maximumExistingRecurringChargeDay.Value ? maximumRecurringChargeDate : maximumExistingRecurringChargeDay;

            if (maximumRecurChargeBalanceUpdateSummaryChargeDay.HasValue)
                maximumRecurringChargeDate = maximumRecurringChargeDate.HasValue && maximumRecurringChargeDate.Value > maximumRecurChargeBalanceUpdateSummaryChargeDay.Value ? maximumRecurringChargeDate : maximumRecurChargeBalanceUpdateSummaryChargeDay;

            List<DateTime> chargingDatesPeriod = GetChargingDatesPeriod(effectiveDate, maximumRecurringChargeDate);
            this.ChargingDatesPeriod.Set(context, chargingDatesPeriod);
        }

        private List<DateTime> GetChargingDatesPeriod(DateTime startDate, DateTime? endDate)
        {
            if (!endDate.HasValue)
                return null;

            List<DateTime> dates = new List<DateTime>();
            DateTime chargingDate = startDate;

            while (chargingDate <= endDate.Value)
            {
                dates.Add(chargingDate);
                chargingDate = chargingDate.AddDays(1);
            }
            return dates;
        }
    }
}