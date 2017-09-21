using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public abstract class ServiceTypeExtendedSettings
    {
        public abstract Guid ConfigId { get; }

        public virtual List<GenericFieldDefinition> GetFieldDefinitions()
        {
            return null;
        }

        public virtual void ExportRates(IServiceTypeExtendedSettingsExportRatesContext context)
        {
            
        }
    }

    public interface IServiceTypeExtendedSettingsExportRatesContext
    {
        Guid ServiceTypeId { get; }

        DateTime EffectiveDate { get; }

        long AccountId { get; }

        int ChargingPolicyId { get; }

        ExportRuleData RateValueRuleData {  set; }

        ExportRuleData TariffRuleData {  set; }
    }

    public class ServiceTypeExtendedSettingsExportRatesContext : IServiceTypeExtendedSettingsExportRatesContext
    {
        public Guid ServiceTypeId { get; set; }

        public DateTime EffectiveDate { get; set; }

        public long AccountId { get; set; }

        public int ChargingPolicyId { get; set; }

        public ExportRuleData RateValueRuleData { get; set; }

        public ExportRuleData TariffRuleData { get; set; }
    }
}
