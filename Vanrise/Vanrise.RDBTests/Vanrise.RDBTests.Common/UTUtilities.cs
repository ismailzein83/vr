using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.RDBTests.Common
{
    public static class UTUtilities
    {
        public static void TruncateTable(string connStringName, string schemaName, string tableName)
        {
            string sqlConnString;
            string rdbConnString;
            GetConnStringsWithValidate(connStringName, out sqlConnString, out rdbConnString);
            TruncateTableInOneDB(sqlConnString, schemaName, tableName);
            TruncateTableInOneDB(rdbConnString, schemaName, tableName);
        }

        public static void AssertDBTablesAreSimilar(string connStringName, string schemaName, string tableName)
        {
            string sqlConnString;
            string rdbConnString;
            GetConnStringsWithValidate(connStringName, out sqlConnString, out rdbConnString);
            DataTable sqlTable = FillDBTable(sqlConnString, schemaName, tableName);
            DataTable rdbTable = FillDBTable(rdbConnString, schemaName, tableName);
            UTAssert.ObjectsAreSimilar(sqlTable, rdbTable);
        }

        private static DataTable FillDBTable(string connString, string schemaName, string tableName)
        {
            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(connString))
            {
                conn.Open();
                SqlDataAdapter adp = new SqlDataAdapter(string.Format("SELECT * FROM {0}", GetTableName(schemaName, tableName)), conn);
                adp.Fill(dt);
                conn.Close();
            }
            if (dt.Columns.Contains("timestamp"))
                dt.Columns.Remove("timestamp");
            if (dt.Columns.Contains("CreatedTime"))
                dt.Columns.Remove("CreatedTime");
            return dt;
        }

        private static void TruncateTableInOneDB(string connString, string schemaName, string tableName)
        {
            string queryText = string.Format("Truncate Table {0}", GetTableName(schemaName, tableName));
            ExecuteDBNonQuery(connString, queryText);
        }

        private static void ExecuteDBNonQuery(string connString, string queryText)
        {
            using (var conn = new SqlConnection(connString))
            {
                conn.Open();
                using (var cmd = new SqlCommand(queryText, conn))
                {
                    cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
        }

        private static string GetTableName(string schemaName, string tableName)
        {
            if (string.IsNullOrEmpty(schemaName))
                return tableName;
            else
                return string.Concat(schemaName, ".", tableName);
        }

        private static void GetConnStringsWithValidate(string connStringName, out string sqlConnString, out string rdbConnString)
        {
            var sqlConn = ConfigurationManager.ConnectionStrings[connStringName];
            sqlConn.ThrowIfNull("sqlConn", connStringName);
            sqlConnString = sqlConn.ConnectionString;
            var rdbConn = ConfigurationManager.ConnectionStrings[string.Concat(connStringName, "_RDB")];
            rdbConn.ThrowIfNull("rdbConn", connStringName);
            rdbConnString = rdbConn.ConnectionString;
        }
    }
}
