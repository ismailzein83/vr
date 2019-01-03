using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public class RDBSortContext
    {
        RDBQueryBuilderContext _queryBuilderContext;
        List<RDBSelectSortColumn> _columns;
        IRDBTableQuerySource _table;
        string _tableAlias;

        internal RDBSortContext(RDBQueryBuilderContext queryBuilderContext, List<RDBSelectSortColumn> columns, IRDBTableQuerySource table, string tableAlias)
        {
            _queryBuilderContext = queryBuilderContext;
            _columns = columns;
            _table = table;
            _tableAlias = tableAlias;
        }

        public RDBExpressionContext ByExpression(RDBSortDirection direction)
        {
            return new RDBExpressionContext(this._queryBuilderContext, (exp) => ByExpression(exp, direction), this._tableAlias);
        }

        public void ByExpression(BaseRDBExpression expression, RDBSortDirection direction)
        {
            _columns.Add(new RDBSelectSortColumn
                {
                    Expression = expression,
                    SortDirection = direction
                });
        }

        public void ByColumn(string tableAlias, string columnName, RDBSortDirection direction)
        {
            ByExpression(new RDBColumnExpression { TableAlias = tableAlias, ColumnName = columnName }, direction);
        }

        public void ByColumn(string columnName, RDBSortDirection direction)
        {
            ByColumn(this._tableAlias, columnName, direction);
        }

        public void ByAlias(string alias, RDBSortDirection direction)
        {
            _columns.Add(new RDBSelectSortColumn
            {
                ColumnAlias = alias,
                SortDirection = direction
            });
        }
    }
}