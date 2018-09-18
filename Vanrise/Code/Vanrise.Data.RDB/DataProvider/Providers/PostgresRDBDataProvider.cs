using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Npgsql;

namespace Vanrise.Data.RDB.DataProvider.Providers
{
    public class PostgresRDBDataProvider : CommonRDBDataProvider
    {
        public PostgresRDBDataProvider(string connString)
            : base(connString)
        {
            _dataManager = new PostgresDataManager(connString, this);
        }

        PostgresDataManager _dataManager;

        public override string GetTableDBName(string schemaName, string tableName)
        {
            if (!string.IsNullOrEmpty(schemaName))
                schemaName = string.Concat("\"", schemaName, "\"");
            if (!string.IsNullOrEmpty(tableName))
                tableName = string.Concat("\"", tableName, "\"");
            return base.GetTableDBName(schemaName, tableName);
        }

        public override string GetColumnDBName(string columnDBName)
        {
            if (!string.IsNullOrEmpty(columnDBName))
                columnDBName = string.Concat("\"", columnDBName, "\"");
            return base.GetColumnDBName(columnDBName);
        }

        public override string GetDBAlias(string alias)
        {
            if (string.IsNullOrEmpty(alias))
                return alias;
            return string.Concat("\"", alias, "\"");
        }

        public override string ConvertToDBParameterName(string parameterName, RDBParameterDirection parameterDirection)
        {
            return string.Concat("@", parameterName);
        }

        public const string UNIQUE_NAME = "POSTGRES";
        public override string UniqueName
        {
            get { return UNIQUE_NAME; }
        }

        public override bool UseLimitForTopRecords
        {
            get
            {
                return true;
            }
        }

        public override string GetQueryConcatenatedStrings(params string[] strings)
        {
            if (strings != null && strings.Length > 0)
                return string.Concat("CONCAT(", string.Join(", ", strings), ")");
            else
                return "";
        }

        public override RDBResolvedQuery ResolveInsertQuery(IRDBDataProviderResolveInsertQueryContext context)
        {
            context.Table.ThrowIfNull("context.Table");
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append("INSERT INTO ");
            AddTableToDBQueryBuilder(queryBuilder, context.Table, null, context);
            queryBuilder.Append(" (");
            var rdbExpressionToDBQueryContext = new RDBExpressionToDBQueryContext(context, context.QueryBuilderContext);
            var rdbConditionToDBQueryContext = new RDBConditionToDBQueryContext(context, context.QueryBuilderContext);

            if (context.ColumnValues != null && context.ColumnValues.Count > 0)
            {
                StringBuilder valueBuilder = new StringBuilder();

                int colIndex = 0;
                foreach (var colVal in context.ColumnValues)
                {
                    if (colIndex > 0)
                    {
                        queryBuilder.Append(",");
                        valueBuilder.Append(",");
                    }
                    colIndex++;
                    var getDBColumnNameContext = new RDBTableQuerySourceGetDBColumnNameContext(colVal.ColumnName, context);
                    queryBuilder.Append(context.Table.GetDBColumnName(getDBColumnNameContext));
                    valueBuilder.Append(colVal.Value.ToDBQuery(rdbExpressionToDBQueryContext));
                }
                queryBuilder.Append(") VALUES (");
                queryBuilder.Append(valueBuilder.ToString());
                queryBuilder.Append(")");
            }
            else if (context.SelectQuery != null)
            {
                context.SelectQuery.Columns.ThrowIfNull("context.SelectQuery.Columns");
                if (context.SelectQuery.Columns == null || context.SelectQuery.Columns.Count == 0)
                    throw new Exception("context.SelectQuery.Columns.Count = 0");
                int colIndex = 0;
                foreach (var selectColumn in context.SelectQuery.Columns)
                {
                    if (colIndex > 0)
                    {
                        queryBuilder.Append(",");
                    }
                    colIndex++;
                    var getDBColumnNameContext = new RDBTableQuerySourceGetDBColumnNameContext(selectColumn.Alias, context);
                    queryBuilder.Append(context.Table.GetDBColumnName(getDBColumnNameContext));
                }
                queryBuilder.Append(")");
                queryBuilder.AppendLine();

                var resolvedSelectQuery = context.SelectQuery.GetResolvedQuery(new RDBQueryGetResolvedQueryContext(context));
                StringBuilder selectQueryBuilder = new StringBuilder();
                foreach (var statement in resolvedSelectQuery.Statements)
                {
                    selectQueryBuilder.AppendLine(statement.TextStatement);
                }

                queryBuilder.Append(selectQueryBuilder.ToString());
            }
            else
            {
                throw new Exception("ColumnValues and SelectQuery are both null");
            }

            if (context.AddSelectGeneratedId)
            {
                var getIdColumnInfoContext = new RDBTableQuerySourceGetIdColumnInfoContext(context.DataProvider);
                context.Table.GetIdColumnInfo(getIdColumnInfoContext);
                getIdColumnInfoContext.IdColumnName.ThrowIfNull("getIdColumnInfoContext.IdColumnName");
                getIdColumnInfoContext.IdColumnDefinition.ThrowIfNull("getIdColumnInfoContext.IdColumnDefinition");
                string idColumnDBName = RDBSchemaManager.GetColumnDBName(context.DataProvider, getIdColumnInfoContext.IdColumnName, getIdColumnInfoContext.IdColumnDefinition);
                idColumnDBName.ThrowIfNull("idColumnDBName");
                queryBuilder.Append(" RETURNING ");
                queryBuilder.Append(idColumnDBName);
            }

            queryBuilder.AppendLine();

            var resolvedQuery = new RDBResolvedQuery();
            resolvedQuery.Statements.Add(new RDBResolvedQueryStatement { TextStatement = queryBuilder.ToString() });
            return resolvedQuery;
        }

