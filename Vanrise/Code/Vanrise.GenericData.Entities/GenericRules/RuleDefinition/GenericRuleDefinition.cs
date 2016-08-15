using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Security.Entities;

namespace Vanrise.GenericData.Entities
{
    public class GenericRuleDefinition
    {
        public int GenericRuleDefinitionId { get; set; }

        public string Name { get; set; }

        public GenericRuleDefinitionCriteria CriteriaDefinition { get; set; }

        public VRObjectVariableCollection Objects { get; set; }

        public GenericRuleDefinitionSettings SettingsDefinition { get; set; }

        public GenericRuleDefinitionSecurity Security { get; set; }
    }

    public class GenericRuleDefinitionSecurity
    {
        public RequiredPermissionSettings ViewRequiredPermission { get; set; } 
        public RequiredPermissionSettings AddRequiredPermission { get; set; }
        public RequiredPermissionSettings EditRequiredPermission { get; set; }

    }
}
