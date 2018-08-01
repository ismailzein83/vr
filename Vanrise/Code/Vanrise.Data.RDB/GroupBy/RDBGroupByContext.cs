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

        internal RDBGroupByContext(RDBQueryBuilderContext queryBuilderContext, RDBGroupBy groupBy, IRDBTableQuerySource table, string tableAlias)
        {
            _queryBuilderContext = queryBuilderContext;
            _groupBy = groupBy;
            _table = table;
            _tableAlias = tableAlias;
        }

        RDBSelectColumnsContext _selectColumnsContext;
        public RDBSelectColumnsContext Select()
        {
            if (_selectColumnsContext != null)
            {
                _groupBy.Columns = new List<RDBSelectColumn>();
                _selectColumnsContext = new RDBSelectColumnsContext(_queryBuilderContext, _groupBy.Columns, _table, _tableAlias);
            }
            return _selectColumnsContext;
        }

        RDBSelectAggregateContext _selectAggregatesContext;
        public RDBSelectAggregateContext SelectAggregates()
        {
            if (_selectAggregatesContext == null)
            {
                _groupBy.AggregateColumns = new List<RDBSelectColumn>();
                _selectAggregatesContext = new RDBSelectAggregateContext(_groupBy.AggregateColumns, _table, _tableAlias);
            }
            return _selectAggregatesContext;
        }

        RDBGroupByHavingContext _havingContext;
        public RDBGroupByHavingContext Having(RDBConditionGroupOperator conditionGroupOperator = RDBConditionGroupOperator.AND)
        {
            if (_havingContext == null)
            {
                var conditionGroup = new RDBConditionGroup(conditionGroupOperator);
                _groupBy.HavingCondition = conditionGroup;
                _havingContext = new RDBGroupByHavingContext(conditionGroup, _table, _tableAlias);
            }
            return _havingContext;
        }
    }
}
