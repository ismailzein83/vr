using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public class RDBAndConditionContext<T> : RDBConditionContext<RDBAndConditionContext<T>>
    {
        T _parent;
        Action<BaseRDBCondition> _setCondition;
        List<BaseRDBCondition> _conditions;

        public RDBAndConditionContext(T parent, RDBQueryBuilderContext queryBuilderContext, Action<BaseRDBCondition> setCondition, string tableAlias)
            : base(queryBuilderContext, tableAlias)
        {
            _parent = parent;
            _setCondition = setCondition;
            _conditions = new List<BaseRDBCondition>();
            base.Parent = this;
            base.SetConditionAction = (condition) => _conditions.Add(condition);
        }

        public T EndAnd()
        {
            _setCondition(new RDBAndCondition { Conditions = _conditions });
            return _parent;
        }
    }

    public enum RDBGroupConditionType {  And = 0, Or = 1}
    public class RDBGroupConditionContext<T> : RDBConditionContext<RDBGroupConditionContext<T>>
    {
        T _parent;
        RDBGroupConditionType _groupConditionType;
        Action<BaseRDBCondition> _setCondition;
        List<BaseRDBCondition> _conditions;

        public RDBGroupConditionContext(T parent, RDBGroupConditionType groupConditionType, RDBQueryBuilderContext queryBuilderContext, Action<BaseRDBCondition> setCondition, string tableAlias)
            : base(queryBuilderContext, tableAlias)
        {
            _parent = parent;
            _groupConditionType = groupConditionType;
            _setCondition = setCondition;
            _conditions = new List<BaseRDBCondition>();
            base.Parent = this;
            base.SetConditionAction = (condition) => _conditions.Add(condition);
        }

        public T EndConditionGroup()
        {
            if (_groupConditionType == RDBGroupConditionType.And)
                _setCondition(new RDBAndCondition { Conditions = _conditions });
            else
                _setCondition(new RDBOrCondition { Conditions = _conditions });
            return _parent;
        }
    }
}
