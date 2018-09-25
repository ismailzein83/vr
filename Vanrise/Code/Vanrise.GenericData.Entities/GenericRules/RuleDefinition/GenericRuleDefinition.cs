using System;
using Vanrise.Entities;
using Vanrise.Security.Entities;

namespace Vanrise.GenericData.Entities
{
    public class GenericRuleDefinition
    {
        public Guid GenericRuleDefinitionId { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }

        public VRObjectVariableCollection Objects { get; set; }

        public CriteriaDefinition CriteriaDefinition { get; set; }

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