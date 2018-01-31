using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public class RDBGroupByHavingAndConditionContext<T> : RDBGroupByHavingContext<RDBGroupByHavingAndConditionContext<T>>
    {
        T _parent;
        Action<BaseRDBCondition> _setCondition;
        List<BaseRDBCondition> _conditions;

        public RDBGroupByHavingAndConditionContext(T parent, Action<BaseRDBCondition> setCondition, IRDBTableQuerySource table)
        {
            _parent = parent;
            _setCondition = setCondition;
            _conditions = new List<BaseRDBCondition>();
            base.Parent = this;
            base.SetConditionAction = (condition) => _conditions.Add(condition);
            base.Table = table;
        }

        public T EndAnd()
        {
            _setCondition(new RDBAndCondition { Conditions = _conditions });
            return _parent;
        }
    }
}
