using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public class RDBJoinSelectContext : RDBJoinStatementContext
    {
        RDBSelectQuery _selectQuery;

        internal RDBJoinSelectContext(RDBQueryBuilderContext queryBuilderContext, RDBJoin join, string tableAlias, RDBSelectQuery selectQuery)
            : base(queryBuilderContext, join, tableAlias)
        {
            _selectQuery = selectQuery;
        }

        public RDBSelectQuery SelectQuery()
        {
            return _selectQuery;
        }
    }
}
