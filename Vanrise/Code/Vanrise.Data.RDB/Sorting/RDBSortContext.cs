using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public class RDBSortContext
    {
        List<RDBSelectSortColumn> _columns;
        IRDBTableQuerySource _table;
        string _tableAlias;

        public RDBSortContext(List<RDBSelectSortColumn> columns, IRDBTableQuerySource table, string tableAlias)
        {
            _columns = columns;
            _table = table;
            _tableAlias = tableAlias;
        }

        public RDBSortContext ByExpression(BaseRDBExpression expression, RDBSortDirection direction)
        {
            _columns.Add(new RDBSelectSortColumn
                {
                    Expression = expression,
                    SortDirection = direction
                });
            return this;
        }

        public RDBSortContext ByColumn(string tableAlias, string columnName, RDBSortDirection direction)
        {
            return ByExpression(new RDBColumnExpression { TableAlias = tableAlias, ColumnName = columnName }, direction);
        }

        public RDBSortContext ByColumn(string columnName, RDBSortDirection direction)
        {
            return ByColumn(this._tableAlias, columnName, direction);
        }

        public RDBSortContext ByAlias(string alias, RDBSortDirection direction)
        {
            _columns.Add(new RDBSelectSortColumn
            {
                ColumnAlias = alias,
                SortDirection = direction
            });
            return this;
        }
    }
}