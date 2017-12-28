using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
namespace Vanrise.CaseManagement.Business
{
    public class CaseManagementDefinitionManager
    {
        public BusinessEntityDefinition GetCaseManagementDefinition(Guid caseManagementDefinitionId)
        {
            BusinessEntityDefinitionManager manager = new BusinessEntityDefinitionManager();
            return manager.GetBusinessEntityDefinition(caseManagementDefinitionId);
        }
        public string GetCaseManagementDefinitionName(Guid caseManagementDefinitionId)
        {
            var caseManagementDefinition = GetCaseManagementDefinition(caseManagementDefinitionId);
            caseManagementDefinition.ThrowIfNull("caseManagementDefinition", caseManagementDefinitionId);
            return caseManagementDefinition.Name;
        }
        public CaseManagementBEDefinitionSettings GetCaseManagementDefinitionSettings(Guid caseManagementDefinitionId)
        {
            var caseManagementDefinition = GetCaseManagementDefinition(caseManagementDefinitionId);
            caseManagementDefinition.ThrowIfNull("caseManagementDefinition", caseManagementDefinitionId);
            caseManagementDefinition.Settings.ThrowIfNull("caseManagementDefinition.Settings");
            return caseManagementDefinition.Settings.CastWithValidate<CaseManagementBEDefinitionSettings>("caseManagementDefinition.Settings");
        }

    }
}
