using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Entities;
using TOne.WhS.Routing.Entities;
using Vanrise.Common;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class RulesDBSyncDataManager : BaseSQLDataManager, IDBSyncDataManager
    {
        string _TableName = Vanrise.Common.Utilities.GetEnumDescription(DBTableName.Rule);
        string _Schema = "rules";
        bool _UseTempTables;
        public RulesDBSyncDataManager(bool useTempTables) :
            base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
        {
            _UseTempTables = useTempTables;
        }


        public void ApplyRouteRulesToTemp(List<RouteRule> routeRules)
        {
            DataTable dt = new DataTable();
            dt.TableName = MigrationUtils.GetTableName(_Schema, _TableName, _UseTempTables);
            dt.Columns.Add("RuleDetails", typeof(string));
            dt.Columns.Add("TypeID", typeof(int));
            dt.Columns.Add("BED", typeof(DateTime));
            dt.Columns.Add("EED", typeof(DateTime));

            dt.BeginLoadData();
            foreach (var routeRule in routeRules)
            {
                DataRow row = dt.NewRow();
                row["RuleDetails"] = Serializer.Serialize(routeRule);
                row["TypeID"] = 10;
                row["BED"] = routeRule.BeginEffectiveTime;
                row["EED"] = routeRule.EndEffectiveTime;

            }
            dt.EndLoadData();
            WriteDataTableToDB(dt, System.Data.SqlClient.SqlBulkCopyOptions.KeepNulls);
        }

        public Dictionary<string, RouteRule> GetRouteRules(bool useTempTables)
        {
            return GetItemsText("SELECT [ID] ,[RuleDetails]  ,[BED] ,EED, SourceID FROM"
                + MigrationUtils.GetTableName(_Schema, _TableName, useTempTables), RouteRuleMapper, null).ToDictionary(k => k.SourceId, v => v);
        }

        private RouteRule RouteRuleMapper(IDataReader reader)
        {
            RouteRule rule = Vanrise.Common.Serializer.Deserialize<RouteRule>(reader["RuleDetails"] as string);
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
            return _TableName;
        }

        public string GetSchema()
        {
            return _Schema;
        }
    }
}
