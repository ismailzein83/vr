using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.Data.RDB;

namespace Vanrise.BusinessProcess.Data.RDB
{
    public class BPEventDataManager : IBPEventDataManager
    {

        static string TABLE_NAME = "bp_BPEvent";
        static string TABLE_ALIAS = "BPEvent";

        const string COL_ID = "ID";
        const string COL_ProcessInstanceID = "ProcessInstanceID";
        const string COL_Bookmark = "Bookmark";
        const string COL_Payload = "Payload";
        const string COL_CreatedTime = "CreatedTime";


        static BPEventDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_ProcessInstanceID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_Bookmark, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 1000 });
            columns.Add(COL_Payload, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "bp",
                DBTableName = "BPEvent",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime
            });
        }
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("BusinessProcess", "BusinessProcessDBConnStringKey", "BusinessProcessDBConnString");
        }

        public void DeleteEvent(long eventId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var deleteQuery = queryContext.AddDeleteQuery();
            deleteQuery.FromTable(TABLE_NAME);

            var whereContext = deleteQuery.Where();
            whereContext.EqualsCondition(COL_ID).Value(eventId);

            queryContext.ExecuteNonQuery();
        }

        public void DeleteProcessInstanceEvents(long processInstanceId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var deleteQuery = queryContext.AddDeleteQuery();
            deleteQuery.FromTable(TABLE_NAME);

            var whereContext = deleteQuery.Where();
            whereContext.EqualsCondition(COL_ProcessInstanceID).Value(processInstanceId);

            queryContext.ExecuteNonQuery();
        }

        public List<long> GetEventsDistinctProcessInstanceIds()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);

            var groupByContext = selectQuery.GroupBy();
            groupByContext.Select().Column(COL_ProcessInstanceID);

            return queryContext.GetItems(reader => reader.GetLong("ProcessInstanceID"));
        }

        public IEnumerable<BPEvent> GetInstancesEvents(List<long> instancesIds)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereContext = selectQuery.Where();
            whereContext.ListCondition(COL_ProcessInstanceID, RDBListConditionOperator.IN, instancesIds);

            return queryContext.GetItems(BPEventMapper);
        }

        public int InsertEvent(long processInstanceId, string bookmarkName, object eventData)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.Column(COL_ProcessInstanceID).Value(processInstanceId);
            insertQuery.Column(COL_Bookmark).Value(bookmarkName);
            if (eventData != null)
                insertQuery.Column(COL_Payload).Value(Serializer.Serialize(eventData));

            return queryContext.ExecuteNonQuery();
        }

        #region Private Methods

        BPEvent BPEventMapper(IRDBDataReader reader)
        {
            BPEvent instance = new BPEvent
            {
                BPEventID = reader.GetLong("ID"),
                ProcessInstanceID = reader.GetLong("ProcessInstanceID"),
                Bookmark = reader.GetString("Bookmark")
            };
            string payload = reader.GetString("Payload");
            if (!String.IsNullOrWhiteSpace(payload))
                instance.Payload = Serializer.Deserialize(payload);
            return instance;
        }

        #endregion
    }
}
