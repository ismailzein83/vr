using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public class RDBDeleteQuery<T> : BaseRDBQuery, IRDBDeleteQuery<T>, IRDBDeleteQueryTableDefined<T>, IRDBDeleteQueryJoined<T>, IRDBDeleteQueryFiltered<T>
    {
        T _parent;
        RDBQueryBuilderContext _queryBuilderContext;
        string _tableAlias;

        public RDBDeleteQuery(T parent, RDBQueryBuilderContext queryBuilderContext)
        {
            _parent = parent;
            _queryBuilderContext = queryBuilderContext;
        }

        public IRDBTableQuerySource Table { get; private set; }

        public BaseRDBCondition Condition { get; private set; }

        public List<RDBJoin> Joins { get; private set; }

        public IRDBDeleteQueryTableDefined<T> FromTable(IRDBTableQuerySource table)
        {
            this.Table = table; 
            _queryBuilderContext.SetMainQueryTable(table);
            return this;
        }

        public IRDBDeleteQueryTableDefined<T> FromTable(string tableName)
        {
            return FromTable(new RDBTableDefinitionQuerySource(tableName));
        }

        public RDBJoinContext<IRDBDeleteQueryJoined<T>> Join(string tableAlias)
        {
            this._tableAlias = tableAlias;
            _queryBuilderContext.AddTableAlias(this.Table, tableAlias);
            this.Joins = new List<RDBJoin>();
            return new RDBJoinContext<IRDBDeleteQueryJoined<T>>(this, _queryBuilderContext, this.Joins);
        }

        public RDBConditionContext<IRDBDeleteQueryFiltered<T>> Where()
        {
            return new RDBConditionContext<IRDBDeleteQueryFiltered<T>>(this, _queryBuilderContext, (condition) => this.Condition = condition, _tableAlias);
        }

        public T EndDelete()
        {
            return _parent;
        }

        protected override RDBResolvedQuery GetResolvedQuery(IRDBQueryGetResolvedQueryContext context)
        {
            var resolveDeleteQueryContext = new RDBDataProviderResolveDeleteQueryContext(this.Table, this._tableAlias, this.Condition, this.Joins, context, _queryBuilderContext);
            return context.DataProvider.ResolveDeleteQuery(resolveDeleteQueryContext);
        }
    }

    public interface IRDBDeleteQuery<T> : IRDBDeleteQueryCanDefineTable<T>
    {

    }

    public interface IRDBDeleteQueryTableDefined<T> : IRDBDeleteQueryCanJoin<T>, IRDBDeleteQueryCanFilter<T>, IRDBDeleteQueryReady<T>
    {

    }

    public interface IRDBDeleteQueryJoined<T> : IRDBDeleteQueryCanFilter<T>, IRDBDeleteQueryReady<T>
    {
    }

    public interface IRDBDeleteQueryFiltered<T> : IRDBDeleteQueryReady<T>
    { 
    }

    public interface IRDBDeleteQueryReady<T>
    {
        T EndDelete();
    }

    public interface IRDBDeleteQueryCanDefineTable<T>
    {
        IRDBDeleteQueryTableDefined<T> FromTable(IRDBTableQuerySource table);

        IRDBDeleteQueryTableDefined<T> FromTable(string tableName);
    }

    public interface IRDBDeleteQueryCanJoin<T>
    {
        RDBJoinContext<IRDBDeleteQueryJoined<T>> Join(string tableAlias);
    }

    public interface IRDBDeleteQueryCanFilter<T>
    {
        RDBConditionContext<IRDBDeleteQueryFiltered<T>> Where();
    }
}
