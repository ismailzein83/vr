using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Data.RDB;

namespace Vanrise.BusinessProcess.Data.RDB
{
    public class BPTimeSubscriptionDataManager : IBPTimeSubscriptionDataManager
    {
        static string TABLE_NAME = "bp_BPTimeSubscription";
        static string TABLE_ALIAS = "TimeSub";

        const string COL_ID = "ID";
        const string COL_ProcessInstanceID = "ProcessInstanceID";
        const string COL_Bookmark = "Bookmark";
        const string COL_DueTime = "DueTime";
        const string COL_Payload = "Payload";
        const string COL_CreatedTime = "CreatedTime";


        static BPTimeSubscriptionDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_ProcessInstanceID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_Bookmark, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 100 });
            columns.Add(COL_DueTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_Payload, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "bp",
                DBTableName = "BPTimeSubscription",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime
            });
        }
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("BusinessProcess", "BusinessProcessDBConnStringKey", "BusinessProcessDBConnString");
        }

        #region Public Methods
        public bool DeleteBPTimeSubscription(long bpTimeSubscriptionId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var deleteQuery = queryContext.AddDeleteQuery();
            deleteQuery.FromTable(TABLE_NAME);

            var where = deleteQuery.Where();
            where.EqualsCondition(COL_ID).Value(bpTimeSubscriptionId);

            return queryContext.ExecuteNonQuery() > 0;
        }
        public IEnumerable<BPTimeSubscription> GetDueBPTimeSubscriptions()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var where = selectQuery.Where();
            where.LessOrEqualCondition(COL_DueTime).DateNow();

            return queryContext.GetItems(BPTimeSubscriptionMapper);
        }
        public int InsertBPTimeSubscription(long processInstanceId, string bookmarkName, TimeSpan delay, BPTimeSubscriptionPayload payload)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.Column(COL_ProcessInstanceID).Value(processInstanceId);
            insertQuery.Column(COL_Bookmark).Value(bookmarkName);

            var dateTimeValueToAddContext = insertQuery.Column(COL_DueTime).DateTimeAdd(RDBDateTimeAddInterval.Seconds);
            dateTimeValueToAddContext.ValueToAdd().Value((int)delay.TotalSeconds);
            dateTimeValueToAddContext.ValueToAdd().DateNow();

            if (payload != null)
                insertQuery.Column(COL_Payload).Value(Vanrise.Common.Serializer.Serialize(payload));

            return queryContext.ExecuteNonQuery();
        }
        #endregion

        #region Private Methods
        BPTimeSubscription BPTimeSubscriptionMapper(IRDBDataReader reader)
        {
            string serializedPayload = reader.GetString("Payload");

            return new BPTimeSubscription
            {
                BPTimeSubscriptionId = reader.GetLong("ID"),
                ProcessInstanceId = reader.GetLong("ProcessInstanceID"),
                Bookmark = reader.GetString("Bookmark"),
                Payload = !string.IsNullOrEmpty(serializedPayload) ? Vanrise.Common.Serializer.Deserialize<BPTimeSubscriptionPayload>(serializedPayload) : null,
                DueTime = reader.GetDateTime("DueTime")
            };
        }
        #endregion
    }
}
