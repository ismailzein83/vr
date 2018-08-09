using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.Data.RDB
{
    public class RDBTableDefinition
    {
        public string DBSchemaName { get; set; }

        public string DBTableName { get; set; }

        public string IdColumnName { get; set; }

        public string CreatedTimeColumnName { get; set; }

        public string ModifiedTimeColumnName { get; set; }

        public Dictionary<string, RDBTableColumnDefinition> Columns { get; set; }
    }

    public class RDBTableDefinitionQuerySource : IRDBTableQuerySource
    {
        RDBSchemaManager _schemaManager;

        public RDBTableDefinitionQuerySource(string tableName, RDBSchemaManager schemaManager)
        {
            this.TableName = tableName;
            _schemaManager = schemaManager;
        }

        public RDBTableDefinitionQuerySource(string tableName)
            : this(tableName, RDBSchemaManager.Current)
        {
        }

        public string TableName { get; private set; }

        public string GetDescription()
        {
            return this.TableName;
        }

        public string ToDBQuery(IRDBTableQuerySourceToDBQueryContext context)
        {
            return _schemaManager.GetTableDBName(context.DataProvider, this.TableName);
        }


        public string GetDBColumnName(IRDBTableQuerySourceGetDBColumnNameContext context)
        {
            return _schemaManager.GetColumnDBName(context.DataProvider, this.TableName, context.ColumnName);
        }

        public string GetUniqueName()
        {
            return this.TableName;
        }


        public void GetIdColumnInfo(IRDBTableQuerySourceGetIdColumnInfoContext context)
        {
            var tableDefinition = _schemaManager.GetTableDefinitionWithValidate(context.DataProvider, this.TableName);
            if(!String.IsNullOrEmpty( tableDefinition.IdColumnName ))
            {
                context.IdColumnName = tableDefinition.IdColumnName;
                context.IdColumnDefinition = _schemaManager.GetColumnDefinitionWithValidate(tableDefinition, this.TableName, tableDefinition.IdColumnName);
            }
        }


        public List<string> GetColumnNames(IRDBTableQuerySourceGetColumnNamesContext context)
        {
            return _schemaManager.GetTableColumns(this.TableName);
        }


        public void GetCreatedAndModifiedTime(IRDBTableQuerySourceGetCreatedAndModifiedTimeContext context)
        {
            var tableDefinition = _schemaManager.GetTableDefinitionWithValidate(context.DataProvider, this.TableName);
            context.CreatedTimeColumnName = tableDefinition.CreatedTimeColumnName;
            context.ModifiedTimeColumnName = tableDefinition.ModifiedTimeColumnName;
        }
    }

    public class RDBTempPhysicalTableQuerySource : IRDBTableQuerySource
    {
        string _tableName;
        RDBTableDefinition _tableDefinition;

        public RDBTempPhysicalTableQuerySource(string tableName, RDBTableDefinition tableDefinition)
        {
            tableName.ThrowIfNull("tableName");
            tableDefinition.ThrowIfNull("tableDefinition", tableName);
            tableDefinition.DBTableName.ThrowIfNull("tableDefinition.DBTableName", tableName);
            tableDefinition.Columns.ThrowIfNull("tableDefinition.Columns", tableName);
            this._tableName = tableName;
            this._tableDefinition = tableDefinition;
        }

        public string GetUniqueName()
        {
            return this._tableName;
        }

        public string GetDescription()
        {
            return this._tableName;
        }

        public string ToDBQuery(IRDBTableQuerySourceToDBQueryContext context)
        {
            return RDBSchemaManager.GetTableDBName(context.DataProvider, this._tableDefinition);
        }

        public string GetDBColumnName(IRDBTableQuerySourceGetDBColumnNameContext context)
        {
            string columnName = context.ColumnName;
            RDBTableColumnDefinition columnDefinition;
            if (!this._tableDefinition.Columns.TryGetValue(columnName, out columnDefinition))
                throw new Exception(String.Format(" Column '{0}' not found in table '{1}'", columnName, this._tableName));
            return RDBSchemaManager.GetColumnDBName(context.DataProvider, columnName, columnDefinition);
        }


        public void GetIdColumnInfo(IRDBTableQuerySourceGetIdColumnInfoContext context)
        {
            throw new NotImplementedException();
        }


        public List<string> GetColumnNames(IRDBTableQuerySourceGetColumnNamesContext context)
        {
            return this._tableDefinition.Columns.Keys.ToList();
        }


        public void GetCreatedAndModifiedTime(IRDBTableQuerySourceGetCreatedAndModifiedTimeContext context)
        {
            context.CreatedTimeColumnName = this._tableDefinition.CreatedTimeColumnName;
            context.ModifiedTimeColumnName = this._tableDefinition.ModifiedTimeColumnName;
        }
    }
}
