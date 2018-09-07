using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.Data.RDB.DataProvider.Providers
{
     public class MSSQLRDBDataProvider : CommonRDBDataProvider
     {

         public MSSQLRDBDataProvider(string connString)
             : base(connString)
         {
             _dataManager = new SQLDataManager(connString, this);
         }

         SQLDataManager _dataManager;

         public override string ConvertToDBParameterName(string parameterName, RDBParameterDirection parameterDirection)
         {
             return string.Concat("@", parameterName);
         }

         public const string UNIQUE_NAME = "MSSQL";
         public override string UniqueName
         {
             get { return UNIQUE_NAME; }
         }

         public override void ExecuteReader(IRDBDataProviderExecuteReaderContext context)
         {
             _dataManager.ExecuteReader(context.Query, context.Parameters, context.ExecuteTransactional, (originalReader) => context.OnReaderReady(new MSSQLRDBDataReader(originalReader)));
         }

         public override int ExecuteNonQuery(IRDBDataProviderExecuteNonQueryContext context)
         {
             return _dataManager.ExecuteNonQuery(context.Query, context.Parameters, context.ExecuteTransactional);
         }

         public override RDBFieldValue ExecuteScalar(IRDBDataProviderExecuteScalarContext context)
         {
             return new MSSQLRDBFieldValue(_dataManager.ExecuteScalar(context.Query, context.Parameters, context.ExecuteTransactional));
         }
         
         public override string NowDateTimeFunction
         {
             get { return " GETDATE() "; }
         }

         protected override string GetGuidDBType()
         {
             return "uniqueidentifier";
         }

         protected override string GenerateTempTableName()
         {
             return String.Concat("#TempTable_", Guid.NewGuid().ToString().Replace("-", ""));
         }

         protected override string GetGenerateIdFunction()
         {
             return "SCOPE_IDENTITY()";
         }

         #region Private Classes

         private class SQLDataManager : Vanrise.Data.SQL.BaseSQLDataManager
         {
             string _connString;
            MSSQLRDBDataProvider _dataProvider;

             public SQLDataManager(string connString, MSSQLRDBDataProvider dataProvider)
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
                             cmd.Cancel();
                             reader.Close();
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

             void CreateCommandWithParams(RDBResolvedQuery query, Dictionary<string, RDBParameter> parameters, bool executeTransactional, Action<SqlCommand> onCommandReady)
             {
                 using (var connection = new SqlConnection(_connString))
                 {
                     connection.Open();

                     using (var cmd = new SqlCommand(GetQueryAsText(query, parameters), connection))
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


             public string GetQueryAsText(RDBResolvedQuery resolvedQuery, Dictionary<string, RDBParameter> parameters)
             {
                 var queryBuilder = new StringBuilder();

                 if (parameters != null)
                 {
                     foreach (var prm in parameters.Values)
                     {
                         if (prm.Direction == RDBParameterDirection.Declared)
                         {
                             if (queryBuilder.Length == 0)
                                 queryBuilder.Append("DECLARE ");
                             else
                                 queryBuilder.Append(", ");
                             queryBuilder.Append(string.Concat(prm.DBParameterName, " ", _dataProvider.GetColumnDBType(prm.DBParameterName, prm.Type, prm.Size, prm.Precision)));
                             queryBuilder.AppendLine();
                         }
                     }
                 }

                 foreach (var statement in resolvedQuery.Statements)
                 {
                     queryBuilder.Append(statement.TextStatement);
                     queryBuilder.Append(";");
                     queryBuilder.AppendLine();
                     queryBuilder.AppendLine();
                 }
                 return queryBuilder.ToString();
             }


             private static void AddParameters(SqlCommand cmd, Dictionary<string, RDBParameter> parameters)
             {
                 foreach (var prm in parameters)
                 {
                     if (prm.Value.Direction == RDBParameterDirection.In)
                         cmd.Parameters.Add(new SqlParameter(prm.Value.DBParameterName, prm.Value.Value != null ? prm.Value.Value : DBNull.Value));
                 }
             }

             //protected override string GetConnectionString()
             //{
             //    return _connString;
             //}

             //public void ExecuteReader(RDBResolvedQuery query, Dictionary<string, RDBParameter> parameters, Action<IDataReader> onReaderReady)
             //{
             //    base.ExecuteReaderText(GetQueryAsText(query, parameters), onReaderReady, (cmd) =>
             //    {
             //        if (parameters != null)
             //        {
             //            AddParameters(cmd, parameters);
             //        }
             //    });
             //}

             //public int ExecuteNonQuery(RDBResolvedQuery query, Dictionary<string, RDBParameter> parameters)
             //{
             //    return base.ExecuteNonQueryText(GetQueryAsText(query, parameters), (cmd) =>
             //    {
             //        if (parameters != null)
             //        {
             //            AddParameters(cmd, parameters);
             //        }
             //    });
             //}

             //public Object ExecuteScalar(RDBResolvedQuery query, Dictionary<string, RDBParameter> parameters)
             //{
             //    return base.ExecuteScalarText(GetQueryAsText(query, parameters), (cmd) =>
             //    {
             //        if (parameters != null)
             //        {
             //            AddParameters(cmd, parameters);
             //        }
             //    });
             //}

             //private string GetQueryAsText(RDBResolvedQuery resolvedQuery, Dictionary<string, RDBParameter> parameters)
             //{
             //    var queryBuilder = new StringBuilder();

             //    if (parameters != null)
             //    {
             //        foreach (var prm in parameters.Values)
             //        {
             //            if (prm.Direction == RDBParameterDirection.Declared)
             //            {
             //                if (queryBuilder.Length == 0)
             //                    queryBuilder.Append("DECLARE ");
             //                else
             //                    queryBuilder.Append(", ");
             //                queryBuilder.Append(string.Concat(prm.DBParameterName, " ", _dataProvider.GetColumnDBType(prm.DBParameterName, prm.Type, prm.Size, prm.Precision)));
             //                queryBuilder.AppendLine();
             //            }
             //        }
             //    }

             //    foreach (var statement in resolvedQuery.Statements)
             //    {
             //        queryBuilder.Append(statement.TextStatement);
             //        queryBuilder.AppendLine();
             //        queryBuilder.AppendLine();
             //    }
             //    return queryBuilder.ToString();
             //}

             //private static void AddParameters(System.Data.Common.DbCommand cmd, Dictionary<string, RDBParameter> parameters)
             //{
             //    foreach (var prm in parameters)
             //    {
             //        if (prm.Value.Direction == RDBParameterDirection.In)
             //            cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter(prm.Value.DBParameterName, prm.Value.Value != null ? prm.Value.Value : DBNull.Value));
             //    }
             //}

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
     }
}