        public override RDBResolvedQuery ResolveUpdateQuery(IRDBDataProviderResolveUpdateQueryContext context)
        {
            context.Table.ThrowIfNull("context.Table");
            context.ColumnValues.ThrowIfNull("context.ColumnValues");

            var rdbExpressionToDBQueryContext = new RDBExpressionToDBQueryContext(context, context.QueryBuilderContext);
            var rdbConditionToDBQueryContext = new RDBConditionToDBQueryContext(context, context.QueryBuilderContext);

            StringBuilder queryBuilder = new StringBuilder();

            string mainTableAlias = null;
            string mainJoinCondition = null;
            if (context.Joins != null && context.Joins.Count > 0)
            {
                queryBuilder.Append(" FROM ");

                if (context.Joins.Count == 1)
                {
                    mainTableAlias = context.TableAlias;
                    var firstJoin = context.Joins[0];
                    AddTableToDBQueryBuilder(queryBuilder, firstJoin.Table, firstJoin.TableAlias, context);
                    if (firstJoin.Condition != null)
                        mainJoinCondition = firstJoin.Condition.ToDBQuery(rdbConditionToDBQueryContext);
                }
                else
                {
                    mainTableAlias = string.Concat("MainTable_", Guid.NewGuid().ToString().Replace("-", ""));
                    AddTableToDBQueryBuilder(queryBuilder, context.Table, context.TableAlias, context);
                    AddJoinsToQuery(context, queryBuilder, context.Joins, rdbConditionToDBQueryContext);

                    var getIdColumnInfoContext = new RDBTableQuerySourceGetIdColumnInfoContext(context.DataProvider);
                    context.Table.GetIdColumnInfo(getIdColumnInfoContext);
                    getIdColumnInfoContext.IdColumnName.ThrowIfNull("getIdColumnInfoContext.IdColumnName");
                    getIdColumnInfoContext.IdColumnDefinition.ThrowIfNull("getIdColumnInfoContext.IdColumnDefinition");
                    string idColumnDBName = RDBSchemaManager.GetColumnDBName(context.DataProvider, getIdColumnInfoContext.IdColumnName, getIdColumnInfoContext.IdColumnDefinition);
                    idColumnDBName.ThrowIfNull("idColumnDBName");
                    mainJoinCondition = string.Concat(mainTableAlias, ".", idColumnDBName, " = ", context.TableAlias, ".", idColumnDBName);
                }
            }
            else
            {
                AddTableToDBQueryBuilder(queryBuilder, context.Table, null, context);
            }

            StringBuilder columnQueryBuilder = new StringBuilder(" SET ");

            Dictionary<string, RDBUpdateSelectColumn> selectColumnsByColumnName = null;
            StringBuilder selectColumnsQueryBuilder = null;

            if (context.SelectColumns != null && context.SelectColumns.Count > 0)
            {
                selectColumnsByColumnName = new Dictionary<string, RDBUpdateSelectColumn>();
                foreach (var selectColumn in context.SelectColumns)
                {
                    selectColumnsByColumnName.Add(selectColumn.ColumnName, selectColumn);
                }
            }

            int colIndex = 0;

            foreach (var colVal in context.ColumnValues)
            {
                if (colIndex > 0)
                    columnQueryBuilder.Append(", ");
                colIndex++;
                var getDBColumnNameContext = new RDBTableQuerySourceGetDBColumnNameContext(colVal.ColumnName, context);
                string columnDBName = context.Table.GetDBColumnName(getDBColumnNameContext);
                string value = colVal.Value.ToDBQuery(rdbExpressionToDBQueryContext);
                columnQueryBuilder.Append(columnDBName);
                columnQueryBuilder.Append(" = ");
                columnQueryBuilder.Append(value);

                RDBUpdateSelectColumn selectColumn;
                if (selectColumnsByColumnName != null && selectColumnsByColumnName.TryGetValue(colVal.ColumnName, out selectColumn))
                {
                    if (selectColumnsQueryBuilder == null)
                        selectColumnsQueryBuilder = new StringBuilder();
                    else
                        selectColumnsQueryBuilder.Append(", ");
                    selectColumnsQueryBuilder.Append(columnDBName);
                    selectColumnsQueryBuilder.Append(" ");
                    selectColumnsQueryBuilder.Append(GetDBAlias(selectColumn.Alias));

                    selectColumnsByColumnName.Remove(colVal.ColumnName);
                }
            }

            //add columns that are not added through the ColumnValues iterations
            if (selectColumnsByColumnName != null && selectColumnsByColumnName.Count > 0)
            {
                foreach (var selectColumnEntry in selectColumnsByColumnName)
                {
                    var getDBColumnNameContext = new RDBTableQuerySourceGetDBColumnNameContext(selectColumnEntry.Key, context);
                    string columnDBName = context.Table.GetDBColumnName(getDBColumnNameContext);

                    if (selectColumnsQueryBuilder == null)
                        selectColumnsQueryBuilder = new StringBuilder();
                    else
                        selectColumnsQueryBuilder.Append(", ");
                    selectColumnsQueryBuilder.Append(columnDBName);
                    selectColumnsQueryBuilder.Append(" ");
                    selectColumnsQueryBuilder.Append(GetDBAlias(selectColumnEntry.Value.Alias));
                }
            }

            if (context.Joins != null && context.Joins.Count > 0)
            {
                var tableQuerySourceContext = new RDBTableQuerySourceToDBQueryContext(context);
                string tableName = context.Table.ToDBQuery(tableQuerySourceContext);
                queryBuilder.Insert(0, String.Format("UPDATE {0} {1} {2}", tableName, mainTableAlias, columnQueryBuilder.ToString()));
            }
            else
            {
                queryBuilder.Insert(0, "UPDATE ");
                queryBuilder.Append(columnQueryBuilder.ToString());
            }

            bool joinConditionAdded = false;
            if (context.Condition != null)
            {
                string conditionDBQuery = context.Condition.ToDBQuery(rdbConditionToDBQueryContext);
                if (!string.IsNullOrEmpty(conditionDBQuery))
                {
                    queryBuilder.Append(" WHERE ");
                    if(mainJoinCondition != null)
                    {
                        queryBuilder.Append(mainJoinCondition);
                        queryBuilder.Append(" AND ");
                        joinConditionAdded = true;
                    }
                    queryBuilder.Append(conditionDBQuery);
                }
            }

            if(mainJoinCondition != null && !joinConditionAdded)
            {
                queryBuilder.Append(" WHERE ");
                queryBuilder.Append(mainJoinCondition);
            }

            if(selectColumnsQueryBuilder != null )
            {
                queryBuilder.Append(" RETURNING ");
                queryBuilder.Append(selectColumnsQueryBuilder.ToString());
            }
            var resolvedQuery = new RDBResolvedQuery();
            resolvedQuery.Statements.Add(new RDBResolvedQueryStatement { TextStatement = queryBuilder.ToString() });
            return resolvedQuery;
        }
        
