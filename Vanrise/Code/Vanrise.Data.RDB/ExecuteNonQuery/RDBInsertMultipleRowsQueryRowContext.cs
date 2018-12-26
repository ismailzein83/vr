using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public class RDBInsertMultipleRowsQueryRowContext
    {
        RDBQueryBuilderContext _queryBuilderContext;
        HashSet<string> _columns;
        RDBInsertMultipleRowsQueryRow _row;

        internal RDBInsertMultipleRowsQueryRowContext(RDBQueryBuilderContext queryBuilderContext, HashSet<string> columns, RDBInsertMultipleRowsQueryRow row)
        {
            this._columns = columns;
            this._queryBuilderContext = queryBuilderContext;
            this._row = row;
        }


        public RDBExpressionContext Column(string columnName)
        {
            return new RDBExpressionContext(_queryBuilderContext, (expression) => ColumnValue(columnName, expression), null);
        }

        public void ColumnValue(string columnName, BaseRDBExpression value)
        {
            _columns.Add(columnName);
            this._row.ColumnValues.Add(new RDBInsertColumn
            {
                ColumnName = columnName,
                Value = value
            });
        }
    }
}
