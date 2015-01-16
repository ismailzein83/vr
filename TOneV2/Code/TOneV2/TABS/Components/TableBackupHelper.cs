using System;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.IO.Compression;
using System.IO;
using System.Text.RegularExpressions;

namespace TABS.Components
{
    public class TableBackupHelper
    {

        protected static log4net.ILog log = log4net.LogManager.GetLogger(typeof(TableBackupHelper));
        /// <summary>
        /// Get an array of decoding instructions for each column in the DataTable dt
        /// </summary>
        /// <param name="stringColumns"></param>
        /// <param name="dt">the data table to get instructions for</param>
        /// <returns>Array containing 0 -> do nothing, 1 -> decode, 2 -> decode with UTF8 conversion</returns>
        private static int[] GetDecodingInstructions(string[] stringColumns, DataTable dt)
        {
            int[] results = new int[dt.Columns.Count];
            for( int i =0; i < dt.Columns.Count; i++)
            {
                DataColumn col = dt.Columns[i];
                if(col.ColumnName.EndsWith("_Base64"))
                {
                    string columnName = col.ColumnName.Substring(0, col.ColumnName.IndexOf("_Base64"));
                    bool isString = stringColumns.Where(c => c.Equals(columnName, StringComparison.InvariantCultureIgnoreCase)).Count() > 0;
                    if(isString)
                        results[i] = 2;
                    else
                        results[i] = 1;
                }
                else
                    results[i] = 0;
            }
            return results;
        }


        /// <summary>
        /// Based on the <para>decodingInstruction</para> return the same or decoded string item.
        /// </summary>
        /// <param name="item">to decode</param>
        /// <param name="decodingInstruction">the instruction being 0, 1, or 2</param>
        /// <returns></returns>
        private static object DecodeIfNecessary(string item, int decodingInstruction)
        {
            if (string.IsNullOrEmpty(item)) return null;
            switch (decodingInstruction)
            {
                case 2:
                        return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(item));
                case 1:
                        return Convert.FromBase64String(item);
                case 0:
                default:
                        return item;
            }
        }

        /// <summary>
        /// Remove the '_Base64' suffix from all columns
        /// </summary>
        /// <param name="data"></param>
        private static void RemoveColumnSuffixes(DataSet data)
        {
            foreach (DataTable table in data.Tables)
            {
                if (table.Rows.Count > 0)
                {
                    int index = 0;
                    while (index < table.Columns.Count)
                    {
                        DataColumn column = table.Columns[index++];
                        if (column.ColumnName.EndsWith("_Base64"))
                            column.ColumnName = column.ColumnName.Substring(0, column.ColumnName.IndexOf("_Base64"));
                    }
                }
            }
        }

        public static DataSet GetData(string[] stringColumns, byte[] compressedTablesData)
        {
            DataSet data = new DataSet();
            DataTable workTable = null;
            int[] decodingInstructions = null;

            // Decompress the state data
            using (System.IO.MemoryStream unCompressed = WebHelperLibrary.StreamHelper.Decompress(compressedTablesData))
            {
                StreamReader reader = new StreamReader(unCompressed);
                string line = reader.ReadLine();
                int lineNumber = 1;
                if (line != null)
                {
                    lineNumber++;
                    line = reader.ReadLine();
                    while (line != null)
                    {
                        // Empty line? 
                        if (line == string.Empty)
                        {
                            if (workTable != null)
                                workTable.EndLoadData();
                            workTable = null;
                        }
                        else
                        {
                            bool createNewTable = workTable == null;
                            if (createNewTable)
                            {
                                try
                                {
                                    workTable = new DataTable();
                                    workTable.TableName = Regex.Match
                                        (line, @"\[(?<TABLENAME>\w+)\]:\s+(?<FIELDS>\d+)\s+Fields,\s+(?<ROWS>\d+)\s+Rows").Groups["TABLENAME"].Value;
                                    // Columns
                                    lineNumber++;
                                    string fieldsLine = reader.ReadLine().Trim();
                                    BuildTable(stringColumns, workTable, fieldsLine);
                                    data.Tables.Add(workTable);
                                    workTable.BeginLoadData();
                                    decodingInstructions = GetDecodingInstructions(stringColumns, workTable);
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception("Error while paring table definition, line #" + lineNumber, ex);
                                }
                            }
                            else //read the data into the datatable
                            {
                               
                                    string[] items = line.Split('\t');
                                    DataRow row = workTable.NewRow();
                                    object[] itemArray = new object[items.Length];
                                    for (int i = 0; i < items.Length; i++)
                                        itemArray[i] = items[i] == string.Empty ? null : DecodeIfNecessary(items[i], decodingInstructions[i]);
                                    try
                                    {
                                        row.ItemArray = itemArray;
                                    }
                                    catch (Exception ex)
                                    {
                                        throw ex;
                                    }
                                    workTable.Rows.Add(row);
                               
                            }
                        }
                        lineNumber++;
                        if (lineNumber % 100 == 0)
                            log.InfoFormat("Reading line: {0}", lineNumber);
                        line = reader.ReadLine();
                    }
                }
            }
            RemoveColumnSuffixes(data);
            return data;
        }

