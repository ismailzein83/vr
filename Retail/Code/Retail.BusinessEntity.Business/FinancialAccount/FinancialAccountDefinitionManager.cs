using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace Retail.BusinessEntity.Business
{
    public class FinancialAccountDefinitionManager
    {
        static Vanrise.Common.Business.VRComponentTypeManager s_componentTypeManager = new Vanrise.Common.Business.VRComponentTypeManager();
        public FinancialAccountDefinitionSettings GetFinancialAccountDefinitionSettings(Guid financialAccountDefinitionId)
        {
            var financialAccountDefinitionSettings = s_componentTypeManager.GetComponentTypeSettings<FinancialAccountDefinitionSettings>(financialAccountDefinitionId);
            financialAccountDefinitionSettings.ThrowIfNull("financialAccountDefinitionSettings");
            return financialAccountDefinitionSettings;
        }
        public Guid? GetBalanceAccountTypeId(Guid financialAccountDefinitionId)
        {
            var financialAccountDefinitionSettings =  GetFinancialAccountDefinitionSettings(financialAccountDefinitionId);
            return financialAccountDefinitionSettings.BalanceAccountTypeId;
        }

        public IEnumerable<FinancialAccountDefinitionInfo> GetFinancialAccountDefinitionsInfo(FinancialAccountDefinitionInfoFilter financialAccountDefinitionFilter)
        {
            Func<FinancialAccountDefinition, bool> filterExpression = (financialAccountDefinition) =>
            {
                if (financialAccountDefinitionFilter == null)
                    return true;
                if (financialAccountDefinition.Settings.AccountBEDefinitionId != financialAccountDefinitionFilter.AccountBEDefinitionId)
                    return false;
                if (financialAccountDefinitionFilter.Filters != null)
                {
                    var FinancialAccountDefinitionFilterContext = new FinancialAccountDefinitionFilterContext
                        {
                            FinancialAccountDefinitionId = financialAccountDefinition.VRComponentTypeId,
                            DefinitionSettings = financialAccountDefinition.Settings,
                        };
                    foreach (var filter in financialAccountDefinitionFilter.Filters)
                    {

                        if (!filter.IsMatched(FinancialAccountDefinitionFilterContext))
                            return false;
                    }
                }
                return true;
            };
            return s_componentTypeManager.GetComponentTypes<FinancialAccountDefinitionSettings, FinancialAccountDefinition>().MapRecords(FinancialAccountDefinitionInfoMapper,filterExpression);
        }
        public string GetFinancialAccountDefinitionName(Guid financialAccountDefinitionId)
        {
           var financialAccountDefinition = GetFinancialAccountDefinition(financialAccountDefinitionId);
           return financialAccountDefinition != null ? financialAccountDefinition.Name : null;
        }
        public FinancialAccountDefinition GetFinancialAccountDefinition(Guid financialAccountDefinitionId)
        {
            return s_componentTypeManager.GetComponentType<FinancialAccountDefinitionSettings, FinancialAccountDefinition>(financialAccountDefinitionId);
        }
        public IEnumerable<FinancialAccountDefinitionConfig> GetFinancialAccountDefinitionsConfigs()
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<FinancialAccountDefinitionConfig>(FinancialAccountDefinitionConfig.EXTENSION_TYPE);
        }
     
        #region Mappers
        private FinancialAccountDefinitionInfo FinancialAccountDefinitionInfoMapper(FinancialAccountDefinition financialAccountDefinition)
        {
            return new FinancialAccountDefinitionInfo
            {
                FinancialAccountDefinitionId = financialAccountDefinition.VRComponentTypeId,
                Name = financialAccountDefinition.Name
            };
        }

        #endregion
    }
}
