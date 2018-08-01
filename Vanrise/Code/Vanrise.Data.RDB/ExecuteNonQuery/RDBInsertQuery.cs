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

        public RDBInsertQuery(RDBQueryBuilderContext queryBuilderContext)
        {
            _queryBuilderContext = queryBuilderContext;
        }

        public IRDBTableQuerySource _table { get; private set; }

        public List<RDBInsertColumn> _columnValues { get; private set; }

        public BaseRDBCondition _notExistCondition { get; private set; }
        
        string _generatedIdParameterName;
        bool _declareGeneratedIdParameter;

        bool _addSelectGeneratedId;
        string _selectGeneratedIdAlias;

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

        public RDBConditionContext IfNotExists(string tableAlias)
        {
            this._notExistConditionTableAlias = tableAlias;
            return new RDBConditionContext(_queryBuilderContext, (condition) => this._notExistCondition = condition, this._notExistConditionTableAlias);
        }

        public void GenerateIdAndAssignToParameter(string parameterName, bool isParameterAlreadyDeclared = false, bool addSelectQuery = true, bool selectQueryWithAlias = false)
        {
            _generatedIdParameterName = parameterName;
            _declareGeneratedIdParameter = !isParameterAlreadyDeclared;
            _addSelectGeneratedId = addSelectQuery;
            if (selectQueryWithAlias)
                _selectGeneratedIdAlias = _generatedIdParameterName;
        }

        public void ColumnValue(string columnName, BaseRDBExpression value)
        {
            this._columnValues.Add(new RDBInsertColumn
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

        public void ColumnValue(string columnName, Guid value)
        {
            this.ColumnValue(columnName, new RDBFixedGuidExpression { Value = value });
        }

        public void ColumnValue(string columnName, bool value)
        {
             this.ColumnValue(columnName, new RDBFixedBooleanExpression { Value = value });
        }

        public void ColumnValue(string columnName, byte[] value)
        {
             this.ColumnValue(columnName, new RDBFixedBytesExpression { Value = value });
        }

        public override RDBResolvedQuery GetResolvedQuery(IRDBQueryGetResolvedQueryContext context)
        {
            RDBResolvedQuery resolvedQuery;
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
                var selectIdQuery = new RDBSelectQuery(_queryBuilderContext.CreateChildContext());
                selectIdQuery.SelectColumns().Parameter(_generatedIdParameterName, _selectGeneratedIdAlias);
                RDBResolvedQuery selectIdResolvedQuery = selectIdQuery.GetResolvedQuery(context);
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
                    this._table.GetIdColumnInfo(getIdColumnInfoContext);
                    getIdColumnInfoContext.IdColumnName.ThrowIfNull("getIdColumnInfoContext.IdColumnName", this._table.GetUniqueName());
                    getIdColumnInfoContext.IdColumnDefinition.ThrowIfNull("getIdColumnInfoContext.IdColumnDefinition", this._table.GetUniqueName());
                    context.AddParameter(new RDBParameter
                    {
                        Name = _generatedIdParameterName,
                        DBParameterName = generatedIdDBParameterName,
                        Type = getIdColumnInfoContext.IdColumnDefinition.DataType,
                        Direction = RDBParameterDirection.Declared
                    });
                }
            }

            var resolveInsertQueryContext = new RDBDataProviderResolveInsertQueryContext(this._table, this._columnValues, generatedIdDBParameterName, context, _queryBuilderContext);
            resolvedQuery = context.DataProvider.ResolveInsertQuery(resolveInsertQueryContext);
            return resolvedQuery;
        }
        
        #region Private Classes




        #endregion
    }

    public class RDBInsertColumn
    {
        public string ColumnName { get; set; }

        public BaseRDBExpression Value { get; set; }
    }
}