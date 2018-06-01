using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.Data.RDB
{
    public class RDBUpdateQuery<T> : BaseRDBQuery, IRDBUpdateQuery<T>, IRDBUpdateQueryTableDefined<T>, IRDBUpdateQueryFiltered<T>, IRDBUpdateQueryColumnsAssigned<T>, IRDBUpdateQueryNotExistsConditionDefined<T>, IRDBUpdateQueryJoined<T>
    {
        T _parent;
        RDBQueryBuilderContext _queryBuilderContext;
        string _notExistConditionTableAlias;
        string _tableAlias;

        public RDBUpdateQuery(T parent, RDBQueryBuilderContext queryBuilderContext)
        {
            _parent = parent;
            _queryBuilderContext = queryBuilderContext;
        }

        public IRDBTableQuerySource Table { get; set; }

        public List<RDBUpdateColumn> ColumnValues { get; private set; }

        public BaseRDBCondition Condition { get; private set; }

        public BaseRDBCondition NotExistCondition { get; private set; }

        public List<RDBJoin> Joins
        {
            get;
            private set;
        }

        public IRDBUpdateQueryTableDefined<T> FromTable(string tableName)
        {
            return FromTable(new RDBTableDefinitionQuerySource(tableName));
        }

        public IRDBUpdateQueryTableDefined<T> FromTable(IRDBTableQuerySource table)
        {
            this.Table = table;
            _queryBuilderContext.SetMainQueryTable(table);
            this.ColumnValues = new List<RDBUpdateColumn>();
            return this;
        }

        public RDBConditionContext<IRDBUpdateQueryNotExistsConditionDefined<T>> IfNotExists(string tableAlias)
        {
            this._notExistConditionTableAlias = tableAlias;
            return new RDBConditionContext<IRDBUpdateQueryNotExistsConditionDefined<T>>(this, (condition) => this.NotExistCondition = condition, this._notExistConditionTableAlias);
        }
        
        public IRDBUpdateQueryColumnsAssigned<T> ColumnValue(string columnName, BaseRDBExpression value)
        {
            this.ColumnValues.Add(new RDBUpdateColumn
            {
                ColumnName = columnName,
                Value = value
            });
            return this;
        }

        public IRDBUpdateQueryColumnsAssigned<T> ColumnValue(string columnName, string value)
        {
            return this.ColumnValue(columnName, new RDBFixedTextExpression { Value = value });
        }

        public IRDBUpdateQueryColumnsAssigned<T> ColumnValue(string columnName, int value)
        {
            return this.ColumnValue(columnName, new RDBFixedIntExpression { Value = value });
        }

        public IRDBUpdateQueryColumnsAssigned<T> ColumnValue(string columnName, long value)
        {
            return this.ColumnValue(columnName, new RDBFixedLongExpression { Value = value });
        }

        public IRDBUpdateQueryColumnsAssigned<T> ColumnValue(string columnName, decimal value)
        {
            return this.ColumnValue(columnName, new RDBFixedDecimalExpression { Value = value });
        }

        public IRDBUpdateQueryColumnsAssigned<T> ColumnValue(string columnName, float value)
        {
            return this.ColumnValue(columnName, new RDBFixedFloatExpression { Value = value });
        }

        public IRDBUpdateQueryColumnsAssigned<T> ColumnValue(string columnName, DateTime value)
        {
            return this.ColumnValue(columnName, new RDBFixedDateTimeExpression { Value = value });
        }

        public IRDBUpdateQueryColumnsAssigned<T> ColumnValue(string columnName, bool value)
        {
            return this.ColumnValue(columnName, new RDBFixedBooleanExpression { Value = value });
        }

        public IRDBUpdateQueryColumnsAssigned<T> ColumnValue(string columnName, Guid value)
        {
            return this.ColumnValue(columnName, new RDBFixedGuidExpression { Value = value });
        }

        public IRDBUpdateQueryColumnsAssigned<T> ColumnValueIf(Func<bool> shouldUpdateColumnValue, Action<IRDBUpdateQueryColumnsAssigned<T>> trueAction, Action<IRDBUpdateQueryColumnsAssigned<T>> falseAction)
        {
            if (shouldUpdateColumnValue())
                trueAction(this);
            else if (falseAction != null)
                falseAction(this);
            return this;
        }

        public IRDBUpdateQueryColumnsAssigned<T> ColumnValueIf(Func<bool> shouldUpdateColumnValue, Action<IRDBUpdateQueryColumnsAssigned<T>> trueAction)
        {
            return ColumnValueIf(shouldUpdateColumnValue, trueAction, null);
        }

        public IRDBUpdateQueryColumnsAssigned<T> ColumnValueIfNotDefaultValue<Q>(Q value, Action<IRDBUpdateQueryColumnsAssigned<T>> trueAction)
        {
            return ColumnValueIf(() => value != null && !value.Equals(default(Q)), trueAction);
        }

        public RDBJoinContext<IRDBUpdateQueryJoined<T>> Join(string tableAlias)
        {
            this._tableAlias = tableAlias;
            _queryBuilderContext.AddTableAlias(this.Table, tableAlias);
            this.Joins = new List<RDBJoin>();
            return new RDBJoinContext<IRDBUpdateQueryJoined<T>>(this, _queryBuilderContext, this.Joins);
        }
        
        public RDBConditionContext<IRDBUpdateQueryFiltered<T>> Where()
        {
            return new RDBConditionContext<IRDBUpdateQueryFiltered<T>>(this, (condition) => this.Condition = condition, _tableAlias);
        }
        
        protected override RDBResolvedQuery GetResolvedQuery(IRDBQueryGetResolvedQueryContext context)
        {
            if (this.NotExistCondition != null)
            {
                var selectQuery = new RDBSelectQuery<RDBUpdateQuery<T>>(this, _queryBuilderContext.CreateChildContext());
                selectQuery.From(this.Table, _notExistConditionTableAlias, 1).Where().Condition(this.NotExistCondition).SelectColumns().Column(new RDBNullExpression(), "nullColumn").EndColumns();
                var rdbNotExistsCondition = new RDBNotExistsCondition<RDBUpdateQuery<T>>()
                {
                    SelectQuery = selectQuery
                };
                IRDBQueryReady ifQuery = new RDBIfQuery<RDBUpdateQuery<T>>(this, _queryBuilderContext.CreateChildContext())
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
            var resolveUpdateQueryContext = new RDBDataProviderResolveUpdateQueryContext(this.Table, this._tableAlias, this.ColumnValues, this.Condition, this.Joins, context, _queryBuilderContext);
            return context.DataProvider.ResolveUpdateQuery(resolveUpdateQueryContext);
        }

        public T EndUpdate()
        {
            return _parent;
        }

        #region Private Classes

        #endregion
    }

    public class RDBUpdateColumn
    {
        public string ColumnName { get; set; }

        public BaseRDBExpression Value { get; set; }
    }

    public interface IRDBUpdateQuery<T> : IRDBUpdateQueryCanDefineTable<T>
    {
    }

    public interface IRDBUpdateQueryTableDefined<T> : IRDBUpdateQueryCanAssignColumns<T>, IRDBUpdateQueryCanFilter<T>, IRDBUpdateQueryCanJoin<T>, IRDBUpdateQueryCanDefineNotExistsCondition<T>
    {

    }

    public interface IRDBUpdateQueryReady<T> : IRDBQueryReady
    {
        T EndUpdate();
    }

    public interface IRDBUpdateQueryColumnsAssigned<T> : IRDBUpdateQueryReady<T>, IRDBUpdateQueryCanAssignColumns<T>
    {

    }

    public interface IRDBUpdateQueryFiltered<T> : IRDBUpdateQueryReady<T>, IRDBUpdateQueryCanAssignColumns<T>
    {

    }
    public interface IRDBUpdateQueryNotExistsConditionDefined<T> : IRDBUpdateQueryReady<T>, IRDBUpdateQueryCanFilter<T>, IRDBUpdateQueryCanAssignColumns<T>, IRDBUpdateQueryCanJoin<T>
    {

    }
    public interface IRDBUpdateQueryJoined<T> : IRDBUpdateQueryReady<T>, IRDBUpdateQueryCanFilter<T>, IRDBUpdateQueryCanAssignColumns<T>
    {

    }

    public interface IRDBUpdateQueryCanDefineTable<T>
    {
        IRDBUpdateQueryTableDefined<T> FromTable(string tableName);

        IRDBUpdateQueryTableDefined<T> FromTable(IRDBTableQuerySource table);
    }

    public interface IRDBUpdateQueryCanAssignColumns<T>
    {
        IRDBUpdateQueryColumnsAssigned<T> ColumnValue(string columnName, BaseRDBExpression value);

        IRDBUpdateQueryColumnsAssigned<T> ColumnValue(string columnName, string value);

        IRDBUpdateQueryColumnsAssigned<T> ColumnValue(string columnName, int value);

        IRDBUpdateQueryColumnsAssigned<T> ColumnValue(string columnName, long value);

        IRDBUpdateQueryColumnsAssigned<T> ColumnValue(string columnName, decimal value);

        IRDBUpdateQueryColumnsAssigned<T> ColumnValue(string columnName, float value);

        IRDBUpdateQueryColumnsAssigned<T> ColumnValue(string columnName, DateTime value);

        IRDBUpdateQueryColumnsAssigned<T> ColumnValue(string columnName, bool value);

        IRDBUpdateQueryColumnsAssigned<T> ColumnValue(string columnName, Guid value);

        IRDBUpdateQueryColumnsAssigned<T> ColumnValueIf(Func<bool> shouldUpdateColumnValue, Action<IRDBUpdateQueryColumnsAssigned<T>> trueAction, Action<IRDBUpdateQueryColumnsAssigned<T>> falseAction);
        
        IRDBUpdateQueryColumnsAssigned<T> ColumnValueIf(Func<bool> shouldUpdateColumnValue, Action<IRDBUpdateQueryColumnsAssigned<T>> trueAction);
        
        IRDBUpdateQueryColumnsAssigned<T> ColumnValueIfNotDefaultValue<Q>(Q value, Action<IRDBUpdateQueryColumnsAssigned<T>> trueAction);
    }

    public interface IRDBUpdateQueryCanFilter<T>
    {
        RDBConditionContext<IRDBUpdateQueryFiltered<T>> Where();
    }

    public interface IRDBUpdateQueryCanJoin<T>
    {
        RDBJoinContext<IRDBUpdateQueryJoined<T>> Join(string tableAlias);
    }

    public interface IRDBUpdateQueryCanDefineNotExistsCondition<T>
    {
        RDBConditionContext<IRDBUpdateQueryNotExistsConditionDefined<T>> IfNotExists(string tableAlias);
    }
}