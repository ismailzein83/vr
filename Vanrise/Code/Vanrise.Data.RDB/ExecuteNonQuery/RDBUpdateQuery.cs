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

        public RDBUpdateQuery(RDBQueryBuilderContext queryBuilderContext)
        {
            _queryBuilderContext = queryBuilderContext;
        }

        IRDBTableQuerySource _table;

        List<RDBUpdateColumn> _columnValues;

        BaseRDBCondition _condition;

        BaseRDBCondition _notExistCondition;

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

        public RDBConditionContext IfNotExists(string tableAlias)
        {
            this._notExistConditionTableAlias = tableAlias;
            return new RDBConditionContext(_queryBuilderContext, (condition) => this._notExistCondition = condition, this._notExistConditionTableAlias);
        }

        public void ColumnValue(string columnName, BaseRDBExpression value)
        {
            this._columnValues.Add(new RDBUpdateColumn
            {
                ColumnName = columnName,
                Value = value
            });
        }

        public void ColumnValue(string columnName, string value)
        {
            this.ColumnValue(columnName, new RDBFixedTextExpression { Value = value });
        }

        public void ColumnValue(string columnName, int value)
        {
            this.ColumnValue(columnName, new RDBFixedIntExpression { Value = value });
        }

        public void ColumnValue(string columnName, long value)
        {
            this.ColumnValue(columnName, new RDBFixedLongExpression { Value = value });
        }

        public void ColumnValue(string columnName, decimal value)
        {
            this.ColumnValue(columnName, new RDBFixedDecimalExpression { Value = value });
        }

        public void ColumnValue(string columnName, float value)
        {
            this.ColumnValue(columnName, new RDBFixedFloatExpression { Value = value });
        }

        public void ColumnValue(string columnName, DateTime value)
        {
            this.ColumnValue(columnName, new RDBFixedDateTimeExpression { Value = value });
        }

        public void ColumnValue(string columnName, bool value)
        {
            this.ColumnValue(columnName, new RDBFixedBooleanExpression { Value = value });
        }

        public void ColumnValue(string columnName, Guid value)
        {
            this.ColumnValue(columnName, new RDBFixedGuidExpression { Value = value });
        }

        public void ColumnValue(string columnName, byte[] value)
        {
            this.ColumnValue(columnName, new RDBFixedBytesExpression { Value = value });
        }

        public RDBJoinContext Join(string tableAlias)
        {
            this._tableAlias = tableAlias;
            _queryBuilderContext.AddTableAlias(this._table, tableAlias);
            this._joins = new List<RDBJoin>();
            return new RDBJoinContext(_queryBuilderContext, this._joins);
        }

        public RDBConditionContext Where()
        {
            return new RDBConditionContext(_queryBuilderContext, (condition) => this._condition = condition, _tableAlias);
        }

        public override RDBResolvedQuery GetResolvedQuery(IRDBQueryGetResolvedQueryContext context)
        {
            if (this._notExistCondition != null)
            {
                var selectQuery = new RDBSelectQuery(_queryBuilderContext.CreateChildContext());
                selectQuery.From(this._table, _notExistConditionTableAlias, 1);
                selectQuery.SelectColumns().Column(new RDBNullExpression(), "nullColumn");
                selectQuery.Where().Condition(this._notExistCondition);
                var rdbNotExistsCondition = new RDBNotExistsCondition()
                {
                    SelectQuery = selectQuery
                };
                IRDBQueryReady ifQuery = new RDBIfQuery(_queryBuilderContext.CreateChildContext())
                {
                    Condition = rdbNotExistsCondition,
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
            var resolveUpdateQueryContext = new RDBDataProviderResolveUpdateQueryContext(this._table, this._tableAlias, this._columnValues, this._condition, this._joins, context, _queryBuilderContext);
            return context.DataProvider.ResolveUpdateQuery(resolveUpdateQueryContext);
        }
    }

    public class RDBUpdateColumn
    {
        public string ColumnName { get; set; }

        public BaseRDBExpression Value { get; set; }
    }
}