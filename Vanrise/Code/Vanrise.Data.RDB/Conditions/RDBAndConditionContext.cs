using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public class RDBAndConditionContext : RDBConditionContext
    {
        public RDBAndConditionContext(RDBQueryBuilderContext queryBuilderContext, Action<BaseRDBCondition> setCondition, string tableAlias)
            : base(queryBuilderContext, tableAlias)
        {
            var conditions = new List<BaseRDBCondition>();
            setCondition(new RDBAndCondition { Conditions = conditions });
            base.SetConditionAction = (condition) => conditions.Add(condition);
        }
    }

    public enum RDBGroupConditionType {  And = 0, Or = 1}
    public class RDBGroupConditionContext : RDBConditionContext
    {
        public RDBGroupConditionContext(RDBGroupConditionType groupConditionType, RDBQueryBuilderContext queryBuilderContext, Action<BaseRDBCondition> setCondition, string tableAlias)
            : base(queryBuilderContext, tableAlias)
        {
            var conditions = new List<BaseRDBCondition>();
            if (groupConditionType == RDBGroupConditionType.And)
                setCondition(new RDBAndCondition { Conditions = conditions });
            else
                setCondition(new RDBOrCondition { Conditions = conditions });
            base.SetConditionAction = (condition) => conditions.Add(condition);
        }
    }
}
