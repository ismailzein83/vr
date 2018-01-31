using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.Data.RDB.DataProvider.Providers
{
    public class MSSQLRDBDataProvider : BaseRDBDataProvider
    {
        public const string MSSQL_DATAPROVIDER_UNIQUENAME = "MSSQL";
        public override string UniqueName
        {
            get { return MSSQL_DATAPROVIDER_UNIQUENAME; }
        }

        SQLDataManager _dataManager = new SQLDataManager();
        public override RDBResolvedSelectQuery ResolveSelectQuery(IRDBDataProviderResolveSelectQueryContext context)
        {
            var selectQuery = context.SelectQuery;
            selectQuery.ThrowIfNull("selectQuery");
            selectQuery.Table.ThrowIfNull("selectQuery.Table");
            selectQuery.Columns.ThrowIfNull("selectQuery.Columns");
            StringBuilder queryBuilder = new StringBuilder("");
            var rdbExpressionToDBQueryContext = new RDBExpressionToDBQueryContext(context, false);
            var rdbConditionToDBQueryContext = new RDBConditionToDBQueryContext(context, false);

            queryBuilder.Append(" FROM ");
            AddTableToDBQueryBuilder(queryBuilder, selectQuery.Table, context, true);

            if (selectQuery.Joins != null && selectQuery.Joins.Count > 0)
                AddJoinsToQuery(context, queryBuilder, selectQuery.Joins, rdbConditionToDBQueryContext);

            StringBuilder columnQueryBuilder = new StringBuilder(" SELECT ");
            if (selectQuery.NbOfRecords.HasValue)
                columnQueryBuilder.AppendFormat(" TOP({0}) ", selectQuery.NbOfRecords.Value);
            List<RDBSelectColumn> selectColumns = selectQuery.Columns;
            if(selectColumns == null || selectColumns.Count == 0)
            {
                if(selectQuery.GroupBy != null && selectQuery.GroupBy.Columns != null && selectQuery.GroupBy.Columns.Count > 0)
                {
                    selectColumns = new List<RDBSelectColumn>();
                    selectColumns.AddRange(selectQuery.GroupBy.Columns);
                    if (selectQuery.GroupBy.AggregateColumns != null && selectQuery.GroupBy.AggregateColumns.Count > 0)
                        selectColumns.AddRange(selectQuery.GroupBy.AggregateColumns);
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

            if (selectQuery.Condition != null)
            {
                string conditionDBQuery = selectQuery.Condition.ToDBQuery(rdbConditionToDBQueryContext);
                if (!string.IsNullOrEmpty(conditionDBQuery))
                {
                    queryBuilder.Append(" WHERE ");
                    queryBuilder.Append(conditionDBQuery);
                }
            }
            if (selectQuery.GroupBy != null && selectQuery.GroupBy.Columns != null && selectQuery.GroupBy.Columns.Count > 0)
            {
                queryBuilder.Append(" GROUP BY ");
                int groupByColumnIndex = 0;
                foreach(var column in selectQuery.GroupBy.Columns)
                {
                    if (groupByColumnIndex > 0)
                        queryBuilder.Append(", ");
                    groupByColumnIndex++;
                    queryBuilder.Append(column.Expression.ToDBQuery(rdbExpressionToDBQueryContext));
                }
                if(selectQuery.GroupBy.HavingCondition != null)
                {
                    string havingConditionDBQuery = selectQuery.GroupBy.HavingCondition.ToDBQuery(rdbConditionToDBQueryContext);
                    if (!string.IsNullOrEmpty(havingConditionDBQuery))
                    {
                        queryBuilder.Append(" HAVING ");
                        queryBuilder.Append(havingConditionDBQuery);
                    }
                }

            }
            if(selectQuery.SortColumns != null && selectQuery.SortColumns.Count > 0)
            {
                queryBuilder.Append(" ORDER BY ");
                int sortColumnIndex = 0;
                foreach (var column in selectQuery.SortColumns)
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
            return new RDBResolvedSelectQuery
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
                AddTableToDBQueryBuilder(queryBuilder, join.Table, context, true);
                string joinConditionDBQuery = join.Condition.ToDBQuery(rdbConditionToDBQueryContext);
                joinConditionDBQuery.ThrowIfNull("joinConditionDBQuery");
                queryBuilder.Append(" ON ");
                queryBuilder.Append(joinConditionDBQuery);
            }
        }

        public override RDBResolvedNoDataQuery ResolveInsertQuery(IRDBDataProviderResolveInsertQueryContext context)
        {
            var insertQuery = context.InsertQuery;
            insertQuery.ThrowIfNull("insertQuery");
            insertQuery.Table.ThrowIfNull("insertQuery.TableName");
            insertQuery.ColumnValues.ThrowIfNull("insertQuery.ColumnValues");
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append("INSERT INTO ");
            AddTableToDBQueryBuilder(queryBuilder, insertQuery.Table, context, false);
            queryBuilder.Append(" (");
            var rdbExpressionToDBQueryContext = new RDBExpressionToDBQueryContext(context, false);
            var rdbConditionToDBQueryContext = new RDBConditionToDBQueryContext(context, false);

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
                var getDBColumnNameContext = new RDBTableQuerySourceGetDBColumnNameContext(colVal.ColumnName, context, false);
                queryBuilder.Append(insertQuery.Table.GetDBColumnName(getDBColumnNameContext));
                valueBuilder.Append(colVal.Value.ToDBQuery(rdbExpressionToDBQueryContext));
            }
            RDBTableDefinitionQuerySource tableAsTableDefinitionQuerySource = context.InsertQuery.Table as RDBTableDefinitionQuerySource;
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
            return new RDBResolvedNoDataQuery
            {
                QueryText = queryBuilder.ToString()
            };
        }

        public override RDBResolvedNoDataQuery ResolveUpdateQuery(IRDBDataProviderResolveUpdateQueryContext context)
        {
            var updateQuery = context.UpdateQuery;
            updateQuery.ThrowIfNull("updateQuery");
            updateQuery.Table.ThrowIfNull("updateQuery.Table");
            updateQuery.ColumnValues.ThrowIfNull("updateQuery.ColumnValues");
            
            var rdbExpressionToDBQueryContext = new RDBExpressionToDBQueryContext(context, false);
            var rdbConditionToDBQueryContext = new RDBConditionToDBQueryContext(context, false);

            StringBuilder queryBuilder = new StringBuilder();
            if (updateQuery.Joins != null && updateQuery.Joins.Count > 0)
            {
                queryBuilder.Append(" FROM ");
                AddTableToDBQueryBuilder(queryBuilder, updateQuery.Table, context, true);
                AddJoinsToQuery(context, queryBuilder, updateQuery.Joins, rdbConditionToDBQueryContext);
            }
            else
            {
                AddTableToDBQueryBuilder(queryBuilder, updateQuery.Table, context, false);
            }

            StringBuilder columnQueryBuilder = new StringBuilder(" SET ");
            int colIndex = 0;

            foreach (var colVal in updateQuery.ColumnValues)
            {
                if (colIndex > 0)
                    columnQueryBuilder.Append(", ");
                colIndex++;
                var getDBColumnNameContext = new RDBTableQuerySourceGetDBColumnNameContext(colVal.ColumnName, context, false);
                columnQueryBuilder.Append(updateQuery.Table.GetDBColumnName(getDBColumnNameContext));
                columnQueryBuilder.Append(" = ");
                columnQueryBuilder.Append(colVal.Value.ToDBQuery(rdbExpressionToDBQueryContext));
            }

            RDBTableDefinitionQuerySource tableAsTableDefinitionQuerySource = context.UpdateQuery.Table as RDBTableDefinitionQuerySource;
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

            if (updateQuery.Joins != null && updateQuery.Joins.Count > 0)
            {
                queryBuilder.Insert(0, String.Format("UPDATE {0} {1}", context.GetTableAlias(updateQuery.Table), columnQueryBuilder.ToString()));
            }
            else
            {
                queryBuilder.Insert(0, "UPDATE ");
                queryBuilder.Append(columnQueryBuilder.ToString());
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
                QueryText = queryBuilder.ToString()
            };
        }

        public override RDBResolvedNoDataQuery ResolveTempTableCreationQuery(IRDBDataProviderResolveTempTableCreationQueryContext context)
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
                queryBuilder.Append(colDefEntry.Value.DBColumnName);
                queryBuilder.Append(" ");
                queryBuilder.Append(GetColumnDBType(colDefEntry.Value));
            }
            queryBuilder.Append(" ) ");
            return new RDBResolvedNoDataQuery
            {
                QueryText = queryBuilder.ToString()
            };
        }

        private string GetColumnDBType(RDBTableColumnDefinition columnDef)
        {
            switch(columnDef.DataType)
            {
                case RDBDataType.Int: return "int";
                case RDBDataType.BigInt: return "bigint";
                case RDBDataType.Decimal:
                        if (!columnDef.Size.HasValue)
                            throw new NullReferenceException(String.Format("columnDef.Size of column '{0}'", columnDef.DBColumnName));
                        if (!columnDef.Precision.HasValue)
                            throw new NullReferenceException(String.Format("columnDef.Precision of column '{0}'", columnDef.DBColumnName));
                        return String.Format("DECIMAL({0}, {1})", columnDef.Size.Value, columnDef.Precision.Value);
                case RDBDataType.DateTime: return "datetime";
                case RDBDataType.Varchar:
                    if (!columnDef.Size.HasValue)
                        throw new NullReferenceException(String.Format("columnDef.Size of column '{0}'", columnDef.DBColumnName));
                    return String.Format("varchar({0})", columnDef.Size.Value);
                case RDBDataType.NVarchar:
                    if (!columnDef.Size.HasValue)
                        throw new NullReferenceException(String.Format("columnDef.Size of column '{0}'", columnDef.DBColumnName));
                    return String.Format("nvarchar({0})", columnDef.Size.Value);
                default:
                    throw new NotSupportedException(String.Format("columnDef.DataType '{0}'", columnDef.DataType.ToString()));


            }
        }

        private void AddTableToDBQueryBuilder(StringBuilder queryBuilder, IRDBTableQuerySource table, IBaseRDBResolveQueryContext parentContext, bool assignAlias)
        {
            var tableQuerySourceContext = new RDBTableQuerySourceToDBQueryContext(parentContext, false);
            queryBuilder.Append(table.ToDBQuery(tableQuerySourceContext));
            if (assignAlias)
            {
                queryBuilder.Append(" ");
                string tableAlias = tableQuerySourceContext.GenerateTableAliasIfNotExists(table);
                queryBuilder.Append(tableAlias);
            }
            queryBuilder.Append(" ");
        }

        public override void ExecuteReader(IRDBDataProviderExecuteReaderContext context)
        {
            _dataManager.ExecuteReader(context.ResolvedQuery.QueryText, context.ParameterValues, (originalReader) => context.OnReaderReady(new MSSQLRDBDataReader(originalReader)));
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

        #endregion

        public override string ParameterNamePrefix
        {
            get { return "@"; }
        }

        public override int ExecuteNonQuery(IRDBDataProviderExecuteNonQueryContext context)
        {
            throw new NotImplementedException();
        }

        public override string NowDateTimeFunction
        {
            get { return " GETDATE() "; }
        }
    }
}
