using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Vanrise.Common;

namespace Vanrise.Data.RDB.DataProvider.Providers
{
    public class MySQLRDBDataProvider : CommonRDBDataProvider
    {
        public MySQLRDBDataProvider(string connString)
             : base(connString)
         {
             connString.ThrowIfNull("connString");
             var validConnString = string.Concat(connString, "Allow User Variables=True");
             _dataManager = new MySQLDataManager(validConnString);
         }

        MySQLDataManager _dataManager;

        public override bool UseLimitForTopRecords
        {
            get
            {
                return true;
            }
        }

        public override string GetTableDBName(string schemaName, string tableName)
        {
            if (!String.IsNullOrEmpty(schemaName))
                return String.Concat(schemaName, "_", tableName);
            else
                return tableName;
        }

        public override string ConvertToDBParameterName(string parameterName, RDBParameterDirection parameterDirection)
         {
             return string.Concat("@", parameterName);
         }

         public const string UNIQUE_NAME = "MYSQL";
         public override string UniqueName
         {
             get { return UNIQUE_NAME; }
         }
        
         //public override RDBResolvedQueryStatement GetStatementSetParameterValue(string parameterDBName, string parameterValue)
         //{
         //    return new RDBResolvedQueryStatement { TextStatement = string.Concat("SET ", parameterDBName, " := ", parameterValue) };
         //}

         protected override string GenerateCreateTempTableQuery(string tempTableName, string columns)
         {
             return string.Concat("CREATE TEMPORARY TABLE ", tempTableName, " ", columns);
         }

         protected override string GetGenerateIdFunction()
         {
             return "LAST_INSERT_ID()";
         }

         //public override RDBResolvedQuery ResolveParameterDeclarations(IRDBDataProviderResolveParameterDeclarationsContext context)
         //{
         //    return new RDBResolvedQuery();
         //}

         public override RDBResolvedQuery ResolveUpdateQuery(IRDBDataProviderResolveUpdateQueryContext context)
         {
             context.Table.ThrowIfNull("context.Table");
             context.ColumnValues.ThrowIfNull("context.ColumnValues");

             var rdbExpressionToDBQueryContext = new RDBExpressionToDBQueryContext(context, context.QueryBuilderContext);
             var rdbConditionToDBQueryContext = new RDBConditionToDBQueryContext(context, context.QueryBuilderContext);

             StringBuilder queryBuilder = new StringBuilder(" UPDATE ");
             if (context.Joins != null && context.Joins.Count > 0)
             {                 
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
                 if (!string.IsNullOrEmpty(context.TableAlias))
                     columnDBName = string.Concat(context.TableAlias, ".", columnDBName);
                 string value = colVal.Value.ToDBQuery(rdbExpressionToDBQueryContext);

                 string selectColumnDBParameterName = null;
                 RDBUpdateSelectColumn selectColumn;
                 if (selectColumnsByColumnName != null && selectColumnsByColumnName.TryGetValue(colVal.ColumnName, out selectColumn))
                 {
                     DefineParameterFromColumn(context, colVal.ColumnName, out selectColumnDBParameterName);

                     if (selectColumnsQueryBuilder == null)
                         selectColumnsQueryBuilder = new StringBuilder();
                     else
                         selectColumnsQueryBuilder.Append(", ");
                     selectColumnsQueryBuilder.Append(selectColumnDBParameterName);
                     selectColumnsQueryBuilder.Append(" ");
                     selectColumnsQueryBuilder.Append(selectColumn.Alias);

                     selectColumnsByColumnName.Remove(colVal.ColumnName);
                 }

                 AddStatementSetColumnValueInUpdateQuery(columnQueryBuilder, columnDBName, selectColumnDBParameterName, value);                 
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
                     if (selectColumnsQueryBuilder == null)
                         selectColumnsQueryBuilder = new StringBuilder();
                     else
                         selectColumnsQueryBuilder.Append(", ");
                     selectColumnsQueryBuilder.Append(selectColumnDBParameterName);
                     selectColumnsQueryBuilder.Append(" ");
                     selectColumnsQueryBuilder.Append(selectColumnEntry.Value.Alias);

                     AddStatementSetColumnValueInUpdateQuery(columnQueryBuilder, columnDBName, selectColumnDBParameterName, columnDBName);
                 }
             }

             queryBuilder.Append(columnQueryBuilder.ToString());

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
             columnQueryBuilder.Append(columnDBName);
             columnQueryBuilder.Append("=");
             if (parameterDBName != null)
             {
                 columnQueryBuilder.AppendLine("(");
                 columnQueryBuilder.Append(parameterDBName);
                 columnQueryBuilder.Append(":=");
             }
             columnQueryBuilder.Append(value);
             if (parameterDBName != null)
                 columnQueryBuilder.Append(")");
         }


         public override string GetQueryConcatenatedStrings(params string[] strings)
         {
             if (strings != null && strings.Length > 0)
                 return string.Concat("CONCAT(", string.Join(", ", strings), ")");
             else
                 return "";
         }

         public override void ExecuteReader(IRDBDataProviderExecuteReaderContext context)
         {
             _dataManager.ExecuteReader(context.Query, context.Parameters, context.ExecuteTransactional, (originalReader) => context.OnReaderReady(new MySQLRDBDataReader(originalReader)));
         }

         public override int ExecuteNonQuery(IRDBDataProviderExecuteNonQueryContext context)
         {
             return _dataManager.ExecuteNonQuery(context.Query, context.Parameters, context.ExecuteTransactional);
         }

