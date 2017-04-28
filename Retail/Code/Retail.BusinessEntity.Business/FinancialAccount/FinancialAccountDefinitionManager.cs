using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

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
        public IEnumerable<FinancialAccountDefinitionInfo> GetFinancialAccountDefinitionsInfo(FinancialAccountDefinitionInfoFilter financialAccountDefinitionFilter)
        {
            Func<FinancialAccountDefinition, bool> filterExpression = (financialAccountDefinition) =>
            {
                if (financialAccountDefinitionFilter == null)
                    return true;
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
