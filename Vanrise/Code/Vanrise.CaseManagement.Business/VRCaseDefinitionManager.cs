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
    public class VRCaseDefinitionManager
    {
        public BusinessEntityDefinition GetVRCaseDefinition(Guid vrCaseDefinitionId)
        {
            BusinessEntityDefinitionManager manager = new BusinessEntityDefinitionManager();
            return manager.GetBusinessEntityDefinition(vrCaseDefinitionId);
        }
        public string GetVRCaseDefinitionName(Guid vrCaseDefinitionId)
        {
            var vrCaseDefinition = GetVRCaseDefinition(vrCaseDefinitionId);
            vrCaseDefinition.ThrowIfNull("vrCaseDefinition", vrCaseDefinitionId);
            return vrCaseDefinition.Name;
        }
        public VRCaseBEDefinitionSettings GetVRCaseDefinitionSettings(Guid vrCaseDefinitionId)
        {
            var vrCaseDefinition = GetVRCaseDefinition(vrCaseDefinitionId);
            vrCaseDefinition.ThrowIfNull("vrCaseDefinition", vrCaseDefinitionId);
            vrCaseDefinition.Settings.ThrowIfNull("vrCaseDefinition.Settings");
            return vrCaseDefinition.Settings.CastWithValidate<VRCaseBEDefinitionSettings>("vrCaseDefinition.Settings");
        }
        public VRCaseGridDefinition GetVRCaseGridDefinition(Guid vrCaseDefinitionId)
        {
            var vrCaseDefinitionSettings = GetVRCaseDefinitionSettings(vrCaseDefinitionId);
            vrCaseDefinitionSettings.ThrowIfNull("vrCaseDefinitionSettings", vrCaseDefinitionId);
            return vrCaseDefinitionSettings.GridDefinition;
        }

    }
}
