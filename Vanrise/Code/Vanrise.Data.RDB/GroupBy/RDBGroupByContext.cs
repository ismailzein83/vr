using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public class RDBGroupByContext
    {
        RDBQueryBuilderContext _queryBuilderContext;
        RDBGroupBy _groupBy;
        IRDBTableQuerySource _table;
        string _tableAlias;

        public RDBGroupByContext(RDBQueryBuilderContext queryBuilderContext, RDBGroupBy groupBy, IRDBTableQuerySource table, string tableAlias)
        {
            _queryBuilderContext = queryBuilderContext;
            _groupBy = groupBy;
            _table = table;
            _tableAlias = tableAlias;
        }

        public RDBSelectColumnsContext Select()
            {
                return new RDBSelectColumnsContext(_queryBuilderContext, _groupBy.Columns, _table, _tableAlias);
            }
        

       public RDBSelectAggregateContext SelectAggregates()
            {
                return new RDBSelectAggregateContext(_groupBy.AggregateColumns, _table, _tableAlias);
            }
        

        public RDBGroupByHavingContext Having()
            {
                return new RDBGroupByHavingContext((condition) => _groupBy.HavingCondition = condition, _table, _tableAlias);
            }
    }
}
