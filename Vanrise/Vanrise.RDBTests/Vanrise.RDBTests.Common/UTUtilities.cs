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




        public static void CallActionIteratively<T, Q, R, S, U, V, W, X, Y, Z, T2, T3>(Action<T, Q, R, S, U, V, W, X, Y, Z, T2, T3> action, List<T> TList, List<Q> QList, List<R> RList, List<S> SList, List<U> UList, List<V> VList, List<W> WList, List<X> XList, List<Y> YList, List<Z> ZList, List<T2> T2List, List<T3> T3List)
        {
            Action<T> TAction = TItem => CallActionIteratively<Q, R, S, U, V, W, X, Y, Z, T2, T3>((QItem, RItem, SITem, UItem, VItem, WItem, XItem, YItem, ZItem, T2Item, T3Item) => action(TItem, QItem, RItem, SITem, UItem, VItem, WItem, XItem, YItem, ZItem, T2Item, T3Item), QList, RList, SList, UList, VList, WList, XList, YList, ZList, T2List, T3List);
            CallActionIteratively<T>(TAction, TList);
        }

        public static void CallActionIteratively<T, Q, R, S, U, V, W, X, Y, Z, T2>(Action<T, Q, R, S, U, V, W, X, Y, Z, T2> action, List<T> TList, List<Q> QList, List<R> RList, List<S> SList, List<U> UList, List<V> VList, List<W> WList, List<X> XList, List<Y> YList, List<Z> ZList, List<T2> T2List)
        {
            Action<T> TAction = TItem => CallActionIteratively<Q, R, S, U, V, W, X, Y, Z, T2>((QItem, RItem, SITem, UItem, VItem, WItem, XItem, YItem, ZItem, T2Item) => action(TItem, QItem, RItem, SITem, UItem, VItem, WItem, XItem, YItem, ZItem, T2Item), QList, RList, SList, UList, VList, WList, XList, YList, ZList, T2List);
            CallActionIteratively<T>(TAction, TList);
        }

        public static void CallActionIteratively<T, Q, R, S, U, V, W, X, Y, Z>(Action<T, Q, R, S, U, V, W, X, Y, Z> action, List<T> TList, List<Q> QList, List<R> RList, List<S> SList, List<U> UList, List<V> VList, List<W> WList, List<X> XList, List<Y> YList, List<Z> ZList)
        {
            Action<T> TAction = TItem => CallActionIteratively<Q, R, S, U, V, W, X, Y, Z>((QItem, RItem, SITem, UItem, VItem, WItem, XItem, YItem, ZItem) => action(TItem, QItem, RItem, SITem, UItem, VItem, WItem, XItem, YItem, ZItem), QList, RList, SList, UList, VList, WList, XList, YList, ZList);
            CallActionIteratively<T>(TAction, TList);
        }

        public static void CallActionIteratively<T, Q, R, S, U, V, W, X, Y>(Action<T, Q, R, S, U, V, W, X, Y> action, List<T> TList, List<Q> QList, List<R> RList, List<S> SList, List<U> UList, List<V> VList, List<W> WList, List<X> XList, List<Y> YList)
        {
            Action<T> TAction = TItem => CallActionIteratively<Q, R, S, U, V, W, X, Y>((QItem, RItem, SITem, UItem, VItem, WItem, XItem, YItem) => action(TItem, QItem, RItem, SITem, UItem, VItem, WItem, XItem, YItem), QList, RList, SList, UList, VList, WList, XList, YList);
            CallActionIteratively<T>(TAction, TList);
        }

        public static void CallActionIteratively<T, Q, R, S, U, V, W, X>(Action<T, Q, R, S, U, V, W, X> action, List<T> TList, List<Q> QList, List<R> RList, List<S> SList, List<U> UList, List<V> VList, List<W> WList, List<X> XList)
        {
            Action<T> TAction = TItem => CallActionIteratively<Q, R, S, U, V, W, X>((QItem, RItem, SITem, UItem, VItem, WItem, XItem) => action(TItem, QItem, RItem, SITem, UItem, VItem, WItem, XItem), QList, RList, SList, UList, VList, WList, XList);
            CallActionIteratively<T>(TAction, TList);
        }

        public static void CallActionIteratively<T, Q, R, S, U, V, W>(Action<T, Q, R, S, U, V, W> action, List<T> TList, List<Q> QList, List<R> RList, List<S> SList, List<U> UList, List<V> VList, List<W> WList)
        {
            Action<T> TAction = TItem => CallActionIteratively<Q, R, S, U, V, W>((QItem, RItem, SITem, UItem, VItem, WItem) => action(TItem, QItem, RItem, SITem, UItem, VItem, WItem), QList, RList, SList, UList, VList, WList);
            CallActionIteratively<T>(TAction, TList);
        }

        public static void CallActionIteratively<T, Q, R, S, U, V>(Action<T, Q, R, S, U, V> action, List<T> TList, List<Q> QList, List<R> RList, List<S> SList, List<U> UList, List<V> VList)
        {
            Action<T> TAction = TItem => CallActionIteratively<Q, R, S, U, V>((QItem, RItem, SITem, UItem, VItem) => action(TItem, QItem, RItem, SITem, UItem, VItem), QList, RList, SList, UList, VList);
            CallActionIteratively<T>(TAction, TList);
        }

        public static void CallActionIteratively<T, Q, R, S, U>(Action<T, Q, R, S, U> action, List<T> TList, List<Q> QList, List<R> RList, List<S> SList, List<U> UList)
        {
            Action<T> TAction = TItem => CallActionIteratively<Q, R, S, U>((QItem, RItem, SITem, UItem) => action(TItem, QItem, RItem, SITem, UItem), QList, RList, SList, UList);
            CallActionIteratively<T>(TAction, TList);
        }

        public static void CallActionIteratively<T, Q, R, S>(Action<T, Q, R, S> action, List<T> TList, List<Q> QList, List<R> RList, List<S> SList)
        {
            Action<T> TAction = TItem => CallActionIteratively<Q, R, S>((QItem, RItem, SITem) => action(TItem, QItem, RItem, SITem), QList, RList, SList);
            CallActionIteratively<T>(TAction, TList);
        }

        public static void CallActionIteratively<T, Q, R>(Action<T, Q, R> action, List<T> TList, List<Q> QList, List<R> RList)
        {
            Action<T> TAction = TItem => CallActionIteratively<Q, R>((QItem, RItem) => action(TItem, QItem, RItem), QList, RList);
            CallActionIteratively<T>(TAction, TList);
        }

        public static void CallActionIteratively<T, Q>(Action<T, Q> action, List<T> TList, List<Q> QList)
        {
            Action<T> TAction = TItem => CallActionIteratively<Q>(QItem => action(TItem, QItem), QList);
            CallActionIteratively<T>(TAction, TList);
        }

        public static void CallActionIteratively<T>(Action<T> action, List<T> TList)
        {
            foreach (var itm in TList)
            {
                action(itm);
            }
        }



        public static List<DateTime> GetDateTimeListsForTesting()
        {
            return new List<DateTime> { DateTime.Now, DateTime.Today, DateTime.Now.AddDays(-5), DateTime.Now.AddDays(-15), DateTime.Now.AddDays(5), DateTime.Now.AddDays(-150), DateTime.Now.AddDays(150) };
        }

        public static List<DateTime?> GetNullableDateTimeListsForTesting()
        {
            var dateTimeList = GetDateTimeListsForTesting();
            List<DateTime?> lst = new List<DateTime?>();
            lst.Add(null);
            lst.AddRange(dateTimeList.Cast<DateTime?>());
            return lst;
        }

        public static List<bool> GetBoolListForTesting()
        {
            return new List<bool> { true, false };
        }

        public static List<bool?> GetNullableBoolListForTesting()
        {
            return new List<bool?> { null, true, false };
        }
    }
}
