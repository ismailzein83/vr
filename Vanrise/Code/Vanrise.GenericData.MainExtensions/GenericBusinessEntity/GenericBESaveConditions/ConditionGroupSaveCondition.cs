using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;

namespace Vanrise.GenericData.MainExtensions.GenericBusinessEntity.GenericBESaveConditions
{
    public enum LogicalOperator
    {
        [Description("AND")]
        And = 0,
        [Description("OR")]
        Or = 1,
    }
    public class ConditionGroupSaveCondition : GenericBESaveCondition
    {
        public override Guid ConfigId
        {
            get { return new Guid("C094566B-B9AE-4D49-9810-A7238503FC1C"); }
        }
        public LogicalOperator Operator { get; set; }
        public List<SaveConditionItem> Conditions { get; set; }
       
        public override bool IsMatch(IGenericBESaveConditionContext context)
        {
            if (this.Conditions != null)
            {
                bool result = false;
                foreach (var condition in this.Conditions)
                {
                    var genericBEConditionContext = new GenericBEConditionContext
                    {
                        DefinitionSettings = context.DefinitionSettings,
                        Entity = condition.ApplicableOnOldEntity ? context.OldEntity : context.NewEntity,
                    };
                    result = condition.Condition.IsMatch(genericBEConditionContext);
                    switch (this.Operator)
                    {
                        case LogicalOperator.And:
                            if (!result)
                                return false;
                            break;
                        case LogicalOperator.Or:
                            if (result)
                                return true;
                            break;
                    }
                }
            }
            return true;
        }
    }
    public class SaveConditionItem
    {
        public string Name { get; set; }
        public bool ApplicableOnOldEntity { get; set; }
        public GenericBECondition Condition { get; set; }
    }
}
