using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace Vanrise.CommonLibrary
{
    public class Manager
    {
        public static bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }

        public static bool IsValidUrl(string url)
        {
            try
            {
                if (!url.Contains("http://") || !url.Contains("https://"))
                {
                    url = "http://" + url;
                }
                WebRequest webRequest = WebRequest.Create(url);
                WebResponse webResponse;
                webResponse = webRequest.GetResponse();
            }
            catch //If exception thrown then couldn't get response from address
            {
                return false;
            }
            return true;
        }

        public static string  IsValidPassword(string NewPassword, string ConfirmPassword) 
        {
            if (string.IsNullOrWhiteSpace(NewPassword))
            {
                return "Password is required!";
            }


            if (NewPassword != ConfirmPassword)
            {
                return "Passwords should match";
            }

            return string.Empty;
        }

        public static int GetInteger(string text)
        {
            int value;
            if (!int.TryParse(text, out value))
                value = 0;
            return value;
        }

        public static int GetInteger(string text, int defaultValue)
        {
            int value;
            if (!int.TryParse(text, out value))
                return defaultValue;
            return value;
        }

        public static bool? GetNullableBoolean(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;
            return value.ToLower() == "true";
        }

        public static void BindCombo<T>(RadComboBox ddl,List<T> source,  string DataTextField, string DataValueField, string DefaultTextField, string DefaultValueField)
        {
            ddl.DataTextField = DataTextField;
            ddl.DataValueField = DataValueField;
            ddl.DataSource = source;
            ddl.Sort = RadComboBoxSort.Ascending;
            ddl.DataBind();
            ddl.Items.Sort();
            if ( DataTextField !=string.Empty && DataValueField != string.Empty)
                ddl.Items.Insert(0, new RadComboBoxItem(DefaultTextField, DefaultValueField));

        }

        public static void BindCombo<T>(DropDownList ddl, List<T> source, string DataTextField, string DataValueField, string DefaultTextField, string DefaultValueField)
        {
            ddl.DataTextField = DataTextField;
            ddl.DataValueField = DataValueField;
            ddl.DataSource = source;
            ddl.DataBind();
            if (DataTextField != string.Empty && DataValueField != string.Empty)
                ddl.Items.Insert(0, new ListItem (DefaultTextField, DefaultValueField));

        }

        public static void InsertData<T>(List<T> list, string TabelName,string CurrentConnectionString)
        {
            DataTable dt = new DataTable("MyTable");
            dt = ConvertToDataTable(list);

            using (SqlBulkCopy bulkcopy = new SqlBulkCopy(ConfigurationManager.ConnectionStrings[CurrentConnectionString].ConnectionString))
            {

                bulkcopy.BulkCopyTimeout = 660;
                bulkcopy.DestinationTableName = TabelName;

                foreach (var column in dt.Columns)
                    bulkcopy.ColumnMappings.Add(column.ToString(), column.ToString());

                bulkcopy.WriteToServer(dt);



                
                //bulkcopy.BulkCopyTimeout = 660;
                //bulkcopy.DestinationTableName = TabelName;
                //bulkcopy.WriteToServer(dt);
            }
        }

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

                if (prop.PropertyType == typeof(Decimal?))
                {
                    table.Columns.Add(prop.Name, typeof(Decimal));
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

                    if (prop.PropertyType == typeof(int?) || prop.PropertyType == typeof(DateTime?) || prop.PropertyType == typeof(Decimal?) || prop.PropertyType == typeof(Boolean?))
                    {
                        row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                    }
                }

                table.Rows.Add(row);
            }
            return table;
        }

        public static string QueryString(string name)
        {
            string result = string.Empty;
            if (HttpContext.Current != null && HttpContext.Current.Request.QueryString[name] != null)
                result = HttpContext.Current.Request.QueryString[name].ToString();
            return result;
        }

    }
}