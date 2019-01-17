using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.DatabaseStructure
{
    public class VRDBStructureManager
    {
        #region ctor/Fields

        static Dictionary<VRDBType, VRDBStructureConvertor> s_dbStructureConvertors = new Dictionary<VRDBType, VRDBStructureConvertor>();

        static VRDBStructureManager()
        {
            foreach (var convertorType in Utilities.GetAllImplementations<VRDBStructureConvertor>())
            {
                VRDBStructureConvertor convertor = Activator.CreateInstance(convertorType).CastWithValidate<VRDBStructureConvertor>("convertor");
                if (s_dbStructureConvertors.ContainsKey(convertor.DBType))
                    throw new Exception($"Duplicate VRDBStructureConvertor found of DBType '{convertor.DBType.ToString()}'");
                s_dbStructureConvertors.Add(convertor.DBType, convertor);
            }
        }

        #endregion

        #region Public Methods

        public string GetDBScriptFromMSSQLDB(string dbHandlerName, VRDBType targetDBType, string sqlServerName, string sqlDatabaseName, string sqlUsername, string sqlPassword)
        {
            var handlerType = Type.GetType($"Vanrise.DatabaseStructure.DBHandlers.{dbHandlerName}DBHandler, Vanrise.DatabaseStructure");
            handlerType.ThrowIfNull("handlerType", dbHandlerName);
            var handler = Activator.CreateInstance(handlerType).CastWithValidate<VRDBHandler>("handler", dbHandlerName);
            return GetDBScriptFromMSSQLDB(handler, targetDBType, sqlServerName, sqlDatabaseName, sqlUsername, sqlPassword);
        }

        public string GetDBScriptFromMSSQLDB(VRDBHandler dbHandler, VRDBType targetDBType, string sqlServerName, string sqlDatabaseName, string sqlUsername, string sqlPassword)
        {
            VRDBStructure dbStructure = GetMSSQLDBStructure(sqlServerName, sqlDatabaseName, sqlUsername, sqlPassword);
            dbStructure.ThrowIfNull("dbStructure", sqlDatabaseName);
            dbStructure = dbStructure.VRDeepCopy();
            dbHandler.ApplyChangesToDBStructure(new VRDBHandlerApplyChangesContext(targetDBType, dbStructure));
            return GetDBStructureConvertor(targetDBType).GenerateConvertedScript(new VRDBStructureConvertorGenerateConvertedScriptContext(dbStructure));
        }

        public VRDBStructure GetMSSQLDBStructure(string sqlServerName, string sqlDatabaseName, string sqlUsername, string sqlPassword)
        {
            VRDBStructure returnedRDBStructure = new VRDBStructure();

            if (!string.IsNullOrEmpty(sqlServerName) && !string.IsNullOrEmpty(sqlDatabaseName))
            {

                string connectionString = string.Format("Server = {0}; Database = {1}; User ID = {2}; Password = {3}", sqlServerName, sqlDatabaseName, sqlUsername, sqlPassword);
                SqlConnection conn = new SqlConnection(connectionString);
                Server server = new Server(new Microsoft.SqlServer.Management.Common.ServerConnection(conn));

                if (!server.Databases.Contains(conn.Database))
                    throw new Exception(string.Format("Server {0} does not contain database '{1}'", server.Name, conn.Database));

                Database connectedDataBase = server.Databases[conn.Database];

                int tablesCount = connectedDataBase!= null ? connectedDataBase.Tables.Count: 0;
                List<VRDBStructureTable> rdbTables = new List<VRDBStructureTable>();
                for (int i = 0; i < tablesCount; i++)
                {
                    Table table = connectedDataBase.Tables[i];
                    if (table.IsSystemObject) continue;

                    rdbTables.Add(ConvertTable(table));
                }
                returnedRDBStructure.Tables = rdbTables.Count > 0 ? rdbTables : null;
            }
            return returnedRDBStructure;
        }



        #endregion

        #region Private Methods

        private VRDBStructureTable ConvertTable(Table table)
        {
            VRDBStructureTable rdbTable = new VRDBStructureTable();
            rdbTable.SchemaName = table.Schema;
            rdbTable.TableName = table.Name;
            List<VRDBStructureTableColumn> rdbColumns = new List<VRDBStructureTableColumn>();
            List<VRDBStructureTablePrimaryKey> primaryKeys = new List<VRDBStructureTablePrimaryKey>();

            foreach (Column col in table.Columns)
            {
                if (col.DataType.SqlDataType == SqlDataType.Timestamp) continue;
                rdbColumns.Add(ConvertColumn(col));

                if (col.InPrimaryKey)
                    primaryKeys.Add(new VRDBStructureTablePrimaryKey() { ColumnName = col.Name });
            }
            rdbTable.Columns = rdbColumns.Count > 0 ? rdbColumns : null;
            rdbTable.PrimaryKeys = primaryKeys.Count > 0 ? primaryKeys : null;

            List<VRDBStructureTableIndex> indexes = new List<VRDBStructureTableIndex>();
            foreach (Index index in table.Indexes)
            {
                indexes.Add(ConvertIndex(index));
            }
            rdbTable.Indexes = indexes.Count > 0 ? indexes : null;

            List<VRDBStructureTableForeignKey> foreignKeys = new List<VRDBStructureTableForeignKey>();
            foreach (ForeignKey foreignKey in table.ForeignKeys)
            {
                foreignKeys.Add(ConvertForeignKey(foreignKey));
            }
            rdbTable.ForeignKeys = foreignKeys.Count > 0 ? foreignKeys : null;

            return rdbTable;
        }

        private VRDBStructureTableColumn ConvertColumn(Column column)
        {
            VRDBStructureTableColumn rdbColumn = new VRDBStructureTableColumn();

            rdbColumn.ColumnName = column.Name;
            rdbColumn.DataType = ConvertDataType(column.DataType.SqlDataType);

            if (column.DataType.MaximumLength > 0 && (rdbColumn.DataType == VRDBStructureDataType.NVarchar || rdbColumn.DataType == VRDBStructureDataType.Varchar || rdbColumn.DataType == VRDBStructureDataType.VarBinary))
                rdbColumn.Size = column.DataType.MaximumLength;

            if (column.DataType.NumericPrecision > 0 && rdbColumn.DataType == VRDBStructureDataType.Decimal)
            {
                rdbColumn.Size = column.DataType.NumericPrecision;
                rdbColumn.Precision = column.DataType.NumericPrecision;
            }

            rdbColumn.IsNullable = column.Nullable;
            rdbColumn.IsIdentity = column.Identity;
            rdbColumn.IsDefaultDateTime = column.Name.Equals("CreatedTime") && column.DefaultConstraint!= null && !column.DefaultConstraint.IsSystemNamed;

            return rdbColumn;
        }

        private VRDBStructureDataType ConvertDataType(SqlDataType sqlDataType)
        {
            switch(sqlDataType) {
                case SqlDataType.BigInt: return VRDBStructureDataType.BigInt;

                case SqlDataType.NVarChar: return VRDBStructureDataType.NVarchar;

                case SqlDataType.VarChar: return VRDBStructureDataType.Varchar;

                case SqlDataType.Decimal: return VRDBStructureDataType.Decimal;

                case SqlDataType.DateTime: return VRDBStructureDataType.DateTime;

                case SqlDataType.UniqueIdentifier: return VRDBStructureDataType.UniqueIdentifier;

                case SqlDataType.Bit: default: return VRDBStructureDataType.Boolean;

                case SqlDataType.VarBinary: return VRDBStructureDataType.VarBinary;

                }
        }

        private VRDBStructureTableIndex ConvertIndex(Index index)
        {
            VRDBStructureTableIndex rdbIndex = new VRDBStructureTableIndex();

            rdbIndex.IndexName = index.Name;
            rdbIndex.IsClustered = index.IsClustered;
            rdbIndex.IsUnique = index.IsUnique;
            List<VRDBStructureTableIndexColumn> indexedColumns = new List<VRDBStructureTableIndexColumn>();
            foreach (IndexedColumn indexedColumn in index.IndexedColumns)
            {
                indexedColumns.Add(ConvertIndexedColumn(indexedColumn));
            }

            rdbIndex.Columns = indexedColumns;

            return rdbIndex;
        }

        private VRDBStructureTableIndexColumn ConvertIndexedColumn(IndexedColumn indexedColumn)
        {
            return new VRDBStructureTableIndexColumn()
            {
                ColumnName = indexedColumn.Name,
                IsAscending = !indexedColumn.Descending
            };
        }

        private VRDBStructureTableForeignKey ConvertForeignKey(ForeignKey foreignKey)
        {
            VRDBStructureTableForeignKey rdbForeignKey = new VRDBStructureTableForeignKey()
            {
                ForeignKeyName = foreignKey.Name,
                ColumnName = foreignKey.Columns != null && foreignKey.Columns.Count > 0 ? foreignKey.Columns[0].Name : null,
                ParentTableSchema = foreignKey.ReferencedTableSchema,
                ParentTable = foreignKey.ReferencedTable,
                ParentColumnName = foreignKey.Columns != null && foreignKey.Columns.Count > 0 ? foreignKey.Columns[0].ReferencedColumn : null
            };

            return rdbForeignKey;
        }

        VRDBStructureConvertor GetDBStructureConvertor(VRDBType dbType)
        {
            VRDBStructureConvertor convertor;
            if (!s_dbStructureConvertors.TryGetValue(dbType, out convertor))
                throw new Exception($"Cannot find VRDBStructureConvertor of DBType '{dbType}'");
            return convertor;
        }

        #endregion

        #region Private Classes

        private class VRDBStructureConvertorGenerateConvertedScriptContext : IVRDBStructureConvertorGenerateConvertedScriptContext
        {
            public VRDBStructureConvertorGenerateConvertedScriptContext(VRDBStructure dbStructure)
            {
                this.DBStructure = dbStructure;
            }

            public VRDBStructure DBStructure { get; private set; }
        }

        private class VRDBHandlerApplyChangesContext : IVRDBHandlerApplyChangesContext
        {
            public VRDBHandlerApplyChangesContext(VRDBType dbType, VRDBStructure dbStructure)
            {
                this.DBType = DBType;
                this.DBStructure = dbStructure;
            }

            public VRDBType DBType { get; private set; }

            public VRDBStructure DBStructure { get; private set; }
        }

        #endregion
    }
}