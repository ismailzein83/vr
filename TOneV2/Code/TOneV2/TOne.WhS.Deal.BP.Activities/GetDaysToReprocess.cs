using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.Deal.Business;
using TOne.WhS.Deal.Entities;

namespace TOne.WhS.Deal.BP.Activities
{
    public sealed class GetDaysToReprocess : CodeActivity
    {
        [RequiredArgument]
        public OutArgument<List<DayToReprocessSummary>> DaysToReprocessSummary { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var daysToReprocess = new DaysToReprocessManager().GetAllDaysToReprocess();
            var daysToReprocessSummaryByDate = new Dictionary<DateTime, DayToReprocessSummary>();

            List<DayToReprocessSummary> daysToReprocessSummary = null;
            if (daysToReprocess != null)
            {
                daysToReprocessSummary = new List<DayToReprocessSummary>();
                foreach (var dayToReprocess in daysToReprocess)
                {
                    DayToReprocessSummary dayToReprocesSummary;
                    if (!daysToReprocessSummaryByDate.TryGetValue(dayToReprocess.Date, out dayToReprocesSummary))
                    {
                        dayToReprocesSummary = new DayToReprocessSummary() { Date = dayToReprocess.Date, CustomerIds = new List<int>(), SupplierIds = new List<int>() };
                        daysToReprocessSummaryByDate.Add(dayToReprocess.Date, dayToReprocesSummary);
                        daysToReprocessSummary.Add(dayToReprocesSummary);
                    }

                    if (dayToReprocess.IsSale)
                        dayToReprocesSummary.CustomerIds.Add(dayToReprocess.CarrierAccountId);
                    else
                        dayToReprocesSummary.SupplierIds.Add(dayToReprocess.CarrierAccountId);
                }
            }

            daysToReprocessSummary = daysToReprocessSummary != null ? daysToReprocessSummary.OrderByDescending(itm => itm.Date).ToList() : null;
            this.DaysToReprocessSummary.Set(context, daysToReprocessSummary);
        }
    }
}