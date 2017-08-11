using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Retail.BusinessEntity.MainExtensions.RecurringChargeEvaluators
{
    public enum PeriodicRecurringChargePeriodType
    {
        Monthly = 0,
        Days = 1
    }

    public class PeriodicRecurringChargeEvaluator : RecurringChargeEvaluatorSettings
    {
        public Decimal Price { get; set; }

        public int CurrencyId { get; set; }

        public PeriodicRecurringChargePeriodType PeriodType { get; set; }

        public int? NumberOfDays { get; set; }

        public override List<RecurringChargeEvaluatorOutput> Evaluate(IRecurringChargeEvaluatorContext context)
        {
            context.ThrowIfNull("context");
            PeriodicRecurringChargeEvaluatorDefinitionSettings periodicChargeDefinition = context.EvaluatorDefinitionSettings.CastWithValidate<PeriodicRecurringChargeEvaluatorDefinitionSettings>("periodicChargeDefinition");
            int daysToCharge = CalculateNbOfDaysToCharge(context);
            int pricePeriodDays;
            switch (this.PeriodType)
            {
                case PeriodicRecurringChargePeriodType.Days:
                    if (!this.NumberOfDays.HasValue)
                        throw new NullReferenceException("NumberOfDays");
                    pricePeriodDays = this.NumberOfDays.Value;
                    break;
                case PeriodicRecurringChargePeriodType.Monthly:
                    DateTime oneMonthFromToday = DateTime.Today.AddMonths(-1);
                    DateTime monthDateToTake = Utilities.Max(oneMonthFromToday, context.PackageAssignmentStart);
                    pricePeriodDays = DateTime.DaysInMonth(monthDateToTake.Year, monthDateToTake.Month);
                    break;
                default:
                    throw new NotSupportedException(string.Format("PeriodType '{0}'", this.PeriodType));
            }
            if (pricePeriodDays == 0)
                throw new NullReferenceException("priceDays");
            Decimal priceToCharge = this.Price * daysToCharge / pricePeriodDays;
            return new List<RecurringChargeEvaluatorOutput>
            {
                new RecurringChargeEvaluatorOutput 
                { 
                    ChargeableEntityId = periodicChargeDefinition.ChargeableEntityId,
                    Amount = priceToCharge,
                    CurrencyId = this.CurrencyId,
                    ChargingStart = context.ChargingStart,
                    ChargingEnd = context.ChargingEnd
                }
            };
        }

        public override void ValidatePackageAssignment(IValidateAssignmentRecurringChargeEvaluatorContext context)
        {
            context.IsValid = true;
        }
    }
}
