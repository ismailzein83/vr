using System.Collections.Generic;
using TOne.WhS.Routing.Entities;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace TOne.WhS.Routing.Data.RDB
{
    public class SwitchSyncDataDataManager : RoutingDataManager, ISwitchSyncDataDataManager
    {
        private static string DBTABLE_SCHEMA = "dbo";
        internal static string DBTABLE_NAME = "SwitchSyncData";
        private static string TABLE_NAME = "dbo_SwitchSyncData";
        private static string TABLE_ALIAS = "ssd";

        private const string COL_SwitchId = "SwitchId";
        private const string COL_LastVersionNumber = "LastVersionNumber";

        internal static Dictionary<string, RoutingTableColumnDefinition> s_SwitchSyncDataColumnDefinitions;

        static SwitchSyncDataDataManager()
        {
            s_SwitchSyncDataColumnDefinitions = BuildSwitchSyncDataColumnDefinitions();
            Dictionary<string, RDBTableColumnDefinition> columns = Helper.GetRDBTableColumnDefinitions(s_SwitchSyncDataColumnDefinitions);

            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = DBTABLE_SCHEMA,
                DBTableName = DBTABLE_NAME,
                Columns = columns
            });
        }

        public List<SwitchSyncData> GetSwitchSyncDataByIds(List<string> switchIds)
        {
            if (switchIds == null || switchIds.Count == 0)
                return null;

            RDBQueryContext queryContext = new RDBQueryContext(GetDataProvider());

            RDBSelectQuery selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereContext = selectQuery.Where();
            whereContext.ListCondition(COL_SwitchId, RDBListConditionOperator.IN, switchIds);

            return queryContext.GetItems<SwitchSyncData>(SwitchSyncDataMapper);
        }

        public void ApplySwitchesSyncData(List<string> switchIds, int versionNumber)
        {
            if (switchIds == null || switchIds.Count == 0)
                return;
           
            var queryContext = new RDBQueryContext(GetDataProvider());
            List<string> syncSwitchIds = this.GetSyncSwitchIds();

            foreach (var switchId in switchIds)
            {
                if (syncSwitchIds.Contains(switchId))
                {
                    var updateQuery = queryContext.AddUpdateQuery();
                    updateQuery.FromTable(TABLE_NAME);
                    updateQuery.Column(COL_LastVersionNumber).Value(versionNumber);

                    var whereContext = updateQuery.Where();
                    whereContext.EqualsCondition(COL_SwitchId).Value(switchId);
                }
                else
                {
                    var insertQuery = queryContext.AddInsertQuery();
                    insertQuery.IntoTable(TABLE_NAME);
                    insertQuery.Column(COL_SwitchId).Value(switchId);
                    insertQuery.Column(COL_LastVersionNumber).Value(versionNumber);
                }
            }

            queryContext.ExecuteNonQuery();
        }

        public void ResetSwitchSyncData(string switchId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var deleteQuery = queryContext.AddDeleteQuery();
            deleteQuery.FromTable(TABLE_NAME);

            var whereContext = deleteQuery.Where();
            whereContext.EqualsCondition(COL_SwitchId).Value(switchId);
        }

        private List<string> GetSyncSwitchIds()
        {
            RDBQueryContext queryContext = new RDBQueryContext(GetDataProvider());

            RDBSelectQuery selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().Columns(COL_SwitchId);

            return queryContext.GetItems((reader) => { return reader.GetString(COL_SwitchId); });
        }

        private SwitchSyncData SwitchSyncDataMapper(IRDBDataReader reader)
        {
            SwitchSyncData switchSyncData = new SwitchSyncData
            {
                SwitchId = reader.GetString(COL_SwitchId),
                LastVersionNumber = reader.GetInt(COL_LastVersionNumber)
            };

            return switchSyncData;
        }

        private static Dictionary<string, RoutingTableColumnDefinition> BuildSwitchSyncDataColumnDefinitions()
        {
            var columnDefinitions = new Dictionary<string, RoutingTableColumnDefinition>();
            columnDefinitions.Add(COL_SwitchId, new RoutingTableColumnDefinition(COL_SwitchId, RDBDataType.NVarchar, true));
            columnDefinitions.Add(COL_LastVersionNumber, new RoutingTableColumnDefinition(COL_LastVersionNumber, RDBDataType.Int, true));
            return columnDefinitions;
        }
    }
}