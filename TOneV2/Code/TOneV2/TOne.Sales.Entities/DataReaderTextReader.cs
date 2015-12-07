
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Sales.Entities
{
    public class DataReaderTextReader : System.IO.TextReader, IDisposable
    {
        SqlDataReader _DataReader;
        DataTable _Table;
        TextDataFormat _TextDataFormat = TextDataFormat.TabSeparatedValues;
        string _NewLine = "\n";
        public string NewLine { get { return _NewLine; } set { _NewLine = value; } }

        public string[] _RemovableColumns, _UTF8Columns;
        public string[] RemovableColumns
        {
            get { return _RemovableColumns; }
            set
            {
                _RemovableColumns = value;
                DataTable dtSchema = null;
                if (_DataReader != null)
                    dtSchema = _DataReader.GetSchemaTable();
                else
                {
                    if (_Table != null)
                    {
                        using (DataTableReader r = new DataTableReader(_Table))
                        {
                            dtSchema = r.GetSchemaTable();
                        }
                    }
                }
                List<int> columnIndexesList = new List<int>();
                for (int i = 0; i < _RemovableColumns.Length; i++)
                {
                    foreach (DataRow dr in dtSchema.Rows)
                    {
                        if (dr["ColumnName"].ToString().Equals(_RemovableColumns[i], StringComparison.InvariantCultureIgnoreCase))
                            columnIndexesList.Add((int)dr["ColumnOrdinal"]);
                    }
                }
                _RemovableColumnIndexes = columnIndexesList.ToArray();
            }
        }
        public string[] UTF8Columns
        {
            get { return _UTF8Columns; }
            set
            {
                _UTF8Columns = value;
                DataTable dtSchema = null;
                if (_DataReader != null)
                    dtSchema = _DataReader.GetSchemaTable();
                else
                {
                    if (_Table != null)
                    {
                        using (DataTableReader r = new DataTableReader(_Table))
                        {
                            dtSchema = r.GetSchemaTable();
                        }
                    }
                }
                List<int> columnIndexesList = new List<int>();
                for (int i = 0; i < _UTF8Columns.Length; i++)
                {
                    foreach (DataRow dr in dtSchema.Rows)
                    {
                        if (dr["ColumnName"].ToString().Equals(_UTF8Columns[i], StringComparison.InvariantCultureIgnoreCase))
                            columnIndexesList.Add((int)dr["ColumnOrdinal"]);
                    }
                }
                _UTF8ColumnIndexes = columnIndexesList.ToArray();
            }
        }
        private int[] _RemovableColumnIndexes, _UTF8ColumnIndexes;



        /// <summary>
        /// Create a Data Reader Text Reader for the specified data table using the layout text data format.
        /// </summary>
        /// <param name="data">The Data Table to read</param>
        /// <param name="dataFormat">The format in which to layout the rows as lines</param>
        public DataReaderTextReader(SqlDataReader data, TextDataFormat dataFormat, string[] removableColumns, string[] utf8Columns)
        {
            if (data == null) throw new InvalidOperationException("Cannot Start a reader from a Null Data Reader");

            _DataReader = data;
            _TextDataFormat = dataFormat;
            RemovableColumns = removableColumns;
            UTF8Columns = utf8Columns;
        }
        public DataReaderTextReader(DataTable dataTable, TextDataFormat dataFormat, string[] removableColumns, string[] utf8Columns)
        {
            if (dataTable == null) throw new InvalidOperationException("Cannot Start a reader from a Null Data Table");

            _DataReader = null;
            _Table = dataTable;
            _TextDataFormat = dataFormat;
            RemovableColumns = removableColumns;
            UTF8Columns = utf8Columns;
        }

        /// <summary>
        /// Create a Data Reader Text Reader for the specified data reader using tab separated fields, each row on a line
        /// </summary>
        /// <param name="data">The data to read as text</param>
        public DataReaderTextReader(SqlDataReader data, string[] removableColumns, string[] utf8Columns)
            : this(data, TextDataFormat.TabSeparatedValues, removableColumns, utf8Columns)
        {

        }
        //public DataReaderTextReader(DataTable data, string[] removableColumns, string[] utf8Columns)
        //    : this(data, TextDataFormat.TabSeparatedValues, removableColumns, utf8Columns)
        //{

        //}


        /// <summary>
        /// The Text Layout for each line 
        /// </summary>
        public TextDataFormat TextDataFormat { get { return _TextDataFormat; } }

        /// <summary>
        /// Read a row as a line (CSV or Tab delimited), without the end-of-line terminator(s).
        /// Will return a null if no data is available any more.
        /// </summary>
        /// <returns></returns>
        public override string ReadLine()
        {
            if (_DataReader.Read())
            {
                char separator = _TextDataFormat == TextDataFormat.CommaSeparatedValues ? ',' : '\t';
                int columnCount = _DataReader.FieldCount;
                StringBuilder sb = new StringBuilder();
                int[] encodingInstructions = GetColumnEncodingInstructions();
                for (int i = 0; i < columnCount; i++)
                {
                    if (!_RemovableColumnIndexes.Contains(i))
                    {
                        if (sb.Length > 0)
                            sb.Append(separator);
                        if (_DataReader[i] != null && _DataReader[i] != DBNull.Value)
                            sb.Append(EncodeIfNeeded(_DataReader[i], encodingInstructions[i]));
                    }
                }
                return sb.ToString();
            }
            else
                return null;
        }
        public string ReadLine(DataRow row)
        {
            if (row != null)
            {
                char separator = _TextDataFormat == TextDataFormat.CommaSeparatedValues ? ',' : '\t';
                int columnCount = _Table.Columns.Count;
                StringBuilder sb = new StringBuilder();
                int[] encodingInstructions = GetColumnEncodingInstructions();
                for (int i = 0; i < columnCount; i++)
                {
                    if (!_RemovableColumnIndexes.Contains(i))
                    {
                        if (sb.Length > 0)
                            sb.Append(separator);
                        if (row != null && row[i] != DBNull.Value)
                            sb.Append(EncodeIfNeeded(row[i], encodingInstructions[i]));
                    }
                }
                return sb.ToString();
            }
            else
                return null;
        }

        /// <summary>
        /// 0 => do nothing
        /// 1 => encode base 64
        /// 2 => convert to utf8 then encode base 64
        /// </summary>
        /// <param name="data"></param>
        /// <param name="instruction"></param>
        /// <returns></returns>
        private string EncodeIfNeeded(object data, int instruction)
        {
            if (data != null)
                switch (instruction)
                {
                    case 0:
                        if (data.GetType() == typeof(DateTime))
                            return ((DateTime)data).ToString("yyyy-MM-dd HH:mm:ss.fff");
                        return data.ToString();
                    case 1:
                        return Convert.ToBase64String((byte[])data, Base64FormattingOptions.None);
                    case 2:
                        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(data.ToString()), Base64FormattingOptions.None);
                }
            return null;
        }

        /// <summary>
        /// Read the 
        /// </summary>
        /// <returns></returns>
        public override string ReadToEnd()
        {
            StringBuilder sb = new StringBuilder();
            string line = ReadLine();
            while (line != null)
            {
                sb.Append(line).Append(NewLine);
                line = ReadLine();
            }
            return sb.ToString();
        }




        /// <summary>
        /// Get an array of the column names of the data reader where the columns that would
        /// be Base 64 encoded are suffixed by "_Base64"
        /// </summary>
        /// 0 => don't encode
        /// 1 => encode
        /// 2 => encode + utf8
        /// </param>
        /// <returns>array of adjusted column names</returns>
        public string[] GetBase64ColumnNames()
        {
            DataTable dt = _DataReader.GetSchemaTable();
            int columnCount = dt.Rows.Count;
            int[] encodingInstructions = GetColumnEncodingInstructions();
            string[] columnNames = new string[columnCount];
            for (int i = 0; i < columnCount; i++)
                columnNames[i] = dt.Rows[i]["ColumName"] + ((encodingInstructions[i] != 0) ? "_Base64" : string.Empty);
            return columnNames;
        }


        /// <summary>
        /// Get an array of encoding instructions for columns of the data reader where:
        /// 0 => don't encode
        /// 1 => encode
        /// 2 => encode + utf8
        /// </summary>
        /// <returns>array of instructions</returns>
        private int[] GetColumnEncodingInstructions()
        {
            DataTable dt = null;
            int columnCount = 0;
            if (_DataReader != null)
            {
                dt = _DataReader.GetSchemaTable();
                columnCount = dt.Rows.Count;
            }
            else
            {

                using (DataTableReader r = new DataTableReader(_Table))
                {
                    dt = r.GetSchemaTable();
                }
                columnCount = dt.Rows.Count;
            }
            int[] encodingInstructions = new int[columnCount];
            for (int i = 0; i < columnCount; i++)
            {
                DataRow row = dt.Rows[i];
                if (_UTF8ColumnIndexes.Contains((int)row["ColumnOrdinal"]))
                    encodingInstructions[i] = 2;
                else if (row["DataType"] == typeof(byte[]))
                    encodingInstructions[i] = 1;
                else
                    encodingInstructions[i] = 0;
            }
            return encodingInstructions;
        }


        #region IDisposable Members

        /// <summary>
        /// Dispose the underlying data and release reference
        /// </summary>
        void IDisposable.Dispose()
        {
            if (_DataReader != null)
                _DataReader.Dispose();
            _DataReader = null;
        }

        #endregion
    }

    public enum TextDataFormat
    {
        TabSeparatedValues,
        CommaSeparatedValues
    }

}
