using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.Data.RDB
{
    public class RDBInsertQuery : BaseRDBQuery
    {
        RDBQueryBuilderContext _queryBuilderContext;

        string _notExistConditionTableAlias;

        internal RDBInsertQuery(RDBQueryBuilderContext queryBuilderContext)
        {
            _queryBuilderContext = queryBuilderContext;
        }

        IRDBTableQuerySource _table;

        List<RDBInsertColumn> _columnValues;

        RDBConditionGroup _notExistConditionGroup;

        bool _addSelectGeneratedId;

        RDBSelectQuery _selectQuery;

        public void IntoTable(IRDBTableQuerySource table)
        {
            this._table = table;
            _queryBuilderContext.SetMainQueryTable(table);
            this._columnValues = new List<RDBInsertColumn>();
        }

        public void IntoTable(string tableName)
        {
            IntoTable(new RDBTableDefinitionQuerySource(tableName));
        }

        RDBConditionContext _notExistsConditionContext;
        public RDBConditionContext IfNotExists(string tableAlias, RDBConditionGroupOperator groupOperator = RDBConditionGroupOperator.AND)
        {
            this._notExistConditionTableAlias = tableAlias;
            if(_notExistsConditionContext == null)
            {
                _notExistConditionGroup = new RDBConditionGroup(groupOperator);
                _notExistsConditionContext = new RDBConditionContext(_queryBuilderContext, _notExistConditionGroup, this._notExistConditionTableAlias);
            }
            else
            {
                if (_notExistConditionGroup.Operator != groupOperator)
                    throw new Exception("IfNotExists method is called multipe times with different values of groupOperator");
                if(this._notExistConditionTableAlias != tableAlias)
                    throw new Exception("IfNotExists method is called multipe times with different values of tableAlias");
            }
            return _notExistsConditionContext;
        }

        public void AddSelectGeneratedId()
        {
            _addSelectGeneratedId = true;
        }

        public RDBExpressionContext Column(string columnName)
        {
            return new RDBExpressionContext(_queryBuilderContext, (expression) => ColumnValue(columnName, expression), null);
        }
        
        public void ColumnValue(string columnName, BaseRDBExpression value)
        {
            this._columnValues.Add(new RDBInsertColumn
            {
                ColumnName = columnName,
                Value = value
            });
        }

        //public void ColumnValue(string columnName, string value)
        //{
        //     this.ColumnValue(columnName, new RDBFixedTextExpression { Value = value });
        //}

        //public void ColumnValue(string columnName, int value)
        //{
        //     this.ColumnValue(columnName, new RDBFixedIntExpression { Value = value });
        //}

        //public void ColumnValue(string columnName, long value)
        //{
        //     this.ColumnValue(columnName, new RDBFixedLongExpression { Value = value });
        //}

        //public void ColumnValue(string columnName, decimal value)
        //{
        //     this.ColumnValue(columnName, new RDBFixedDecimalExpression { Value = value });
        //}

        //public void ColumnValue(string columnName, float value)
        //{
        //     this.ColumnValue(columnName, new RDBFixedFloatExpression { Value = value });
        //}

        //public void ColumnValue(string columnName, DateTime value)
        //{
        //     this.ColumnValue(columnName, new RDBFixedDateTimeExpression { Value = value });
        //}

        //public void ColumnValue(string columnName, Guid value)
        //{
        //    this.ColumnValue(columnName, new RDBFixedGuidExpression { Value = value });
        //}

        //public void ColumnValue(string columnName, bool value)
        //{
        //     this.ColumnValue(columnName, new RDBFixedBooleanExpression { Value = value });
        //}

        //public void ColumnValue(string columnName, byte[] value)
        //{
        //     this.ColumnValue(columnName, new RDBFixedBytesExpression { Value = value });
        //}

        public RDBSelectQuery FromSelect()
        {
            _selectQuery = new RDBSelectQuery(_queryBuilderContext.CreateChildContext(), false);
            return _selectQuery;
        }

        public override RDBResolvedQuery GetResolvedQuery(IRDBQueryGetResolvedQueryContext context)
        {
            AddCreatedAndModifiedTimeIfNeeded(context);

            if (_notExistConditionGroup != null)
            {
                var notExistsSelectQuery = this.FromSelect().Where().NotExistsCondition();
                notExistsSelectQuery.From(this._table, _notExistConditionTableAlias, 1);
                notExistsSelectQuery.SelectColumns().Expression("nullColumn").Null();
                notExistsSelectQuery.Where().Condition(this._notExistConditionGroup);
            }

            if(this._selectQuery != null && this._columnValues != null)
            {
                var selectColumns = this._selectQuery.SelectColumns();
                foreach (var colVal in this._columnValues)
                {
                    selectColumns.Expression(colVal.ColumnName).Expression(colVal.Value);
                }
                this._columnValues = null;
            }

            var resolveInsertQueryContext = new RDBDataProviderResolveInsertQueryContext(this._table, this._columnValues, this._selectQuery, _addSelectGeneratedId, context, _queryBuilderContext);
            RDBResolvedQuery resolvedQuery = context.DataProvider.ResolveInsertQuery(resolveInsertQueryContext);

            return resolvedQuery;
        }

        private void AddCreatedAndModifiedTimeIfNeeded(IRDBQueryGetResolvedQueryContext context)
        {
            var getCreatedAndModifiedTimeContext = new RDBTableQuerySourceGetCreatedAndModifiedTimeContext(context);
            this._table.GetCreatedAndModifiedTime(getCreatedAndModifiedTimeContext);
            bool addCreatedTime = false;
            bool addModifiedTime = false;
            if (!String.IsNullOrEmpty(getCreatedAndModifiedTimeContext.CreatedTimeColumnName))
                addCreatedTime = true;
            if (!String.IsNullOrEmpty(getCreatedAndModifiedTimeContext.ModifiedTimeColumnName))
                addModifiedTime = true;
            if (addCreatedTime || addModifiedTime)
            {
                if (_columnValues != null && _columnValues.Count > 0)
                {
                    foreach (var colVal in _columnValues)
                    {
                        if (colVal.ColumnName == getCreatedAndModifiedTimeContext.CreatedTimeColumnName)
                        {
                            addCreatedTime = false;
                            if (!addModifiedTime)
                                break;
                            else
                                continue;
                        }
                        if (colVal.ColumnName == getCreatedAndModifiedTimeContext.ModifiedTimeColumnName)
                        {
                            addModifiedTime = false;
                            if (!addCreatedTime)
                                break;
                            else
                                continue;
                        }
                    }
                    if (addCreatedTime)
                        this.Column(getCreatedAndModifiedTimeContext.CreatedTimeColumnName).DateNow();
                    if (addModifiedTime)
                        this.Column(getCreatedAndModifiedTimeContext.ModifiedTimeColumnName).DateNow();
                }
                else if (this._selectQuery != null)
                {
                    foreach (var colVal in this._selectQuery.Columns)
                    {
                        if (colVal.Alias == getCreatedAndModifiedTimeContext.CreatedTimeColumnName)
                        {
                            addCreatedTime = false;
                            if (!addModifiedTime)
                                break;
                            else
                                continue;
                        }
                        if (colVal.Alias == getCreatedAndModifiedTimeContext.ModifiedTimeColumnName)
                        {
                            addModifiedTime = false;
                            if (!addCreatedTime)
                                break;
                            else
                                continue;
                        }
                    }
                    var selectColumns = this._selectQuery.SelectColumns();
                    if (addCreatedTime)
                        selectColumns.Expression(getCreatedAndModifiedTimeContext.CreatedTimeColumnName).DateNow();
                    if (addModifiedTime)
                        selectColumns.Expression(getCreatedAndModifiedTimeContext.ModifiedTimeColumnName).DateNow();
                }
                else
                {
                    throw new NullReferenceException("ColumnValues and SelectQuery are both null");
                }
            }
        }
    }

    public class RDBInsertColumn
    {
        public string ColumnName { get; set; }

        public BaseRDBExpression Value { get; set; }
    }
}