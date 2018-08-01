using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public enum RDBNonCountAggregateType { SUM = 0, AVG = 1, MAX = 2, MIN = 3 }
    public class RDBSelectAggregateContext
    {
        List<RDBSelectColumn> _columns;
        IRDBTableQuerySource _table;
        string _tableAlias;

        public RDBSelectAggregateContext(List<RDBSelectColumn> columns, IRDBTableQuerySource table, string tableAlias)
        {
            _columns = columns;
            _table = table;
            _tableAlias = tableAlias;
        }

        void Column(BaseRDBExpression expression, string alias)
        {
            _columns.Add(new RDBSelectColumn
            {
                Expression = expression,
                Alias = alias
            });
        }

        public void Count(string alias)
        {
            Column(new RDBCountExpression(), alias);
        }

        public void Aggregate(RDBNonCountAggregateType aggregateType, BaseRDBExpression expression, string alias)
        {
            BaseRDBExpression aggregateExpression = CreateNonCountAggregate(aggregateType, expression);
            Column(aggregateExpression, alias);
        }

        public static BaseRDBExpression CreateNonCountAggregate(RDBNonCountAggregateType aggregateType, BaseRDBExpression expression)
        {
            BaseRDBExpression aggregateExpression;
            switch (aggregateType)
            {
                case RDBNonCountAggregateType.SUM: aggregateExpression = new RDBSumExpression { Expression = expression }; break;
                case RDBNonCountAggregateType.AVG: aggregateExpression = new RDBAvgExpression { Expression = expression }; break;
                case RDBNonCountAggregateType.MAX: aggregateExpression = new RDBMaxExpression { Expression = expression }; break;
                case RDBNonCountAggregateType.MIN: aggregateExpression = new RDBMinExpression { Expression = expression }; break;
                default: throw new NotSupportedException(String.Format("aggregateType '{0}'", aggregateType.ToString()));
            }
            return aggregateExpression;
        }

        public void Aggregate(RDBNonCountAggregateType aggregateType, string tableAlias, string columnName, string alias)
        {
             Aggregate(aggregateType, new RDBColumnExpression { TableAlias = tableAlias, ColumnName = columnName }, alias);
        }

        public void Aggregate(RDBNonCountAggregateType aggregateType, string columnName, string alias)
        {
             Aggregate(aggregateType, _tableAlias, columnName, alias);
        }

        public void Aggregate(RDBNonCountAggregateType aggregateType, string columnName)
        {
             Aggregate(aggregateType, columnName, columnName);
        }
    }
}