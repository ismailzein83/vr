using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class RuleDBSyncDataManager : BaseSQLDataManager, IDBSyncDataManager
    {
        public RuleDBSyncDataManager()
            : base(GetConnectionStringName("RulesDBConnStringKey", "RulesDBConnString"))
        {

        }

        public void DeleteRuleChangedTables()
        {
            string query = "DELETE FROM [rules].[RuleChangedForProcessing];DELETE FROM [rules].[RuleChanged];";
            ExecuteNonQueryText(query, null);
        }

        public string GetConnection()
        {
            return base.GetConnectionString();
        }

        public string GetSchema()
        {
            return "rules";
        }
    }
}
