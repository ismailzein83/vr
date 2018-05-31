using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.Data.RDB
{
    public class RDBInsertQuery<T> : BaseRDBQuery, IInsertQuery<T>, IInsertQueryGenerateIdCalled<T>, IInsertQueryTableDefined<T>, IInsertQueryReady<T>, IInsertQueryColumnsAssigned<T>, IInsertQueryNotExistsConditionDefined<T>
    {
        T _parent;
        RDBQueryBuilderContext _queryBuilderContext;

        string _notExistConditionTableAlias;

        public RDBInsertQuery(T parent, RDBQueryBuilderContext queryBuilderContext)
        {
            _parent = parent;
            _queryBuilderContext = queryBuilderContext;
        }

        public IRDBTableQuerySource Table { get; private set; }

        public List<RDBInsertColumn> ColumnValues { get; private set; }

        public BaseRDBCondition NotExistCondition { get; private set; }

        string _generatedIdParameterName;

        public IInsertQueryTableDefined<T> IntoTable(IRDBTableQuerySource table)
        {
            this.Table = table;
            this.ColumnValues = new List<RDBInsertColumn>();
            return this;
        }

        public IInsertQueryTableDefined<T> IntoTable(string tableName)
        {
            return IntoTable(new RDBTableDefinitionQuerySource(tableName));
        }

        public RDBConditionContext<IInsertQueryNotExistsConditionDefined<T>> IfNotExists(string tableAlias)
        {
            this._notExistConditionTableAlias = tableAlias;
            return new RDBConditionContext<IInsertQueryNotExistsConditionDefined<T>>(this, (condition) => this.NotExistCondition = condition, this._notExistConditionTableAlias);
        }

        public IInsertQueryGenerateIdCalled<T> GenerateIdAndAssignToParameter(string parameterName)
        {
            _generatedIdParameterName = parameterName;            
            return this;
        }

        public IInsertQueryColumnsAssigned<T> ColumnValue(string columnName, BaseRDBExpression value)
        {
            this.ColumnValues.Add(new RDBInsertColumn
            {
                ColumnName = columnName,
                Value = value
            });
            return this;
        }

        public IInsertQueryColumnsAssigned<T> ColumnValue(string columnName, string value)
        {
            return this.ColumnValue(columnName, new RDBFixedTextExpression { Value = value });
        }

        public IInsertQueryColumnsAssigned<T> ColumnValue(string columnName, int value)
        {
            return this.ColumnValue(columnName, new RDBFixedIntExpression { Value = value });
        }

        public IInsertQueryColumnsAssigned<T> ColumnValue(string columnName, long value)
        {
            return this.ColumnValue(columnName, new RDBFixedLongExpression { Value = value });
        }

        public IInsertQueryColumnsAssigned<T> ColumnValue(string columnName, decimal value)
        {
            return this.ColumnValue(columnName, new RDBFixedDecimalExpression { Value = value });
        }

        public IInsertQueryColumnsAssigned<T> ColumnValue(string columnName, float value)
        {
            return this.ColumnValue(columnName, new RDBFixedFloatExpression { Value = value });
        }

        public IInsertQueryColumnsAssigned<T> ColumnValue(string columnName, DateTime value)
        {
            return this.ColumnValue(columnName, new RDBFixedDateTimeExpression { Value = value });
        }

        public IInsertQueryColumnsAssigned<T> ColumnValue(string columnName, Guid value)
        {
            return this.ColumnValue(columnName, new RDBFixedGuidExpression { Value = value });
        }

        public IInsertQueryColumnsAssigned<T> ColumnValue(string columnName, bool value)
        {
            return this.ColumnValue(columnName, new RDBFixedBooleanExpression { Value = value });
        }

        public IInsertQueryColumnsAssigned<T> ColumnValueIf(Func<bool> shouldAddColumnValue, Action<IInsertQueryColumnsAssigned<T>> trueAction, Action<IInsertQueryColumnsAssigned<T>> falseAction)
        {
            if (shouldAddColumnValue())
                trueAction(this);
            else if (falseAction != null)
                falseAction(this);
            return this;
        }

        public IInsertQueryColumnsAssigned<T> ColumnValueIf(Func<bool> shouldAddColumnValue, Action<IInsertQueryColumnsAssigned<T>> trueAction)
        {
            return ColumnValueIf(shouldAddColumnValue, trueAction, null);
        }

        public IInsertQueryColumnsAssigned<T> ColumnValueIfNotDefaultValue<Q>(Q value, Action<IInsertQueryColumnsAssigned<T>> trueAction)
        {
            return ColumnValueIf(() => value != null && !value.Equals(default(Q)), trueAction);
        }

        protected override RDBResolvedQuery GetResolvedQuery(IRDBQueryGetResolvedQueryContext context)
        {
            RDBResolvedQuery resolvedQuery;
            if (this.NotExistCondition != null)
            {
                var selectQuery = new RDBSelectQuery<RDBInsertQuery<T>>(this, _queryBuilderContext.CreateChildContext());
                selectQuery.From(this.Table, _notExistConditionTableAlias, 1).Where().Condition(this.NotExistCondition).SelectColumns().Column(new RDBNullExpression(), "nullColumn").EndColumns();
                var rdbNotExistsCondition = new RDBNotExistsCondition<RDBInsertQuery<T>>()
                {
                    SelectQuery = selectQuery
                };
                IRDBQueryReady ifQuery = new RDBIfQuery<RDBInsertQuery<T>>(this, _queryBuilderContext.CreateChildContext())
                {
                    Condition = rdbNotExistsCondition,
                    _trueQueryText = ResolveInsertQuery(context).QueryText
                };
                resolvedQuery = ifQuery.GetResolvedQuery(context);
            }
            else
            {
                resolvedQuery = ResolveInsertQuery(context);               
            }

            if (!string.IsNullOrEmpty(_generatedIdParameterName))
            {
                RDBResolvedQuery selectIdResolvedQuery = new RDBSelectQuery<RDBInsertQuery<T>>(this, _queryBuilderContext.CreateChildContext()).FromNoTable().SelectColumns().
                    Parameter(_generatedIdParameterName, _generatedIdParameterName).
                    EndColumns().GetResolvedQuery(context);
                resolvedQuery.QueryText = string.Concat(resolvedQuery.QueryText, "\n", selectIdResolvedQuery.QueryText);
            }
            return resolvedQuery;
        }

        private RDBResolvedQuery ResolveInsertQuery(IRDBQueryGetResolvedQueryContext context)
        {
            RDBResolvedQuery resolvedQuery;
            string generatedIdDBParameterName = null;

            if (!string.IsNullOrEmpty(_generatedIdParameterName))
            {
                generatedIdDBParameterName = context.DataProvider.ConvertToDBParameterName(_generatedIdParameterName);
                var getIdColumnInfoContext = new RDBTableQuerySourceGetIdColumnInfoContext(context.DataProvider);
                this.Table.GetIdColumnInfo(getIdColumnInfoContext);
                getIdColumnInfoContext.IdColumnName.ThrowIfNull("getIdColumnInfoContext.IdColumnName", this.Table.GetUniqueName());
                getIdColumnInfoContext.IdColumnDefinition.ThrowIfNull("getIdColumnInfoContext.IdColumnDefinition", this.Table.GetUniqueName());
                context.AddParameter(new RDBParameter
                {
                    Name = _generatedIdParameterName,
                    DBParameterName = generatedIdDBParameterName,
                    Type = getIdColumnInfoContext.IdColumnDefinition.DataType,
                    Direction = RDBParameterDirection.Declared
                });
            }

            var resolveInsertQueryContext = new RDBDataProviderResolveInsertQueryContext(this.Table, this.ColumnValues, generatedIdDBParameterName, context, _queryBuilderContext);
            resolvedQuery = context.DataProvider.ResolveInsertQuery(resolveInsertQueryContext);
            return resolvedQuery;
        }
        
        public T EndInsert()
        {
            return _parent;
        }

        #region Private Classes




        #endregion
    }

    public class RDBInsertColumn
    {
        public string ColumnName { get; set; }

        public BaseRDBExpression Value { get; set; }
    }

    public interface IInsertQuery<T> : IInsertQueryCanDefineTable<T>
    {
    }

    public interface IInsertQueryReady<T> : IRDBQueryReady
    {
        T EndInsert();
    }

    public interface IInsertQueryTableDefined<T> : IInsertQueryCanAssignColumns<T>, IInsertQueryCanDefineNotExistsCondition<T>, IInsertQueryCanCallGenerateId<T>
    {

    }

    public interface IInsertQueryColumnsAssigned<T> : IInsertQueryReady<T>, IInsertQueryCanAssignColumns<T>
    {

    }

    public interface IInsertQueryNotExistsConditionDefined<T> : IInsertQueryReady<T>, IInsertQueryCanAssignColumns<T>, IInsertQueryCanCallGenerateId<T>
    {

    }

    public interface IInsertQueryGenerateIdCalled<T> : IInsertQueryReady<T>, IInsertQueryCanAssignColumns<T>
    {

    }

    public interface IInsertQueryCanDefineTable<T>
    {
        IInsertQueryTableDefined<T> IntoTable(IRDBTableQuerySource table);

        IInsertQueryTableDefined<T> IntoTable(string tableName);
    }

    public interface IInsertQueryCanCallGenerateId<T>
    {
        IInsertQueryGenerateIdCalled<T> GenerateIdAndAssignToParameter(string parameterName);
    }

    public interface IInsertQueryCanAssignColumns<T>
    {
        IInsertQueryColumnsAssigned<T> ColumnValue(string columnName, BaseRDBExpression value);

        IInsertQueryColumnsAssigned<T> ColumnValue(string columnName, string value);

        IInsertQueryColumnsAssigned<T> ColumnValue(string columnName, int value);

        IInsertQueryColumnsAssigned<T> ColumnValue(string columnName, long value);

        IInsertQueryColumnsAssigned<T> ColumnValue(string columnName, decimal value);

        IInsertQueryColumnsAssigned<T> ColumnValue(string columnName, float value);

        IInsertQueryColumnsAssigned<T> ColumnValue(string columnName, DateTime value);

        IInsertQueryColumnsAssigned<T> ColumnValue(string columnName, Guid value);

        IInsertQueryColumnsAssigned<T> ColumnValue(string columnName, bool value);

        IInsertQueryColumnsAssigned<T> ColumnValueIf(Func<bool> shouldAddColumnValue, Action<IInsertQueryColumnsAssigned<T>> trueAction, Action<IInsertQueryColumnsAssigned<T>> falseAction);

        IInsertQueryColumnsAssigned<T> ColumnValueIf(Func<bool> shouldAddColumnValue, Action<IInsertQueryColumnsAssigned<T>> trueAction);

        IInsertQueryColumnsAssigned<T> ColumnValueIfNotDefaultValue<Q>(Q value, Action<IInsertQueryColumnsAssigned<T>> trueAction);
    }

    public interface IInsertQueryCanDefineNotExistsCondition<T>
    {
        RDBConditionContext<IInsertQueryNotExistsConditionDefined<T>> IfNotExists(string tableAlias);
    }
}