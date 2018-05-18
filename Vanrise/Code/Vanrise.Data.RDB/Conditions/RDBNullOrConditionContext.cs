﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public class RDBNullOrConditionContext<T> : RDBConditionContext<T>
    {
        public RDBNullOrConditionContext(T parent, Action<BaseRDBCondition> setCondition, string tableAlias, string columnName)
        {
            base.Parent = parent;
            base.TableAlias = tableAlias;
            base.SetConditionAction = (condition) =>
            {
                var conditions = new List<BaseRDBCondition>();
                conditions.Add(condition);
                RDBNullCondition nullCondition = new RDBNullCondition { Expression = new RDBColumnExpression { TableAlias = tableAlias, ColumnName = columnName } };
                conditions.Add(nullCondition);
                setCondition(new RDBOrCondition { Conditions = conditions });
            };
        }
    }
}
