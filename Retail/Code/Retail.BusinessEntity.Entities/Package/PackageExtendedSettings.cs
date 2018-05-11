using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public abstract class PackageExtendedSettings
    {
        //public abstract Guid ConfigId { get; }

        public virtual PackageExtendedSettingsEditorRuntime GetEditorRuntime()
        {
            return null;
        }

        public virtual void OnPackageAssignmentStarted(IPackageSettingAssignementStartedContext context)
        {

        }

        public virtual void OnPackageAssignmentExpired(IPackageSettingAssignementExpiredContext context)
        {

        }
        public abstract void ValidatePackageAssignment(IPackageSettingAssignementValidateContext context);

        public virtual bool CanAssignPackage(IPackageSettingsCanAssignPackageContext context)
        {
            return true;
        }

        public virtual void ExportRates(IPackageSettingsExportRatesContext context)
        {

        }
    }

    public abstract class PackageExtendedSettingsEditorRuntime
    {

    }

    public interface IPackageSettingsExportRatesContext
    {
        DateTime EffectiveDate { get; }

        long AccountId { get; }

        Guid ServiceTypeId { get; }

        bool IsFinalPricingPackage { set; }

        ExportRuleData RateValueRuleData { set; }

        ExportRuleData TariffRuleData { set; }
    }

    public class PackageSettingsExportRatesContext : IPackageSettingsExportRatesContext
    {
        public DateTime EffectiveDate { get; set; }

        public long AccountId { get; set; }

        public Guid ServiceTypeId { get; set; }

        public bool IsFinalPricingPackage { get; set; }

        public ExportRuleData RateValueRuleData { get; set; }

        public ExportRuleData TariffRuleData { get; set; }
    }

    public class ExportRuleData
    {
        public List<string> Headers { get; set; }
        
        public List<object[]> Data { get; set; }
    }

    public interface IPackageSettingsCanAssignPackageContext
    {
        Package Package { get; }

        Guid AccountDefinitionId { get; }

        long AccountId { get; }
    }

    public interface IPackageSettingAssignementStartedContext
    {
        long AccountId { get; }

        Account Account { get; }

        DateTime BED { get; }

        DateTime? EED { get; }
    }

    public interface IPackageSettingAssignementExpiredContext
    {
        long AccountId { get; }

        Account Account { get; }

        DateTime EED { get; }
    }

    public interface IPackageSettingAssignementValidateContext
    {
        long AccountId { get; }

        Account Account { get; }

        DateTime BED { get; }

        DateTime? EED { get; }
        bool IsValid { set; }
        string ErrorMessage { set; }
    }
}
