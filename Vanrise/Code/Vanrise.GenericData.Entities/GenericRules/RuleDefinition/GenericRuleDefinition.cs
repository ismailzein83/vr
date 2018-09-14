using System;
using Vanrise.Entities;
using Vanrise.Security.Entities;

namespace Vanrise.GenericData.Entities
{
    public enum RuleDefinitionType
    {
        /// <summary>
        /// CompositeConditionDefinition is null
        /// </summary>
        RuleTreeCriteraia = 0,
        /// <summary>
        /// CriteriaDefinition and Objects are null
        /// </summary>
        CompositeCondition = 1
    }

    public class GenericRuleDefinition
    {
        public Guid GenericRuleDefinitionId { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }

        public RuleDefinitionType RuleDefinitionType { get; set; }

        public GenericRuleDefinitionCriteria CriteriaDefinition { get; set; }

        public VRObjectVariableCollection Objects { get; set; }

        //public CompositeRecordConditionDefinition CompositeConditionDefinition { get; set; }

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