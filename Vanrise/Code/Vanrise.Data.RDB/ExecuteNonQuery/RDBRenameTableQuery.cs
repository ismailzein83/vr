using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public class RDBRenameTableQuery : BaseRDBQuery
    {
        RDBQueryBuilderContext _queryBuilderContext;
        DBTableInfo _existingDBTableInfo;
        Dictionary<string, DBTableInfo> _existingDBTableInfosByProviderUniqueName = new Dictionary<string, DBTableInfo>();
        DBTableInfo _newDBTableInfo;
        Dictionary<string, DBTableInfo> _newDBTableInfosByProviderUniqueName = new Dictionary<string, DBTableInfo>();

        internal RDBRenameTableQuery(RDBQueryBuilderContext queryBuilderContext)
        {
            _queryBuilderContext = queryBuilderContext;
        }

        public void ExistingDBTableName(string dbTableName)
        {
            ExistingDBTableName(null, dbTableName);
        }

        public void ExistingDBTableName(string dbSchemaName, string dbTableName)
        {
            _existingDBTableInfo = new DBTableInfo
            {
                DBSchemaName = dbSchemaName,
                DBTableName = dbTableName
            };
        }

        public void OverrideExistingDBTableName(string providerUniqueName, string dbTableName)
        {
            OverrideExistingDBTableName(providerUniqueName, null, dbTableName);
        }

        public void OverrideExistingDBTableName(string providerUniqueName, string dbSchemaName, string dbTableName)
        {
            _existingDBTableInfosByProviderUniqueName.Add(providerUniqueName, new DBTableInfo { DBSchemaName = dbSchemaName, DBTableName = dbTableName });
        }

        public void NewDBTableName(string dbTableName)
        {
            NewDBTableName(null, dbTableName);
        }

        public void NewDBTableName(string dbSchemaName, string dbTableName)
        {
            _newDBTableInfo = new DBTableInfo
            {
                DBSchemaName = dbSchemaName,
                DBTableName = dbTableName
            };
        }

        public void OverrideNewDBTableName(string providerUniqueName, string dbTableName)
        {
            OverrideNewDBTableName(providerUniqueName, null, dbTableName);
        }

        public void OverrideNewDBTableName(string providerUniqueName, string dbSchemaName, string dbTableName)
        {
            _newDBTableInfosByProviderUniqueName.Add(providerUniqueName, new DBTableInfo { DBSchemaName = dbSchemaName, DBTableName = dbTableName });
        }

        public override RDBResolvedQuery GetResolvedQuery(IRDBQueryGetResolvedQueryContext context)
        {
            DBTableInfo existingDBTableInfo;
            if (!_existingDBTableInfosByProviderUniqueName.TryGetValue(context.DataProvider.UniqueName, out existingDBTableInfo))
                existingDBTableInfo = _existingDBTableInfo;
            DBTableInfo newDBTableInfo;
            if (!_newDBTableInfosByProviderUniqueName.TryGetValue(context.DataProvider.UniqueName, out newDBTableInfo))
                newDBTableInfo = _newDBTableInfo;
            var resolveQueryContext = new RDBDataProviderResolveRenameTableQueryContext(existingDBTableInfo.DBSchemaName, existingDBTableInfo.DBTableName, newDBTableInfo.DBSchemaName, newDBTableInfo.DBTableName, context);
            return context.DataProvider.ResolveRenameTableQuery(resolveQueryContext);
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