        private static void BuildTable(string[] stringColumns, DataTable workTable, string fieldsLine)
        {
            string[] fields = fieldsLine.Split('\t');
            foreach (string fieldEntry in fields)
            {
                string[] fieldParts = fieldEntry.Split(':');
                string colName = fieldParts[0], colTypeName = fieldParts[1], noSuffixColName = fieldParts[0];
                Type colType = null;
                if (colName.EndsWith("_Base64"))
                {
                    noSuffixColName = colName.Substring(0, colName.IndexOf("_Base64"));
                    bool isStringCol = stringColumns.Where(c => c.Equals(noSuffixColName, StringComparison.InvariantCultureIgnoreCase)).Count() > 0;
                    colType = isStringCol ? typeof(string) : typeof(byte[]);
                }
                else
                    colType = Type.GetType(fieldParts[1]);
                workTable.Columns.Add(colName, colType);
            }
        }

        
        
        public static void WriteData(SqlDataReader data, System.IO.StreamWriter writer, string tableName)
        {
            string[] removableColumns = new string[] { "iseffective", "timestamp" };
            string[] encodedTextColumns = new string[] { "description", "notes" };
            WriteData(data, writer, tableName, removableColumns, encodedTextColumns);
        }
        public static void WriteData(DataTable data, System.IO.StreamWriter writer, string tableName)
        {
            string[] removableColumns = new string[] { "iseffective", "timestamp" };
            string[] encodedTextColumns = new string[] { "description", "notes" };
            WriteData(data, writer, tableName, removableColumns, encodedTextColumns);
        }
        /// <summary>
        /// Write the Data Reader contents as Tab Delimited Text into the stream writer. 
        /// Any removable columns in all tables are not written.
        /// encodedTextColumns are converted to base 64 before being written.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="writer"></param>
        /// <param name="removableColumns"></param>
        /// <param name="encodedTextColumns"></param>
        public static void WriteData(SqlDataReader data, StreamWriter writer, string tableName, string[] removableColumns, string[] encodedTextColumns)
        {
            // Write table rows
            WebHelperLibrary.DataReaderTextReader reader = new WebHelperLibrary.DataReaderTextReader(data, WebHelperLibrary.TextDataFormat.TabSeparatedValues, removableColumns, encodedTextColumns);
            DataTable dtMeta = data.GetSchemaTable();
            //get the initial position
            MemoryStream ms = ((GZipStream)writer.BaseStream).BaseStream as MemoryStream;
            //long pos = ms.Position;
            // Write table name and header
            writer.WriteLine(string.Format("[{0}]: {1} Fields, {2} Rows", tableName, dtMeta.Rows.Count, 0));
            foreach (DataRow row in dtMeta.Rows)
                if (!removableColumns.Select(c => c.ToLower()).Contains(row["ColumnName"].ToString().ToLower()))
                {
                    Type colType = (Type)row["DataType"];
                    string colName = (string)row["ColumnName"];
                    string base64Suffix = (encodedTextColumns.Select(c => c.ToLower()).Contains(colName.ToLower()) || colType == typeof(byte[]) ?  "_Base64" : string.Empty);
                    string colTypeConvertedName = (colType == typeof(byte[]) ? typeof(string).FullName : colType.FullName);
                    writer.Write(string.Format("{0}:{1}\t", colName + base64Suffix, colTypeConvertedName));
                }
            writer.WriteLine();

            string rowLine = null;
            int rowCount = 0;
            do
            {
                rowLine = reader.ReadLine();
                if (rowLine != null)
                {
                    rowLine = rowLine.Replace('\n', ' ').Replace('\r', ' ');
                    writer.WriteLine(rowLine);
                }
                if (rowLine != null) rowCount++;
            } while (rowLine != null);

            // Separate tables by new line
            writer.WriteLine();
            writer.Flush();

            //write the correct value for row count
            //skip first 2 bytes (some call it magic number) number
            //pos = pos == 0 ? 2 : pos;
            //ms.Seek(pos, SeekOrigin.Begin);
            //writer.WriteLine(string.Format("[{0}]: {1} Fields, {2,10} Rows", tableName, dtMeta.Rows.Count, rowCount));
            //writer.Flush();
            //ms.Seek(0, SeekOrigin.End);

        }
        public static void WriteData(DataTable data, StreamWriter writer, string tableName, string[] removableColumns, string[] encodedTextColumns)
        {
            // Write table rows
            WebHelperLibrary.DataReaderTextReader reader = new WebHelperLibrary.DataReaderTextReader(data, WebHelperLibrary.TextDataFormat.TabSeparatedValues, removableColumns, encodedTextColumns);
            DataTable dtMeta = null;
            using (DataTableReader r = new DataTableReader(data))
            {
                dtMeta=r.GetSchemaTable();
            }
            
            //get the initial position
            MemoryStream ms = ((GZipStream)writer.BaseStream).BaseStream as MemoryStream;
            //long pos = ms.Position;
            // Write table name and header
            writer.WriteLine(string.Format("[{0}]: {1} Fields, {2} Rows", tableName, dtMeta.Rows.Count, 0));
            foreach (DataRow row in dtMeta.Rows)
                if (!removableColumns.Select(c => c.ToLower()).Contains(row["ColumnName"].ToString().ToLower()))
                {
                    Type colType = (Type)row["DataType"];
                    string colName = (string)row["ColumnName"];
                    string base64Suffix = (encodedTextColumns.Select(c => c.ToLower()).Contains(colName.ToLower()) || colType == typeof(byte[]) ? "_Base64" : string.Empty);
                    string colTypeConvertedName = (colType == typeof(byte[]) ? typeof(string).FullName : colType.FullName);
                    writer.Write(string.Format("{0}:{1}\t", colName + base64Suffix, colTypeConvertedName));
                }
            writer.WriteLine();

            string rowLine = null;
            int rowCount = 0;
            do
            {
                rowLine = null;
                if (rowCount < data.Rows.Count)
                {
                    rowLine = reader.ReadLine(data.Rows[rowCount]);
                    if (rowLine != null)
                    {
                        rowLine = rowLine.Replace('\n', ' ').Replace('\r', ' ');
                        writer.WriteLine(rowLine);
                    }
                    if (rowLine != null) rowCount++;
                }
            } while (rowLine != null);

            // Separate tables by new line
            writer.WriteLine();
            writer.Flush();

            //write the correct value for row count
            //skip first 2 bytes (some call it magic number) number
            //pos = pos == 0 ? 2 : pos;
            //ms.Seek(pos, SeekOrigin.Begin);
            //writer.WriteLine(string.Format("[{0}]: {1} Fields, {2,10} Rows", tableName, dtMeta.Rows.Count, rowCount));
            //writer.Flush();
            //ms.Seek(0, SeekOrigin.End);

        }
    }
}
