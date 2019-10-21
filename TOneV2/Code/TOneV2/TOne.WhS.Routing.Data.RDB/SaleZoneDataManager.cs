using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace TOne.WhS.Routing.Data.RDB
{
    public class SaleZoneDataManager : RoutingDataManager
    {
        #region Fields/Ctor

        private static string DBTABLE_SCHEMA = "dbo";
        internal static string DBTABLE_NAME = "SaleZone";
        private static string TABLE_NAME = "dbo_SaleZone";

        private const string COL_ID = "ID";
        internal const string COL_Name = "Name";

        internal static Dictionary<string, RoutingTableColumnDefinition> s_SaleZoneColumnDefinitions;

        static SaleZoneDataManager()
        {
            s_SaleZoneColumnDefinitions = BuildSaleZoneColumnDefinitions();
            Dictionary<string, RDBTableColumnDefinition> columns = Helper.GetRDBTableColumnDefinitions(s_SaleZoneColumnDefinitions);

            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = DBTABLE_SCHEMA,
                DBTableName = DBTABLE_NAME,
                Columns = columns,
                IdColumnName = COL_ID,
            });
        }

        #endregion

        #region Public Methods

        public object InitialiazeStreamForDBApply()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var bulkInsertQueryContext = queryContext.StartBulkInsert();
            bulkInsertQueryContext.IntoTable(TABLE_NAME, '^', COL_ID, COL_Name);
            return bulkInsertQueryContext;
        }

        public void WriteRecordToStream(SaleZone record, object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertQueryContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");

            var recordContext = bulkInsertQueryContext.WriteRecord();
            recordContext.Value(record.SaleZoneId);

            if (!string.IsNullOrEmpty(record.Name))
                recordContext.Value(record.Name);
            else
                recordContext.Value(string.Empty);
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertQueryContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");
            bulkInsertQueryContext.CloseStream();
            return bulkInsertQueryContext;
        }

        public void ApplySaleZonesToTable(object preparedObject)
        {
            preparedObject.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream").Apply();
        }

        public void AddJoinSaleZoneById(RDBJoinContext joinContext, RDBJoinType joinType, string saleZoneTableAlias, string originalTableAlias, string originalTableSaleZoneIdCol, bool withNoLock)
        {
            joinContext.JoinOnEqualOtherTableColumn(joinType, TABLE_NAME, saleZoneTableAlias, COL_ID, originalTableAlias, originalTableSaleZoneIdCol, withNoLock);
        }

        #endregion

        #region Private Methods

        private static Dictionary<string, RoutingTableColumnDefinition> BuildSaleZoneColumnDefinitions()
        {
            var columnDefinitions = new Dictionary<string, RoutingTableColumnDefinition>();
            columnDefinitions.Add(COL_ID, new RoutingTableColumnDefinition(COL_ID, RDBDataType.BigInt, null, null, true, true, false));
            columnDefinitions.Add(COL_Name, new RoutingTableColumnDefinition(COL_Name, RDBDataType.Varchar, true));
            return columnDefinitions;
        }

        #endregion
    }
}