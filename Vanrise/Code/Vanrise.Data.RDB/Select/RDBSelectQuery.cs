using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public class RDBSelectQuery<T> : BaseRDBQuery, IRDBSelectQuery<T>, ISelectQueryTableDefined<T>, ISelectQueryJoined<T>, ISelectQueryColumnsSelected<T>, ISelectQueryFiltered<T>, ISelectQueryGroupByDefined<T>, ISelectQueryAggregateColumnsSelected<T>, ISelectQuerySortDefined<T>, IRDBTableQuerySource
    {
         T _parent;

         internal T Parent
         {
             set
             {
                 _parent = value;
             }
         }
         RDBQueryBuilderContext _queryBuilderContext;

        public RDBSelectQuery(T parent, RDBQueryBuilderContext queryBuilderContext)
        {
            _parent = parent;
            _queryBuilderContext = queryBuilderContext;
        }

        public RDBSelectQuery(RDBQueryBuilderContext queryBuilderContext)            
        {
            _queryBuilderContext = queryBuilderContext;
        }

        private IRDBTableQuerySource _table;

        private string _tableAlias;

        private long? _nbOfRecords;

        private List<RDBSelectColumn> _columns;

        private List<RDBJoin> _joins;

        private BaseRDBCondition _condition;

        private RDBGroupBy _groupBy;

        private List<RDBSelectSortColumn> _sortColumns;

        public ISelectQueryTableDefined<T> FromNoTable()
        {

            return this;
        }

        public ISelectQueryTableDefined<T> From(IRDBTableQuerySource table, string tableAlias, long? nbOfRecords)
        {
            this._table = table;
            this._tableAlias = tableAlias;
            _queryBuilderContext.AddTableAlias(table, tableAlias);
            this._nbOfRecords = nbOfRecords;
            return this;
        }

        public ISelectQueryTableDefined<T> From(IRDBTableQuerySource table, string tableAlias)
        {
            return From(table, tableAlias, null);
        }

        public ISelectQueryTableDefined<T> From(string tableName, string tableAlias, long? nbOfRecords)
        {
            return From(new RDBTableDefinitionQuerySource(tableName), tableAlias, nbOfRecords);
        }
        public ISelectQueryTableDefined<T> From(string tableName, string tableAlias)
        {
            return From(tableName, tableAlias, null);
        }

        public IRDBSelectQuery<ISelectQueryTableDefined<T>> FromSelect(string inlineQueryAlias, long? nbOfRecords)
        {
            var selectQuery = new RDBSelectQuery<ISelectQueryTableDefined<T>>(this, _queryBuilderContext.CreateChildContext());
            From(selectQuery, inlineQueryAlias, nbOfRecords);
            return selectQuery;
        }

        public IRDBSelectQuery<ISelectQueryTableDefined<T>> FromSelect(string inlineQueryAlias)
        {
            return FromSelect(inlineQueryAlias, null);
        }

        public RDBJoinContext<ISelectQueryJoined<T>> Join()
            {
                this._joins = new List<RDBJoin>();
                return new RDBJoinContext<ISelectQueryJoined<T>>(this, _queryBuilderContext, this._joins);
            }
        

        public RDBSelectColumnsContext<ISelectQueryColumnsSelected<T>> SelectColumns()
            {
                this._columns = new List<RDBSelectColumn>();
                return new RDBSelectColumnsContext<ISelectQueryColumnsSelected<T>>(this, _queryBuilderContext, this._columns, this._table, this._tableAlias);
            }
        

        public RDBSelectAggregateContext<ISelectQueryAggregateColumnsSelected<T>> SelectAggregates()
            {
                this._columns = new List<RDBSelectColumn>();
                return new RDBSelectAggregateContext<ISelectQueryAggregateColumnsSelected<T>>(this, this._columns, this._table, this._tableAlias);
            }
        

        public RDBConditionContext<ISelectQueryFiltered<T>> Where()
            {
                return new RDBConditionContext<ISelectQueryFiltered<T>>(this, (condition) => this._condition = condition, this._tableAlias);
            }

        public RDBGroupByContext<ISelectQueryGroupByDefined<T>> GroupBy()
            {
                this._groupBy = new RDBGroupBy { Columns = new List<RDBSelectColumn>(), AggregateColumns = new List<RDBSelectColumn>() };
                return new RDBGroupByContext<ISelectQueryGroupByDefined<T>>(this, _queryBuilderContext, this._groupBy, this._table, this._tableAlias);
            }

        RDBSortContext<ISelectQuerySortDefined<T>> ISelectQueryCanSort<T>.Sort()
        {
            this._sortColumns = new List<RDBSelectSortColumn>();
            return new RDBSortContext<ISelectQuerySortDefined<T>>(this, this._sortColumns, this._table, this._tableAlias);
        }

        public string ToDBQuery(IRDBTableQuerySourceToDBQueryContext context)
        {
            RDBQueryGetResolvedQueryContext getResolvedQueryContext = new RDBQueryGetResolvedQueryContext(context);
            return string.Concat(" (", GetResolvedQuery(getResolvedQueryContext).QueryText, ") ");
        }

        #region Private Classes


        #endregion

        public string GetDescription()
        {
            return String.Format("Inline Query on table '{0}'", this._table.GetDescription());
        }

        public string GetDBColumnName(IRDBTableQuerySourceGetDBColumnNameContext context)
        {
            return context.ColumnName;
        }

        string _queryAsTableSourceUniqueName = Guid.NewGuid().ToString();
        public string GetUniqueName()
        {
            return _queryAsTableSourceUniqueName;
        }

        protected override RDBResolvedQuery GetResolvedQuery(IRDBQueryGetResolvedQueryContext context)
        {
            var resolveSelectQueryContext = new RDBDataProviderResolveSelectQueryContext(this._table, this._tableAlias, this._nbOfRecords, this._columns, this._joins,
             this._condition, this._groupBy, this._sortColumns, context, _queryBuilderContext);
            return context.DataProvider.ResolveSelectQuery(resolveSelectQueryContext);
        }

        public T EndSelect()
        {
            return _parent;
        }


        public void GetIdColumnInfo(IRDBTableQuerySourceGetIdColumnInfoContext context)
        {
            throw new NotImplementedException();
        }
    }

    public class RDBSelectColumn
    {
        public BaseRDBExpression Expression { get; set; }

        public string Alias { get; set; }

        public string SetDBParameterName { get; set; }
    }

    public enum RDBSortDirection { ASC = 0, DESC = 1 }

    public class RDBSelectSortColumn
    {
        /// <summary>
        /// either Expression or ColumnAlias should be filled
        /// </summary>
        public BaseRDBExpression Expression { get; set; }

        public string ColumnAlias { get; set; }

        public RDBSortDirection SortDirection { get; set; }
    }

    public interface ISelectQueryReady<T> : IRDBQueryReady
    {
        T EndSelect();
    }

    public interface IRDBSelectQuery<T> : ISelectQueryCanDefineTable<T>
    {

    }

    public interface ISelectQueryTableDefined<T> : ISelectQueryCanSelectColumns<T>, ISelectQueryCanFilter<T>, ISelectQueryCanGroupBy<T>, ISelectQueryCanJoin<T>, ISelectQueryCanSelectAggregate<T>
    {

    }

    public interface ISelectQueryJoined<T> : ISelectQueryCanFilter<T>, ISelectQueryCanSelectColumns<T>, ISelectQueryCanGroupBy<T>, ISelectQueryCanSelectAggregate<T>
    {
        
    }

    public interface ISelectQueryColumnsSelected<T> : ISelectQueryReady<T>, ISelectQueryCanSort<T>
    {
       
    }

    public interface ISelectQueryFiltered<T> : ISelectQueryCanSelectColumns<T>, ISelectQueryCanGroupBy<T>, ISelectQueryCanSelectAggregate<T>
    {

    }

    public interface ISelectQueryGroupByDefined<T> : ISelectQueryReady<T>, ISelectQueryCanSort<T>
    {
    }

    public interface ISelectQuerySortDefined<T> : ISelectQueryReady<T>
    {
    }

    public interface ISelectQueryAggregateColumnsSelected<T> : ISelectQueryReady<T>, ISelectQueryCanSort<T>
    {

    }

    public interface ISelectQueryCanDefineTable<T>
    {
        ISelectQueryTableDefined<T> FromNoTable();

        ISelectQueryTableDefined<T> From(IRDBTableQuerySource table, string tableAlias, long? nbOfRecords);

        ISelectQueryTableDefined<T> From(IRDBTableQuerySource table, string tableAlias);

        ISelectQueryTableDefined<T> From(string tableName, string tableAlias, long? nbOfRecords);

        ISelectQueryTableDefined<T> From(string tableName, string tableAlias);

        IRDBSelectQuery<ISelectQueryTableDefined<T>> FromSelect(string inlineQueryAlias, long? nbOfRecords);

        IRDBSelectQuery<ISelectQueryTableDefined<T>> FromSelect(string inlineQueryAlias);
    }
    public interface ISelectQueryCanSelectColumns<T>
    {
        RDBSelectColumnsContext<ISelectQueryColumnsSelected<T>> SelectColumns();
    }

    public interface ISelectQueryCanFilter<T>
    {
        RDBConditionContext<ISelectQueryFiltered<T>> Where();
    }

    public interface ISelectQueryCanGroupBy<T>
    {
        RDBGroupByContext<ISelectQueryGroupByDefined<T>> GroupBy();
    }

    public interface ISelectQueryCanJoin<T>
    {
        RDBJoinContext<ISelectQueryJoined<T>> Join();
    }

    public interface ISelectQueryCanSort<T>
    {
        RDBSortContext<ISelectQuerySortDefined<T>> Sort();
    }

    public interface ISelectQueryCanSelectAggregate<T>
    {
        RDBSelectAggregateContext<ISelectQueryAggregateColumnsSelected<T>> SelectAggregates();
    }
}
