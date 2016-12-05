using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.Business
{
    public class InvoiceTypeConfigsManager
    {
        public IEnumerable<InvoiceGridActionSettingsConfig> GetInvoiceGridActionSettingsConfigs()
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<InvoiceGridActionSettingsConfig>(InvoiceGridActionSettingsConfig.EXTENSION_TYPE);
        }
        public IEnumerable<RDLCDataSourceSettingsConfig> GetRDLCDataSourceSettingsConfigs()
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<RDLCDataSourceSettingsConfig>(RDLCDataSourceSettingsConfig.EXTENSION_TYPE);
        }
        public IEnumerable<RDLCParameterSettingsConfig> GetRDLCParameterSettingsConfigs()
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<RDLCParameterSettingsConfig>(RDLCParameterSettingsConfig.EXTENSION_TYPE);
        }
        public IEnumerable<ItemsFilterConfig> GetItemsFilterConfigs()
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<ItemsFilterConfig>(ItemsFilterConfig.EXTENSION_TYPE);
        }
        public IEnumerable<InvoiceUISubSectionSettingsConfig> GetInvoiceUISubSectionSettingsConfigs()
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<InvoiceUISubSectionSettingsConfig>(InvoiceUISubSectionSettingsConfig.EXTENSION_TYPE);
        }

        public IEnumerable<InvoiceGridFilterConditionConfig> GetInvoiceGridFilterConditionConfigs()
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<InvoiceGridFilterConditionConfig>(InvoiceGridFilterConditionConfig.EXTENSION_TYPE);
        }
        public IEnumerable<InvoiceGeneratorFilterConditionConfig> GetInvoiceGeneratorFilterConditionConfigs()
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<InvoiceGeneratorFilterConditionConfig>(InvoiceGeneratorFilterConditionConfig.EXTENSION_TYPE);
        }


        public IEnumerable<InvoiceExtendedSettingsConfig> GetInvoiceExtendedSettingsConfigs()
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<InvoiceExtendedSettingsConfig>(InvoiceExtendedSettingsConfig.EXTENSION_TYPE);
        }
        public IEnumerable<BillingPeriodConfig> GetBillingPeriodTemplateConfigs()
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<BillingPeriodConfig>(BillingPeriodConfig.EXTENSION_TYPE);
        }
        public IEnumerable<StartDateCalculationMethodConfig> GetStartDateCalculationMethodConfigs()
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<StartDateCalculationMethodConfig>(StartDateCalculationMethodConfig.EXTENSION_TYPE);
        }
    }
}
