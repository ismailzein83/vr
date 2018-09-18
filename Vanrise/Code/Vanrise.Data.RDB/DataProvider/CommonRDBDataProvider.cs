using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.Data.RDB.DataProvider
{
    public abstract class CommonRDBDataProvider : BaseRDBDataProvider
    {
        string _connString;
        protected string ConnectionString
        {
            get
            {
                return _connString;
            }
        }

        public CommonRDBDataProvider(string connString)
        {
            _connString = connString;
        }

        public virtual bool UseLimitForTopRecords
        {
            get
            {
                return false;
            }
        }

        public override string GetTableDBName(string schemaName, string tableName)
        {
            if (!String.IsNullOrEmpty(schemaName))
                return String.Concat(schemaName, ".", tableName);
            else
                return tableName;
        }

        public override string GetQueryConcatenatedStrings(params string[] strings)
        {
            if (strings != null && strings.Length > 0)
                return string.Join(" + ", strings);
            else
                return "";
        }

        //public override RDBResolvedQueryStatement GetStatementSetParameterValue(string parameterDBName, string parameterValue)
        //{
        //    return new RDBResolvedQueryStatement { TextStatement = string.Concat("SET ", parameterDBName, " = ", parameterValue) };
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
        //                else
        //                    builder.Append(", ");
        //                builder.AppendFormat("{0} {1}", prm.DBParameterName, GetColumnDBType(prm.DBParameterName, prm.Type, prm.Size, prm.Precision));
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

            if (context.Table != null)
            {
                queryBuilder.Append(" FROM ");
                AddTableToDBQueryBuilder(queryBuilder, context.Table, context.TableAlias, context);
            }
            if (context.Joins != null && context.Joins.Count > 0)
                AddJoinsToQuery(context, queryBuilder, context.Joins, rdbConditionToDBQueryContext);

            StringBuilder columnQueryBuilder = new StringBuilder(" SELECT ");
            if (context.NbOfRecords.HasValue && !this.UseLimitForTopRecords)
                columnQueryBuilder.AppendFormat(" TOP({0}) ", context.NbOfRecords.Value);
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
                columnQueryBuilder.Append(GetDBAlias(column.Alias));
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
                        queryBuilder.Append(GetDBAlias(column.ColumnAlias));
                    else throw new Exception(String.Format("Sort Column Name not defined. SortColumn Index '{0}'", sortColumnIndex));
                    queryBuilder.Append(column.SortDirection == RDBSortDirection.ASC ? " ASC " : " DESC ");
                }
            }

            if (context.NbOfRecords.HasValue && this.UseLimitForTopRecords)
                queryBuilder.AppendFormat(" Limit {0} ", context.NbOfRecords.Value);
            var resolvedQuery = new RDBResolvedQuery();
            resolvedQuery.Statements.Add(new RDBResolvedQueryStatement { TextStatement = queryBuilder.ToString() });
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
            resolvedQuery.Statements.Add(new RDBResolvedQueryStatement { TextStatement = queryBuilder.ToString() });

            if (context.AddSelectGeneratedId)
                resolvedQuery.Statements.Add(new RDBResolvedQueryStatement { TextStatement = string.Concat(" SELECT ", GetGenerateIdFunction()) });

            return resolvedQuery;
        }

        protected abstract string GetGenerateIdFunction();

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
                    string selectColumnDBParameterName;
                    DefineParameterFromColumn(context, colVal.ColumnName, out selectColumnDBParameterName);

                    columnQueryBuilder.Append(",");
                    columnQueryBuilder.Append(selectColumnDBParameterName);
                    columnQueryBuilder.Append(" = ");
                    columnQueryBuilder.Append(value);

                    if (selectColumnsQueryBuilder == null)
                        selectColumnsQueryBuilder = new StringBuilder();
                    else
                        selectColumnsQueryBuilder.Append(", ");
                    selectColumnsQueryBuilder.Append(selectColumnDBParameterName);
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
                    columnQueryBuilder.Append(", ");

                    var getDBColumnNameContext = new RDBTableQuerySourceGetDBColumnNameContext(selectColumnEntry.Key, context);
                    string columnDBName = context.Table.GetDBColumnName(getDBColumnNameContext);

                    string selectColumnDBParameterName;
                    DefineParameterFromColumn(context, selectColumnEntry.Key, out selectColumnDBParameterName);

                    columnQueryBuilder.Append(selectColumnDBParameterName);
                    columnQueryBuilder.Append(" = ");
                    columnQueryBuilder.Append(columnDBName);

                    if (selectColumnsQueryBuilder == null)
                        selectColumnsQueryBuilder = new StringBuilder();
                    else
                        selectColumnsQueryBuilder.Append(", ");
                    selectColumnsQueryBuilder.Append(selectColumnDBParameterName);
                    selectColumnsQueryBuilder.Append(" ");
                    selectColumnsQueryBuilder.Append(GetDBAlias(selectColumnEntry.Value.Alias));
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
            var resolvedQuery = new RDBResolvedQuery();
            resolvedQuery.Statements.Add(new RDBResolvedQueryStatement { TextStatement = queryBuilder.ToString() });
            if (selectColumnsQueryBuilder != null)
                resolvedQuery.Statements.Add(new RDBResolvedQueryStatement { TextStatement = string.Concat(" SELECT ", selectColumnsQueryBuilder.ToString()) });
            return resolvedQuery;
        }

        private void DefineParameterFromColumn(IRDBDataProviderResolveUpdateQueryContext context, string columnName, out string dbParameterName)
        {
            var parameterName = string.Concat("Col_", Guid.NewGuid().ToString().Replace("-", ""));
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

        private void AddStatementSetColumnValueInUpdateQuery(StringBuilder columnQueryBuilder, string columnDBName, string parameterDBName, string value)
        {
            bool parameterAdded = false;
            if(parameterDBName != null)
            {
                columnQueryBuilder.Append(parameterDBName);
                columnQueryBuilder.Append("=");
                columnQueryBuilder.Append(value);
                parameterAdded = true;
            }
            if (parameterAdded)
                columnQueryBuilder.AppendLine(",");
            columnQueryBuilder.Append(columnDBName);
            columnQueryBuilder.Append("=");
            columnQueryBuilder.Append(value);
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
            resolvedQuery.Statements.Add(new RDBResolvedQueryStatement { TextStatement = queryBuilder.ToString() });
            return resolvedQuery;
        }

        public override RDBResolvedQuery ResolveTableCreationQuery(IRDBDataProviderResolveTableCreationQueryContext context)
        {
            string tableDBName = GetTableDBName(context.SchemaName, context.TableName);
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

                AppendTableColumnDefinition(columnsQueryBuilder, colDefEntry.Key, columnDBName, colDefEntry.Value.ColumnDefinition, colDefEntry.Value.NotNullable, colDefEntry.Value.IsIdentity);
               
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
            resolvedQuery.Statements.Add(new RDBResolvedQueryStatement { TextStatement = string.Concat("CREATE TABLE ", tableDBName, " ", columnsQueryBuilder.ToString()) });
            return resolvedQuery;
        }

        public abstract void AppendTableColumnDefinition(StringBuilder columnsQueryBuilder, string columnName, string columnDBName, 
            RDBTableColumnDefinition columnDefinition, bool notNullable, bool isIdentityColumn);
        
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

                AppendTableColumnDefinition(columnsQueryBuilder, colDefEntry.Key, columnDBName, colDefEntry.Value.ColumnDefinition, false, false);

                if(colDefEntry.Value.IsPrimaryKey)
                {
                    if (primaryKeyDBColumnNames == null)
                        primaryKeyDBColumnNames = new List<string>();
                    primaryKeyDBColumnNames.Add(columnDBName);
                }
            }
            if(primaryKeyDBColumnNames != null)
            {
                columnsQueryBuilder.Append(", Primary Key (");
                columnsQueryBuilder.Append(string.Join(",", primaryKeyDBColumnNames));
                columnsQueryBuilder.Append(")");
            }

            columnsQueryBuilder.Append(" ) ");            

            var resolvedQuery = new RDBResolvedQuery();
            resolvedQuery.Statements.Add(new RDBResolvedQueryStatement { TextStatement = GenerateCreateTempTableQuery(tempTableName, columnsQueryBuilder.ToString()) });
            return resolvedQuery;
        }

        public override RDBResolvedQuery ResolveTempTableDropQuery(IRDBDataProviderResolveTempTableDropQueryContext context)
        {
            var resolvedQuery = new RDBResolvedQuery();
            resolvedQuery.Statements.Add(new RDBResolvedQueryStatement { TextStatement = string.Concat("DROP TABLE ", context.TempTableName) });
            return resolvedQuery;
        }

        protected virtual string GenerateTempTableName()
        {
            return String.Concat("TempTable_", Guid.NewGuid().ToString().Replace("-", ""));
        }

        protected virtual string GenerateCreateTempTableQuery(string tempTableName, string columns)
        {
            return string.Concat("CREATE TABLE ", tempTableName, " ", columns);
        }

        protected string GetColumnDBType(string columnName, RDBTableColumnDefinition columnDef)
        {
            return GetColumnDBType(columnName, columnDef.DataType, columnDef.Size, columnDef.Precision);
        }

        public virtual string GetColumnDBType(string columnName, RDBDataType dataType, int? size, int? precision)
        {
            switch (dataType)
            {
                case RDBDataType.Int: return GetIntDBType();
                case RDBDataType.BigInt: return GetBigIntDBType();
                case RDBDataType.Decimal: return GetDecimalDBType(columnName, size, precision);
                case RDBDataType.DateTime: return GetDatetimeDBType();
                case RDBDataType.Varchar: return GetVarcharDBType(size);
                case RDBDataType.NVarchar: return GetNVarcharDBType(size);
                case RDBDataType.UniqueIdentifier: return GetGuidDBType();
                case RDBDataType.Boolean: return GetBooleanDBType();
                case RDBDataType.VarBinary: return GetVarBinaryDBType(size);
                default:
                    throw new NotSupportedException(String.Format("DataType '{0}'", dataType.ToString()));
            }
        }

        protected virtual string GetBooleanDBType()
        {
            return "bit";
        }

        protected virtual string GetBigIntDBType()
        {
            return "bigint";
        }

        protected virtual string GetIntDBType()
        {
            return "int";
        }

        protected virtual string GetDatetimeDBType()
        {
            return "datetime";
        }

        protected virtual string GetDecimalDBType(string columnName, int? size, int? precision)
        {
            if (!size.HasValue)
                throw new NullReferenceException(String.Format("Size of '{0}'", columnName));
            if (!precision.HasValue)
                throw new NullReferenceException(String.Format("Precision of '{0}'", columnName));
            return String.Format("DECIMAL({0}, {1})", size.Value, precision.Value);
        }

        protected virtual string GetVarBinaryDBType(int? size)
        {
            if (!size.HasValue)
                return "varbinary(max)";
            else
                return String.Format("varbinary({0})", size.Value);
        }

        protected virtual string GetNVarcharDBType(int? size)
        {
            if (!size.HasValue)
                return "nvarchar(max)";
            else
                return String.Format("nvarchar({0})", size.Value);
        }

        protected virtual string GetVarcharDBType(int? size)
        {
            if (!size.HasValue)
                return "varchar(max)";
            else
                return String.Format("varchar({0})", size.Value);
        }

        protected abstract string GetGuidDBType();

        protected void AddTableToDBQueryBuilder(StringBuilder queryBuilder, IRDBTableQuerySource table, string tableAlias, IBaseRDBResolveQueryContext parentContext)
        {
            var tableQuerySourceContext = new RDBTableQuerySourceToDBQueryContext(parentContext);
            queryBuilder.Append(table.ToDBQuery(tableQuerySourceContext));
            if (tableAlias != null)
            {
                queryBuilder.Append(" ");
                queryBuilder.Append(GetDBAlias(tableAlias));
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

    }
}
