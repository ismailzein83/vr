using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public class RDBOrConditionContext<T> : RDBConditionContext<RDBOrConditionContext<T>>
    {
        T _parent;
        Action<BaseRDBCondition> _setCondition;
        List<BaseRDBCondition> _conditions;

        public RDBOrConditionContext(T parent, RDBQueryBuilderContext queryBuilderContext, Action<BaseRDBCondition> setCondition, string tableAlias)
            : base(queryBuilderContext, tableAlias)
        {
            _parent = parent;
            _setCondition = setCondition;
            _conditions = new List<BaseRDBCondition>();
            base.Parent = this;
            base.SetConditionAction = (condition) => _conditions.Add(condition);
        }

        public T EndOr()
        {
            _setCondition(new RDBOrCondition { Conditions = _conditions });
            return _parent;
        }
    }
}
