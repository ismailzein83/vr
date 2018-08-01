using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public class RDBOrConditionContext : RDBConditionContext
    {       

        public RDBOrConditionContext(RDBQueryBuilderContext queryBuilderContext, Action<BaseRDBCondition> setCondition, string tableAlias)
            : base(queryBuilderContext, tableAlias)
        {
            var conditions = new List<BaseRDBCondition>();
            setCondition(new RDBOrCondition { Conditions = conditions });
            base.SetConditionAction = (condition) => conditions.Add(condition);
        }
    }
}
