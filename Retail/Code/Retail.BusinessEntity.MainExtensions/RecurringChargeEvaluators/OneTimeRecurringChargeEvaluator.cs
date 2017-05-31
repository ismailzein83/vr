using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

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
            int daysToCharge = CalculateNbOfDaysToCharge(context);
            Decimal priceToCharge = this.Price * daysToCharge / packageAssignmentDays;
            return new List<RecurringChargeEvaluatorOutput>
            {
                new RecurringChargeEvaluatorOutput 
                { 
                    ChargeableEntityId = oneTimeChargeDefinition.ChargeableEntityId,
                    Amount = priceToCharge,
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
