using System;
using System.Collections.Generic;
using Vanrise.Common;
using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;

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
            periodicChargeDefinition.PricingStatuses.ThrowIfNull("periodicChargeDefinition.PricingStatuses");

            if (!periodicChargeDefinition.PricingStatuses.Contains(context.AccountStatusId))
            {
                return new List<RecurringChargeEvaluatorOutput>
                {
                    new RecurringChargeEvaluatorOutput 
                    { 
                        ChargeableEntityId = periodicChargeDefinition.ChargeableEntityId,
                        AmountPerDay = 0,
                        CurrencyId = this.CurrencyId
                    }
                };
            }

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
                throw new NullReferenceException("pricePeriodDays");

            List<DateTime> endOfPeriods = context.InitializeData as List<DateTime>;

            int chargeAmountPrecision = new AccountPackageRecurChargeManager().GetChargeAmountPrecision();
            Decimal priceToChargePerDay = decimal.Round(this.Price / pricePeriodDays, chargeAmountPrecision);

            Decimal effectivePriceToChargePerDay;

            if (endOfPeriods == null || !endOfPeriods.Contains(context.ChargeDay))
                effectivePriceToChargePerDay = context.ChargeDay >= context.PackageAssignmentStart && (!context.PackageAssignmentEnd.HasValue || context.ChargeDay < context.PackageAssignmentEnd.Value) ? priceToChargePerDay : 0;
            else
                effectivePriceToChargePerDay = Price - priceToChargePerDay * (pricePeriodDays - 1);

            return new List<RecurringChargeEvaluatorOutput>
            {
                new RecurringChargeEvaluatorOutput 
                { 
                    ChargeableEntityId = periodicChargeDefinition.ChargeableEntityId,
                    AmountPerDay = effectivePriceToChargePerDay,
                    CurrencyId = this.CurrencyId
                }
            };
        }

        public override void ValidatePackageAssignment(IValidateAssignmentRecurringChargeEvaluatorContext context)
        {
            context.IsValid = true;
        }

        public override void Initialize(IInitializeRecurringChargeContext context)
        {
            context.ThrowIfNull("context");
            switch (this.PeriodType)
            {
                case PeriodicRecurringChargePeriodType.Days:
                    if (!this.NumberOfDays.HasValue)
                        throw new NullReferenceException("NumberOfDays");

                    DateTime daysBeginDate = context.PackageAssignmentStart > context.ChargingPeriodStart ? context.PackageAssignmentStart : context.ChargingPeriodStart;

                    int periodNumberOfDaysUsed = (Convert.ToInt32(daysBeginDate.Subtract(context.PackageAssignmentStart).TotalDays)) % NumberOfDays.Value;

                    List<DateTime> endOfPeriods = new List<DateTime>();
                    DateTime endPeriod = daysBeginDate.AddDays(NumberOfDays.Value - periodNumberOfDaysUsed - 1);
                    do
                    {
                        if (endPeriod <= context.ChargingPeriodEnd)
                        {
                            endOfPeriods.Add(endPeriod);
                            endPeriod = endPeriod.AddDays(NumberOfDays.Value);
                        }
                        else
                        {
                            break;
                        }
                    } while (true);

                    if (endOfPeriods.Count > 0)
                    {
                        context.InitializeData = endOfPeriods;
                    }
                    break;

                case PeriodicRecurringChargePeriodType.Monthly:
                    DateTime monthlyBeginDate = context.PackageAssignmentStart > context.ChargingPeriodStart ? context.PackageAssignmentStart : context.ChargingPeriodStart;

                    List<DateTime> endOfMonths = new List<DateTime>();
                    DateTime lastDayOfMonth = monthlyBeginDate.GetLastDayOfMonth();
                    do
                    {
                        if (lastDayOfMonth <= context.ChargingPeriodEnd)
                        {
                            endOfMonths.Add(lastDayOfMonth);
                            lastDayOfMonth = lastDayOfMonth.AddMonths(1);
                        }
                        else
                        {
                            break;
                        }
                    } while (true);
                    if (endOfMonths.Count > 0)
                    {
                        context.InitializeData = endOfMonths;
                    }
                    break;
                default:
                    throw new NotSupportedException(string.Format("PeriodType '{0}'", this.PeriodType));
            }
        }
    }
}
