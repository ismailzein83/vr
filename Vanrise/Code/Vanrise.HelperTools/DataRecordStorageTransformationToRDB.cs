using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.RDBDataStorage;
using Vanrise.GenericData.SQLDataStorage;

namespace Vanrise.HelperTools
{
    public partial class DataRecordStorageTransformationToRDB : Form
    {
        public DataRecordStorageTransformationToRDB()
        {
            InitializeComponent();
        }

        private Dictionary<Guid, string> DataRecordStorages { get; set; }
        private void LoadDataRecordTables(object sender, EventArgs e)
        {
            List<string> listOfDataRecords = new List<string>();
            if (!string.IsNullOrEmpty(this.connectionString.Text))
            {
                this.generatedRDB.Text = string.Empty;
                string connection = this.connectionString.Text;
                using (SqlConnection con = new SqlConnection(connection))
                {
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = " SELECT ID, Name FROM [genericdata].DataRecordStorage WITH(NOLOCK)" +
                                      " WHERE Settings like '%Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageSettings, Vanrise.GenericData.SQLDataStorage%'ORDER BY[Name]";
                    cmd.CommandType = CommandType.Text;

                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        DataRecordStorages = new Dictionary<Guid, string>();
                        while (reader.Read())
                        {
                            DataRecordStorage readedObject = DataRecordMapperName(reader);
                            DataRecordStorages.Add(readedObject.DataRecordStorageId, readedObject.Name);
                            dataRecordStorageComboBox.Items.Add(readedObject.Name);
                        }
                        reader.Close();
                    }
                    con.Close();
                }
                dataRecordStorageComboBox.SelectedItem = dataRecordStorageComboBox.Items.Count > 0 ? dataRecordStorageComboBox.Items[0] : "--No Data--";
            }
        }

