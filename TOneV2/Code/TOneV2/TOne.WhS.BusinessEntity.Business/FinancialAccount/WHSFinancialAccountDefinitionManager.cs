using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.GenericData.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class WHSFinancialAccountDefinitionManager
    {
        Vanrise.GenericData.Business.BusinessEntityDefinitionManager s_beDefinitionManager = new Vanrise.GenericData.Business.BusinessEntityDefinitionManager();

        public WHSFinancialAccountDefinitionSettings GetFinancialAccountDefinitionSettings(Guid financialAccountDefinitionId)
        {
            var beDefinition = GetAllFinancialAccountDefinitions().GetRecord(financialAccountDefinitionId);
            beDefinition.ThrowIfNull("beDefinition", financialAccountDefinitionId);
            return beDefinition.Settings.CastWithValidate<WHSFinancialAccountDefinitionSettings>("beDefinition.Settings", financialAccountDefinitionId);
        }

        public Dictionary<Guid, Vanrise.GenericData.Entities.BusinessEntityDefinition> GetAllFinancialAccountDefinitions()
        {
            return s_beDefinitionManager.GetBusinessEntityDefinitionsByConfigId(WHSFinancialAccountDefinitionSettings.S_CONFIGID);
        }

        public string GetFinancialAccountDefinitionName(Guid financialAccountDefinitionId)
        {
            return GetFinancialAccountDefinition(financialAccountDefinitionId).Name;
        }

        public Vanrise.GenericData.Entities.BusinessEntityDefinition GetFinancialAccountDefinition(Guid financialAccountDefinitionId)
        {
            var financialAccountDefinition = GetAllFinancialAccountDefinitions().GetRecord(financialAccountDefinitionId);
            financialAccountDefinition.ThrowIfNull("financialAccountDefinition", financialAccountDefinitionId);
            return financialAccountDefinition;
        }

        public IEnumerable<WHSFinancialAccountDefinitionConfig> GetFinancialAccountDefinitionsConfigs()
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<WHSFinancialAccountDefinitionConfig>(WHSFinancialAccountDefinitionConfig.EXTENSION_TYPE);
        }

        public IEnumerable<WHSFinancialAccountDefinitionInfo> GetFinancialAccountDefinitionInfo(WHSFinancialAccountDefinitionInfoFilter filter)
        {
            var financialAccountDefinitions = GetAllFinancialAccountDefinitions();
            Func<BusinessEntityDefinition, bool> filterExpression = (businessEntityDefinition) =>
            {
                if (filter != null)
                {
                    if (filter.Filters != null)
                    {
                        WHSFinancialAccountDefinitionFilterContext financialAccountDefinitionFilterContext = new WHSFinancialAccountDefinitionFilterContext
                        {
                            FinancialAccountDefinitionId = businessEntityDefinition.BusinessEntityDefinitionId
                        };
                        if (!filter.Filters.Any(x => x.IsMatched(financialAccountDefinitionFilterContext)))
                        {
                            return false;
                        }
                    }
                }
                return true;
            };

            return financialAccountDefinitions.MapRecords(FinancialAccountDefinitionInfoMapper, filterExpression);
        }

        public Guid? GetBalanceAccountTypeId(Guid financialAccountDefinitionId)
        {
            var financialAccountDefinitionSettings = GetFinancialAccountDefinitionSettings(financialAccountDefinitionId);
            return financialAccountDefinitionSettings.BalanceAccountTypeId;
        }

        public List<FinancialAccountInvoiceType> GetFinancialAccountInvoiceTypes(Guid financialAccountDefinitionId)
        {
            var financialAccountDefinitionSettings = GetFinancialAccountDefinitionSettings(financialAccountDefinitionId);
            return financialAccountDefinitionSettings.FinancialAccountInvoiceTypes;
        }

        private WHSFinancialAccountDefinitionInfo FinancialAccountDefinitionInfoMapper(BusinessEntityDefinition businessEntityDefinition)
        {
            var settings = businessEntityDefinition.Settings.CastWithValidate<WHSFinancialAccountDefinitionSettings>("beDefinition.Settings");
            return new WHSFinancialAccountDefinitionInfo
            {
                FinancialAccountDefinitionId = businessEntityDefinition.BusinessEntityDefinitionId,
                Name = businessEntityDefinition.Title,
                BalanceAccountTypeId = settings.BalanceAccountTypeId,
            };
        }
    }
}