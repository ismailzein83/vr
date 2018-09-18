using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public enum RDBCreateIndexType { Clustered, NonClustered, UniqueClustered, UniqueNonClustered }
    public class RDBCreateIndexQuery : BaseRDBQuery
    {
        DBTableInfo _dbTableInfo;
        Dictionary<string, DBTableInfo> _dbTableInfosByProviderUniqueName = new Dictionary<string, DBTableInfo>();

        List<string> _columnNames = new List<string>();
        RDBCreateIndexType _indexType;

        RDBQueryBuilderContext _queryBuilderContext;
        
        internal RDBCreateIndexQuery(RDBQueryBuilderContext queryBuilderContext)
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

        public void IndexType(RDBCreateIndexType indexType)
        {
            _indexType = indexType;
        }

        public void AddColumn(string columnName)
        {
            _columnNames.Add(columnName);
        }

        public override RDBResolvedQuery GetResolvedQuery(IRDBQueryGetResolvedQueryContext context)
        {
            DBTableInfo dbTableInfo;
            if (!_dbTableInfosByProviderUniqueName.TryGetValue(context.DataProvider.UniqueName, out dbTableInfo))
                dbTableInfo = _dbTableInfo;
            var resolveQueryContext = new RDBDataProviderResolveIndexCreationQueryContext(_dbTableInfo.DBSchemaName, _dbTableInfo.DBTableName, _indexType, _columnNames, context);
            return context.DataProvider.ResolveIndexCreationQuery(resolveQueryContext);
        }


        #region Private Classes

        private class DBTableInfo
        {
            public string DBSchemaName { get; set; }

            public string DBTableName { get; set; }
        }

        #endregion
    }
}
