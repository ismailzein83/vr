using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public class RDBCreateTableQuery : BaseRDBQuery
    {
        DBTableInfo _dbTableInfo;

        Dictionary<string, DBTableInfo> _dbTableInfosByProviderUniqueName = new Dictionary<string, DBTableInfo>();

        Dictionary<string, RDBCreateTableColumnDefinition> _columns = new Dictionary<string, RDBCreateTableColumnDefinition>();

        RDBQueryBuilderContext _queryBuilderContext;
        bool _identityColumnSpecified;
        bool _primaryKeyIndexNonClustered;

        internal RDBCreateTableQuery(RDBQueryBuilderContext queryBuilderContext)
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
            AddColumn(columnName, columnName, dataType, null, null, false, false, false);
        }

        public void AddColumn(string columnName, string dbColumnName, RDBDataType dataType, int? size, int? precision, bool notNullable, bool isPrimaryKey, bool isIdentity)
        {
            if(isIdentity)
            {
                if (_identityColumnSpecified)
                    throw new Exception("Identity Column is already specified");
                _identityColumnSpecified = true;
            }
            this._columns.Add(columnName, new RDBCreateTableColumnDefinition
            {
                ColumnDefinition = new RDBTableColumnDefinition
                {
                    DBColumnName = dbColumnName,
                    DataType = dataType,
                    Size = size,
                    Precision = precision
                },
                NotNullable = notNullable || isPrimaryKey,
                IsPrimaryKey = isPrimaryKey,
                IsIdentity = isIdentity
            });            
        }

        public void PrimaryKeyIndexNonClustered()
        {
            _primaryKeyIndexNonClustered = true;
        }

        public override RDBResolvedQuery GetResolvedQuery(IRDBQueryGetResolvedQueryContext context)
        {
            DBTableInfo dbTableInfo;
            if (!_dbTableInfosByProviderUniqueName.TryGetValue(context.DataProvider.UniqueName, out dbTableInfo))
                dbTableInfo = _dbTableInfo;
            var resolveQueryContext = new RDBDataProviderResolveTableCreationQueryContext(_dbTableInfo.DBSchemaName, _dbTableInfo.DBTableName, _columns, _primaryKeyIndexNonClustered, context);
            return context.DataProvider.ResolveTableCreationQuery(resolveQueryContext);
        }

        #region Private Classes

        private class DBTableInfo
        {
            public string DBSchemaName { get; set; }

            public string DBTableName { get; set; }
        }

        #endregion
    }

    public class RDBCreateTableColumnDefinition
    {
        public RDBTableColumnDefinition ColumnDefinition { get; set; }

        public bool NotNullable { get; set; }

        public bool IsPrimaryKey { get; set; }

        public bool IsIdentity { get; set; }
    }
}