using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Entities;

namespace Retail.BusinessEntity.Entities
{
    public abstract class RecurringChargeEvaluatorSettings
    {
        public abstract void ValidatePackageAssignment(IValidateAssignmentRecurringChargeEvaluatorContext context);

        public virtual void Initialize(IInitializeRecurringChargeContext context) { }

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

        DateTime ChargeDay { get; }

        long AccountPackageId { get; }

        Object InitializeData { get; }

        Guid AccountBEDefinitionId { get; }

        Func<int, DateTimeRange, List<AccountPackageRecurCharge>> GetAccountPackageRecurCharges { get; }

        Dictionary<AccountDefinition, IOrderedEnumerable<AccountStatusHistory>> AccountStatusHistoryListByAccountDefinition { get; }
    }

    public interface IInitializeRecurringChargeContext
    {
        DateTime ChargingPeriodStart { get; }

        DateTime ChargingPeriodEnd { get; }

        DateTime PackageAssignmentStart { get; }

        DateTime? PackageAssignmentEnd { get; }

        object InitializeData { set; }

        DateTimeRange RecurringChargePeriod { set; }
    }

    public interface IValidateAssignmentRecurringChargeEvaluatorContext
    {
        Account Account { get; }
        DateTime PackageAssignmentStart { get; }
        DateTime? PackageAssignmentEnd { get; }
        bool IsValid { set; }
        string ErrorMessage { set; }
    }
    public class ValidateAssignmentRecurringChargeEvaluatorContext : IValidateAssignmentRecurringChargeEvaluatorContext
    {
        public Account Account { set; get; }
        public DateTime PackageAssignmentStart { set; get; }
        public DateTime? PackageAssignmentEnd { set; get; }
        public bool IsValid { set; get; }
        public string ErrorMessage { get; set; }
    }
}
