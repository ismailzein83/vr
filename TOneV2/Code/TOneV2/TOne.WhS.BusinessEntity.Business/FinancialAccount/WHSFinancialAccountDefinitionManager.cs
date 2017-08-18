using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

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
    }
}
