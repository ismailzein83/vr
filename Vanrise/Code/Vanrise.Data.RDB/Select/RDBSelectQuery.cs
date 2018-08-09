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

        internal RDBSelectQuery(RDBQueryBuilderContext queryBuilderContext)
        {
            _queryBuilderContext = queryBuilderContext;
        }

        IRDBTableQuerySource _table;

        string _tableAlias;

        long? _nbOfRecords;

        List<RDBSelectColumn> _columns;

        internal List<RDBSelectColumn> Columns
        {
            get
            {
                return _columns;
            }
        }

        List<RDBJoin> _joins;

        RDBConditionGroup _conditionGroup;

        internal RDBConditionGroup ConditionGroup
        {
            set
            {
                _conditionGroup = value;
            }
        }

        RDBGroupBy _groupBy;

        List<RDBSelectSortColumn> _sortColumns;

        bool _withNoLock;

        public void From(IRDBTableQuerySource table, string tableAlias, long? nbOfRecords, bool withNoLock = false)
        {
            this._table = table; 
            _queryBuilderContext.SetMainQueryTable(table);
            this._tableAlias = tableAlias;
            _queryBuilderContext.AddTableAlias(table, tableAlias);
            this._nbOfRecords = nbOfRecords;
            this._withNoLock = withNoLock;
        }

        public void From(IRDBTableQuerySource table, string tableAlias)
        {
            From(table, tableAlias, null);
        }

        public void From(string tableName, string tableAlias, long? nbOfRecords, bool withNoLock = false)
        {
            From(new RDBTableDefinitionQuerySource(tableName), tableAlias, nbOfRecords, withNoLock);
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
                _joinContext = new RDBJoinContext(_queryBuilderContext, this._joins, _tableAlias);
            }
            return _joinContext;
        }


        RDBSelectColumnsContext _selectColumnsContext;
        public RDBSelectColumnsContext SelectColumns()
        {
            if (_selectColumnsContext == null)
            {
                if (this._columns == null)
                    this._columns = new List<RDBSelectColumn>();
                _selectColumnsContext = new RDBSelectColumnsContext(_queryBuilderContext, this._columns, this._table, this._tableAlias);
            }
            return _selectColumnsContext;
        }

        RDBSelectAggregateContext _selectAggregateContext;
        public RDBSelectAggregateContext SelectAggregates()
        {
            if (_selectAggregateContext == null)
            {
                if (this._columns == null)
                    this._columns = new List<RDBSelectColumn>();
                _selectAggregateContext = new RDBSelectAggregateContext(this._columns, this._table, this._tableAlias);
            }
            return _selectAggregateContext;
        }

        RDBConditionContext _conditionContext;
        public RDBConditionContext Where(RDBConditionGroupOperator groupOperator = RDBConditionGroupOperator.AND)
        {
            if (_conditionContext == null)
            {
                _conditionGroup = new RDBConditionGroup(groupOperator);
                _conditionContext = new RDBConditionContext(_queryBuilderContext, _conditionGroup, this._tableAlias);
            }
            else
            {
                if (_conditionGroup.Operator != groupOperator)
                    throw new Exception("Where method is called multipe times with different values of groupOperator");
            }
            return _conditionContext;
        }

        RDBGroupByContext _groupByContext;
        public RDBGroupByContext GroupBy()
        {
            if (_groupByContext == null)
            {
                this._groupBy = new RDBGroupBy { Columns = new List<RDBSelectColumn>(), AggregateColumns = new List<RDBSelectColumn>() };
                _groupByContext = new RDBGroupByContext(_queryBuilderContext, this._groupBy, this._table, this._tableAlias);
            }
            return _groupByContext;
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
             this._conditionGroup, this._groupBy, this._sortColumns, context, _queryBuilderContext);
            return context.DataProvider.ResolveSelectQuery(resolveSelectQueryContext);
        }


        public void GetIdColumnInfo(IRDBTableQuerySourceGetIdColumnInfoContext context)
        {
            throw new NotImplementedException();
        }


        public List<string> GetColumnNames(IRDBTableQuerySourceGetColumnNamesContext context)
        {
            return _columns.Select(col => col.Alias).ToList();
        }


        public void GetCreatedAndModifiedTime(IRDBTableQuerySourceGetCreatedAndModifiedTimeContext context)
        {
            
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