        public override void ExecuteReader(IRDBDataProviderExecuteReaderContext context)
        {
            _dataManager.ExecuteReader(context.Query, context.Parameters, context.ExecuteTransactional, (originalReader) => context.OnReaderReady(new PostgresRDBDataReader(originalReader)));
        }

        public override int ExecuteNonQuery(IRDBDataProviderExecuteNonQueryContext context)
        {
            return _dataManager.ExecuteNonQuery(context.Query, context.Parameters, context.ExecuteTransactional);
        }

        public override RDBFieldValue ExecuteScalar(IRDBDataProviderExecuteScalarContext context)
        {
            return new PostgresRDBFieldValue(_dataManager.ExecuteScalar(context.Query, context.Parameters, context.ExecuteTransactional));
        }

        public override void AppendTableColumnDefinition(StringBuilder columnsQueryBuilder, string columnName, string columnDBName, RDBTableColumnDefinition columnDefinition, bool notNullable, bool isIdentityColumn)
        {
            columnsQueryBuilder.Append(columnDBName);
            columnsQueryBuilder.Append(" ");
            if (isIdentityColumn)
            {
                switch(columnDefinition.DataType)
                {
                    case RDBDataType.Int: columnsQueryBuilder.Append(" serial "); break;
                    case RDBDataType.BigInt: columnsQueryBuilder.Append(" bigserial "); break;
                    default: throw new NotSupportedException(string.Format("columnDefinition.DataType '{0}'", columnDefinition.DataType.ToString()));
                }
            }
            else
            {
                columnsQueryBuilder.Append(GetColumnDBType(columnName, columnDefinition));
            }
            columnsQueryBuilder.Append(notNullable ? " NOT NULL " : " NULL ");
        }

