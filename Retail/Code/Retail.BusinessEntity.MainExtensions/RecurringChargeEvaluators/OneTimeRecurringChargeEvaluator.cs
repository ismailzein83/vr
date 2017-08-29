using System;
using System.Collections.Generic;
using Vanrise.Common;
using Retail.BusinessEntity.Entities;
using Retail.BusinessEntity.Business;

namespace Retail.BusinessEntity.MainExtensions.RecurringChargeEvaluators
{
    public class OneTimeRecurringChargeEvaluator : RecurringChargeEvaluatorSettings
    {
        public Decimal Price { get; set; }

        public int CurrencyId { get; set; }

        public override List<RecurringChargeEvaluatorOutput> Evaluate(IRecurringChargeEvaluatorContext context)
        {
            context.ThrowIfNull("context");
            if (!context.PackageAssignmentEnd.HasValue)
                throw new ArgumentNullException("context.PackageAssignmentEnd");
            OneTimeRecurringChargeEvaluatorDefinitionSettings oneTimeChargeDefinition = context.EvaluatorDefinitionSettings.CastWithValidate<OneTimeRecurringChargeEvaluatorDefinitionSettings>("oneTimeChargeDefinition");
            int packageAssignmentDays = (int)(context.PackageAssignmentEnd.Value - context.PackageAssignmentStart).TotalDays;
            if (packageAssignmentDays == 0)
                throw new NullReferenceException("packageAssignmentDays");

            int chargeAmountPrecision = new AccountPackageRecurChargeManager().GetChargeAmountPrecision();

            Decimal priceToChargePerDay = decimal.Round(this.Price / packageAssignmentDays, chargeAmountPrecision);

            Decimal effectivePriceToChargePerDay;

            if (context.ChargeDay != context.PackageAssignmentEnd.Value.AddDays(-1))
                effectivePriceToChargePerDay = context.ChargeDay >= context.PackageAssignmentStart && context.ChargeDay < context.PackageAssignmentEnd.Value ? priceToChargePerDay : 0;
            else
                effectivePriceToChargePerDay = Price - priceToChargePerDay * (packageAssignmentDays - 1);

            return new List<RecurringChargeEvaluatorOutput>
            {
                new RecurringChargeEvaluatorOutput 
                { 
                    ChargeableEntityId = oneTimeChargeDefinition.ChargeableEntityId,
                    AmountPerDay = effectivePriceToChargePerDay,
                    CurrencyId = this.CurrencyId
                }
            };
        }

        public override void ValidatePackageAssignment(IValidateAssignmentRecurringChargeEvaluatorContext context)
        {
            if (!context.PackageAssignmentEnd.HasValue)
            {
                context.IsValid = false;
                context.ErrorMessage = "Package end date must be specified.";
            }
            else
            {
                context.IsValid = true;

            }
        }
    }
}
