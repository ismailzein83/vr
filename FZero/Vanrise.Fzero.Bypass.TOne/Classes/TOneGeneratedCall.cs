using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using Vanrise.CommonLibrary;


namespace Vanrise.Fzero.Bypass.TOne
{
    public partial class ToneGeneratedCall
    {
        public static DataTable ConvertToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
            {
                if (prop.PropertyType.Name == "Int32" || prop.PropertyType.Name == "String" || prop.PropertyType.Name == "DateTime"
                    || prop.PropertyType == typeof(decimal) || prop.PropertyType == typeof(double))
                    table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);

                if (prop.PropertyType == typeof(int?))
                {
                    table.Columns.Add(prop.Name, typeof(int));
                    table.Columns[table.Columns.Count - 1].AllowDBNull = true;
                }

                if (prop.PropertyType == typeof(DateTime?))
                {
                    table.Columns.Add(prop.Name, typeof(DateTime));
                    table.Columns[table.Columns.Count - 1].AllowDBNull = true;
                }

                if (prop.PropertyType == typeof(Boolean?))
                {
                    table.Columns.Add(prop.Name, typeof(Boolean));
                    table.Columns[table.Columns.Count - 1].AllowDBNull = true;
                }

                if (prop.PropertyType.IsEnum)
                    table.Columns.Add(prop.Name, typeof(int));
            }

            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                {
                    if (prop.PropertyType.Name == "Int32" || prop.PropertyType.Name == "String" || prop.PropertyType.Name == "DateTime"
                        || prop.PropertyType == typeof(decimal) || prop.PropertyType == typeof(double))
                        row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;

                    if (prop.PropertyType.IsEnum)
                        row[prop.Name] = Convert.ToInt32(prop.GetValue(item));

                    if (prop.PropertyType == typeof(int?) || prop.PropertyType == typeof(DateTime?) || prop.PropertyType == typeof(Boolean?))
                    {
                        row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                    }
                }

                table.Rows.Add(row);
            }
            return table;
        }
        public static void InsertData<T>(List<T> list, string TabelName)
        {
            DataTable dt = new DataTable("MyTable");
            dt = ConvertToDataTable(list);
            using (SqlBulkCopy bulkcopy = new SqlBulkCopy(ConfigurationManager.ConnectionStrings["ToneConnectionString"].ConnectionString))
            {
                bulkcopy.BulkCopyTimeout = 660;
                bulkcopy.DestinationTableName = TabelName;
                bulkcopy.WriteToServer(dt);
            }
        }
        public static bool SaveBulk(List<ToneGeneratedCall> listGeneratedCalls)
        {
            bool success = false;
            try
            {
                InsertData(listGeneratedCalls.ToList(), "GeneratedCalls");
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.TOne.SaveBulk(" + listGeneratedCalls.Count.ToString() + ")", err);
            }
            return success;
        }
        public static List<ToneGeneratedCall> GetAnalyzed()
        {
            List<ToneGeneratedCall> GeneratedCallsList = new List<ToneGeneratedCall>();

            try
            {
                using (Entities context = new Entities())
                {
                    GeneratedCallsList = context.GeneratedCalls
                                        .Where(u => u.ID > 0
                                          && u.ToneFeedbackID != null)
                                          .OrderBy(u => u.ID)
                                          .ToList();


                }
            }
            catch (Exception err)
            {
                throw err;
                //FileLogger.Write("Error in Vanrise.Fzero.Bypass.GeneratedCall.GetCallsDidNotPassLevelTwo()", err);
            }

            return GeneratedCallsList;
        }
        public static void FillReceivedCalls()
        {
            try
            {
                using (Entities context = new Entities())
                {
                    ((IObjectContextAdapter)context).ObjectContext.CommandTimeout = 18000;
                    ((IObjectContextAdapter)context).ObjectContext.ExecuteStoreCommand("FillReceivedCalls");
                    //context.FillReceivedCalls();
                }
            }
            catch (Exception err)
            {
                throw err;
                //FileLogger.Write("Error in Vanrise.Fzero.Bypass.FillReceivedCalls()", err);
            }

        }
        public static void TruncateGeneratedCalls()
        {
            try
            {
                using (Entities context = new Entities())
                {
                    context.TruncateGeneratedCalls();
                }
            }
            catch (Exception err)
            {
                throw err;
                //FileLogger.Write("Error in Vanrise.Fzero.Bypass.TruncateGeneratedCalls()", err);
            }

        }
    }
}