        public override string NowDateTimeFunction
        {
            get { return " current_timestamp "; }
        }

        protected override string GetNVarcharDBType(int? size)
        {
            if (!size.HasValue)
                return "text";
            else
                return "character varying";
        }

        protected override string GetVarcharDBType(int? size)
        {
            if (!size.HasValue)
                return "text";
            else
                return "character varying";
        }

        protected override string GetIntDBType()
        {
            return "integer";
        }

        protected override string GetDecimalDBType(string columnName, int? size, int? precision)
        {
            if (!size.HasValue)
                throw new NullReferenceException(String.Format("Size of '{0}'", columnName));
            if (!precision.HasValue)
                throw new NullReferenceException(String.Format("Precision of '{0}'", columnName));
            return String.Format("numeric({0}, {1})", size.Value, precision.Value);
        }

        protected override string GetDatetimeDBType()
        {
            return "timestamp without time zone";
        }

        protected override string GetBooleanDBType()
        {
            return "boolean";
        }

        protected override string GetGuidDBType()
        {
            return "uuid";
        }

        protected override string GetVarBinaryDBType(int? size)
        {
            return "bytea";
        }

        protected override string GenerateTempTableName()
        {
            return String.Concat("TempTable_", Guid.NewGuid().ToString().Replace("-", ""));
        }

        protected override string GenerateCreateTempTableQuery(string tempTableName, string columns)
        {
            return string.Concat("CREATE TEMPORARY TABLE ", tempTableName, " ", columns);
        }

        protected override string GetGenerateIdFunction()
        {
            throw new NotImplementedException();
            //return "LASTVAL()";
        }
        
        #region Private Classes

        private class PostgresDataManager : Vanrise.Data.SQL.BaseSQLDataManager
        {
            string _connString;
            PostgresRDBDataProvider _dataProvider;

            public PostgresDataManager(string connString, PostgresRDBDataProvider dataProvider)
            {
                _connString = connString;
                _dataProvider = dataProvider;
            }