         public override RDBFieldValue ExecuteScalar(IRDBDataProviderExecuteScalarContext context)
         {
             return new MySQLRDBFieldValue(_dataManager.ExecuteScalar(context.Query, context.Parameters, context.ExecuteTransactional));
         }
         
         public override string NowDateTimeFunction
         {
             get { return " NOW() "; }
         }

         protected override string GetGuidDBType()
         {
             return "char(38)";
         }

         protected override string GetVarcharDBType(int? size)
         {
             if (size.HasValue && size.Value <= 1000)
                 return base.GetVarcharDBType(size);
             else
                 return "longtext";
         }

         protected override string GetNVarcharDBType(int? size)
         {
             if (size.HasValue && size.Value <= 1000)
                 return base.GetVarcharDBType(size);
             else
                 return "longtext";
         }

         protected override string GetVarBinaryDBType(int? size)
         {
             if (size.HasValue && size.Value <= 1000)
                 return base.GetVarcharDBType(size);
             else
                 return "longblob";
         }

         #region Private Classes

         private class MySQLDataManager
         {
             string _connString;
            
             public MySQLDataManager(string connString)
             {
                 _connString = connString;
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

             void CreateCommandWithParams(RDBResolvedQuery query, Dictionary<string, RDBParameter> parameters, bool executeTransactional, Action<MySqlCommand> onCommandReady)
             {
                 using (var connection = new MySqlConnection(_connString))
                 {
                     connection.Open();
                     using (var cmd = new MySqlCommand(GetQueryAsText(query), connection))
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


             private static void AddParameters(MySqlCommand cmd, Dictionary<string, RDBParameter> parameters)
             {
                 foreach (var prm in parameters)
                 {
                     if (prm.Value.Direction == RDBParameterDirection.In)
                         cmd.Parameters.Add(new MySqlParameter(prm.Value.DBParameterName, prm.Value.Value != null ? prm.Value.Value : DBNull.Value));
                 }
             }
         }

         private class MySQLRDBDataReader : CommonRDBDataReader
         {
             public MySQLRDBDataReader(IDataReader originalReader)
                 : base(originalReader)
             {
             }

             public override Guid GetGuid(string fieldName)
             {
                 string stringValue = base.GetFieldValue<string>(fieldName);
                 return ParseGuidWithValidate(fieldName, stringValue);
             }

             public override Guid? GetNullableGuid(string fieldName)
             {
                 string stringValue = base.GetFieldValueWithNullHandling<string>(fieldName);
                 if (stringValue != null)
                     return ParseGuidWithValidate(fieldName, stringValue);
                 else
                     return default(Guid?);
             }

             public override Guid GetGuidWithNullHandling(string fieldName)
             {
                 string stringValue = base.GetFieldValueWithNullHandling<string>(fieldName);
                 if (stringValue != null)
                     return ParseGuidWithValidate(fieldName, stringValue);
                 else
                     return default(Guid);
             }

             private Guid ParseGuidWithValidate(string fieldName, string stringValue)
             {
                 Guid guidValue;
                 if (!Guid.TryParse(stringValue, out guidValue))
                     throw new Exception(String.Format("Field '{0}' has invalid Guid Value '{1}'", fieldName, stringValue));
                 return guidValue;
             }

             public override bool GetBoolean(string fieldName)
             {
                 ulong intValue = base.GetFieldValue<ulong>(fieldName);
                 return intValue > 0;
             }

             public override bool? GetNullableBoolean(string fieldName)
             {
                 ulong? intValue = base.GetFieldValueWithNullHandling<ulong?>(fieldName);
                 if (intValue.HasValue)
                     return intValue.Value > 0;
                 else
                     return default(bool?);
             }

             public override bool GetBooleanWithNullHandling(string fieldName)
             {
                 ulong? intValue = base.GetFieldValueWithNullHandling<ulong?>(fieldName);
                 if (intValue.HasValue)
                     return intValue.Value > 0;
                 else
                     return default(bool);
             }
         }

         private class MySQLRDBFieldValue : CommonRDBFieldValue
         {
             public MySQLRDBFieldValue(object value)
                 : base(value)
             {

             }

             public override Guid GuidValue
             {
                 get
                 {
                     string stringValue = base.GetFieldValue<string>();
                     return ParseGuidWithValidate(stringValue);
                 }
             }

             public override Guid? NullableGuidValue
             {
                 get
                 {
                     string stringValue = base.GetFieldValueWithNullHandling<string>();
                     if (stringValue != null)
                         return ParseGuidWithValidate(stringValue);
                     else
                         return default(Guid?);
                 }
             }

             public override Guid GuidWithNullHandlingValue
             {
                 get
                 {
                     string stringValue = base.GetFieldValueWithNullHandling<string>();
                     if (stringValue != null)
                         return ParseGuidWithValidate(stringValue);
                     else
                         return default(Guid);
                 }
             }

             private Guid ParseGuidWithValidate(string stringValue)
             {
                 Guid guidValue;
                 if (!Guid.TryParse(stringValue, out guidValue))
                     throw new Exception(String.Format("invalid Guid Value '{1}'", stringValue));
                 return guidValue;
             }

             public override bool BooleanValue
             {
                 get
                 {
                     ulong intValue = base.GetFieldValue<ulong>();
                     return intValue > 0;
                 }
             }

             public override bool? NullableBooleanValue
             {
                 get
                 {
                     ulong? intValue = base.GetFieldValueWithNullHandling<ulong?>();
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
                     ulong? intValue = base.GetFieldValueWithNullHandling<ulong?>();
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
