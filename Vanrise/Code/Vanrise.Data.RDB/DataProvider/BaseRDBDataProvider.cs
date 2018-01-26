using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.Data.RDB
{
    public abstract class BaseRDBDataProvider
    {
        public abstract string ParameterNamePrefix { get; }

        public abstract RDBResolvedSelectQuery ResolveSelectQuery(IRDBDataProviderResolveSelectQueryContext context);

        public abstract RDBResolvedNoDataQuery ResolveInsertQuery(IRDBDataProviderResolveInsertQueryContext context);

        public abstract RDBResolvedNoDataQuery ResolveUpdateQuery(IRDBDataProviderResolveUpdateQueryContext context);

        public abstract void ExecuteReader(IRDBDataProviderExecuteReaderContext context);

        public abstract Object ExecuteScalar(IRDBDataProviderExecuteScalarContext context);

        public abstract int ExecuteNonQuery(IRDBDataProviderExecuteNonQueryContext context);
    }

    public interface IRDBDataProviderResolveQueryContext
    {
        string GenerateParameterName();

        string GenerateTableAlias();
    }

    public interface IRDBDataProviderResolveSelectQueryContext : IRDBDataProviderResolveQueryContext
    {
        RDBSelectQuery SelectQuery { get; }
    }

    public class RDBResolvedSelectQuery
    {
        public string QueryText { get; set; }

        public Dictionary<string, Object> ParameterValues { get; set; }
    }

    public class RDBResolvedNoDataQuery
    {
        public string QueryText { get; set; }

        public Dictionary<string, Object> ParameterValues { get; set; }
    }

    public interface IRDBDataProviderResolveInsertQueryContext : IRDBDataProviderResolveQueryContext
    {
        RDBInsertQuery InsertQuery { get; }
    }

    public interface IRDBDataProviderResolveUpdateQueryContext : IRDBDataProviderResolveQueryContext
    {
        RDBUpdateQuery UpdateQuery { get; }
    }

    public interface IRDBDataProviderExecuteReaderContext
    {
        RDBResolvedSelectQuery ResolvedQuery { get; }
        void OnReaderReady(IRDBDataReader reader);
    }

    public interface IRDBDataProviderExecuteScalarContext
    {
    }

    public interface IRDBDataProviderExecuteNonQueryContext
    {
        RDBResolvedNoDataQuery ResolvedQuery { get; }
    }

    public class MSSQLRDBDataProvider : BaseRDBDataProvider
    {
        SQLDataManager _dataManager = new SQLDataManager();
        public override RDBResolvedSelectQuery ResolveSelectQuery(IRDBDataProviderResolveSelectQueryContext context)
        {
            var selectQuery = context.SelectQuery;
            selectQuery.ThrowIfNull("selectQuery");
            selectQuery.Table.ThrowIfNull("selectQuery.Table");
            selectQuery.Columns.ThrowIfNull("selectQuery.Columns");
            Dictionary<IRDBTableQuerySource, string> tableAliases = new Dictionary<IRDBTableQuerySource, string>();
            Dictionary<string, Object> parameterValues = new Dictionary<string, object>();
            StringBuilder queryBuilder = new StringBuilder("");
            int colIndex = 0;
            var rdbExpressionToDBQueryContext = new RDBExpressionToDBQueryContext(RDBDataProviderType.MSSQL, tableAliases, context, parameterValues);
            var rdbConditionToDBQueryContext = new RDBConditionToDBQueryContext(RDBDataProviderType.MSSQL, tableAliases, context);
            rdbExpressionToDBQueryContext.ConditionContext = rdbConditionToDBQueryContext;
            rdbConditionToDBQueryContext.ExpressionContext = rdbExpressionToDBQueryContext;

            queryBuilder.Append(" FROM ");
            AddTableToDBQueryBuilder(queryBuilder, selectQuery.Table, context, tableAliases, parameterValues);

            if (selectQuery.Joins != null && selectQuery.Joins.Count > 0)
            {
                foreach (var join in selectQuery.Joins)
                {
                    if (join.JoinType == RDBJoinType.Left)
                        queryBuilder.Append(" LEFT ");
                    queryBuilder.Append(" JOIN ");
                    AddTableToDBQueryBuilder(queryBuilder, join.Table, context, tableAliases, parameterValues);
                    string joinConditionDBQuery = join.Condition.ToDBQuery(rdbConditionToDBQueryContext);
                    joinConditionDBQuery.ThrowIfNull("joinConditionDBQuery");
                    queryBuilder.Append(" ON ");
                    queryBuilder.Append(joinConditionDBQuery);
                }
            }

            StringBuilder columnQueryBuilder = new StringBuilder(" SELECT ");
            foreach (var column in selectQuery.Columns)
            {
                if (colIndex > 0)
                    columnQueryBuilder.Append(", ");
                colIndex++;
                columnQueryBuilder.Append(column.Expression.ToDBQuery(rdbExpressionToDBQueryContext));
                columnQueryBuilder.Append(" ");
                columnQueryBuilder.Append(column.Alias);
            }

            queryBuilder.Insert(0, columnQueryBuilder.ToString());

            if (selectQuery.Condition != null)
            {
                string conditionDBQuery = selectQuery.Condition.ToDBQuery(rdbConditionToDBQueryContext);
                if (!string.IsNullOrEmpty(conditionDBQuery))
                {
                    queryBuilder.Append(" WHERE ");
                    queryBuilder.Append(conditionDBQuery);
                }
            }
            return new RDBResolvedSelectQuery
            {
                QueryText = queryBuilder.ToString(),
                ParameterValues = parameterValues
            };
        }

        public override RDBResolvedNoDataQuery ResolveInsertQuery(IRDBDataProviderResolveInsertQueryContext context)
        {
            var insertQuery = context.InsertQuery;
            insertQuery.ThrowIfNull("insertQuery");
            insertQuery.Table.ThrowIfNull("insertQuery.Table");
            insertQuery.ColumnValues.ThrowIfNull("insertQuery.ColumnValues");
            Dictionary<string, Object> parameterValues = new Dictionary<string, object>();
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append("INSERT INTO ");
            AddTableToDBQueryBuilder(queryBuilder, insertQuery.Table, context, null, parameterValues);
            queryBuilder.Append(" (");
            var rdbExpressionToDBQueryContext = new RDBExpressionToDBQueryContext(RDBDataProviderType.MSSQL, null, context, parameterValues);
            var rdbConditionToDBQueryContext = new RDBConditionToDBQueryContext(RDBDataProviderType.MSSQL, null, context);
            rdbExpressionToDBQueryContext.ConditionContext = rdbConditionToDBQueryContext;
            rdbConditionToDBQueryContext.ExpressionContext = rdbExpressionToDBQueryContext;

            StringBuilder valueBuilder = new StringBuilder();
            int colIndex = 0;
            foreach (var colVal in insertQuery.ColumnValues)
            {
                if (colIndex > 0)
                {
                    queryBuilder.Append(",");
                    valueBuilder.Append(",");
                }
                colIndex++;
                queryBuilder.Append(colVal.ColumnName);
                valueBuilder.Append(colVal.Value.ToDBQuery(rdbExpressionToDBQueryContext));
            }
            if (!String.IsNullOrEmpty(insertQuery.Table.CreatedTimeColumnName))
            {
                queryBuilder.Append(",");
                valueBuilder.Append(",");
                queryBuilder.Append(insertQuery.Table.CreatedTimeColumnName);
                valueBuilder.Append(new RDBNowDateTimeExpression().ToDBQuery(rdbExpressionToDBQueryContext));
            }
            if (!String.IsNullOrEmpty(insertQuery.Table.ModifiedTimeColumnName))
            {
                queryBuilder.Append(",");
                valueBuilder.Append(",");
                queryBuilder.Append(insertQuery.Table.ModifiedTimeColumnName);
                valueBuilder.Append(new RDBNowDateTimeExpression().ToDBQuery(rdbExpressionToDBQueryContext));
            }
            queryBuilder.Append(") VALUES (");
            queryBuilder.Append(valueBuilder.ToString());
            queryBuilder.Append(")");
            return new RDBResolvedNoDataQuery
            {
                QueryText = queryBuilder.ToString(),
                ParameterValues = parameterValues
            };
        }

        public override RDBResolvedNoDataQuery ResolveUpdateQuery(IRDBDataProviderResolveUpdateQueryContext context)
        {
            var updateQuery = context.UpdateQuery;
            updateQuery.ThrowIfNull("updateQuery");
            updateQuery.Table.ThrowIfNull("updateQuery.Table");
            updateQuery.ColumnValues.ThrowIfNull("updateQuery.ColumnValues");
            Dictionary<string, Object> parameterValues = new Dictionary<string, object>();
            StringBuilder queryBuilder = new StringBuilder("UPDATE ");
            AddTableToDBQueryBuilder(queryBuilder, updateQuery.Table, context, null, parameterValues);

            var rdbExpressionToDBQueryContext = new RDBExpressionToDBQueryContext(RDBDataProviderType.MSSQL, null, context, parameterValues);
            var rdbConditionToDBQueryContext = new RDBConditionToDBQueryContext(RDBDataProviderType.MSSQL, null, context);
            rdbExpressionToDBQueryContext.ConditionContext = rdbConditionToDBQueryContext;
            rdbConditionToDBQueryContext.ExpressionContext = rdbExpressionToDBQueryContext;

            int colIndex = 0;

            foreach (var colVal in updateQuery.ColumnValues)
            {
                if (colIndex > 0)
                    queryBuilder.Append(", ");
                colIndex++;
                queryBuilder.Append(colVal.ColumnName);
                queryBuilder.Append(" = ");
                queryBuilder.Append(colVal.Value.ToDBQuery(rdbExpressionToDBQueryContext));
            }

            if (!String.IsNullOrEmpty(updateQuery.Table.ModifiedTimeColumnName))
            {
                queryBuilder.Append(",");
                queryBuilder.Append(updateQuery.Table.ModifiedTimeColumnName);
                queryBuilder.Append(" = ");
                queryBuilder.Append(new RDBNowDateTimeExpression().ToDBQuery(rdbExpressionToDBQueryContext));
            }

            if (updateQuery.Condition != null)
            {
                string conditionDBQuery = updateQuery.Condition.ToDBQuery(rdbConditionToDBQueryContext);
                if (!string.IsNullOrEmpty(conditionDBQuery))
                {
                    queryBuilder.Append(" WHERE ");
                    queryBuilder.Append(conditionDBQuery);
                }
            }
            return new RDBResolvedNoDataQuery
            {
                QueryText = queryBuilder.ToString(),
                ParameterValues = parameterValues
            };
        }

        private void AddTableToDBQueryBuilder(StringBuilder queryBuilder, IRDBTableQuerySource table, IRDBDataProviderResolveQueryContext context, Dictionary<IRDBTableQuerySource, string> tableAliases, Dictionary<string, Object> parameterValues)
        {
            var tableQuerySourceContext = new RDBTableQuerySourceToDBQueryContext(RDBDataProviderType.MSSQL, parameterValues);
            queryBuilder.Append(table.ToDBQuery(tableQuerySourceContext));
            if (tableAliases != null)
            {
                queryBuilder.Append(" ");
                string tableAlias;
                if (!tableAliases.TryGetValue(table, out tableAlias))
                {
                    tableAlias = context.GenerateTableAlias();
                    tableAliases.Add(table, tableAlias);
                }
                queryBuilder.Append(tableAlias);
            }
        }

        public override void ExecuteReader(IRDBDataProviderExecuteReaderContext context)
        {
            _dataManager.ExecuteReader(context.ResolvedQuery.QueryText, context.ResolvedQuery.ParameterValues, (originalReader) => context.OnReaderReady(new MSSQLRDBDataReader(originalReader)));
        }

        public override object ExecuteScalar(IRDBDataProviderExecuteScalarContext context)
        {
            throw new NotImplementedException();
        }

        #region Private Classes

        private class SQLDataManager : Vanrise.Data.SQL.BaseSQLDataManager
        {
            public void ExecuteReader(string sqlQuery, Dictionary<string, Object> parameterValues, Action<IDataReader> onReaderReady)
            {
                base.ExecuteReaderText(sqlQuery, onReaderReady, (cmd) =>
                {
                    if (parameterValues != null)
                    {
                        foreach (var prm in parameterValues)
                        {
                            cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter(prm.Key, prm.Value));
                        }
                    }
                });
            }
        }

        private class MSSQLRDBDataReader : IRDBDataReader
        {
            IDataReader _originalReader;
            public MSSQLRDBDataReader(IDataReader originalReader)
            {
                _originalReader = originalReader;
            }

            public bool NextResult()
            {
                return _originalReader.NextResult();
            }

            public bool Read()
            {
                return _originalReader.Read();
            }

            public object this[string name]
            {
                get { return _originalReader[name]; }
            }
        }

        public class RDBExpressionToDBQueryContext : IRDBExpressionToDBQueryContext
        {
            Dictionary<IRDBTableQuerySource, string> _tableAliases;
            IRDBDataProviderResolveQueryContext _resolveQueryContext;
            Dictionary<string, Object> _parameterValues;

            public RDBExpressionToDBQueryContext(RDBDataProviderType dataProviderType, Dictionary<IRDBTableQuerySource, string> tableAliases, IRDBDataProviderResolveQueryContext resolveQueryContext, Dictionary<string, Object> parameterValues)
            {
                this.DataProviderType = dataProviderType;
                _tableAliases = tableAliases;
                _resolveQueryContext = resolveQueryContext;
                _parameterValues = parameterValues;
            }

            public RDBDataProviderType DataProviderType
            {
                get;
                private set;
            }

            public string GetTableAlias(IRDBTableQuerySource table)
            {
                string alias;
                if (!_tableAliases.TryGetValue(table, out alias))
                    throw new Exception(String.Format("table Source '{0}' not found in _tableAliases", table.GetDescription()));
                return alias;
            }


            public void AddParameterValue(string parameterName, object value)
            {
                _parameterValues.Add(parameterName, value);
            }

            public string GenerateParameterName()
            {
                return _resolveQueryContext.GenerateParameterName();
            }


            public IRDBConditionToDBQueryContext ConditionContext
            {
                get;
                set;
            }
        }

        public class RDBConditionToDBQueryContext : IRDBConditionToDBQueryContext
        {
            Dictionary<IRDBTableQuerySource, string> _tableAliases;
            IRDBDataProviderResolveQueryContext _resolveQueryContext;

            public RDBConditionToDBQueryContext(RDBDataProviderType dataProviderType, Dictionary<IRDBTableQuerySource, string> tableAliases, IRDBDataProviderResolveQueryContext resolveQueryContext)
            {
                this.DataProviderType = dataProviderType;
                _tableAliases = tableAliases;
                _resolveQueryContext = resolveQueryContext;
            }

            public RDBDataProviderType DataProviderType
            {
                get;
                private set;
            }

            public string GetTableAlias(IRDBTableQuerySource table)
            {
                string alias;
                if (!_tableAliases.TryGetValue(table, out alias))
                    throw new Exception(String.Format("table Source '{0}' not found in _tableAliases", table.GetDescription()));
                return alias;
            }


            public IRDBExpressionToDBQueryContext ExpressionContext
            {
                get;
                set;
            }
        }

        public class RDBTableQuerySourceToDBQueryContext : IRDBTableQuerySourceToDBQueryContext
        {
            public RDBTableQuerySourceToDBQueryContext(RDBDataProviderType dataProviderType, Dictionary<string, object> parameterValues)
            {
                this.DataProviderType = dataProviderType;
                this.ParameterValues = parameterValues;
            }

            public RDBDataProviderType DataProviderType
            {
                get;
                private set;
            }


            public Dictionary<string, object> ParameterValues
            {
                get;
                private set;
            }
        }


        #endregion

        public override string ParameterNamePrefix
        {
            get { return "@"; }
        }

        public override int ExecuteNonQuery(IRDBDataProviderExecuteNonQueryContext context)
        {
            throw new NotImplementedException();
        }
    }

}
