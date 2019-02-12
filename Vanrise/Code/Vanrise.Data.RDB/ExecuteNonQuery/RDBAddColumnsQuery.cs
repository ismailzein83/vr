using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Data.RDB
{
    public class RDBAddColumnsQuery : BaseRDBQuery
    {
        RDBQueryBuilderContext _queryBuilderContext;
        DBTableInfo _dbTableInfo;
        Dictionary<string, DBTableInfo> _dbTableInfosByProviderUniqueName = new Dictionary<string, DBTableInfo>();
        Dictionary<string, RDBAddColumnsColumnDefinition> _columns = new Dictionary<string, RDBAddColumnsColumnDefinition>();

        internal RDBAddColumnsQuery(RDBQueryBuilderContext queryBuilderContext)
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
            AddColumn(columnName, columnName, dataType, size, precision, notNullable, false);
        }

        public void AddColumn(string columnName, string dbColumnName, RDBDataType dataType, int? size, int? precision, bool? notNullable, bool isIdentity)
        {

            this._columns.Add(columnName, new RDBAddColumnsColumnDefinition
            {
                ColumnDefinition = new RDBTableColumnDefinition
                {
                    DBColumnName = dbColumnName,
                    DataType = dataType,
                    Size = size,
                    Precision = precision
                },
                NotNullable = notNullable,
                IsIdentity = isIdentity
            });
        }

        public override RDBResolvedQuery GetResolvedQuery(IRDBQueryGetResolvedQueryContext context)
        {
            DBTableInfo dbTableInfo;
            if (!_dbTableInfosByProviderUniqueName.TryGetValue(context.DataProvider.UniqueName, out dbTableInfo))
                dbTableInfo = _dbTableInfo;
            var resolveQueryContext = new RDBDataProviderResolveAddColumnsQueryContext(dbTableInfo.DBSchemaName, dbTableInfo.DBTableName, _columns, context);
            return context.DataProvider.ResolveAddColumnsQuery(resolveQueryContext);
        }

        #region Private Classes

        private class DBTableInfo
        {
            public string DBSchemaName { get; set; }

            public string DBTableName { get; set; }
        }

        #endregion
    }

    public class RDBAddColumnsColumnDefinition
    {
        public RDBTableColumnDefinition ColumnDefinition { get; set; }

        public bool? NotNullable { get; set; }

        public bool IsIdentity { get; set; }
    }
}
