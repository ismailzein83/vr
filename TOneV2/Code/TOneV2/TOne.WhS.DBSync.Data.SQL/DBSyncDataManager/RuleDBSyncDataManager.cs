using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class RuleDBSyncDataManager : BaseSQLDataManager
    {
        readonly string _tableName = Vanrise.Common.Utilities.GetEnumDescription(DBTableName.Rule);
        readonly string _schema = "rules";

        public RuleDBSyncDataManager()
            : base(GetConnectionStringName("RulesDBConnStringKey", "RulesDBConnString"))
        {

        }

        public void DeleteRuleChangedTables()
        {
            string query = "DELETE FROM [rules].[RuleChangedForProcessing];DELETE FROM [rules].[RuleChanged];";
            ExecuteNonQueryText(query, null);
        }

        public void SyncRoutingProductRules(bool useTempTables)
        {
            ExecuteNonQueryText(string.Format(@"Insert Into {0} ([TypeID], [RuleDetails], [BED], [EED], [SourceID], [IsDeleted], [CreatedTime], [CreatedBy], [LastModifiedTime], [LastModifiedBy])
                                                SELECT [TypeID], [RuleDetails], [BED], [EED], [SourceID], [IsDeleted], [CreatedTime], [CreatedBy], [LastModifiedTime], [LastModifiedBy] FROM [rules].[Rule]
                                                Where [RuleDetails] like '%""RoutingProductId"":%'", MigrationUtils.GetTableName(_schema, _tableName, useTempTables)), null);
        }
    }
}