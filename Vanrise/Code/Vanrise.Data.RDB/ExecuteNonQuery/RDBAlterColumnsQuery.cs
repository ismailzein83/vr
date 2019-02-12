using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Data.RDB
{
    public class RDBAlterColumnsQuery : BaseRDBQuery
    {
        RDBQueryBuilderContext _queryBuilderContext;
        DBTableInfo _dbTableInfo;
        Dictionary<string, DBTableInfo> _dbTableInfosByProviderUniqueName = new Dictionary<string, DBTableInfo>();
        Dictionary<string, RDBAlterColumnsColumnDefinition> _columns = new Dictionary<string, RDBAlterColumnsColumnDefinition>();

        internal RDBAlterColumnsQuery(RDBQueryBuilderContext queryBuilderContext)
        {
            _queryBuilderContext = queryBuilderContext;
        }

        public void DBTableName(string dbTableName)
        {
            DBTableName(null, dbTableName);
        }

        public void DBTableName(string dbSchemaName, string dbTableName)
        {
            _dbTableInfo = new DBTableInfo
            {
                DBSchemaName = dbSchemaName,
                DBTableName = dbTableName
            };
        }

        public void OverrideDBTableName(string providerUniqueName, string dbTableName)
        {
            OverrideDBTableName(providerUniqueName, null, dbTableName);
        }

        public void OverrideDBTableName(string providerUniqueName, string dbSchemaName, string dbTableName)
        {
            _dbTableInfosByProviderUniqueName.Add(providerUniqueName, new DBTableInfo { DBSchemaName = dbSchemaName, DBTableName = dbTableName });
        }

        public void AddColumn(string columnName, RDBDataType dataType)
        {
            AddColumn(columnName, dataType, null);
        }

        public void AddColumn(string columnName, RDBDataType dataType, bool? notNullable)
        {
            AddColumn(columnName, dataType, null, null, notNullable);
        }

        public void AddColumn(string columnName, RDBDataType dataType, int? size, int? precision, bool? notNullable)
        {
            AddColumn(columnName, columnName, dataType, size, precision, notNullable);
        }

        public void AddColumn(string columnName, string dbColumnName, RDBDataType dataType, int? size, int? precision, bool? notNullable)
        {

            this._columns.Add(columnName, new RDBAlterColumnsColumnDefinition
            {
                ColumnDefinition = new RDBTableColumnDefinition
                {
                    DBColumnName = dbColumnName,
                    DataType = dataType,
                    Size = size,
                    Precision = precision
                },
                NotNullable = notNullable
            });
        }

        public override RDBResolvedQuery GetResolvedQuery(IRDBQueryGetResolvedQueryContext context)
        {
            DBTableInfo dbTableInfo;
            if (!_dbTableInfosByProviderUniqueName.TryGetValue(context.DataProvider.UniqueName, out dbTableInfo))
                dbTableInfo = _dbTableInfo;
            var resolveQueryContext = new RDBDataProviderResolveAlterColumnsQueryContext(dbTableInfo.DBSchemaName, dbTableInfo.DBTableName, _columns, context);
            return context.DataProvider.ResolveAlterColumnsQuery(resolveQueryContext);
        }

        #region Private Classes

        private class DBTableInfo
        {
            public string DBSchemaName { get; set; }

            public string DBTableName { get; set; }
        }

        #endregion
    }

    public class RDBAlterColumnsColumnDefinition
    {
        public RDBTableColumnDefinition ColumnDefinition { get; set; }

        public bool? NotNullable { get; set; }
    }
}
