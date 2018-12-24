using Microsoft.SqlServer.Management.Smo;
using System;
using Vanrise.Common;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vanrise.HelperTools
{
    public partial class RDBGenerator : Form
    {
        //NVarchar, Int, BigInt, Decimal, DateTime, UniqueIdentifier, Boolean, VarBinary, Cursor
        private Dictionary<SqlDataType, string> dataTypes = new Dictionary<SqlDataType, string>(){
          {SqlDataType.NChar ,"NVarchar"},
          {SqlDataType.VarChar ,"Varchar"},
          {SqlDataType.VarCharMax ,"Varchar"},
          {SqlDataType.NVarChar ,"NVarchar"},
          {SqlDataType.NVarCharMax ,"NVarchar"},
          {SqlDataType.Int , "Int"},
          {SqlDataType.BigInt , "BigInt"},
          {SqlDataType.Decimal , "Decimal"},
          {SqlDataType.DateTime , "DateTime"},
          {SqlDataType.Date, "DateTime"},
          {SqlDataType.UniqueIdentifier,"UniqueIdentifier"},
          {SqlDataType.Bit,"Boolean"},
          {SqlDataType.VarBinary,"VarBinary"},
          {SqlDataType.VarBinaryMax,"VarBinary"},
          {SqlDataType.TinyInt, "Int"}
        };
        public RDBGenerator()
        {
            InitializeComponent();

        }

        private void GenerateRDBCode_Click(object sender, EventArgs e)
        {
            GenerateRDBCode.Enabled = false;
            if (!string.IsNullOrEmpty(this.connectionString.Text))
            {
                this.generatedCode.Text = string.Empty;
                string connection = this.connectionString.Text;
                System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(connection);
                Server s = new Server(new Microsoft.SqlServer.Management.Common.ServerConnection(conn));
                Database connectedDataBase = s.Databases[conn.Database];
                StringBuilder generatedCode = new StringBuilder();
                int tablesCount = connectedDataBase.Tables.Count;
                for (int i = 0; i < tablesCount; i++)
                {
                   
                    Table t = connectedDataBase.Tables[i];
                    if (t.IsSystemObject) continue;
                    string tablefullname = string.Format("'{0}.{1}'", t.Schema, t.Name);
                    generatedCode.Append(string.Format("_ _ _ _ _ _ _ _ START{0} _ _ _ _ _ _", tablefullname));
                    generatedCode.Append(string.Format("_ _ _ _ _ _ _ _ begin{0} _ _ _ _ _ _", DateTime.Now.ToString()));
                    AppendLines(generatedCode, 2);

                    generatedCode.Append(BuildTableStaticFields(t));

                    AppendLines(generatedCode, 2);

                    generatedCode.Append(BuildTableDataManagerConstructor(t));

                    AppendLines(generatedCode, 2);
                    generatedCode.Append(string.Format("_ _ _ _ _ _ _ _ ends{0} _ _ _ _ _ _", DateTime.Now.ToString()));
                    generatedCode.Append(string.Format("_ _ _ _ _ _ _ _ END _ _ _ _ _ _"));

                    AppendLines(generatedCode, 2);
                }

                this.generatedCode.Text = generatedCode.ToString();
                GenerateRDBCode.Enabled = true;
            }
        }

        string BuildTableStaticFields(Table table)
        {
            StringBuilder staticFields = new StringBuilder();
            staticFields.Append(string.Format("static string TABLE_NAME =\"{0}_{1}\";", table.Schema, table.Name));
            staticFields.AppendLine();
            foreach (Column col in table.Columns)
            {
                if (col.DataType.SqlDataType == SqlDataType.Timestamp) continue;
                staticFields.Append(string.Format("const string COL_{0} = \"{0}\";", col.Name));
                staticFields.AppendLine();
            }

            return staticFields.ToString();
        }


        string BuildTableDataManagerConstructor(Table table)
        {
            StringBuilder dataManagerConstructor = new StringBuilder();
            dataManagerConstructor.Append(string.Format("static {0}DataManager()", table.Name));
            dataManagerConstructor.AppendLine();
            dataManagerConstructor.Append(string.Format("{{"));
            dataManagerConstructor.AppendLine();
            dataManagerConstructor.Append(string.Format("\t var columns = new Dictionary<string, RDBTableColumnDefinition>();"));
            dataManagerConstructor.AppendLine();
            Column identifierColumn = null;
            foreach (Column col in table.Columns)
            {
                if (col.DataType.SqlDataType == SqlDataType.Timestamp) continue;
                dataManagerConstructor.Append(string.Format("\t columns.Add(COL_{0}, new RDBTableColumnDefinition{{{1}}});", col.Name, BuildRDBTableColumnDefinition(col)));
                dataManagerConstructor.AppendLine();
                if (col.InPrimaryKey && identifierColumn == null)
                {
                    identifierColumn = col;
                }
                else if (col.InPrimaryKey && identifierColumn != null)
                {
                    identifierColumn = null;
                }
            }
            dataManagerConstructor.Append(string.Format("\t RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition"));
            dataManagerConstructor.Append(string.Format("{{"));
            dataManagerConstructor.AppendLine();
            dataManagerConstructor.Append(string.Format("\t \t DBSchemaName = \"{0}\",", table.Schema));
            dataManagerConstructor.AppendLine();
            dataManagerConstructor.Append(string.Format("\t \t DBTableName = \"{0}\",", table.Name));
            dataManagerConstructor.AppendLine();
            dataManagerConstructor.Append(string.Format("\t \t Columns = columns", table.Name));
            if (identifierColumn != null)
            {
                dataManagerConstructor.Append(",");
                dataManagerConstructor.AppendLine();
                dataManagerConstructor.Append(string.Format("\t \t IdColumnName = COL_{0}", identifierColumn.Name));
               
            }

            if (table.Columns["CreatedTime"] != null)
            {
                dataManagerConstructor.Append(",");
                dataManagerConstructor.AppendLine();
                dataManagerConstructor.Append(string.Format("\t \t CreatedTimeColumnName = COL_CreatedTime"));
            }

            if (table.Columns["LastModifiedTime"] != null)
            {
                dataManagerConstructor.Append(",");
                dataManagerConstructor.AppendLine();
                dataManagerConstructor.Append(string.Format("\t \t ModifiedTimeColumnName = COL_LastModifiedTime"));
                dataManagerConstructor.AppendLine();
            }
            dataManagerConstructor.AppendLine();
            dataManagerConstructor.Append(string.Format("\t }});"));
            dataManagerConstructor.AppendLine();
            dataManagerConstructor.Append(string.Format("}}"));

            return dataManagerConstructor.ToString();
        }

        string BuildRDBTableColumnDefinition(Column column)
        {
            StringBuilder columnDefinition = new StringBuilder();
             string recordType = dataTypes.GetRecord(column.DataType.SqlDataType);
             if (recordType == null)
                throw new NotImplementedException();
           
            columnDefinition.Append(string.Format("DataType = RDBDataType.{0}", dataTypes.GetRecord(column.DataType.SqlDataType)));
            if (column.DataType.MaximumLength > 0 && (recordType.Equals("NVarchar") || recordType.Equals("Varchar")))
                columnDefinition.Append(string.Format(", Size = {0} ", column.DataType.MaximumLength));
            if (column.DataType.NumericPrecision > 0 && recordType.Equals("Decimal"))
                columnDefinition.Append(string.Format(", Size = {0}, Precision = {1} ", column.DataType.NumericPrecision, column.DataType.NumericScale));
            return columnDefinition.ToString();
        }
        void AppendLines(StringBuilder stringBuilder, int numberOflines)
        {

            for (int i = 0; i < numberOflines; i++)
            {
                stringBuilder.AppendLine();
            }
        }
    }
}
