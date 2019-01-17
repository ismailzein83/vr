using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.DatabaseStructure.DBConvertors
{
    public class MySqlDBConvertor : VRDBStructureConvertor
    {
        public override VRDBType DBType { get { return VRDBType.MySQL; } }

        public override string GenerateConvertedScript(IVRDBStructureConvertorGenerateConvertedScriptContext context)
        {
            VRDBStructure DBStructure = context.DBStructure;
            DBStructure.ThrowIfNull("DBStructure", DBStructure);

            StringBuilder generatedScript = new StringBuilder();
            foreach (VRDBStructureTable table in DBStructure.Tables)
            {
                StringBuilder tableCreator = new StringBuilder();
                tableCreator.Append(createTable.Replace("#SCHEMA#", table.SchemaName));
                tableCreator.Replace("#TABLENAME#", table.TableName);

                StringBuilder columnsCreator = CreateColumns(table.Columns, table.PrimaryKeys);

                StringBuilder foreignKeysCreator = CreateForeignKeys(table.ForeignKeys, table.SchemaName, table.TableName);

                StringBuilder indexesCreator = CreateIndexes(table.Indexes, table.SchemaName, table.TableName);

                tableCreator = tableCreator.Replace("#COLUMNS#", columnsCreator.ToString());
                tableCreator = tableCreator.Replace("#ADDFOREIGNKEYS#", foreignKeysCreator.ToString());
                tableCreator = tableCreator.Replace("#ADDINDEXES#", indexesCreator.ToString());
                generatedScript.Append(tableCreator);
                AppendLines(generatedScript, 2);
            }

            return generatedScript.ToString();
        }

        #region Private Methods

        private StringBuilder CreateColumns(List<VRDBStructureTableColumn> columns, List<VRDBStructureTablePrimaryKey> primaryKeys)
        {
            StringBuilder columnsCreator = new StringBuilder();
            columnsCreator.AppendLine();
            if (columns != null)
            {
                VRDBStructureTableColumn lastColumn = columns.Last();
                foreach (VRDBStructureTableColumn column in columns)
                {
                    columnsCreator.Append(createColumn);
                    if (column != lastColumn)
                        columnsCreator.Append(',');

                    columnsCreator = columnsCreator.Replace("#COLUMNNAME#", column.ColumnName);
                    columnsCreator = columnsCreator.Replace("#DATATYPE#", ConvertDataType(column));

                    if (column.IsIdentity)
                        columnsCreator = columnsCreator.Replace("#AUTO_INCREMENT#", "AUTO_INCREMENT");
                    else
                        columnsCreator = columnsCreator.Replace("#AUTO_INCREMENT#", "");

                    if (column.IsNullable)
                        columnsCreator = columnsCreator.Replace("#NOT#", "");
                    else
                        columnsCreator = columnsCreator.Replace("#NOT#", "NOT");

                    AppendLines(columnsCreator, 1);
                }
                if (primaryKeys != null)
                {
                    string primaryColumnKeys = string.Join(",", primaryKeys.Select(item => item.ColumnName).ToList());
                    primaryColumnKeys = addPrimaryKeys.Replace("#COLUMNS#", primaryColumnKeys);
                    columnsCreator.Append(",");
                    columnsCreator.Append(primaryColumnKeys);
                }
            }
            return columnsCreator;
        }

        private StringBuilder CreateForeignKeys(List<VRDBStructureTableForeignKey> foreignKeys, string schemaName, string tableName)
        {
            StringBuilder foreignKeysCreator = new StringBuilder();
            foreignKeysCreator.AppendLine();
            if (foreignKeys != null)
            {
                foreach (VRDBStructureTableForeignKey foreignKey in foreignKeys)
                {
                    foreignKeysCreator.Append(addForeignKey);

                    foreignKeysCreator = foreignKeysCreator.Replace("#SCHEMA#", schemaName);
                    foreignKeysCreator = foreignKeysCreator.Replace("#TABLE#", tableName);
                    foreignKeysCreator = foreignKeysCreator.Replace("#FOREIGNKEYNAME#", foreignKey.ForeignKeyName);
                    foreignKeysCreator = foreignKeysCreator.Replace("#FOREIGNKEYNAME#", foreignKey.ForeignKeyName);
                    foreignKeysCreator = foreignKeysCreator.Replace("#COLUMNNAME#", foreignKey.ColumnName);
                    foreignKeysCreator = foreignKeysCreator.Replace("#PARENTTABLESCHEMA#", foreignKey.ParentTableSchema);
                    foreignKeysCreator = foreignKeysCreator.Replace("#PARENTTABLE#", foreignKey.ParentTable);
                    foreignKeysCreator = foreignKeysCreator.Replace("#PARENTCOLUMNNAME#", foreignKey.ParentColumnName);

                    AppendLines(foreignKeysCreator, 1);
                }
            }

            return foreignKeysCreator;
        }

        private StringBuilder CreateIndexes(List<VRDBStructureTableIndex> indexes, string schemaName, string tableName)
        {
            StringBuilder indexesCreator = new StringBuilder();
            if (indexes != null)
            {
                foreach (VRDBStructureTableIndex index in indexes)
                {
                    indexesCreator.Append(addIndex);
                    indexesCreator = indexesCreator.Replace("#SCHEMA#", schemaName);
                    indexesCreator = indexesCreator.Replace("#TABLE#", tableName);
                    indexesCreator = indexesCreator.Replace("#INDEXNAME#", index.IndexName);

                    if (index.IsUnique)
                        indexesCreator = indexesCreator.Replace("#UNIQUE#", "UNIQUE");
                    else
                        indexesCreator = indexesCreator.Replace("#UNIQUE#", "");

                    if (index.Columns == null)
                        throw new ArgumentNullException("index.Columns", "An index should have a column list");

                    StringBuilder indexedColumnsCreator = new StringBuilder();
                    VRDBStructureTableIndexColumn lastColumn = index.Columns.Last();
                    foreach (VRDBStructureTableIndexColumn column in index.Columns)
                    {
                        indexedColumnsCreator.Append(addIndexedColumn);

                        if (column != lastColumn)
                            indexedColumnsCreator.Append(',');

                        indexedColumnsCreator = indexedColumnsCreator.Replace("#COLUMNNAME#", column.ColumnName);
                        if (column.IsAscending)
                            indexedColumnsCreator = indexedColumnsCreator.Replace("#ASC|DESC#", "ASC");
                        else
                            indexedColumnsCreator = indexedColumnsCreator.Replace("#ASC|DESC#", "DESC");
                    }

                    indexesCreator = indexesCreator.Replace("#INDEXEDCOLUMNS#", indexedColumnsCreator.ToString());

                    AppendLines(indexesCreator, 1);
                }
            }

            return indexesCreator;
        }

        private void AppendLines(StringBuilder stringBuilder, int numberOflines)
        {
            for (int i = 0; i < numberOflines; i++)
            {
                stringBuilder.AppendLine();
            }
        }

        private bool IsPrimaryKey(List<VRDBStructureTablePrimaryKey> primaryKeys, string columnName)
        {
            foreach (var primaryKey in primaryKeys)
            {
                if (primaryKey.ColumnName.Equals(columnName))
                    return true;
            }

            return false;
        }

        private string ConvertDataType(VRDBStructureTableColumn column)
        {
            switch (column.DataType)
            {
                case VRDBStructureDataType.BigInt: return "BIGINT";

                case VRDBStructureDataType.Boolean: return "BOOLEAN";

                case VRDBStructureDataType.DateTime: return "DATETIME(3)";

                case VRDBStructureDataType.Int: return "Int";

                case VRDBStructureDataType.UniqueIdentifier: return "CHAR(38)";

                case VRDBStructureDataType.Decimal:
                    if (!column.Size.HasValue || !column.Precision.HasValue)
                        throw new ArgumentNullException("column.Size|column.Precision", "This parameter should take a value");

                    return string.Format("DECIMAL({0}, {1})", column.Size.Value, column.Precision.Value);

                case VRDBStructureDataType.VarBinary:
                    return column.Size.HasValue ? string.Format("VarBinary({0})", column.Size.Value) : "LONGBLOB";

                case VRDBStructureDataType.Varchar:
                    return column.Size.HasValue ? string.Format("Varchar({0})", column.Size.Value) : "longtext";

                case VRDBStructureDataType.NVarchar:
                    return column.Size.HasValue ? string.Format("NVarchar({0})", column.Size.Value) : "LONGTEXT";

                default: return null;
            }
        }

        #endregion

        #region Queries

        private const string createTable = @"CREATE TABLE #SCHEMA#_#TABLENAME# (#COLUMNS#); #ADDFOREIGNKEYS# #ADDINDEXES#";

        private const string addPrimaryKeys = @"Primary Key (#COLUMNS#)";

        private const string createColumn = @"'#COLUMNNAME#' #DATATYPE# #AUTO_INCREMENT# #NOT# NULL";

        private const string addIndex = "ALTER TABLE #SCHEMA#_#TABLE# ADD #UNIQUE# '#INDEXNAME#' (#INDEXEDCOLUMNS#);";

        private const string addIndexedColumn = "'#COLUMNNAME#' #ASC|DESC#";

        private const string addForeignKey = "ALTER TABLE #SCHEMA#_#TABLE# ADD CONSTRAINT '#FOREIGNKEYNAME#' FOREIGN KEY ('#COLUMNNAME#') REFERENCES #PARENTTABLESCHEMA#_#PARENTTABLE#('#PARENTCOLUMNNAME#');";

        #endregion

    }


}
