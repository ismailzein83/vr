using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common;
using System.ComponentModel;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Business
{
    public enum GenericRuleVRActionAuditActionType
    {
        [Description(ConstVRActionAuditActionTypes.GETFILTERED)]
        GetFiltered,
        [Description(ConstVRActionAuditActionTypes.GETITEM)]
        GetItem,
        [Description(ConstVRActionAuditActionTypes.ADD)]
        Add,
        [Description(ConstVRActionAuditActionTypes.UPDATE)]
        Update,
        [Description(ConstVRActionAuditActionTypes.DELETE)]
        Delete
    }
    public class GenericRuleVRActionAuditAttribute : VRActionAuditAttribute
    {
        public GenericRuleVRActionAuditAttribute(GenericRuleVRActionAuditActionType actionType)
        {
            this.ActionType = actionType;
        }

        public GenericRuleVRActionAuditActionType ActionType { get; set; }


        static GenericRuleDefinitionManager s_ruleDefinitionManager = new GenericRuleDefinitionManager();

        public override void GetAuditDetails(IVRActionAuditAttributeContext context)
        {
            context.ModuleName = "Rules";
            int? ruleId;
            Guid ruleDefinitionId = GetDefinitionId(context, out ruleId);
            string ruleName = s_ruleDefinitionManager.GetGenericRuleDefinitionName(ruleDefinitionId);
            if (!ruleName.Contains("Rules"))
                ruleName = String.Format("{0} Rules", ruleName);
            context.EntityName = ruleName;
            context.ActionName = Utilities.GetEnumDescription<GenericRuleVRActionAuditActionType>(this.ActionType);
            if (ruleId.HasValue)
            {
                context.ObjectId = ruleId.Value.ToString();
            }
        }

        Guid GetDefinitionId(IVRActionAuditAttributeContext context, out int? ruleId)
        {
            ruleId = null;
            switch (this.ActionType)
            {
                case GenericRuleVRActionAuditActionType.GetFiltered:
                    Vanrise.Entities.DataRetrievalInput<GenericRuleQuery> input = context.GetActionArgument<Vanrise.Entities.DataRetrievalInput<GenericRuleQuery>>("input");
                    return input.Query.RuleDefinitionId;
                case GenericRuleVRActionAuditActionType.GetItem:
                    ruleId = context.GetActionArgument<int>("ruleId");
                    return context.GetActionArgument<Guid>("ruleDefinitionId");
                case GenericRuleVRActionAuditActionType.Add:
                case GenericRuleVRActionAuditActionType.Update:
                    GenericRule rule = context.GetActionArgument<GenericRule>("rule");
                    ruleId = rule.RuleId;
                    return rule.DefinitionId;
                default:
                    throw new NotSupportedException(String.Format("Action Type '{0}'", this.ActionType));
            }
        }
    }
}