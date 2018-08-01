using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public class RDBSelectQuery : BaseRDBQuery, IRDBTableQuerySource
    {
        RDBQueryBuilderContext _queryBuilderContext;

        public RDBSelectQuery(RDBQueryBuilderContext queryBuilderContext)
        {
            _queryBuilderContext = queryBuilderContext;
        }

        IRDBTableQuerySource _table;

        string _tableAlias;

        long? _nbOfRecords;

        List<RDBSelectColumn> _columns;

        List<RDBJoin> _joins;

        BaseRDBCondition _condition;

        RDBConditionGroup _conditionGroup;

        RDBGroupBy _groupBy;

        List<RDBSelectSortColumn> _sortColumns;
        
        public void From(IRDBTableQuerySource table, string tableAlias, long? nbOfRecords)
        {
            this._table = table; 
            _queryBuilderContext.SetMainQueryTable(table);
            this._tableAlias = tableAlias;
            _queryBuilderContext.AddTableAlias(table, tableAlias);
            this._nbOfRecords = nbOfRecords;
        }

        public void From(IRDBTableQuerySource table, string tableAlias)
        {
            From(table, tableAlias, null);
        }

        public void From(string tableName, string tableAlias, long? nbOfRecords)
        {
            From(new RDBTableDefinitionQuerySource(tableName), tableAlias, nbOfRecords);
        }
        public void From(string tableName, string tableAlias)
        {
            From(tableName, tableAlias, null);
        }

        public RDBSelectQuery FromSelect(string inlineQueryAlias, long? nbOfRecords)
        {
            var selectQuery = new RDBSelectQuery(_queryBuilderContext.CreateChildContext());
            From(selectQuery, inlineQueryAlias, nbOfRecords);
            return selectQuery;
        }

        public RDBSelectQuery FromSelect(string inlineQueryAlias)
        {
            return FromSelect(inlineQueryAlias, null);
        }

        RDBJoinContext _joinContext;
        public RDBJoinContext Join()
            {
                if (_joinContext == null)
                {
                    this._joins = new List<RDBJoin>();
                    _joinContext = new RDBJoinContext(_queryBuilderContext, this._joins);
                }
                return _joinContext;
            }


        RDBSelectColumnsContext _selectColumnsContext;
        public RDBSelectColumnsContext SelectColumns()
        {
            if (_selectColumnsContext == null)
            {
                this._columns = new List<RDBSelectColumn>();
                _selectColumnsContext = new RDBSelectColumnsContext(_queryBuilderContext, this._columns, this._table, this._tableAlias);
            }
            return _selectColumnsContext;
        }
        

        public RDBSelectAggregateContext SelectAggregates()
            {
                this._columns = new List<RDBSelectColumn>();
                return new RDBSelectAggregateContext(this._columns, this._table, this._tableAlias);
            }

        RDBConditionContext _conditionContext;
        public RDBConditionContext Where()
        {
            if (_conditionContext == null)
                _conditionContext = new RDBConditionContext(_queryBuilderContext, (condition) => this._condition = condition, this._tableAlias);
            return _conditionContext;
        }

        public RDBGroupByContext GroupBy()
            {
                this._groupBy = new RDBGroupBy { Columns = new List<RDBSelectColumn>(), AggregateColumns = new List<RDBSelectColumn>() };
                return new RDBGroupByContext(_queryBuilderContext, this._groupBy, this._table, this._tableAlias);
            }

        RDBSortContext _sortContext;
        public RDBSortContext Sort()
        {
            if (_sortContext == null)
            {
                this._sortColumns = new List<RDBSelectSortColumn>();
                _sortContext = new RDBSortContext(this._sortColumns, this._table, this._tableAlias);
            }
            return _sortContext;
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

        public override RDBResolvedQuery GetResolvedQuery(IRDBQueryGetResolvedQueryContext context)
        {
            var resolveSelectQueryContext = new RDBDataProviderResolveSelectQueryContext(this._table, this._tableAlias, this._nbOfRecords, this._columns, this._joins,
             this._condition, this._groupBy, this._sortColumns, context, _queryBuilderContext);
            return context.DataProvider.ResolveSelectQuery(resolveSelectQueryContext);
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
}
