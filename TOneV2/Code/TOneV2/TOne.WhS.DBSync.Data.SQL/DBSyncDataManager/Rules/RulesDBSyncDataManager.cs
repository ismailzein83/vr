using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TOne.WhS.DBSync.Entities;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Entities;
using Vanrise.Common;
using Vanrise.Data.SQL;
using Vanrise.Rules;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class RulesDBSyncDataManager : BaseSQLDataManager, IDBSyncDataManager
    {
        readonly string _tableName = Utilities.GetEnumDescription(DBTableName.Rule);
        readonly string _schema = "rules";
        readonly bool _useTempTables;
        public RulesDBSyncDataManager(bool useTempTables) :
            base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
        {
            _useTempTables = useTempTables;
        }

        public void ApplyRouteRulesToTemp(List<BaseRule> routeRules)
        {
            RouteRuleManager routeRuleManager = new RouteRuleManager();
            DataTable dt = new DataTable { TableName = MigrationUtils.GetTableName(_schema, _tableName, _useTempTables) };
            dt.Columns.Add("RuleDetails", typeof(string));
            dt.Columns.Add("TypeID", typeof(int));
            dt.Columns.Add("BED", typeof(DateTime));
            dt.Columns.Add("EED", typeof(DateTime));

            dt.BeginLoadData();
            foreach (var routeRule in routeRules)
            {
                DataRow row = dt.NewRow();
                row["RuleDetails"] = Serializer.Serialize(routeRule);
                row["TypeID"] = routeRuleManager.GetRuleTypeId();
                row["BED"] = routeRule.BeginEffectiveTime;
                row["EED"] = routeRule.EndEffectiveTime ?? (object)DBNull.Value;
                dt.Rows.Add(row);
            }
            dt.EndLoadData();
            WriteDataTableToDB(dt, System.Data.SqlClient.SqlBulkCopyOptions.KeepNulls);
        }
        public Dictionary<string, RouteRule> GetRouteRules(bool useTempTables)
        {
            return GetItemsText("SELECT [ID] ,[RuleDetails]  ,[BED] ,EED, SourceID FROM"
                + MigrationUtils.GetTableName(_schema, _tableName, useTempTables), RouteRuleMapper, null).ToDictionary(k => k.SourceId, v => v);
        }
        private RouteRule RouteRuleMapper(IDataReader reader)
        {
            RouteRule rule = Serializer.Deserialize<RouteRule>(reader["RuleDetails"] as string);
            if (rule != null)
                rule.SourceId = reader["SourceID"] as string;
            return rule;

        }
        public string GetConnection()
        {
            return base.GetConnectionString();
        }
        public string GetTableName()
        {
            return _tableName;
        }
        public string GetSchema()
        {
            return _schema;
        }
    }
}
