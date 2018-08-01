using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public class RDBGroupByHavingAndConditionContext : RDBGroupByHavingContext
    {       
        public RDBGroupByHavingAndConditionContext( Action<BaseRDBCondition> setCondition, IRDBTableQuerySource table)
        {
            var conditions = new List<BaseRDBCondition>();
            base.SetConditionAction = (condition) => conditions.Add(condition);
            base.Table = table;
            setCondition(new RDBAndCondition { Conditions = conditions });
        }
    }
}
