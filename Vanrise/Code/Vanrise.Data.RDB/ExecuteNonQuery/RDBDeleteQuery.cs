using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public class RDBDeleteQuery : BaseRDBQuery
    {
        RDBQueryBuilderContext _queryBuilderContext;
        string _tableAlias;

        public RDBDeleteQuery(RDBQueryBuilderContext queryBuilderContext)
        {
            _queryBuilderContext = queryBuilderContext;
        }

        public IRDBTableQuerySource Table { get; private set; }

        public BaseRDBCondition Condition { get; private set; }

        public List<RDBJoin> Joins { get; private set; }

        public RDBDeleteQuery FromTable(IRDBTableQuerySource table)
        {
            this.Table = table; 
            _queryBuilderContext.SetMainQueryTable(table);
            return this;
        }

        public RDBDeleteQuery FromTable(string tableName)
        {
            return FromTable(new RDBTableDefinitionQuerySource(tableName));
        }

        public RDBJoinContext Join(string tableAlias)
        {
            this._tableAlias = tableAlias;
            _queryBuilderContext.AddTableAlias(this.Table, tableAlias);
            this.Joins = new List<RDBJoin>();
            return new RDBJoinContext(_queryBuilderContext, this.Joins);
        }

        public RDBConditionContext Where()
        {
            return new RDBConditionContext(_queryBuilderContext, (condition) => this.Condition = condition, _tableAlias);
        }

        public override RDBResolvedQuery GetResolvedQuery(IRDBQueryGetResolvedQueryContext context)
        {
            var resolveDeleteQueryContext = new RDBDataProviderResolveDeleteQueryContext(this.Table, this._tableAlias, this.Condition, this.Joins, context, _queryBuilderContext);
            return context.DataProvider.ResolveDeleteQuery(resolveDeleteQueryContext);
        }
    }
}
