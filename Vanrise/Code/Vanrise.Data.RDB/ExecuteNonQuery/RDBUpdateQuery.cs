using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.Data.RDB
{
    public class RDBUpdateQuery : BaseRDBQuery
    {
        RDBQueryBuilderContext _queryBuilderContext;
        string _notExistConditionTableAlias;
        string _tableAlias;

        internal RDBUpdateQuery(RDBQueryBuilderContext queryBuilderContext)
        {
            _queryBuilderContext = queryBuilderContext;
        }

        IRDBTableQuerySource _table;

        List<RDBUpdateColumn> _columnValues;

        RDBConditionGroup _conditionGroup;

        RDBConditionGroup _notExistConditionGroup;

        List<RDBJoin> _joins;

        public RDBUpdateQuery FromTable(string tableName)
        {
            return FromTable(new RDBTableDefinitionQuerySource(tableName));
        }

        public RDBUpdateQuery FromTable(IRDBTableQuerySource table)
        {
            this._table = table;
            _queryBuilderContext.SetMainQueryTable(table);
            this._columnValues = new List<RDBUpdateColumn>();
            return this;
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

        public RDBExpressionContext Column(string columnName)
        {
            return new RDBExpressionContext(_queryBuilderContext, (expression) => ColumnValue(columnName, expression), null);
        }

        public void ColumnValue(string columnName, BaseRDBExpression value)
        {
            this._columnValues.Add(new RDBUpdateColumn
            {
                ColumnName = columnName,
                Value = value
            });
        }

        //public void ColumnValue(string columnName, string value)
        //{
        //    this.ColumnValue(columnName, new RDBFixedTextExpression { Value = value });
        //}

        //public void ColumnValue(string columnName, int value)
        //{
        //    this.ColumnValue(columnName, new RDBFixedIntExpression { Value = value });
        //}

        //public void ColumnValue(string columnName, long value)
        //{
        //    this.ColumnValue(columnName, new RDBFixedLongExpression { Value = value });
        //}

        //public void ColumnValue(string columnName, decimal value)
        //{
        //    this.ColumnValue(columnName, new RDBFixedDecimalExpression { Value = value });
        //}

        //public void ColumnValue(string columnName, float value)
        //{
        //    this.ColumnValue(columnName, new RDBFixedFloatExpression { Value = value });
        //}

        //public void ColumnValue(string columnName, DateTime value)
        //{
        //    this.ColumnValue(columnName, new RDBFixedDateTimeExpression { Value = value });
        //}

        //public void ColumnValue(string columnName, bool value)
        //{
        //    this.ColumnValue(columnName, new RDBFixedBooleanExpression { Value = value });
        //}

        //public void ColumnValue(string columnName, Guid value)
        //{
        //    this.ColumnValue(columnName, new RDBFixedGuidExpression { Value = value });
        //}

        //public void ColumnValue(string columnName, byte[] value)
        //{
        //    this.ColumnValue(columnName, new RDBFixedBytesExpression { Value = value });
        //}

        RDBJoinContext _joinContext;

        public RDBJoinContext Join(string tableAlias)
        {
            if (_joinContext == null)
            {
                this._tableAlias = tableAlias;
                _queryBuilderContext.AddTableAlias(this._table, tableAlias);
                this._joins = new List<RDBJoin>();
                _joinContext = new RDBJoinContext(_queryBuilderContext, this._joins, _tableAlias);
            }
            return _joinContext;
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

        public override RDBResolvedQuery GetResolvedQuery(IRDBQueryGetResolvedQueryContext context)
        {
            if (this._notExistConditionGroup != null)
            {
                var selectQuery = new RDBSelectQuery(_queryBuilderContext.CreateChildContext());
                selectQuery.From(this._table, _notExistConditionTableAlias, 1);
                selectQuery.SelectColumns().Column(new RDBNullExpression(), "nullColumn");
                selectQuery.ConditionGroup = this._notExistConditionGroup;
                var rdbNotExistsCondition = new RDBNotExistsCondition()
                {
                    SelectQuery = selectQuery
                };
                var conditionGroup = new RDBConditionGroup(RDBConditionGroupOperator.AND);
                conditionGroup.Conditions.Add(rdbNotExistsCondition);
                IRDBQueryReady ifQuery = new RDBIfQuery(_queryBuilderContext.CreateChildContext())
                {
                    ConditionGroup = conditionGroup,
                    _trueQueryText = ResolveUpdateQuery(context).QueryText
                };
                return ifQuery.GetResolvedQuery(context);
            }
            else
            {
                return ResolveUpdateQuery(context);
            }
        }

        private RDBResolvedQuery ResolveUpdateQuery(IRDBQueryGetResolvedQueryContext context)
        {
            var resolveUpdateQueryContext = new RDBDataProviderResolveUpdateQueryContext(this._table, this._tableAlias, this._columnValues, this._conditionGroup, this._joins, context, _queryBuilderContext);
            return context.DataProvider.ResolveUpdateQuery(resolveUpdateQueryContext);
        }
    }

    public class RDBUpdateColumn
    {
        public string ColumnName { get; set; }

        public BaseRDBExpression Value { get; set; }
    }
}