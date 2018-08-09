using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.Data.RDB
{
    public class RDBSchemaManager
    {
        #region Singleton

        static RDBSchemaManager s_current = new RDBSchemaManager();

        public static RDBSchemaManager Current
        {
            get
            {
                return s_current;
            }
        }

        private RDBSchemaManager()
        {

        }

        #endregion

        TableDefinitionsByName _defaultTableDefinitionsByName = new TableDefinitionsByName();
        TableDefinitionsByProviderUniqueName _tableDefinitionsByProviderUniqueName = new TableDefinitionsByProviderUniqueName();

        #region Public Methods

        public void RegisterDefaultTableDefinition(string tableName, RDBTableDefinition tableDefinition)
        {
            lock (this)
            {
                if (_defaultTableDefinitionsByName.ContainsKey(tableName))
                    throw new Exception(String.Format("Table '{0}' already registered in default table definitions", tableName));
                _defaultTableDefinitionsByName.Add(tableName, tableDefinition);
            }
        }

        public void OverrideTableInfo(string providerUniqueName, string tableName, string dbSchemaName, string dbTableName)
        {
            lock(this)
            {
                RDBTableDefinition tableDefinition = GetOrAddTableDefinitionByProvider(providerUniqueName, tableName);
                tableDefinition.DBSchemaName = dbSchemaName;
                tableDefinition.DBTableName = dbTableName;
            }
        }

        public void OverrideColumnDBName(string providerUniqueName, string tableName, string columnName, string dbColumnName)
        {
            lock (this)
            {
                RDBTableColumnDefinition columnDefinition = GetOrAddColumnDefinitionByProvider(providerUniqueName, tableName, columnName);
                columnDefinition.DBColumnName = dbColumnName;
            }
        }

        public void OverrideColumnDBType(string providerUniqueName, string tableName, string columnName, RDBDataType dataType, int? size)
        {
            lock (this)
            {
                RDBTableColumnDefinition columnDefinition = GetOrAddColumnDefinitionByProvider(providerUniqueName, tableName, columnName);
                columnDefinition.DataType = dataType;
                columnDefinition.Size = size;
            }
        }

        internal RDBTableDefinition GetTableDefinitionWithValidate(BaseRDBDataProvider provider, string tableName)
        {
            RDBTableDefinition tableDefinition = _tableDefinitionsByProviderUniqueName.GetRecord(provider.UniqueName).GetRecord(tableName);
            if (tableDefinition == null)
                tableDefinition = _defaultTableDefinitionsByName.GetRecord(tableName);
            tableDefinition.ThrowIfNull("tableDefinition", tableName);
            return tableDefinition;
        }

        internal string GetTableDBName(BaseRDBDataProvider provider, string tableName)
        {
            RDBTableDefinition tableDefinition = GetTableDefinitionWithValidate(provider, tableName);
            return GetTableDBName(provider, tableDefinition);
        }

        internal static string GetTableDBName(BaseRDBDataProvider provider, RDBTableDefinition tableDefinition)
        {
            return provider.GetTableDBName(tableDefinition.DBSchemaName, tableDefinition.DBTableName);
        }

        internal List<string> GetTableColumns(string tableName)
        {
            var tableDefinition = _defaultTableDefinitionsByName.GetRecord(tableName);
            tableDefinition.ThrowIfNull("tableDefinition", tableName);
            return tableDefinition.Columns.Select(colEntry => colEntry.Key).ToList();
        }

        internal string GetColumnDBName(BaseRDBDataProvider provider, string tableName, string columnName)
        {
            RDBTableColumnDefinition columnDefinition = GetColumnDefinitionWithValidate(provider, tableName, columnName);
            return GetColumnDBName(provider, columnName, columnDefinition);
        }

        internal static string GetColumnDBName(BaseRDBDataProvider provider, string columnName, RDBTableColumnDefinition columnDefinition)
        {
            return columnDefinition.DBColumnName != null ? columnDefinition.DBColumnName : columnName;
        }

        internal RDBTableColumnDefinition GetColumnDefinitionWithValidate(BaseRDBDataProvider provider, string tableName, string columnName)
        {
            RDBTableDefinition tableDefinition = GetTableDefinitionWithValidate(provider, tableName);
            return GetColumnDefinitionWithValidate(tableDefinition, tableName, columnName);
        }

        internal RDBTableColumnDefinition GetColumnDefinitionWithValidate(RDBTableDefinition tableDefinition, string tableName, string columnName)
        {
            RDBTableColumnDefinition columnDefinition;
            if (!tableDefinition.Columns.TryGetValue(columnName, out columnDefinition))
                throw new Exception(String.Format(" Column '{0}' not found in table '{1}'", columnName, tableName));
            return columnDefinition;
        }

        #endregion

        #region Private Classes

        private class TableDefinitionsByName : Dictionary<string, RDBTableDefinition>
        {

        }

        private class TableDefinitionsByProviderUniqueName : Dictionary<string, TableDefinitionsByName>
        {

        }

        #endregion

        #region Private Methods

        private RDBTableDefinition GetDefaultTableDefWithValidate(string tableName)
        {
            RDBTableDefinition tableDefinition;
            if (!_defaultTableDefinitionsByName.TryGetValue(tableName, out tableDefinition))
                throw new Exception(string.Format("Table '{0}' not found in _defaultTableDefinitionsByName", tableName));
            return tableDefinition;
        }

        private RDBTableDefinition GetOrAddTableDefinitionByProvider(string providerUniqueName, string tableName)
        {
            RDBTableDefinition tableDefinition;
            TableDefinitionsByName tableDefinitionsByNameForMatchProvider = _tableDefinitionsByProviderUniqueName.GetOrCreateItem(providerUniqueName);
            if (!tableDefinitionsByNameForMatchProvider.TryGetValue(tableName, out tableDefinition))
            {
                RDBTableDefinition defaultTableDefinition = GetDefaultTableDefWithValidate(tableName);
                tableDefinition = defaultTableDefinition.VRDeepCopy();
                tableDefinitionsByNameForMatchProvider.Add(tableName, tableDefinition);
            }
            return tableDefinition;
        }

        private RDBTableColumnDefinition GetOrAddColumnDefinitionByProvider(string providerUniqueName, string tableName, string columnName)
        {
            RDBTableDefinition tableDefinition = GetOrAddTableDefinitionByProvider(providerUniqueName, tableName);
            tableDefinition.Columns.ThrowIfNull("tableDefinition.Columns", tableName);
            RDBTableColumnDefinition columnDefinition;
            if (!tableDefinition.Columns.TryGetValue(columnName, out columnDefinition))
                throw new Exception(String.Format("Column '{0}' not found in table '{1}'", columnName, tableName));
            return columnDefinition;
        }

        #endregion
    }
}
