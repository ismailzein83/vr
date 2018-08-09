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

        public override string PrepareQueryStatementToAddToBatch(string queryStatement)
        {
            return queryStatement;
        }

        public override string GetQueryConcatenatedStrings(params string[] strings)
        {
            if (strings != null && strings.Length > 0)
                return string.Join(" + ", strings);
            else
                return "";
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

            if (context.NbOfRecords.HasValue && this.UseLimitForTopRecords)
                queryBuilder.AppendFormat(" Limit {0} ", context.NbOfRecords.Value);
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

                queryBuilder.Append(context.SelectQuery.ToDBQuery(new RDBTableQuerySourceToDBQueryContext(context)));
            }
            else
            {
                throw new Exception("ColumnValues and SelectQuery are both null");
            }
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
                else if (colVal.ExpressionToSet != null)
                {
                    columnQueryBuilder.Append(colVal.ExpressionToSet.ToDBQuery(rdbExpressionToDBQueryContext));
                }
                else
                {
                    throw new NullReferenceException("colVal.ColumnName & colVal.ExpressionToSet");
                }
                columnQueryBuilder.Append(" = ");
                columnQueryBuilder.Append(colVal.Value.ToDBQuery(rdbExpressionToDBQueryContext));
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
            string tempTableName = GenerateTempTableName();
            context.TempTableName = tempTableName;
           
            var columnsQueryBuilder = new StringBuilder();
            columnsQueryBuilder.Append(" ( ");
            
            int colIndex = 0;
            foreach (var colDefEntry in context.Columns)
            {
                if (colIndex > 0)
                    columnsQueryBuilder.Append(",");
                colIndex++;
                columnsQueryBuilder.AppendLine();
                columnsQueryBuilder.Append(RDBSchemaManager.GetColumnDBName(context.DataProvider, colDefEntry.Key, colDefEntry.Value));
                columnsQueryBuilder.Append(" ");
                columnsQueryBuilder.Append(GetColumnDBType(colDefEntry.Key, colDefEntry.Value));
            }
            columnsQueryBuilder.Append(" ) ");
            return new RDBResolvedQuery
            {
                QueryText = GenerateCreateTempTableQuery(tempTableName, columnsQueryBuilder.ToString())
            };
        }

        protected virtual string GenerateTempTableName()
        {
            return String.Concat("TempTable_", Guid.NewGuid().ToString().Replace("-", ""));
        }

        protected virtual string GenerateCreateTempTableQuery(string tempTableName, string columns)
        {
            return string.Concat("CREATE TABLE ", tempTableName, " ", columns);
        }

        private string GetColumnDBType(string columnName, RDBTableColumnDefinition columnDef)
        {
            return GetColumnDBType(columnName, columnDef.DataType, columnDef.Size, columnDef.Precision);
        }

        public virtual string GetColumnDBType(string columnName, RDBDataType dataType, int? size, int? precision)
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
                    return GetVarcharDBType(size);
                case RDBDataType.NVarchar:
                    return GetNVarcharDBType(size);
                case RDBDataType.UniqueIdentifier: return GetGuidDBType();
                case RDBDataType.Boolean: return "bit";
                case RDBDataType.VarBinary:
                    return GetVarBinaryDBType(size);
                default:
                    throw new NotSupportedException(String.Format("DataType '{0}'", dataType.ToString()));
            }
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

        public override BaseRDBStreamForBulkInsert InitializeStreamForBulkInsert(IRDBDataProviderInitializeStreamForBulkInsertContext context)
        {
            throw new NotImplementedException();
        }

    }
}
