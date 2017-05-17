using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Retail.BusinessEntity.Entities
{
    public abstract class RecurringChargeEvaluatorSettings
    {
        protected int CalculateNbOfDaysToCharge(IRecurringChargeEvaluatorContext context)
        {
            DateTime chargingStart = Utilities.Max(context.PackageAssignmentStart, context.ChargingPeriodStart);
            DateTime chargingEnd = Utilities.Min(context.PackageAssignmentEnd.HasValue ? context.PackageAssignmentEnd.Value : DateTime.MaxValue, context.ChargingPeriodEnd);
            return (int)(chargingEnd - chargingStart).TotalDays;
        }

        public abstract List<RecurringChargeEvaluatorOutput> Evaluate(IRecurringChargeEvaluatorContext context);
    }

    public interface IRecurringChargeEvaluatorContext
    {
        RecurringChargeEvaluatorDefinitionSettings EvaluatorDefinitionSettings { get; }

        Account Account { get; }

        DateTime ChargingPeriodStart { get; }

        DateTime ChargingPeriodEnd { get; }

        DateTime PackageAssignmentStart { get; }

        DateTime? PackageAssignmentEnd { get; }
    }
}
