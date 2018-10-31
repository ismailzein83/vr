using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Invoice.Entities;

namespace Retail.BusinessEntity.MainExtensions.PackageTypes
{
    public class RecurChargePackageSettings : PackageExtendedSettings
    {
        public RecurringChargeEvaluatorSettings Evaluator { get; set; }

        public override void ValidatePackageAssignment(IPackageSettingAssignementValidateContext context)
        {
            ValidateAssignmentRecurringChargeEvaluatorContext validateAssignmentRecurringChargeEvaluatorContext = new ValidateAssignmentRecurringChargeEvaluatorContext
            {
                Account = context.Account,
                PackageAssignmentStart = context.BED,
                PackageAssignmentEnd = context.EED
            };
            this.Evaluator.ValidatePackageAssignment(validateAssignmentRecurringChargeEvaluatorContext);
            context.IsValid = validateAssignmentRecurringChargeEvaluatorContext.IsValid;
            context.ErrorMessage = validateAssignmentRecurringChargeEvaluatorContext.ErrorMessage;
        }
		public override void GetExtraFields(IPackageSettingExtraFieldsContext context)
		{
			RecurringChargeEvaluatorExtraFieldsContext recurringChargeEvaluatorExtraFieldsContext = new RecurringChargeEvaluatorExtraFieldsContext();
	
			Evaluator.GetExtraFields(recurringChargeEvaluatorExtraFieldsContext);
			context.ChargeValue = recurringChargeEvaluatorExtraFieldsContext.ChargeValue;
			context.PeriodType = recurringChargeEvaluatorExtraFieldsContext.PeriodType;
			context.CurrencyId = recurringChargeEvaluatorExtraFieldsContext.CurrencyId;

		}
	}
}
