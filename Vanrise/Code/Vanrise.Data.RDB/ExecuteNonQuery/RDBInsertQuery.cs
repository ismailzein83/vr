using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.Data.RDB
{
    public class RDBInsertQuery<T> : BaseRDBQuery, IRDBInsertQuery<T>, IRDBInsertQueryGenerateIdCalled<T>, IRDBInsertQueryTableDefined<T>, IRDBInsertQueryReady<T>, IRDBInsertQueryColumnsAssigned<T>, IRDBInsertQueryNotExistsConditionDefined<T>
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
        bool _declareGeneratedIdParameter;

        bool _addSelectGeneratedId;
        string _selectGeneratedIdAlias;

        public IRDBInsertQueryTableDefined<T> IntoTable(IRDBTableQuerySource table)
        {
            this.Table = table;
            _queryBuilderContext.SetMainQueryTable(table);
            this.ColumnValues = new List<RDBInsertColumn>();
            return this;
        }

        public IRDBInsertQueryTableDefined<T> IntoTable(string tableName)
        {
            return IntoTable(new RDBTableDefinitionQuerySource(tableName));
        }

        public RDBConditionContext<IRDBInsertQueryNotExistsConditionDefined<T>> IfNotExists(string tableAlias)
        {
            this._notExistConditionTableAlias = tableAlias;
            return new RDBConditionContext<IRDBInsertQueryNotExistsConditionDefined<T>>(this, _queryBuilderContext, (condition) => this.NotExistCondition = condition, this._notExistConditionTableAlias);
        }

        public IRDBInsertQueryGenerateIdCalled<T> GenerateIdAndAssignToParameter(string parameterName, bool isParameterAlreadyDeclared = false, bool addSelectQuery = true, bool selectQueryWithAlias = false)
        {
            _generatedIdParameterName = parameterName;
            _declareGeneratedIdParameter = !isParameterAlreadyDeclared;
            _addSelectGeneratedId = addSelectQuery;
            if (selectQueryWithAlias)
                _selectGeneratedIdAlias = _generatedIdParameterName;
            return this;
        }

        public IRDBInsertQueryColumnsAssigned<T> ColumnValue(string columnName, BaseRDBExpression value)
        {
            this.ColumnValues.Add(new RDBInsertColumn
            {
                ColumnName = columnName,
                Value = value
            });
            return this;
        }

        public IRDBInsertQueryColumnsAssigned<T> ColumnValue(string columnName, string value)
        {
            return this.ColumnValue(columnName, new RDBFixedTextExpression { Value = value });
        }

        public IRDBInsertQueryColumnsAssigned<T> ColumnValue(string columnName, int value)
        {
            return this.ColumnValue(columnName, new RDBFixedIntExpression { Value = value });
        }

        public IRDBInsertQueryColumnsAssigned<T> ColumnValue(string columnName, long value)
        {
            return this.ColumnValue(columnName, new RDBFixedLongExpression { Value = value });
        }

        public IRDBInsertQueryColumnsAssigned<T> ColumnValue(string columnName, decimal value)
        {
            return this.ColumnValue(columnName, new RDBFixedDecimalExpression { Value = value });
        }

        public IRDBInsertQueryColumnsAssigned<T> ColumnValue(string columnName, float value)
        {
            return this.ColumnValue(columnName, new RDBFixedFloatExpression { Value = value });
        }

        public IRDBInsertQueryColumnsAssigned<T> ColumnValue(string columnName, DateTime value)
        {
            return this.ColumnValue(columnName, new RDBFixedDateTimeExpression { Value = value });
        }

        public IRDBInsertQueryColumnsAssigned<T> ColumnValue(string columnName, Guid value)
        {
            return this.ColumnValue(columnName, new RDBFixedGuidExpression { Value = value });
        }

        public IRDBInsertQueryColumnsAssigned<T> ColumnValue(string columnName, bool value)
        {
            return this.ColumnValue(columnName, new RDBFixedBooleanExpression { Value = value });
        }

        public IRDBInsertQueryColumnsAssigned<T> ColumnValue(string columnName, byte[] value)
        {
            return this.ColumnValue(columnName, new RDBFixedBytesExpression { Value = value });
        }

        public IRDBInsertQueryColumnsAssigned<T> ColumnValueIf(Func<bool> shouldAddColumnValue, Action<IRDBInsertQueryColumnsAssigned<T>> trueAction, Action<IRDBInsertQueryColumnsAssigned<T>> falseAction)
        {
            if (shouldAddColumnValue())
                trueAction(this);
            else if (falseAction != null)
                falseAction(this);
            return this;
        }

        public IRDBInsertQueryColumnsAssigned<T> ColumnValueIf(Func<bool> shouldAddColumnValue, Action<IRDBInsertQueryColumnsAssigned<T>> trueAction)
        {
            return ColumnValueIf(shouldAddColumnValue, trueAction, null);
        }

        //public IRDBInsertQueryColumnsAssigned<T> ColumnValueIfNotDefaultValue<Q>(Q value, Action<IRDBInsertQueryColumnsAssigned<T>> trueAction)
        //{
        //    return ColumnValueIf(() => value != null && !value.Equals(default(Q)), trueAction);
        //}

        private IRDBInsertQueryColumnsAssigned<T> ColumnValueDBNullIfDefaultValue<Q>(string columnName, Q value, Action<IRDBInsertQueryColumnsAssigned<T>> actionIfNotDefaultValue)
        {
            if (value == null || value.Equals(default(Q)))
            {
                return ColumnValue(columnName, new RDBNullExpression());
            }
            else
            {
                actionIfNotDefaultValue(this);
                return this;
            }
        }

        public IRDBInsertQueryColumnsAssigned<T> ColumnValueDBNullIfDefaultValue(string columnName, int? value)
        {
            return ColumnValueDBNullIfDefaultValue(columnName, value, (ctx) => ctx.ColumnValue(columnName, value.Value));
        }

        public IRDBInsertQueryColumnsAssigned<T> ColumnValueDBNullIfDefaultValue(string columnName, long? value)
        {
            return ColumnValueDBNullIfDefaultValue(columnName, value, (ctx) => ctx.ColumnValue(columnName, value.Value));
        }

        public IRDBInsertQueryColumnsAssigned<T> ColumnValueDBNullIfDefaultValue(string columnName, decimal? value)
        {
            return ColumnValueDBNullIfDefaultValue(columnName, value, (ctx) => ctx.ColumnValue(columnName, value.Value));
        }

        public IRDBInsertQueryColumnsAssigned<T> ColumnValueDBNullIfDefaultValue(string columnName, float? value)
        {
            return ColumnValueDBNullIfDefaultValue(columnName, value, (ctx) => ctx.ColumnValue(columnName, value.Value));
        }

        public IRDBInsertQueryColumnsAssigned<T> ColumnValueDBNullIfDefaultValue(string columnName, DateTime? value)
        {
            return ColumnValueDBNullIfDefaultValue(columnName, value, (ctx) => ctx.ColumnValue(columnName, value.Value));
        }

        public IRDBInsertQueryColumnsAssigned<T> ColumnValueDBNullIfDefaultValue(string columnName, bool? value)
        {
            return ColumnValueDBNullIfDefaultValue(columnName, value, (ctx) => ctx.ColumnValue(columnName, value.Value));
        }

        public IRDBInsertQueryColumnsAssigned<T> ColumnValueDBNullIfDefaultValue(string columnName, Guid? value)
        {
            return ColumnValueDBNullIfDefaultValue(columnName, value, (ctx) => ctx.ColumnValue(columnName, value.Value));
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

            if (!string.IsNullOrEmpty(_generatedIdParameterName) && _addSelectGeneratedId)
            {
                RDBResolvedQuery selectIdResolvedQuery = new RDBSelectQuery<RDBInsertQuery<T>>(this, _queryBuilderContext.CreateChildContext()).FromNoTable().SelectColumns().
                    Parameter(_generatedIdParameterName, _selectGeneratedIdAlias).
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
                if (_declareGeneratedIdParameter)
                {
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

    public interface IRDBInsertQuery<T> : IRDBInsertQueryCanDefineTable<T>
    {
    }

    public interface IRDBInsertQueryReady<T> : IRDBQueryReady
    {
        T EndInsert();
    }

    public interface IRDBInsertQueryTableDefined<T> : IRDBInsertQueryCanAssignColumns<T>, IRDBInsertQueryCanDefineNotExistsCondition<T>, IRDBInsertQueryCanCallGenerateId<T>
    {

    }

    public interface IRDBInsertQueryColumnsAssigned<T> : IRDBInsertQueryReady<T>, IRDBInsertQueryCanAssignColumns<T>
    {

    }

    public interface IRDBInsertQueryNotExistsConditionDefined<T> : IRDBInsertQueryCanAssignColumns<T>, IRDBInsertQueryCanCallGenerateId<T>
    {

    }

    public interface IRDBInsertQueryGenerateIdCalled<T> : IRDBInsertQueryCanAssignColumns<T>
    {

    }

    public interface IRDBInsertQueryCanDefineTable<T>
    {
        IRDBInsertQueryTableDefined<T> IntoTable(IRDBTableQuerySource table);

        IRDBInsertQueryTableDefined<T> IntoTable(string tableName);
    }

    public interface IRDBInsertQueryCanCallGenerateId<T>
    {
        IRDBInsertQueryGenerateIdCalled<T> GenerateIdAndAssignToParameter(string parameterName, bool isParameterAlreadyDeclared = false, bool addSelectQuery = true, bool selectQueryWithAlias = false);
    }

    public interface IRDBInsertQueryCanAssignColumns<T>
    {
        IRDBInsertQueryColumnsAssigned<T> ColumnValue(string columnName, BaseRDBExpression value);

        IRDBInsertQueryColumnsAssigned<T> ColumnValue(string columnName, string value);

        IRDBInsertQueryColumnsAssigned<T> ColumnValue(string columnName, int value);

        IRDBInsertQueryColumnsAssigned<T> ColumnValue(string columnName, long value);

        IRDBInsertQueryColumnsAssigned<T> ColumnValue(string columnName, decimal value);

        IRDBInsertQueryColumnsAssigned<T> ColumnValue(string columnName, float value);

        IRDBInsertQueryColumnsAssigned<T> ColumnValue(string columnName, DateTime value);

        IRDBInsertQueryColumnsAssigned<T> ColumnValue(string columnName, Guid value);

        IRDBInsertQueryColumnsAssigned<T> ColumnValue(string columnName, bool value);

        IRDBInsertQueryColumnsAssigned<T> ColumnValue(string columnName, byte[] value);

        IRDBInsertQueryColumnsAssigned<T> ColumnValueIf(Func<bool> shouldAddColumnValue, Action<IRDBInsertQueryColumnsAssigned<T>> trueAction, Action<IRDBInsertQueryColumnsAssigned<T>> falseAction);

        IRDBInsertQueryColumnsAssigned<T> ColumnValueIf(Func<bool> shouldAddColumnValue, Action<IRDBInsertQueryColumnsAssigned<T>> trueAction);

        //IRDBInsertQueryColumnsAssigned<T> ColumnValueIfNotDefaultValue<Q>(Q value, Action<IRDBInsertQueryColumnsAssigned<T>> trueAction);

        IRDBInsertQueryColumnsAssigned<T> ColumnValueDBNullIfDefaultValue(string columnName, int? value);

        IRDBInsertQueryColumnsAssigned<T> ColumnValueDBNullIfDefaultValue(string columnName, long? value);

        IRDBInsertQueryColumnsAssigned<T> ColumnValueDBNullIfDefaultValue(string columnName, decimal? value);

        IRDBInsertQueryColumnsAssigned<T> ColumnValueDBNullIfDefaultValue(string columnName, float? value);

        IRDBInsertQueryColumnsAssigned<T> ColumnValueDBNullIfDefaultValue(string columnName, DateTime? value);

        IRDBInsertQueryColumnsAssigned<T> ColumnValueDBNullIfDefaultValue(string columnName, bool? value);

        IRDBInsertQueryColumnsAssigned<T> ColumnValueDBNullIfDefaultValue(string columnName, Guid? value);
    }

    public interface IRDBInsertQueryCanDefineNotExistsCondition<T>
    {
        RDBConditionContext<IRDBInsertQueryNotExistsConditionDefined<T>> IfNotExists(string tableAlias);
    }
}