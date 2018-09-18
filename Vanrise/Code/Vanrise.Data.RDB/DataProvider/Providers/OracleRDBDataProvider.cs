using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.Data.RDB.DataProvider.Providers
{
    public class OracleRDBDataProvider : BaseRDBDataProvider
    {
        public OracleRDBDataProvider(string connString)
         {
             _dataManager = new OracleDataManager(connString);
         }

        OracleDataManager _dataManager;

        public override string GetTableDBName(string schemaName, string tableName)
        {
            if (schemaName == "VR_AccountBalance")
                schemaName = "VRAcBal";
            else if (schemaName == "VR_Invoice")
                schemaName = "VRInv";
            if (!String.IsNullOrEmpty(schemaName))
                return String.Concat(schemaName, "_", tableName);
            else
                return tableName;
        }

        public override string GetQueryConcatenatedStrings(params string[] strings)
        {
            if (strings != null && strings.Length > 0)
                return string.Concat("CONCAT(", string.Join(", ", strings), ")");
            else
                return "";
        }

        //public override RDBResolvedQueryStatement GetStatementSetParameterValue(string parameterDBName, string parameterValue)
        //{
        //    return new OracleRDBResolvedQueryStatement(OracleRDBResolvedQueryStatementType.Regular) { TextStatement = string.Concat(parameterDBName, " := ", parameterValue) };
        //}

        //public override RDBResolvedQuery ResolveParameterDeclarations(IRDBDataProviderResolveParameterDeclarationsContext context)
        //{
        //    StringBuilder builder = null;
        //    if (context.Parameters != null)
        //    {
        //        foreach (var prm in context.Parameters.Values)
        //        {
        //            if (prm.Direction == RDBParameterDirection.Declared)
        //            {
        //                if (builder == null)
        //                    builder = new StringBuilder("DECLARE ");
        //                builder.AppendFormat("{0} {1};", prm.DBParameterName, GetColumnDBType(prm.DBParameterName, prm.Type, prm.Size, prm.Precision));
        //            }
        //        }
        //    }
        //    var resolvedQuery = new RDBResolvedQuery();
        //    if (builder != null)
        //        resolvedQuery.Statements.Add(builder.ToString());
        //    return resolvedQuery;
        //}

        public override RDBResolvedQuery ResolveSelectQuery(IRDBDataProviderResolveSelectQueryContext context)
        {
            StringBuilder queryBuilder = new StringBuilder("");
            var rdbExpressionToDBQueryContext = new RDBExpressionToDBQueryContext(context, context.QueryBuilderContext);
            var rdbConditionToDBQueryContext = new RDBConditionToDBQueryContext(context, context.QueryBuilderContext);

            queryBuilder.Append(" FROM ");
            if (context.Table != null)
            {
                AddTableToDBQueryBuilder(queryBuilder, context.Table, context.TableAlias, context);
            }
            else
                queryBuilder.Append(" DUAL ");
            if (context.Joins != null && context.Joins.Count > 0)
                AddJoinsToQuery(context, queryBuilder, context.Joins, rdbConditionToDBQueryContext);

            StringBuilder columnQueryBuilder = new StringBuilder(" SELECT ");
            
            List<RDBSelectColumn> selectColumns = context.Columns;
            if (selectColumns == null || selectColumns.Count == 0)
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

                columnQueryBuilder.Append(column.Expression.ToDBQuery(rdbExpressionToDBQueryContext));
                columnQueryBuilder.Append(" ");
                columnQueryBuilder.Append(column.Alias);
            }

            queryBuilder.Insert(0, columnQueryBuilder.ToString());

            if (context.IsMainStatement)
            {
                //string cursorIndex = (context.Parameters.Values.Count(prm => prm.Type == RDBDataType.Cursor) + 1).ToString();
                string cursorParameterName = string.Concat("cursor_", Guid.NewGuid().ToString().Replace("-", "").Substring(0,22));
                string cursorDBParameterName = string.Concat(":", cursorParameterName);
                queryBuilder.Insert(0, string.Concat(" OPEN ", cursorDBParameterName, " FOR "));
                context.AddParameter(new RDBParameter { Name = cursorParameterName, DBParameterName = cursorDBParameterName, Direction = RDBParameterDirection.In, Type = RDBDataType.Cursor });
            }

            List<string> conditions = new List<string>();
            if (context.Condition != null)
            {
                string conditionDBQuery = context.Condition.ToDBQuery(rdbConditionToDBQueryContext);
                if (!string.IsNullOrEmpty(conditionDBQuery))
                {
                    conditions.Add(conditionDBQuery);
                }
            }

            if (context.NbOfRecords.HasValue)
                conditions.Add(string.Concat(" ROWNUM <= ", context.NbOfRecords.Value, " "));

            if (conditions.Count > 0)
            {
                queryBuilder.Append(" WHERE ");
                queryBuilder.Append(string.Join(" AND ", conditions));
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

            
            var resolvedQuery = new RDBResolvedQuery();
            resolvedQuery.Statements.Add(new OracleRDBResolvedQueryStatement(OracleRDBResolvedQueryStatementType.Regular) { TextStatement = queryBuilder.ToString() });
            return resolvedQuery;
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
            OracleRDBResolvedQueryStatement selectIdStatement = null;
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

                if(context.AddSelectGeneratedId)
                {
                    var getIdColumnInfoContext = new RDBTableQuerySourceGetIdColumnInfoContext(context.DataProvider);
                    context.Table.GetIdColumnInfo(getIdColumnInfoContext);
                    getIdColumnInfoContext.IdColumnName.ThrowIfNull("getIdColumnInfoContext.IdColumnName");
                    getIdColumnInfoContext.IdColumnDefinition.ThrowIfNull("getIdColumnInfoContext.IdColumnDefinition");
                    string idColumnDBName = RDBSchemaManager.GetColumnDBName(context.DataProvider, getIdColumnInfoContext.IdColumnName, getIdColumnInfoContext.IdColumnDefinition);
                    idColumnDBName.ThrowIfNull("idColumnDBName");

                    string idParameterName = string.Concat("GenId_", Guid.NewGuid().ToString().Replace("-", "").Substring(0, 23));
                    string idDBParameterName = this.ConvertToDBParameterName(idParameterName, RDBParameterDirection.Declared);
                    context.AddParameter(new RDBParameter
                        {
                            Name = idParameterName,
                            DBParameterName = idDBParameterName,
                            Direction = RDBParameterDirection.Declared,
                            Type = getIdColumnInfoContext.IdColumnDefinition.DataType,
                            Size = getIdColumnInfoContext.IdColumnDefinition.Size,
                            Precision = getIdColumnInfoContext.IdColumnDefinition.Precision
                        });
                    queryBuilder.Append(string.Concat(" RETURNING ", idColumnDBName, " INTO ", idDBParameterName));

                    string cursorParameterName = string.Concat("cursor_", Guid.NewGuid().ToString().Replace("-", "").Substring(0, 22));
                    string cursorDBParameterName = this.ConvertToDBParameterName(cursorParameterName, RDBParameterDirection.In);
                    context.AddParameter(new RDBParameter { Name = cursorParameterName, DBParameterName = cursorDBParameterName, Direction = RDBParameterDirection.In, Type = RDBDataType.Cursor });

                    selectIdStatement = new OracleRDBResolvedQueryStatement(OracleRDBResolvedQueryStatementType.Regular)
                    {
                        TextStatement = string.Concat(" OPEN ", cursorDBParameterName, " FOR SELECT ", idDBParameterName, " From DUAL ")
                    };
                }

                queryBuilder.AppendLine();
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
            var resolvedQuery = new RDBResolvedQuery();
            resolvedQuery.Statements.Add(new OracleRDBResolvedQueryStatement(OracleRDBResolvedQueryStatementType.Regular) { TextStatement = queryBuilder.ToString() });
            if (selectIdStatement != null)
                resolvedQuery.Statements.Add(selectIdStatement);
            return resolvedQuery;
        }

        public override RDBResolvedQuery ResolveUpdateQuery(IRDBDataProviderResolveUpdateQueryContext context)
        {
            context.Table.ThrowIfNull("context.Table");
            context.ColumnValues.ThrowIfNull("context.ColumnValues");

            var rdbExpressionToDBQueryContext = new RDBExpressionToDBQueryContext(context, context.QueryBuilderContext);
            var rdbConditionToDBQueryContext = new RDBConditionToDBQueryContext(context, context.QueryBuilderContext);

            StringBuilder queryBuilder = new StringBuilder("UPDATE ");

            bool conditionAdded = false;

            StringBuilder columnQueryBuilder = new StringBuilder(" SET ");

            List<string> columnsToSetToParameters = null;
            List<string> parameterDBNamesToSet = null;


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


            if (context.Joins != null && context.Joins.Count > 0)
            {
                queryBuilder.Append("( SELECT ");

                context.TableAlias.ThrowIfNull("context.TableAlias");

                int colIndex = 0;
                foreach (var colVal in context.ColumnValues)
                {
                    if (colIndex > 0)
                    {
                        queryBuilder.Append(", ");
                        columnQueryBuilder.Append(", ");
                    }
                    colIndex++;
                    var getDBColumnNameContext = new RDBTableQuerySourceGetDBColumnNameContext(colVal.ColumnName, context);
                    string columnDBName = string.Concat(context.TableAlias, ".", context.Table.GetDBColumnName(getDBColumnNameContext));
                    string value = colVal.Value.ToDBQuery(rdbExpressionToDBQueryContext);

                    string colAlias = string.Concat("Col_", colIndex);
                    string valAlias = string.Concat("Val_", colIndex);

                    queryBuilder.Append(columnDBName);
                    queryBuilder.Append(" ");
                    queryBuilder.Append(colAlias);

                    queryBuilder.Append(", ");

                    queryBuilder.Append(value);
                    queryBuilder.Append(" ");
                    queryBuilder.Append(valAlias);

                    columnQueryBuilder.Append(colAlias);
                    columnQueryBuilder.Append("=");
                    columnQueryBuilder.Append(valAlias);
                }

                queryBuilder.Append(" FROM ");
                AddTableToDBQueryBuilder(queryBuilder, context.Table, context.TableAlias, context);
                AddJoinsToQuery(context, queryBuilder, context.Joins, rdbConditionToDBQueryContext);

                if (context.Condition != null)
                {
                    string conditionDBQuery = context.Condition.ToDBQuery(rdbConditionToDBQueryContext);
                    if (!string.IsNullOrEmpty(conditionDBQuery))
                    {
                        queryBuilder.Append(" WHERE ");
                        queryBuilder.Append(conditionDBQuery);
                    }
                    conditionAdded = true;
                }
                queryBuilder.Append(")");
            }
            else
            {
                AddTableToDBQueryBuilder(queryBuilder, context.Table, null, context);

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
                        string selectColumnDBParameterName = null;
                        DefineParameterFromColumn(context, colVal.ColumnName, out selectColumnDBParameterName);
                        if (selectColumnsQueryBuilder == null)
                            selectColumnsQueryBuilder = new StringBuilder();
                        else
                            selectColumnsQueryBuilder.Append(", ");
                        selectColumnsQueryBuilder.Append(selectColumnDBParameterName);
                        selectColumnsQueryBuilder.Append(" ");
                        selectColumnsQueryBuilder.Append(selectColumn.Alias);

                        if (columnsToSetToParameters == null)
                        {
                            columnsToSetToParameters = new List<string>();
                            parameterDBNamesToSet = new List<string>();
                        }

                        columnsToSetToParameters.Add(columnDBName);
                        parameterDBNamesToSet.Add(selectColumnDBParameterName);

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

                        string selectColumnDBParameterName;
                        DefineParameterFromColumn(context, selectColumnEntry.Key, out selectColumnDBParameterName);

                        if (selectColumnsQueryBuilder == null)
                            selectColumnsQueryBuilder = new StringBuilder();
                        else
                            selectColumnsQueryBuilder.Append(", ");
                        selectColumnsQueryBuilder.Append(selectColumnDBParameterName);
                        selectColumnsQueryBuilder.Append(" ");
                        selectColumnsQueryBuilder.Append(selectColumnEntry.Value.Alias);

                        if (columnsToSetToParameters == null)
                        {
                            columnsToSetToParameters = new List<string>();
                            parameterDBNamesToSet = new List<string>();
                        }

                        columnsToSetToParameters.Add(columnDBName);
                        parameterDBNamesToSet.Add(selectColumnDBParameterName);
                    }
                }
            }

            queryBuilder.Append(columnQueryBuilder.ToString());

            if (!conditionAdded && context.Condition != null)
            {
                string conditionDBQuery = context.Condition.ToDBQuery(rdbConditionToDBQueryContext);
                if (!string.IsNullOrEmpty(conditionDBQuery))
                {
                    queryBuilder.Append(" WHERE ");
                    queryBuilder.Append(conditionDBQuery);
                }
            }

            if (columnsToSetToParameters != null)
                queryBuilder.Append(string.Concat(" RETURNING ", string.Join(",", columnsToSetToParameters), " INTO ", string.Join(",", parameterDBNamesToSet)));

            var resolvedQuery = new RDBResolvedQuery();
            resolvedQuery.Statements.Add(new OracleRDBResolvedQueryStatement(OracleRDBResolvedQueryStatementType.Regular) { TextStatement = queryBuilder.ToString() });
            if (selectColumnsQueryBuilder != null)
            {
                string cursorParameterName = string.Concat("cursor_", Guid.NewGuid().ToString().Replace("-", "").Substring(0, 22));
                string cursorDBParameterName = this.ConvertToDBParameterName(cursorParameterName, RDBParameterDirection.In);
                context.AddParameter(new RDBParameter { Name = cursorParameterName, DBParameterName = cursorDBParameterName, Direction = RDBParameterDirection.In, Type = RDBDataType.Cursor });

                resolvedQuery.Statements.Add(new OracleRDBResolvedQueryStatement(OracleRDBResolvedQueryStatementType.Regular)
                {
                    TextStatement = string.Concat(" OPEN ", cursorDBParameterName, " FOR SELECT ", selectColumnsQueryBuilder.ToString(), " From DUAL ")
                });
            }
            return resolvedQuery;
        }

        private void DefineParameterFromColumn(IRDBDataProviderResolveUpdateQueryContext context, string columnName, out string dbParameterName)
        {
            var parameterName = string.Concat("Col_", Guid.NewGuid().ToString().Replace("-", "").Substring(0, 25));
            dbParameterName = this.ConvertToDBParameterName(parameterName, RDBParameterDirection.Declared);
            var getColumnDefinitionContext = new RDBTableQuerySourceGetColumnDefinitionContext(columnName, context);
            context.Table.GetColumnDefinition(getColumnDefinitionContext);
            getColumnDefinitionContext.ColumnDefinition.ThrowIfNull("getColumnDefinitionContext.ColumnDefinition", columnName);
            var selectColumnDefinition = getColumnDefinitionContext.ColumnDefinition;
            context.AddParameter(new RDBParameter
            {
                Name = parameterName,
                DBParameterName = dbParameterName,
                Direction = RDBParameterDirection.Declared,
                Type = selectColumnDefinition.DataType,
                Size = selectColumnDefinition.Size,
                Precision = selectColumnDefinition.Precision
            });
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
            var resolvedQuery = new RDBResolvedQuery();
            resolvedQuery.Statements.Add(new OracleRDBResolvedQueryStatement(OracleRDBResolvedQueryStatementType.Regular) { TextStatement = queryBuilder.ToString() });
            return resolvedQuery;
        }

        public override RDBResolvedQuery ResolveTableCreationQuery(IRDBDataProviderResolveTableCreationQueryContext context)
        {
            throw new NotImplementedException();
        }

        public override RDBResolvedQuery ResolveTempTableCreationQuery(IRDBDataProviderResolveTempTableCreationQueryContext context)
        {
            string tempTableName = GenerateTempTableName();
            context.TempTableName = tempTableName;

            var columnsQueryBuilder = new StringBuilder();
            columnsQueryBuilder.Append(" ( ");

            int colIndex = 0;
            List<string> primaryKeyDBColumnNames = null;
            foreach (var colDefEntry in context.Columns)
            {
                if (colIndex > 0)
                    columnsQueryBuilder.Append(",");
                colIndex++;
                columnsQueryBuilder.AppendLine();
                string columnDBName = RDBSchemaManager.GetColumnDBName(context.DataProvider, colDefEntry.Key, colDefEntry.Value.ColumnDefinition);
                columnsQueryBuilder.Append(columnDBName);
                columnsQueryBuilder.Append(" ");
                columnsQueryBuilder.Append(GetColumnDBType(colDefEntry.Key, colDefEntry.Value.ColumnDefinition));
                if (colDefEntry.Value.IsPrimaryKey)
                {
                    if (primaryKeyDBColumnNames == null)
                        primaryKeyDBColumnNames = new List<string>();
                    primaryKeyDBColumnNames.Add(columnDBName);
                }
            }
            if (primaryKeyDBColumnNames != null)
            {
                columnsQueryBuilder.Append(", Primary Key (");
                columnsQueryBuilder.Append(string.Join(",", primaryKeyDBColumnNames));
                columnsQueryBuilder.Append(")");
            }

            columnsQueryBuilder.Append(" ) ");

            var resolvedQuery = new RDBResolvedQuery();
            resolvedQuery.Statements.Add(new OracleRDBResolvedQueryStatement(OracleRDBResolvedQueryStatementType.CreateTempTable) { 
                TextStatement = string.Concat("CREATE GLOBAL TEMPORARY TABLE ", tempTableName, " ", columnsQueryBuilder.ToString(), " ON COMMIT PRESERVE ROWS ") });
            return resolvedQuery;
        }

        public override RDBResolvedQuery ResolveTempTableDropQuery(IRDBDataProviderResolveTempTableDropQueryContext context)
        {
            var resolvedQuery = new RDBResolvedQuery();
            resolvedQuery.Statements.Add(new OracleRDBResolvedQueryStatement(OracleRDBResolvedQueryStatementType.DropTempTable) { TextStatement = string.Concat("DROP TABLE ", context.TempTableName) });
            return resolvedQuery;
        }

        protected virtual string GenerateTempTableName()
        {
            return String.Concat("TempTable_", Guid.NewGuid().ToString().Replace("-", "").Substring(0, 20));
        }

        private static string GetColumnDBType(string columnName, RDBTableColumnDefinition columnDef)
        {
            return GetColumnDBType(columnName, columnDef.DataType, columnDef.Size, columnDef.Precision);
        }

        public static string GetColumnDBType(string columnName, RDBDataType dataType, int? size, int? precision)
        {
            switch (dataType)
            {
                case RDBDataType.Int: return "int";
                case RDBDataType.BigInt: return "Number(19)";
                case RDBDataType.Decimal:
                    if (!size.HasValue)
                        throw new NullReferenceException(String.Format("Size of '{0}'", columnName));
                    if (!precision.HasValue)
                        throw new NullReferenceException(String.Format("Precision of '{0}'", columnName));
                    return String.Format("Number({0}, {1})", size.Value, precision.Value);
                case RDBDataType.DateTime: return "date";
                case RDBDataType.Varchar:
                    return GetVarcharDBType(size);
                case RDBDataType.NVarchar:
                    return GetNVarcharDBType(size);
                case RDBDataType.UniqueIdentifier: return GetGuidDBType();
                case RDBDataType.Boolean: return "Number(1)";
                case RDBDataType.VarBinary:
                    return GetVarBinaryDBType(size);
                default:
                    throw new NotSupportedException(String.Format("DataType '{0}'", dataType.ToString()));
            }
        }

        protected static string GetVarBinaryDBType(int? size)
        {
            return "BLOB";
        }

        protected static string GetNVarcharDBType(int? size)
        {
            if (!size.HasValue)
                return "NCLOB";
            else
                return String.Format("varchar2({0} char)", size.Value);
        }

        protected static string GetVarcharDBType(int? size)
        {
            if (!size.HasValue)
                return "NCLOB";
            else
                return String.Format("varchar2({0} char)", size.Value);
        }

        protected static string GetGuidDBType()
        {
            return "RAW(16)";
        }


        protected void AddTableToDBQueryBuilder(StringBuilder queryBuilder, IRDBTableQuerySource table, string tableAlias, IBaseRDBResolveQueryContext parentContext)
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

        protected void AddJoinsToQuery(IBaseRDBResolveQueryContext context, StringBuilder queryBuilder, List<RDBJoin> joins, RDBConditionToDBQueryContext rdbConditionToDBQueryContext)
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

        public override BaseRDBStreamForBulkInsert InitializeStreamForBulkInsert(IRDBDataProviderInitializeStreamForBulkInsertContext context)
        {
            throw new NotImplementedException();
        }

        public const string UNIQUE_NAME = "ORACLE";
        public override string UniqueName
        {
            get { return UNIQUE_NAME; }
        }

        public override string NowDateTimeFunction
        {
            get { return "SYSDATE"; }
        }

        const string ORACLE_EMPTY_STRING_VALUE = "ORCLEMTYNTNUL";

        public override string EmptyStringValue
        {
            get
            {
                return ORACLE_EMPTY_STRING_VALUE;
            }
        }

        public override string ConvertToDBParameterName(string parameterName, RDBParameterDirection parameterDirection)
        {
            return parameterDirection == RDBParameterDirection.In ? string.Concat(":", parameterName) : parameterName;
        }

        public override void ExecuteReader(IRDBDataProviderExecuteReaderContext context)
        {
            _dataManager.ExecuteReader(context.Query, context.Parameters, context.ExecuteTransactional, (originalReader) => context.OnReaderReady(new OracleRDBDataReader(originalReader)));
        }

        public override int ExecuteNonQuery(IRDBDataProviderExecuteNonQueryContext context)
        {
            return _dataManager.ExecuteNonQuery(context.Query, context.Parameters, context.ExecuteTransactional);
        }

        public override RDBFieldValue ExecuteScalar(IRDBDataProviderExecuteScalarContext context)
        {
            return new OracleRDBFieldValue(_dataManager.ExecuteScalar(context.Query, context.Parameters, context.ExecuteTransactional));
        }

        #region Private Classes

        public enum OracleRDBResolvedQueryStatementType { Regular, CreateTempTable, DropTempTable }

        private class OracleRDBResolvedQueryStatement : RDBResolvedQueryStatement
        {
            public OracleRDBResolvedQueryStatement(OracleRDBResolvedQueryStatementType statementType)
            {
                this.StatementType = statementType;
            }

            public OracleRDBResolvedQueryStatementType StatementType { get; private set; }
        }

        private class OracleDataManager
        {
            string _connString;

            public OracleDataManager(string connString)
            {
                _connString = connString;
            }

            public void ExecuteReader(RDBResolvedQuery query, Dictionary<string, RDBParameter> parameters, bool executeTransactional, Action<IDataReader> onReaderReady)
            {
                CreateCommand(query, parameters, executeTransactional,
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
                int nbOfRowsAffected;
                CreateCommand(query, parameters, executeTransactional,
                    (cmd) =>
                    {
                        rslt = cmd.ExecuteNonQuery();
                    }, true, out nbOfRowsAffected);
                return nbOfRowsAffected;
            }

            public Object ExecuteScalar(RDBResolvedQuery query, Dictionary<string, RDBParameter> parameters, bool executeTransactional)
            {
                Object rslt = 0;
                CreateCommand(query, parameters, executeTransactional,
                    (cmd) =>
                    {
                        rslt = cmd.ExecuteScalar();
                    });
                return rslt;
            }

            void CreateCommand(RDBResolvedQuery query, Dictionary<string, RDBParameter> parameters, bool executeTransactional, Action<OracleCommand> onCommandReady)
            {
                int nbOfRowsAffected;
                CreateCommand(query, parameters, executeTransactional, onCommandReady, false, out nbOfRowsAffected);
            }

            void CreateCommand(RDBResolvedQuery query, Dictionary<string, RDBParameter> parameters, bool executeTransactional, Action<OracleCommand> onCommandReady, bool readNbOfRowsAffected, out int nbOfRowsAffected)
            {
                executeTransactional = false;
                string createTempTablesQuery;
                string dropTempTablesQuery;
                string queryText = GetQueryAsText(query,readNbOfRowsAffected , parameters, out createTempTablesQuery, out dropTempTablesQuery);

                bool tempTablesCreated = false;
                try
                {
                    using (var connection = new OracleConnection(_connString))
                    {
                        connection.Open();
                        if(createTempTablesQuery != null)
                        {
                            using(var createTempTablesCmd = new OracleCommand(createTempTablesQuery, connection))
                            {
                                createTempTablesCmd.ExecuteNonQuery();
                                tempTablesCreated = true;
                            }
                        }
                        using (var cmd = new OracleCommand(queryText, connection))
                        {
                            if (parameters != null)
                                AddParameters(cmd, parameters);
                            OracleParameter retval = null;
                            if (readNbOfRowsAffected)
                            {
                                retval = new OracleParameter("nbOfRowsAffected", OracleDbType.Int32);
                                retval.Direction = ParameterDirection.Output;
                                cmd.Parameters.Add(retval);
                            }

                            if (executeTransactional)
                            {
                                using (var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted))
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

                            if (readNbOfRowsAffected)
                                nbOfRowsAffected = (int)((Oracle.ManagedDataAccess.Types.OracleDecimal)retval.Value).Value;
                            else
                                nbOfRowsAffected = 0;
                        }
                        connection.Close();
                    }
                }
                finally
                {
                    if (tempTablesCreated)
                    {
                        try
                        {
                            using (var connection = new OracleConnection(_connString))
                            {
                                connection.Open();
                                using (var dropTempTablesCmd = new OracleCommand(dropTempTablesQuery, connection))
                                {
                                    dropTempTablesCmd.ExecuteNonQuery();
                                }
                                connection.Close();
                            }
                        }
                        catch (Exception ex)
                        {
                            LoggerFactory.GetExceptionLogger().WriteException(ex);
                        }
                    }
                }
            }

            string GetQueryAsText(RDBResolvedQuery resolvedQuery, bool readNbOfRowsAffected, Dictionary<string, RDBParameter> parameters, out string createTempTablesQuery, out string dropTempTablesQuery)
            {
                StringBuilder regularQueryBuilder = new StringBuilder();
                StringBuilder createTempTablesQueryBuilder = null;
                StringBuilder dropTempTablesQueryBuilder = null;

                if(parameters != null)
                {
                    bool anyParameterAdded = false;
                    foreach(var prm in parameters.Values)
                    {
                        if(prm.Direction == RDBParameterDirection.Declared)
                        {
                            if (!anyParameterAdded)
                                regularQueryBuilder.Append("DECLARE ");
                            anyParameterAdded = true;
                            regularQueryBuilder.Append(string.Concat(prm.DBParameterName, " ", GetColumnDBType(prm.DBParameterName, prm.Type, prm.Size, prm.Precision), ";"));
                            regularQueryBuilder.AppendLine();
                        }
                    }
                }

                regularQueryBuilder.Append("BEGIN");
                regularQueryBuilder.AppendLine();

                foreach(var statement in resolvedQuery.Statements)
                {
                    OracleRDBResolvedQueryStatement oracleStatement = statement.CastWithValidate<OracleRDBResolvedQueryStatement>("statement");
                    switch (oracleStatement.StatementType)
                    {

                        case OracleRDBResolvedQueryStatementType.Regular:
                            regularQueryBuilder.AppendLine(statement.TextStatement);
                            regularQueryBuilder.Append(";");
                            regularQueryBuilder.AppendLine();
                            break;
                        case OracleRDBResolvedQueryStatementType.CreateTempTable:
                            if (createTempTablesQueryBuilder == null)
                                createTempTablesQueryBuilder = new StringBuilder();
                            createTempTablesQueryBuilder.AppendLine(statement.TextStatement);
                            createTempTablesQueryBuilder.AppendLine();
                            break;
                        case OracleRDBResolvedQueryStatementType.DropTempTable:
                            if (dropTempTablesQueryBuilder == null)
                                dropTempTablesQueryBuilder = new StringBuilder();
                            dropTempTablesQueryBuilder.AppendLine(statement.TextStatement);
                            dropTempTablesQueryBuilder.AppendLine();
                            break;
                        default: throw new NotSupportedException(string.Format("StatementType '{0}'", oracleStatement.StatementType.ToString()));
                    }
                    
                }

                if (readNbOfRowsAffected)
                {
                    regularQueryBuilder.AppendLine();
                    regularQueryBuilder.Append(":nbOfRowsAffected := SQL%ROWCOUNT;");
                }

                regularQueryBuilder.AppendLine();
                regularQueryBuilder.Append("END;");

                if (createTempTablesQueryBuilder != null)
                    createTempTablesQuery = createTempTablesQueryBuilder.ToString();
                else
                    createTempTablesQuery = null;

                if (dropTempTablesQueryBuilder != null)
                    dropTempTablesQuery = dropTempTablesQueryBuilder.ToString();
                else
                    dropTempTablesQuery = null;

                return regularQueryBuilder.ToString();
            }

            private static void AddParameters(OracleCommand cmd, Dictionary<string, RDBParameter> parameters)
            {
                foreach (var prm in parameters)
                {
                    if (prm.Value.Direction == RDBParameterDirection.In)
                    {
                        if (prm.Value.Type == RDBDataType.Cursor)
                            cmd.Parameters.Add(new OracleParameter(prm.Value.Name, OracleDbType.RefCursor, DBNull.Value, ParameterDirection.Output));
                        else
                        {
                            Object value = prm.Value.Value;
                            if (value == null)
                            {
                                value = DBNull.Value;
                            }
                            else
                            {
                                switch (prm.Value.Type)
                                {
                                    case RDBDataType.UniqueIdentifier:
                                        value = ((Guid)value).ToByteArray(); break;
                                    case RDBDataType.Boolean:
                                        value = (bool)value ? 1 : 0; break;
                                }
                            }
                            var oraPrm = new OracleParameter(prm.Value.Name, value);
                            oraPrm.Direction = ParameterDirection.Input;
                            cmd.Parameters.Add(oraPrm);
                        }
                    }
                }

               
            }
        }

        private class OracleRDBDataReader : CommonRDBDataReader
        {
            public OracleRDBDataReader(IDataReader originalReader)
                : base(originalReader)
            {
            }

            public override string GetStringWithEmptyHandling(string fieldName)
            {
                var stringValue = this.GetString(fieldName);
                if (stringValue == ORACLE_EMPTY_STRING_VALUE)
                    return string.Empty;
                else
                    return stringValue;
            }


            public override Guid GetGuid(string fieldName)
            {
                byte[] bytes = base.GetFieldValue<byte[]>(fieldName);
                return new Guid(bytes);
            }

            public override Guid? GetNullableGuid(string fieldName)
            {
                byte[] bytes = base.GetFieldValueWithNullHandling<byte[]>(fieldName);
                if (bytes != null)
                    return new Guid(bytes);
                else
                    return default(Guid?);
            }

            public override Guid GetGuidWithNullHandling(string fieldName)
            {
                byte[] bytes = base.GetFieldValueWithNullHandling<byte[]>(fieldName);
                if (bytes != null)
                    return new Guid(bytes);
                else
                    return default(Guid);
            }
            

            public override bool GetBoolean(string fieldName)
            {
                short intValue = base.GetFieldValue<short>(fieldName);
                return intValue > 0;
            }

            public override bool? GetNullableBoolean(string fieldName)
            {
                short? intValue = base.GetFieldValueWithNullHandling<short?>(fieldName);
                if (intValue.HasValue)
                    return intValue.Value > 0;
                else
                    return default(bool?);
            }

            public override bool GetBooleanWithNullHandling(string fieldName)
            {
                short? intValue = base.GetFieldValueWithNullHandling<short?>(fieldName);
                if (intValue.HasValue)
                    return intValue.Value > 0;
                else
                    return default(bool);
            }
        }

        private class OracleRDBFieldValue : CommonRDBFieldValue
        {
            public OracleRDBFieldValue(object value)
                : base(value)
            {

            }

            public override string StringValueWithEmptyHandling
            {
                get
                {
                    var stringValue = this.StringValue;
                    if (stringValue == ORACLE_EMPTY_STRING_VALUE)
                        return string.Empty;
                    else
                        return stringValue;
                }
            }


            public override Guid GuidValue
            {
                get
                {
                    byte[] bytes = base.GetFieldValue<byte[]>();
                    return new Guid(bytes);
                }
            }

            public override Guid? NullableGuidValue
            {
                get
                {
                    byte[] bytes = base.GetFieldValueWithNullHandling<byte[]>();
                    if (bytes != null)
                        return new Guid(bytes);
                    else
                        return default(Guid?);
                }
            }

            public override Guid GuidWithNullHandlingValue
            {
                get
                {
                    byte[] bytes = base.GetFieldValueWithNullHandling<byte[]>();
                    if (bytes != null)
                        return new Guid(bytes);
                    else
                        return default(Guid);
                }
            }


            public override bool BooleanValue
            {
                get
                {
                    short intValue = base.GetFieldValue<short>();
                    return intValue > 0;
                }
            }

            public override bool? NullableBooleanValue
            {
                get
                {
                    short? intValue = base.GetFieldValueWithNullHandling<short?>();
                    if (intValue.HasValue)
                        return intValue.Value > 0;
                    else
                        return default(bool?);
                }
            }

            public override bool BooleanWithNullHandlingValue
            {
                get
                {
                    short? intValue = base.GetFieldValueWithNullHandling<short?>();
                    if (intValue.HasValue)
                        return intValue.Value > 0;
                    else
                        return default(bool);
                }
            }
        }

        #endregion
    }
}