            public void ExecuteReader(RDBResolvedQuery query, Dictionary<string, RDBParameter> parameters, bool executeTransactional, Action<IDataReader> onReaderReady)
            {
                CreateCommandWithParams(query, parameters, executeTransactional,
                    (cmd) =>
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            onReaderReady(reader);
                            //cmd.Cancel();
                            //reader.Close();
                        }
                    });
            }

            public int ExecuteNonQuery(RDBResolvedQuery query, Dictionary<string, RDBParameter> parameters, bool executeTransactional)
            {
                int rslt = 0;
                CreateCommandWithParams(query, parameters, executeTransactional,
                    (cmd) =>
                    {
                        rslt = cmd.ExecuteNonQuery();
                    });
                return rslt;
            }

            public Object ExecuteScalar(RDBResolvedQuery query, Dictionary<string, RDBParameter> parameters, bool executeTransactional)
            {
                Object rslt = 0;
                CreateCommandWithParams(query, parameters, executeTransactional,
                    (cmd) =>
                    {
                        rslt = cmd.ExecuteScalar();
                    });
                return rslt;
            }

            void CreateCommandWithParams(RDBResolvedQuery query, Dictionary<string, RDBParameter> parameters, bool executeTransactional, Action<NpgsqlCommand> onCommandReady)
            {
                using (var connection = new NpgsqlConnection(_connString))
                {
                    connection.Open();

                    using (var cmd = new NpgsqlCommand(GetQueryAsText(query), connection))
                    {
                        if (parameters != null)
                            AddParameters(cmd, parameters);
                        if (executeTransactional)
                        {
                            using (var transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted))
                            {
                                cmd.Transaction = transaction;
                                try
                                {
                                    onCommandReady(cmd);
                                    transaction.Commit();
                                }
                                catch
                                {
                                    transaction.Rollback();
                                    throw;
                                }
                            }
                        }
                        else
                        {
                            onCommandReady(cmd);
                        }
                    }

                    connection.Close();
                }
            }


            public string GetQueryAsText(RDBResolvedQuery resolvedQuery)
            {
                var queryBuilder = new StringBuilder();
                foreach (var statement in resolvedQuery.Statements)
                {
                    queryBuilder.Append(statement.TextStatement);
                    queryBuilder.Append(";");
                    queryBuilder.AppendLine();
                    queryBuilder.AppendLine();
                }
                return queryBuilder.ToString();
            }


            private static void AddParameters(NpgsqlCommand cmd, Dictionary<string, RDBParameter> parameters)
            {
                foreach (var prm in parameters)
                {
                    if (prm.Value.Direction == RDBParameterDirection.In)
                        cmd.Parameters.Add(new NpgsqlParameter(prm.Value.DBParameterName, prm.Value.Value != null ? prm.Value.Value : DBNull.Value));
                    else
                        throw new Exception(string.Format("Parameter Direction '{0}' not supported. Parameter Name '{1}'", prm.Value.Direction.ToString(), prm.Key));
                }
            }

            //private DbType GetSQLDBType(RDBDataType dataType, out int? size, out byte? precision)
            //{
            //    size = null;
            //    precision = null;
            //    switch (dataType)
            //    {
            //        case RDBDataType.Int: return DbType.Int32;
            //        case RDBDataType.BigInt: return DbType.Int64;
            //        case RDBDataType.Decimal:
            //            size = 20;
            //            precision = 8;
            //            return DbType.Decimal;
            //        case RDBDataType.DateTime: return DbType.DateTime;
            //        case RDBDataType.Varchar: return DbType.String;
            //        case RDBDataType.NVarchar: return DbType.String;
            //        case RDBDataType.UniqueIdentifier: return DbType.Guid;
            //        case RDBDataType.Boolean: return DbType.Boolean;
            //        case RDBDataType.VarBinary: return DbType.Binary;
            //        default:
            //            throw new NotSupportedException(String.Format("DataType '{0}'", dataType.ToString()));
            //    }
            //}
        }

        private class PostgresRDBDataReader : CommonRDBDataReader
        {
            public PostgresRDBDataReader(IDataReader originalReader)
                : base(originalReader)
            {
            }
        }

        private class PostgresRDBFieldValue : CommonRDBFieldValue
        {
            public PostgresRDBFieldValue(object value)
                : base(value)
            {

            }
        }

        #endregion
    }
}
