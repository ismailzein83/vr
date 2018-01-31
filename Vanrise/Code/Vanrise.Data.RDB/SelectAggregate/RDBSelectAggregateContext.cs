using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public enum RDBNonCountAggregateType { SUM = 0, AVG = 1, MAX = 2, MIN = 3 }
    public class RDBSelectAggregateContext<T> : IRDBSelectAggregateContextReady<T>
    {
        T _parent;
        List<RDBSelectColumn> _columns;
        IRDBTableQuerySource _table;

        public RDBSelectAggregateContext(T parent, List<RDBSelectColumn> columns, IRDBTableQuerySource table)
        {
            _parent = parent;
            _columns = columns;
            _table = table;
        }

        IRDBSelectAggregateContextReady<T> Column(BaseRDBExpression expression, string alias)
        {
            _columns.Add(new RDBSelectColumn
            {
                Expression = expression,
                Alias = alias
            });
            return this;
        }

        public IRDBSelectAggregateContextReady<T> Count(string alias)
        {
            return Column(new RDBCountExpression(), alias);
        }

        public IRDBSelectAggregateContextReady<T> Aggregate(RDBNonCountAggregateType aggregateType, BaseRDBExpression expression, string alias)
        {
            BaseRDBExpression aggregateExpression = CreateNonCountAggregate(aggregateType, expression);
            return Column(aggregateExpression, alias);
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

        public IRDBSelectAggregateContextReady<T> Aggregate(RDBNonCountAggregateType aggregateType, IRDBTableQuerySource table, string columnName, string alias)
        {
            return Aggregate(aggregateType, new RDBColumnExpression { Table = table, ColumnName = columnName }, alias);
        }

        public IRDBSelectAggregateContextReady<T> Aggregate(RDBNonCountAggregateType aggregateType, string tableName, string columnName, string alias)
        {
            return Aggregate(aggregateType, new RDBTableDefinitionQuerySource(tableName), columnName, alias);
        }

        public IRDBSelectAggregateContextReady<T> Aggregate(RDBNonCountAggregateType aggregateType, string columnName, string alias)
        {
            return Aggregate(aggregateType, _table, columnName, alias);
        }

        public IRDBSelectAggregateContextReady<T> Aggregate(RDBNonCountAggregateType aggregateType, string columnName)
        {
            return Aggregate(aggregateType, columnName, columnName);
        }

        public T EndSelectAggregates()
        {
            return _parent;
        }
    }

    public interface IRDBSelectAggregateContextReady<T>
    {
        IRDBSelectAggregateContextReady<T> Count(string alias);

        IRDBSelectAggregateContextReady<T> Aggregate(RDBNonCountAggregateType aggregateType, BaseRDBExpression expression, string alias);

        IRDBSelectAggregateContextReady<T> Aggregate(RDBNonCountAggregateType aggregateType, IRDBTableQuerySource table, string columnName, string alias);

        IRDBSelectAggregateContextReady<T> Aggregate(RDBNonCountAggregateType aggregateType, string tableName, string columnName, string alias);

        IRDBSelectAggregateContextReady<T> Aggregate(RDBNonCountAggregateType aggregateType, string columnName, string alias);

        IRDBSelectAggregateContextReady<T> Aggregate(RDBNonCountAggregateType aggregateType, string columnName);

        T EndSelectAggregates();
    }
}