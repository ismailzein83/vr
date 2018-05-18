using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public class RDBSortContext<T> : IRDBSortContextReady<T>
    {
        T _parent;
        List<RDBSelectSortColumn> _columns;
        IRDBTableQuerySource _table;
        string _tableAlias;

        public RDBSortContext(T parent, List<RDBSelectSortColumn> columns, IRDBTableQuerySource table, string tableAlias)
        {
            _parent = parent;
            _columns = columns;
            _table = table;
            _tableAlias = tableAlias;
        }

        public IRDBSortContextReady<T> ByExpression(BaseRDBExpression expression, RDBSortDirection direction)
        {
            _columns.Add(new RDBSelectSortColumn
                {
                    Expression = expression,
                    SortDirection = direction
                });
            return this;
        }

        public IRDBSortContextReady<T> ByColumn(string tableAlias, string columnName, RDBSortDirection direction)
        {
            return ByExpression(new RDBColumnExpression { TableAlias = tableAlias, ColumnName = columnName }, direction);
        }

        public IRDBSortContextReady<T> ByColumn(string columnName, RDBSortDirection direction)
        {
            return ByColumn(this._tableAlias, columnName, direction);
        }

        public IRDBSortContextReady<T> ByAlias(string alias, RDBSortDirection direction)
        {
            _columns.Add(new RDBSelectSortColumn
            {
                ColumnAlias = alias,
                SortDirection = direction
            });
            return this;
        }

        public T EndSort()
        {
            return _parent;
        }
    }

    public interface IRDBSortContextReady<T>
    {
        IRDBSortContextReady<T> ByExpression(BaseRDBExpression expression, RDBSortDirection direction);

        IRDBSortContextReady<T> ByColumn(string tableAlias, string columnName, RDBSortDirection direction);

        IRDBSortContextReady<T> ByColumn(string columnName, RDBSortDirection direction);

        IRDBSortContextReady<T> ByAlias(string alias, RDBSortDirection direction);

        T EndSort();
    }
}