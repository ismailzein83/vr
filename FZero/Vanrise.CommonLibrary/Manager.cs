using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace Vanrise.CommonLibrary
{
    public class Manager
    {
        private static bool ContainIllegalCharacter(string text)
        {
            string illegalChar = "()/|?,;:'~<>\\+=.[]{}";

            text = text.Trim();
            char[] characters = text.ToCharArray();

            foreach (char character in characters)
            {
                if (illegalChar.Contains(character))
                    return true;
            }
            return false;
        }

        public static bool ContainWhiteSpace(string text)
        {
            string whiteSpace = " \t\n\r";

            text = text.Trim();
            char[] characters = text.ToCharArray();

            foreach (char character in characters)
            {
                if (whiteSpace.Contains(character))
                    return true;
            }
            return false;
        }

        public static bool ContainIllegalOrWhiteSpace(string text)
        {
            string whiteSpace = " \t\n\r";
            string illegalChar = "()/|?,;:'~<>\\+=.[]{}";
            string pattern = illegalChar + whiteSpace;

            char[] characters = text.ToCharArray();

            foreach (char character in characters)
            {
                if (pattern.Contains(character))
                    return true;
            }
            return false;
        }

        public static bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }

        /// <summary>
        /// This method will check a url to see that it does not return server or protocol errors
        /// </summary>
        /// <param name="url">The path to check</param>
        /// <returns></returns>
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

        public static bool IsValidZipCode(string zipCode)
        {
            return Regex.IsMatch(zipCode, @"^(\d{5})(-\d{4})?$");
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

        public static bool IsDecimal(string text)
        {
            string pattern = @"^\d+(\.\d)?$";
            return Regex.IsMatch(text, pattern);
        }

        public static bool IsInteger(string text)
        {
            string pattern = @"^\d+$";
            return Regex.IsMatch(text, pattern);
        }

        public static double GetDouble(string text)
        {
            double value;
            if (!double.TryParse(text, out value))
                value = 0;
            return value;
        }

        public static double GetDouble(string text, double defaultValue)
        {
            double value;
            if (!double.TryParse(text, out value))
                return defaultValue;
            return value;
        }

        public static decimal GetDecimal(string text)
        {
            decimal value;
            if (!decimal.TryParse(text, out value))
                value = 0;
            return value;
        }

        public static decimal GetDecimal(string text, decimal defaultValue)
        {
            decimal value;
            if (!decimal.TryParse(text, out value))
                return defaultValue;
            return value;
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

        public static short GetShort(string text)
        {
            short value;
            if (!short.TryParse(text, out value))
                value = 0;
            return value;
        }

        public static string GetRandom()
        {
            return new Random().Next(10, 999999).ToString();
        }

        //public static string GetHash(string data)
        //{
        //    MD5 md = MD5CryptoServiceProvider.Create();
        //    byte[] hash;
        //    StringBuilder sb = new StringBuilder();
        //    ASCIIEncoding enc = new ASCIIEncoding();
        //    byte[] buffer = enc.GetBytes(data);
        //    hash = md.ComputeHash(buffer);
        //    foreach (byte b in hash)
        //    {
        //        sb.Append(b.ToString("x2"));
        //    }
        //    return sb.ToString();
        //}

        public static string GetMultilingualProperty(object obj, string propertyName)
        {
            string value = string.Empty;

            //string postfix = System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == Language.English.Culture.TwoLetterISOLanguageName ? "En" : "Ar";

            //try
            //{
            //    value = obj.GetType().GetProperty(propertyName + postfix).GetValue(obj, null).ToString();
            //}
            //catch (Exception err)
            //{
            //    FileLogger.Write("Error in MultilingualHelper.GetMultilingualProperty( propertyName: " + propertyName + " )", err);
            //}

            return value;
        }

        public static DateTime GetDate(string date)
        {
            DateTime result = DateTime.MinValue;
            DateTime.TryParseExact(date, Formatter.DateFormat, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out result);
            return result;
        }

        public static DateTime GetDate(string date, string format)
        {
            DateTime result = DateTime.MinValue;
            DateTime.TryParseExact(date, format, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out result);
            return result;
        }

        public static string FromDate(DateTime date)
        {
            return date.ToString(Formatter.DateFormat);
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

        public static void BindRadioButtonList<T>(RadioButtonList rbl, List<T> source, string DataTextField, string DataValueField)
        {
            rbl.DataTextField = DataTextField;
            rbl.DataValueField = DataValueField;
            rbl.DataSource = source;
            rbl.DataBind();
        }

        public static void InsertData<T>(List<T> list, string TabelName,string CurrentConnectionString)
        {
            DataTable dt = new DataTable("MyTable");
            dt = ConvertToDataTable(list);

            using (SqlBulkCopy bulkcopy = new SqlBulkCopy(ConfigurationManager.ConnectionStrings[CurrentConnectionString].ConnectionString))
            {
                bulkcopy.BulkCopyTimeout = 660;
                bulkcopy.DestinationTableName = TabelName;
                bulkcopy.WriteToServer(dt);
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

        /// <summary>
        /// Gets query string value by name
        /// </summary>
        /// <param name="name">Parameter name</param>
        /// <returns>Query string value</returns>
        public static string QueryString(string name)
        {
            string result = string.Empty;
            if (HttpContext.Current != null && HttpContext.Current.Request.QueryString[name] != null)
                result = HttpContext.Current.Request.QueryString[name].ToString();
            return result;
        }

        //private static readonly Random strRandom = new Random();
        //private static const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ123456789"; //Added 1-9

        //private static string RandomString(int size)
        //{
        //    char[] buffer = new char[size];

        //    for (int i = 0; i < size; i++)
        //    {
        //        buffer[i] = chars[strRandom.Next(chars.Length)];
        //    }
        //    return new string(buffer);
        //}

        public static string GetString(int id, int length)
        {
            StringBuilder format = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                format.Append("0");
            }

            return id.ToString(format.ToString());
        }

        public static T GetEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value);
        }
      

        public void BindComboBox(dynamic source, DropDownList ddl, string DataTextField, string DataValueField)
        {
            ddl.Items.Clear();
            ddl.DataTextField = DataTextField;
            ddl.DataValueField = DataValueField;
            ddl.DataSource = source;
            ddl.DataBind();
            ddl.Items.Insert(0, new ListItem((string.Empty != null ? string.Empty : string.Empty), string.Empty));

        }


        //Radiobuttonlists, Checkboxlists, ListBox
        public void BindLists<T>(IList<T> source, ListControl LC, string DataTextField, string DataValueField)
        {
            LC.DataTextField = DataTextField;
            LC.DataValueField = DataValueField;
            LC.DataSource = source;
            LC.DataBind();
        }

        //Radiobuttonlists, Checkboxlists, ListBox
        public void BindLists(dynamic source, ListControl LC, string DataTextField, string DataValueField)
        {
            LC.DataTextField = DataTextField;
            LC.DataValueField = DataValueField;
            LC.DataSource = source;
            LC.DataBind();
        }


        public void BindGrid<T>(IList<T> source, GridView gv)
        {
            gv.DataSource = source;
            gv.DataBind();
        }


        public void BindGrid(dynamic source, GridView gv)
        {
            gv.DataSource = source;
            gv.DataBind();

        }

        #region Security
        static byte[] bytes = ASCIIEncoding.ASCII.GetBytes("ZeroCool");

        public static string Encrypt(string originalPassword)
        {
            try
            {
                if (String.IsNullOrEmpty(originalPassword))
                {
                    throw new ArgumentNullException("The string which needs to be encrypted can not be null.");
                }

                DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
                MemoryStream memoryStream = new MemoryStream();
                CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoProvider.CreateEncryptor(bytes, bytes), CryptoStreamMode.Write);

                StreamWriter writer = new StreamWriter(cryptoStream);
                writer.Write(originalPassword);
                writer.Flush();
                cryptoStream.FlushFinalBlock();
                writer.Flush();

                return Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
            }
            catch (Exception ex) { FileLogger.Write("Error in Encrypt",ex); }

            return "";
        }

        public static string Decrypt(string cryptedPassword)
        {
            try
            {
                if (String.IsNullOrEmpty(cryptedPassword))
                {
                    throw new ArgumentNullException("The string which needs to be decrypted can not be null.");
                    //return "";
                }

                DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
                MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(cryptedPassword));
                CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoProvider.CreateDecryptor(bytes, bytes), CryptoStreamMode.Read);
                StreamReader reader = new StreamReader(cryptoStream);

                return reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                FileLogger.Write("Error in Decrypt", ex);
            }
            return "";
        }
        #endregion


    }
}