﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.Data.RDB.DataProvider.Providers
{
    public class MSSQLRDBDataProvider : BaseRDBDataProvider
    {        
        string _connString;
        public MSSQLRDBDataProvider(string connString)
        {
            _connString = connString;
            _dataManager = new SQLDataManager(_connString);
        }
        public const string UNIQUE_NAME = "MSSQL";
        public override string UniqueName
        {
            get { return UNIQUE_NAME; }
        }

        SQLDataManager _dataManager;

        public override string ConvertToDBParameterName(string parameterName)
        {
            return string.Concat("@", parameterName);
        }

        public override RDBResolvedQuery ResolveParameterDeclarations(IRDBDataProviderResolveParameterDeclarationsContext context)
        {
            StringBuilder builder = null;
            if (context.Parameters != null)
            {
                foreach (var prm in context.Parameters.Values)
                {
                    if (prm.Direction == RDBParameterDirection.Declared)
                    {
                        if (builder == null)
                            builder = new StringBuilder("DECLARE ");
                        else
                            builder.Append(", ");
                        builder.AppendFormat("{0} {1}", prm.DBParameterName, GetColumnDBType(prm.DBParameterName, prm.Type, prm.Size, prm.Precision));
                    }
                }
            }
            return builder != null ? new RDBResolvedQuery { QueryText = builder.ToString() } : null;
        }

        public override RDBResolvedQuery ResolveSelectQuery(IRDBDataProviderResolveSelectQueryContext context)
        {
            StringBuilder queryBuilder = new StringBuilder("");
            var rdbExpressionToDBQueryContext = new RDBExpressionToDBQueryContext(context, context.QueryBuilderContext);
            var rdbConditionToDBQueryContext = new RDBConditionToDBQueryContext(context, context.QueryBuilderContext);

            if (context.Table != null)
            {
                queryBuilder.Append(" FROM ");
                AddTableToDBQueryBuilder(queryBuilder, context.Table, context.TableAlias, context);
            }
            if (context.Joins != null && context.Joins.Count > 0)
                AddJoinsToQuery(context, queryBuilder, context.Joins, rdbConditionToDBQueryContext);

            StringBuilder columnQueryBuilder = new StringBuilder(" SELECT ");
            if (context.NbOfRecords.HasValue)
                columnQueryBuilder.AppendFormat(" TOP({0}) ", context.NbOfRecords.Value);
            List<RDBSelectColumn> selectColumns = context.Columns;
            if(selectColumns == null || selectColumns.Count == 0)
            {
                if (context.GroupBy != null && context.GroupBy.Columns != null && context.GroupBy.Columns.Count > 0)
                {
                    selectColumns = new List<RDBSelectColumn>();
                    selectColumns.AddRange(context.GroupBy.Columns);
                    if (context.GroupBy.AggregateColumns != null && context.GroupBy.AggregateColumns.Count > 0)
                        selectColumns.AddRange(context.GroupBy.AggregateColumns);
                }
                else
                {
                    throw new Exception("Select Columns are not defined for the Select Query");
                }
            }

            int colIndex = 0;
            foreach (var column in selectColumns)
            {
                if (colIndex > 0)
                    columnQueryBuilder.Append(", ");
                colIndex++;
                if (column.SetDBParameterName != null)
                {
                    columnQueryBuilder.Append(column.SetDBParameterName);
                    columnQueryBuilder.Append(" = ");
                }
                columnQueryBuilder.Append(column.Expression.ToDBQuery(rdbExpressionToDBQueryContext));
                columnQueryBuilder.Append(" ");
                columnQueryBuilder.Append(column.Alias);
            }

            queryBuilder.Insert(0, columnQueryBuilder.ToString());

            if (context.Condition != null)
            {
                string conditionDBQuery = context.Condition.ToDBQuery(rdbConditionToDBQueryContext);
                if (!string.IsNullOrEmpty(conditionDBQuery))
                {
                    queryBuilder.Append(" WHERE ");
                    queryBuilder.Append(conditionDBQuery);
                }
            }
            if (context.GroupBy != null && context.GroupBy.Columns != null && context.GroupBy.Columns.Count > 0)
            {
                queryBuilder.Append(" GROUP BY ");
                int groupByColumnIndex = 0;
                foreach (var column in context.GroupBy.Columns)
                {
                    if (groupByColumnIndex > 0)
                        queryBuilder.Append(", ");
                    groupByColumnIndex++;
                    queryBuilder.Append(column.Expression.ToDBQuery(rdbExpressionToDBQueryContext));
                }
                if (context.GroupBy.HavingCondition != null)
                {
                    string havingConditionDBQuery = context.GroupBy.HavingCondition.ToDBQuery(rdbConditionToDBQueryContext);
                    if (!string.IsNullOrEmpty(havingConditionDBQuery))
                    {
                        queryBuilder.Append(" HAVING ");
                        queryBuilder.Append(havingConditionDBQuery);
                    }
                }

            }
            if (context.SortColumns != null && context.SortColumns.Count > 0)
            {
                queryBuilder.Append(" ORDER BY ");
                int sortColumnIndex = 0;
                foreach (var column in context.SortColumns)
                {
                    if (sortColumnIndex > 0)
                        queryBuilder.Append(", ");
                    sortColumnIndex++;
                    if (column.Expression != null)
                        queryBuilder.Append(column.Expression.ToDBQuery(rdbExpressionToDBQueryContext));
                    else if (column.ColumnAlias != null)
                        queryBuilder.Append(column.ColumnAlias);
                    else throw new Exception(String.Format("Sort Column Name not defined. SortColumn Index '{0}'", sortColumnIndex));
                    queryBuilder.Append(column.SortDirection == RDBSortDirection.ASC ? " ASC " : " DESC ");
                }
            }
            return new RDBResolvedQuery
            {
                QueryText = queryBuilder.ToString()
            };
        }

        private void AddJoinsToQuery(IBaseRDBResolveQueryContext context, StringBuilder queryBuilder, List<RDBJoin> joins, RDBConditionToDBQueryContext rdbConditionToDBQueryContext)
        {
            foreach (var join in joins)
            {
                if (join.JoinType == RDBJoinType.Left)
                    queryBuilder.Append(" LEFT ");
                queryBuilder.Append(" JOIN ");
                AddTableToDBQueryBuilder(queryBuilder, join.Table, join.TableAlias, context);
                string joinConditionDBQuery = join.Condition.ToDBQuery(rdbConditionToDBQueryContext);
                joinConditionDBQuery.ThrowIfNull("joinConditionDBQuery");
                queryBuilder.Append(" ON ");
                queryBuilder.Append(joinConditionDBQuery);
            }
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
            RDBTableDefinitionQuerySource tableAsTableDefinitionQuerySource = context.Table as RDBTableDefinitionQuerySource;
            if (tableAsTableDefinitionQuerySource != null)
            {
                RDBTableDefinition tableDefinition = RDBSchemaManager.Current.GetTableDefinitionWithValidate(context.DataProvider, tableAsTableDefinitionQuerySource.TableName);
                if (!String.IsNullOrEmpty(tableDefinition.CreatedTimeColumnName))
                {
                    queryBuilder.Append(",");
                    valueBuilder.Append(",");
                    queryBuilder.Append(RDBSchemaManager.Current.GetColumnDBName(this, tableAsTableDefinitionQuerySource.TableName, tableDefinition.CreatedTimeColumnName));
                    valueBuilder.Append(new RDBNowDateTimeExpression().ToDBQuery(rdbExpressionToDBQueryContext));
                }
                if (!String.IsNullOrEmpty(tableDefinition.ModifiedTimeColumnName))
                {
                    queryBuilder.Append(",");
                    valueBuilder.Append(",");
                    queryBuilder.Append(RDBSchemaManager.Current.GetColumnDBName(this, tableAsTableDefinitionQuerySource.TableName, tableDefinition.ModifiedTimeColumnName));
                    valueBuilder.Append(new RDBNowDateTimeExpression().ToDBQuery(rdbExpressionToDBQueryContext));
                }
            }
            queryBuilder.Append(") VALUES (");
            queryBuilder.Append(valueBuilder.ToString());
            queryBuilder.Append(")");
            queryBuilder.AppendLine();
            if (!String.IsNullOrEmpty(context.GeneratedIdDBParameterName))
                queryBuilder.AppendFormat(" SET {0} = SCOPE_IDENTITY() ", context.GeneratedIdDBParameterName);

            return new RDBResolvedQuery
            {
                QueryText = queryBuilder.ToString()
            };
        }

        public override RDBResolvedQuery ResolveUpdateQuery(IRDBDataProviderResolveUpdateQueryContext context)
        {
            context.Table.ThrowIfNull("context.Table");
            context.ColumnValues.ThrowIfNull("context.ColumnValues");

            var rdbExpressionToDBQueryContext = new RDBExpressionToDBQueryContext(context, context.QueryBuilderContext);
            var rdbConditionToDBQueryContext = new RDBConditionToDBQueryContext(context, context.QueryBuilderContext);

            StringBuilder queryBuilder = new StringBuilder();
            if (context.Joins != null && context.Joins.Count > 0)
            {
                queryBuilder.Append(" FROM ");
                AddTableToDBQueryBuilder(queryBuilder, context.Table, context.TableAlias, context);
                AddJoinsToQuery(context, queryBuilder, context.Joins, rdbConditionToDBQueryContext);
            }
            else
            {
                AddTableToDBQueryBuilder(queryBuilder, context.Table, null, context);
            }

            StringBuilder columnQueryBuilder = new StringBuilder(" SET ");
            int colIndex = 0;

            foreach (var colVal in context.ColumnValues)
            {
                if (colIndex > 0)
                    columnQueryBuilder.Append(", ");
                colIndex++;
                if (colVal.ColumnName != null)
                {
                    var getDBColumnNameContext = new RDBTableQuerySourceGetDBColumnNameContext(colVal.ColumnName, context);
                    columnQueryBuilder.Append(context.Table.GetDBColumnName(getDBColumnNameContext));
                }
                else
                {
                    if (colVal.ExpressionToSet != null)
                        throw new NullReferenceException("colVal.ColumnName & colVal.ExpressionToSet");
                    columnQueryBuilder.Append(colVal.ExpressionToSet.ToDBQuery(rdbExpressionToDBQueryContext));
                }
                columnQueryBuilder.Append(" = ");
                columnQueryBuilder.Append(colVal.Value.ToDBQuery(rdbExpressionToDBQueryContext));
            }

            RDBTableDefinitionQuerySource tableAsTableDefinitionQuerySource = context.Table as RDBTableDefinitionQuerySource;
            if (tableAsTableDefinitionQuerySource != null)
            {
                RDBTableDefinition tableDefinition = RDBSchemaManager.Current.GetTableDefinitionWithValidate(context.DataProvider, tableAsTableDefinitionQuerySource.TableName);
                if (!String.IsNullOrEmpty(tableDefinition.ModifiedTimeColumnName))
                {
                    columnQueryBuilder.Append(",");
                    columnQueryBuilder.Append(RDBSchemaManager.Current.GetColumnDBName(this, tableAsTableDefinitionQuerySource.TableName, tableDefinition.ModifiedTimeColumnName));
                    columnQueryBuilder.Append(" = ");
                    columnQueryBuilder.Append(new RDBNowDateTimeExpression().ToDBQuery(rdbExpressionToDBQueryContext));
                }
            }

            if (context.Joins != null && context.Joins.Count > 0)
            {
                queryBuilder.Insert(0, String.Format("UPDATE {0} {1}", context.TableAlias, columnQueryBuilder.ToString()));
            }
            else
            {
                queryBuilder.Insert(0, "UPDATE ");
                queryBuilder.Append(columnQueryBuilder.ToString());
            }

            if (context.Condition != null)
            {
                string conditionDBQuery = context.Condition.ToDBQuery(rdbConditionToDBQueryContext);
                if (!string.IsNullOrEmpty(conditionDBQuery))
                {
                    queryBuilder.Append(" WHERE ");
                    queryBuilder.Append(conditionDBQuery);
                }
            }
            return new RDBResolvedQuery
            {
                QueryText = queryBuilder.ToString()
            };
        }

        public override RDBResolvedQuery ResolveDeleteQuery(IRDBDataProviderResolveDeleteQueryContext context)
        {
            context.Table.ThrowIfNull("context.Table");

            var rdbExpressionToDBQueryContext = new RDBExpressionToDBQueryContext(context, context.QueryBuilderContext);
            var rdbConditionToDBQueryContext = new RDBConditionToDBQueryContext(context, context.QueryBuilderContext);

            StringBuilder queryBuilder = new StringBuilder(" DELETE FROM ");
            if (context.Joins != null && context.Joins.Count > 0)
            {
                AddTableToDBQueryBuilder(queryBuilder, context.Table, context.TableAlias, context);
                AddJoinsToQuery(context, queryBuilder, context.Joins, rdbConditionToDBQueryContext);
            }
            else
            {
                AddTableToDBQueryBuilder(queryBuilder, context.Table, null, context);
            }

            if (context.Condition != null)
            {
                string conditionDBQuery = context.Condition.ToDBQuery(rdbConditionToDBQueryContext);
                if (!string.IsNullOrEmpty(conditionDBQuery))
                {
                    queryBuilder.Append(" WHERE ");
                    queryBuilder.Append(conditionDBQuery);
                }
            }
            return new RDBResolvedQuery
            {
                QueryText = queryBuilder.ToString()
            };
        }

        public override RDBResolvedQuery ResolveTempTableCreationQuery(IRDBDataProviderResolveTempTableCreationQueryContext context)
        {
            string tempTableName = String.Concat("#TempTable_", Guid.NewGuid().ToString().Replace("-", ""));
            context.TempTableName = tempTableName;
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append("CREATE TABLE ");
            queryBuilder.Append(tempTableName);
            queryBuilder.Append(" ( ");
            int colIndex = 0;
            foreach(var colDefEntry in context.Columns)
            {
                if (colIndex > 0)
                    queryBuilder.Append(",");
                colIndex++;
                queryBuilder.AppendLine();
                queryBuilder.Append(RDBSchemaManager.GetColumnDBName(context.DataProvider, colDefEntry.Key, colDefEntry.Value));
                queryBuilder.Append(" ");
                queryBuilder.Append(GetColumnDBType(colDefEntry.Key, colDefEntry.Value));
            }
            queryBuilder.Append(" ) ");
            return new RDBResolvedQuery
            {
                QueryText = queryBuilder.ToString()
            };
        }

        private string GetColumnDBType(string columnName, RDBTableColumnDefinition columnDef)
        {
            return GetColumnDBType(columnName, columnDef.DataType, columnDef.Size, columnDef.Precision);
        }

        public static string GetColumnDBType(string columnName, RDBDataType dataType, int? size, int? precision)
        {
            switch (dataType)
            {
                case RDBDataType.Int: return "int";
                case RDBDataType.BigInt: return "bigint";
                case RDBDataType.Decimal:
                    if (!size.HasValue)
                        throw new NullReferenceException(String.Format("Size of '{0}'", columnName));
                    if (!precision.HasValue)
                        throw new NullReferenceException(String.Format("Precision of '{0}'", columnName));
                    return String.Format("DECIMAL({0}, {1})", size.Value, precision.Value);
                case RDBDataType.DateTime: return "datetime";
                case RDBDataType.Varchar:
                    if (!size.HasValue)
                        return "varchar(max)";
                    else
                        return String.Format("varchar({0})", size.Value);
                case RDBDataType.NVarchar:
                    if (!size.HasValue)
                        return "nvarchar(max)";
                    else
                        return String.Format("nvarchar({0})", size.Value);
                case RDBDataType.UniqueIdentifier: return "uniqueidentifier";
                case RDBDataType.Boolean: return "bit";
                case RDBDataType.VarBinary:
                    if (!size.HasValue)
                        return "varbinary(max)";
                    else
                        return String.Format("varbinary({0})", size.Value);
                default:
                    throw new NotSupportedException(String.Format("DataType '{0}'", dataType.ToString()));
            }
        }

        private void AddTableToDBQueryBuilder(StringBuilder queryBuilder, IRDBTableQuerySource table, string tableAlias, IBaseRDBResolveQueryContext parentContext)
        {
            var tableQuerySourceContext = new RDBTableQuerySourceToDBQueryContext(parentContext);
            queryBuilder.Append(table.ToDBQuery(tableQuerySourceContext));
            if (tableAlias != null)
            {
                queryBuilder.Append(" ");
                queryBuilder.Append(tableAlias);
            }
            queryBuilder.Append(" ");
        }

        public override void ExecuteReader(IRDBDataProviderExecuteReaderContext context)
        {
            _dataManager.ExecuteReader(context.ResolvedQuery.QueryText, context.Parameters, (originalReader) => context.OnReaderReady(new MSSQLRDBDataReader(originalReader)));
        }

        public override int ExecuteNonQuery(IRDBDataProviderExecuteNonQueryContext context)
        {
            return _dataManager.ExecuteNonQuery(context.ResolvedQuery.QueryText, context.Parameters);
        }

        public override RDBFieldValue ExecuteScalar(IRDBDataProviderExecuteScalarContext context)
        {
            return new MSSQLRDBFieldValue(_dataManager.ExecuteScalar(context.ResolvedQuery.QueryText, context.Parameters));
        }

        public override BaseRDBStreamForBulkInsert InitializeStreamForBulkInsert(IRDBDataProviderInitializeStreamForBulkInsertContext context)
        {
            throw new NotImplementedException();
        }

        #region Private Classes

        private class SQLDataManager : Vanrise.Data.SQL.BaseSQLDataManager
        {
            string _connString;
            public SQLDataManager()
            {
            }
            public SQLDataManager(string connString)
            {
                _connString = connString;
            }

            protected override string GetConnectionString()
            {
                return _connString;
            }

            public void ExecuteReader(string sqlQuery, Dictionary<string, RDBParameter> parameters, Action<IDataReader> onReaderReady)
            {
                base.ExecuteReaderText(sqlQuery, onReaderReady, (cmd) =>
                {
                    if (parameters != null)
                    {
                        AddParameters(cmd, parameters);
                    }
                });
            }

            public int ExecuteNonQuery(string sqlQuery, Dictionary<string, RDBParameter> parameters)
            {
                return base.ExecuteNonQueryText(sqlQuery, (cmd) =>
                {
                    if (parameters != null)
                    {
                        AddParameters(cmd, parameters);
                    }
                });
            }

            public Object ExecuteScalar(string sqlQuery, Dictionary<string, RDBParameter> parameters)
            {
                return base.ExecuteScalarText(sqlQuery, (cmd) =>
                {
                    if (parameters != null)
                    {
                        AddParameters(cmd, parameters);
                    }
                });
            }

            private static void AddParameters(System.Data.Common.DbCommand cmd, Dictionary<string, RDBParameter> parameters)
            {
                foreach (var prm in parameters)
                {
                    if (prm.Value.Direction == RDBParameterDirection.In)
                        cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter(prm.Value.DBParameterName, prm.Value.Value != null ? prm.Value.Value : DBNull.Value));
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

        private class MSSQLRDBDataReader : CommonRDBDataReader
        {
            public MSSQLRDBDataReader(IDataReader originalReader)
                : base(originalReader)
            {
            }
        }
        
        private class MSSQLRDBFieldValue : CommonRDBFieldValue
        {
            public MSSQLRDBFieldValue(object value)
                : base(value)
            {

            }
        }

        #endregion

        public override string NowDateTimeFunction
        {
            get { return " GETDATE() "; }
        }
    }
}
