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
             _dataManager = new SQLDataManager(connString);
         }

         SQLDataManager _dataManager;

         public override string ConvertToDBParameterName(string parameterName)
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
             _dataManager.ExecuteReader(context.Query, context.Parameters, (originalReader) => context.OnReaderReady(new MSSQLRDBDataReader(originalReader)));
         }

         public override int ExecuteNonQuery(IRDBDataProviderExecuteNonQueryContext context)
         {
             return _dataManager.ExecuteNonQuery(context.Query, context.Parameters);
         }

         public override RDBFieldValue ExecuteScalar(IRDBDataProviderExecuteScalarContext context)
         {
             return new MSSQLRDBFieldValue(_dataManager.ExecuteScalar(context.Query, context.Parameters));
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
     }
}
