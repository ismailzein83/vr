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

         public override string ConvertToDBParameterName(string parameterName)
         {
             return string.Concat("@", parameterName);
         }

         public const string UNIQUE_NAME = "MySQL";
         public override string UniqueName
         {
             get { return UNIQUE_NAME; }
         }
        
         public override string GetQueryAsText(RDBResolvedQuery resolvedQuery)
         {
             var queryBuilder = new StringBuilder();
             foreach (var statement in resolvedQuery.Statements)
             {
                 queryBuilder.Append(statement);
                 queryBuilder.Append(";");
                 queryBuilder.AppendLine();
                 queryBuilder.AppendLine();
             }
             return queryBuilder.ToString();
         }

         public override string GetStatementSetParameterValue(string parameterDBName, string parameterValue)
         {
             return string.Concat("SET ", parameterDBName, " := ", parameterValue);
         }

         protected override void AddStatementSetColumnValueInUpdateQuery(StringBuilder columnQueryBuilder, string columnDBName, string parameterDBName, string value)
         {
             columnQueryBuilder.Append(columnDBName);
             columnQueryBuilder.Append("=");
             if(parameterDBName != null)
             {
                 columnQueryBuilder.AppendLine("(");
                 columnQueryBuilder.Append(parameterDBName);
                 columnQueryBuilder.Append(":=");
             }
             columnQueryBuilder.Append(value);
             if (parameterDBName != null)
                 columnQueryBuilder.Append(")");
         }

         protected override string GenerateCreateTempTableQuery(string tempTableName, string columns)
         {
             return string.Concat("CREATE TEMPORARY TABLE ", tempTableName, " ", columns);
         }

         protected override string GetGenerateIdFunction()
         {
             return "LAST_INSERT_ID()";
         }

         public override RDBResolvedQuery ResolveParameterDeclarations(IRDBDataProviderResolveParameterDeclarationsContext context)
         {
             return new RDBResolvedQuery();
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
             _dataManager.ExecuteReader(context.Query, context.Parameters, (originalReader) => context.OnReaderReady(new MySQLRDBDataReader(originalReader)));
         }

         public override int ExecuteNonQuery(IRDBDataProviderExecuteNonQueryContext context)
         {
             return _dataManager.ExecuteNonQuery(context.Query, context.Parameters);
         }

         public override RDBFieldValue ExecuteScalar(IRDBDataProviderExecuteScalarContext context)
         {
             return new MySQLRDBFieldValue(_dataManager.ExecuteScalar(context.Query, context.Parameters));
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

             public void ExecuteReader(string sqlQuery, Dictionary<string, RDBParameter> parameters, Action<IDataReader> onReaderReady)
             {
                 CreateCommandWithParams(sqlQuery, parameters,
                     (cmd) =>
                     {
                         using (var reader = cmd.ExecuteReader())
                         {
                             onReaderReady(reader);
                             cmd.Cancel();
                             reader.Close();
                         }
                     });
             }

             public int ExecuteNonQuery(string sqlQuery, Dictionary<string, RDBParameter> parameters)
             {
                 int rslt = 0;
                 CreateCommandWithParams(sqlQuery, parameters,
                     (cmd) =>
                     {
                         rslt = cmd.ExecuteNonQuery();
                     });
                 return rslt;
             }

             public Object ExecuteScalar(string sqlQuery, Dictionary<string, RDBParameter> parameters)
             {
                 Object rslt = 0;
                 CreateCommandWithParams(sqlQuery, parameters,
                     (cmd) =>
                     {
                         rslt = cmd.ExecuteScalar();
                     });
                 return rslt;
             }

             void CreateCommandWithParams(string sqlQuery, Dictionary<string, RDBParameter> parameters, Action<MySqlCommand> onCommandReady)
             {
                 using (var connection = new MySqlConnection(_connString))
                 {
                     connection.Open();
                     using (var cmd = new MySqlCommand(sqlQuery, connection))
                     {
                         if (parameters != null)
                             AddParameters(cmd, parameters);
                         onCommandReady(cmd);
                     }
                     connection.Close();
                 }
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
