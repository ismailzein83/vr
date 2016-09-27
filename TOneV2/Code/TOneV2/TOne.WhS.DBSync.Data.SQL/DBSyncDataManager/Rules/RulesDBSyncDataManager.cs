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
using Rule = Vanrise.Rules.Entities.Rule;

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

        public void ApplyRouteRulesToTemp(List<Rule> routeRules)
        {
            DataTable dt = new DataTable { TableName = MigrationUtils.GetTableName(_schema, _tableName, _useTempTables) };
            dt.Columns.Add("RuleDetails", typeof(string));
            dt.Columns.Add("TypeID", typeof(int));
            dt.Columns.Add("BED", typeof(DateTime));
            dt.Columns.Add("EED", typeof(DateTime));

            dt.BeginLoadData();
            foreach (var rule in routeRules)
            {
                DataRow row = dt.NewRow();
                row["RuleDetails"] = rule.RuleDetails;
                row["TypeID"] = rule.TypeId;
                row["BED"] = rule.BED;
                row["EED"] = rule.EED ?? (object)DBNull.Value;
                dt.Rows.Add(row);
            }
            dt.EndLoadData();
            WriteDataTableToDB(dt, System.Data.SqlClient.SqlBulkCopyOptions.KeepNulls);
        }
        public Dictionary<string, Rule> GetRouteRules(bool useTempTables)
        {
            return GetItemsText("SELECT [ID] ,[RuleDetails]  ,[BED] ,EED, SourceID FROM"
                + MigrationUtils.GetTableName(_schema, _tableName, useTempTables), RouteRuleMapper, null).ToDictionary(k => k.SourceId, v => v);
        }
        private Rule RouteRuleMapper(IDataReader reader)
        {
            Rule rule = Serializer.Deserialize<Rule>(reader["RuleDetails"] as string);
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
