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
        RDBInsertMultipleRowsQueryRow _row;

        internal RDBInsertMultipleRowsQueryRowContext(RDBQueryBuilderContext queryBuilderContext, RDBInsertMultipleRowsQueryRow row)
        {
            this._queryBuilderContext = queryBuilderContext;
            this._row = row;
        }

        public RDBExpressionContext Value()
        {
            return new RDBExpressionContext(_queryBuilderContext, (expression) => this.Value(expression), null);
        }

        void Value(BaseRDBExpression value)
        {
            this._row.Values.Add(value);
        }
    }
}
