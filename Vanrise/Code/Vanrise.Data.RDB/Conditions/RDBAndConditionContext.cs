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

        public RDBAndConditionContext(T parent, Action<BaseRDBCondition> setCondition)
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
}
