using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace TOne.WhS.Routing.Data.RDB
{
    public class RPZoneCodeGroupDataManager : RoutingDataManager, IRPZoneCodeGroupDataManager
    {
        private readonly string[] columns = { "ZoneId", "IsSale", "CodeGroups" };

        private static string DBTABLE_SCHEMA = "dbo";
        internal static string DBTABLE_NAME = "ZoneCodeGroup";
        private static string TABLE_NAME = "dbo_ZoneCodeGroup";
        private static string TABLE_ALIAS = "zcg";

        private const string COL_ZoneId = "ZoneId";
        private const string COL_CodeGroups = "CodeGroups";
        private const string COL_IsSale = "IsSale";

        internal static Dictionary<string, RoutingTableColumnDefinition> s_ZoneCodeGroupColumnDefinitions;

        static RPZoneCodeGroupDataManager()
        {
            s_ZoneCodeGroupColumnDefinitions = BuildZoneCodeGroupColumnDefinitions();
            Dictionary<string, RDBTableColumnDefinition> columns = Helper.GetRDBTableColumnDefinitions(s_ZoneCodeGroupColumnDefinitions);

            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = DBTABLE_SCHEMA,
                DBTableName = DBTABLE_NAME,
                Columns = columns
            });
        }

        #region Public Methods

        public object InitialiazeStreamForDBApply()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var bulkInsertQueryContext = queryContext.StartBulkInsert();
            bulkInsertQueryContext.IntoTable(TABLE_NAME, '^', COL_ZoneId, COL_CodeGroups, COL_IsSale);
            return bulkInsertQueryContext;
        }

        public void WriteRecordToStream(ZoneCodeGroup record, object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertQueryContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");

            var recordContext = bulkInsertQueryContext.WriteRecord();
            recordContext.Value(record.ZoneId);

            if (record.CodeGroups != null && record.CodeGroups.Count > 0)
                recordContext.Value(string.Join<string>(",", record.CodeGroups));
            else
                recordContext.Value(string.Empty);

            recordContext.Value(record.IsSale);
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertQueryContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");
            bulkInsertQueryContext.CloseStream();
            return bulkInsertQueryContext;
        }

        public void ApplyZoneCodeGroupsForDB(object preparedObject)
        {
            preparedObject.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream").Apply();
        }

        public Dictionary<bool, Dictionary<long, HashSet<string>>> GetZoneCodeGroups()
        {
            Dictionary<bool, Dictionary<long, HashSet<string>>> result = new Dictionary<bool, Dictionary<long, HashSet<string>>>();

            RDBQueryContext queryContext = new RDBQueryContext(GetDataProvider());

            RDBSelectQuery selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            queryContext.ExecuteReader((reader) =>
            {
                while (reader.Read())
                {
                    long zoneId = reader.GetLong("ZoneId");
                    bool isSale = reader.GetBoolean("IsSale");

                    string codeGroupsAsString = reader.GetString("CodeGroups");
                    HashSet<string> codeGroups = ExtensionMethods.ToHashSet(codeGroupsAsString.Split(','));

                    Dictionary<long, HashSet<string>> zoneCodeGroupsByZoneId = result.GetOrCreateItem(isSale);
                    zoneCodeGroupsByZoneId.Add(zoneId, codeGroups);
                }
            });

            return result;
        }

        #endregion

        #region Private Methods

        private static Dictionary<string, RoutingTableColumnDefinition> BuildZoneCodeGroupColumnDefinitions()
        {
            var columnDefinitions = new Dictionary<string, RoutingTableColumnDefinition>();
            columnDefinitions.Add(COL_ZoneId, new RoutingTableColumnDefinition(COL_ZoneId, RDBDataType.BigInt, true));
            columnDefinitions.Add(COL_CodeGroups, new RoutingTableColumnDefinition(COL_CodeGroups, RDBDataType.NVarchar, true));
            columnDefinitions.Add(COL_IsSale, new RoutingTableColumnDefinition(COL_IsSale, RDBDataType.Boolean, true));
            return columnDefinitions;
        }

        #endregion
    }
}