        private void GenerateRDB(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.connectionString.Text))
            {
                this.generatedRDB.Text = string.Empty;
                string connection = this.connectionString.Text;
                string dataRecordName = dataRecordStorageComboBox.Text.ToString();
                Guid dataRecordStorageID = DataRecordStorages.FirstOrDefault(x => x.Value == dataRecordName).Key;
                using (SqlConnection con = new SqlConnection(connection))
                {
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "SELECT Settings FROM [genericdata].DataRecordStorage WITH(NOLOCK) WHERE ID = @ID";
                    cmd.CommandType = CommandType.Text;


                    con.Open();
                    cmd.Parameters.AddWithValue("ID", dataRecordStorageID);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            SQLDataRecordStorageSettings sqlSettings = Vanrise.Common.Serializer.Deserialize<SQLDataRecordStorageSettings>(reader["Settings"] as string);
                            RDBDataRecordStorageSettings rdbDataRecordStorageSettings = SettingsTransformToRDB(sqlSettings);
                            generatedRDB.Text = Vanrise.Common.Serializer.Serialize(rdbDataRecordStorageSettings);
                        }
                        reader.Close();
                    }
                    con.Close();

                }
            }
        }

        private RDBDataRecordStorageSettings SettingsTransformToRDB(SQLDataRecordStorageSettings sqlDataRecordStorageSettings)
        {
            RDBDataRecordStorageSettings rdbDataRecordStorageSettings = new RDBDataRecordStorageSettings();
            rdbDataRecordStorageSettings.TableName = sqlDataRecordStorageSettings.TableName;
            rdbDataRecordStorageSettings.TableSchema = sqlDataRecordStorageSettings.TableSchema;
            rdbDataRecordStorageSettings.IncludeQueueItemId = sqlDataRecordStorageSettings.IncludeQueueItemId;
            rdbDataRecordStorageSettings.NullableFields = new List<RDBNullableField>();
            if (sqlDataRecordStorageSettings.NullableFields != null && sqlDataRecordStorageSettings.NullableFields.Count>0)
            {
                foreach (NullableField nullableField in sqlDataRecordStorageSettings.NullableFields)
                {
                    RDBNullableField rdbNullableField = new RDBNullableField();
                    rdbNullableField.Name = nullableField.Name;
                    rdbDataRecordStorageSettings.NullableFields.Add(rdbNullableField);
                }
            }
            
            rdbDataRecordStorageSettings.Columns = new List<RDBDataRecordStorageColumn>();
            foreach (SQLDataRecordStorageColumn sqlDataRecordStorageColumn in sqlDataRecordStorageSettings.Columns)
            {   
                // RDBDataRecordStorageSettings 
                RDBDataRecordStorageColumn rdbDataRecordStorageColumn = new RDBDataRecordStorageColumn();
                rdbDataRecordStorageColumn.ColumnName = sqlDataRecordStorageColumn.ColumnName;
                rdbDataRecordStorageColumn.FieldName = sqlDataRecordStorageColumn.ValueExpression;
                rdbDataRecordStorageColumn.IsUnique = sqlDataRecordStorageColumn.IsUnique;
                rdbDataRecordStorageColumn.IsIdentity = sqlDataRecordStorageColumn.IsIdentity;
                string sqlDataRecordStorageColumnDataType = new string(sqlDataRecordStorageColumn.SQLDataType.ToLower().Where(c => !char.IsWhiteSpace(c)).ToArray());
                switch (sqlDataRecordStorageColumnDataType)
                {
                    case string a when a.Contains("bit"):
                        rdbDataRecordStorageColumn.DataType = RDBDataType.Boolean;
                        break;

                    case string a when a.Contains("bigint"):
                        rdbDataRecordStorageColumn.DataType = RDBDataType.BigInt;
                        break;

                    case string a when a.Contains("int"):
                        rdbDataRecordStorageColumn.DataType = RDBDataType.Int;
                        break;

                    case string a when a.Contains("datetime"):
                        rdbDataRecordStorageColumn.DataType = RDBDataType.DateTime;
                        break;

                    case string a when a.Contains("uniqueidentifier"):
                        rdbDataRecordStorageColumn.DataType = RDBDataType.UniqueIdentifier;
                        break;

                    case string a when a.Contains("cursor"):
                        rdbDataRecordStorageColumn.DataType = RDBDataType.Cursor;
                        break;

                    case string a when a.Contains("nvarchar"):
                        rdbDataRecordStorageColumn.DataType = RDBDataType.NVarchar;
                        rdbDataRecordStorageColumn.Size = GetSize(sqlDataRecordStorageColumn.SQLDataType);
                        break;

                    case string a when a.Contains("varchar"):
                        rdbDataRecordStorageColumn.DataType = RDBDataType.Varchar;
                        rdbDataRecordStorageColumn.Size = GetSize(sqlDataRecordStorageColumn.SQLDataType);
                        break;

                    case string a when a.Contains("decimal"):
                        rdbDataRecordStorageColumn.DataType = RDBDataType.Decimal;
                        rdbDataRecordStorageColumn.Size = GetSize(sqlDataRecordStorageColumn.SQLDataType);
                        rdbDataRecordStorageColumn.Precision = GetPrecision(sqlDataRecordStorageColumn.SQLDataType);
                        break;

                    case string a when a.Contains("varbinary"):
                        rdbDataRecordStorageColumn.DataType = RDBDataType.VarBinary;
                        rdbDataRecordStorageColumn.Size = GetSize(sqlDataRecordStorageColumn.SQLDataType);
                        break;
                }
                rdbDataRecordStorageSettings.Columns.Add(rdbDataRecordStorageColumn);

                // DataRecordStorageSettings
                rdbDataRecordStorageSettings.DateTimeField = sqlDataRecordStorageSettings.DateTimeField;
                rdbDataRecordStorageSettings.LastModifiedByField = sqlDataRecordStorageSettings.LastModifiedByField;
                rdbDataRecordStorageSettings.CreatedByField = sqlDataRecordStorageSettings.CreatedByField;
                rdbDataRecordStorageSettings.LastModifiedTimeField = sqlDataRecordStorageSettings.LastModifiedTimeField;
                rdbDataRecordStorageSettings.CreatedTimeField = sqlDataRecordStorageSettings.CreatedTimeField;
                rdbDataRecordStorageSettings.EnableUseCaching = sqlDataRecordStorageSettings.EnableUseCaching;
                rdbDataRecordStorageSettings.RequiredLimitResult = sqlDataRecordStorageSettings.RequiredLimitResult;
                rdbDataRecordStorageSettings.DontReflectToDB = sqlDataRecordStorageSettings.DontReflectToDB;
                rdbDataRecordStorageSettings.DenyAPICall = sqlDataRecordStorageSettings.DenyAPICall;
                rdbDataRecordStorageSettings.RequiredPermission = sqlDataRecordStorageSettings.RequiredPermission;
                rdbDataRecordStorageSettings.FieldsPermissions = sqlDataRecordStorageSettings.FieldsPermissions;
            }
            return rdbDataRecordStorageSettings;
        }

        private string GetSubStrings(string input, string startSymbol, string endSymbol)
        {
            int start = input.IndexOf(startSymbol) + 1;
            int end = input.IndexOf(endSymbol, start);
            if (end == -1)
                return null;
            return input.Substring(start, end - start);
        }

        private int? GetSize(string sqlDataRecordStorageColumnDataType)
        {
            string size = GetSubStrings(sqlDataRecordStorageColumnDataType, "(", ",");
            if (size != null)
            {
                return Int32.Parse(size);
            }
            else
            {
                string sizeOnly = GetSubStrings(sqlDataRecordStorageColumnDataType, "(", ")");
                if (sizeOnly.ToLower() == "max")
                {
                    return null;
                }
                else
                    return Int32.Parse(sizeOnly);
            }
        }

        private int GetPrecision(string sqlDataRecordStorageColumnDataType)
        {
            return Int32.Parse(GetSubStrings(sqlDataRecordStorageColumnDataType, ",", ")"));
        }

        private DataRecordStorage DataRecordMapperName(IDataReader reader)
        {
            return new DataRecordStorage(){
                DataRecordStorageId = (Guid)reader["ID"],
               Name =  reader["Name"] as string
            };
        }
        
        private void connectionString_Leave(object sender, EventArgs e)
        {
            dataRecordStorageComboBox.Items.Clear();
            LoadDataRecordTables(sender, e);
        }
    }
}
