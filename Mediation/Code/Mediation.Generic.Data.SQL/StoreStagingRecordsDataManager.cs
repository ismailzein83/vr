using System;
using System.Collections.Generic;
using System.Data;
using Mediation.Generic.Entities;
using Vanrise.Common;
using Vanrise.Data.SQL;

namespace Mediation.Generic.Data.SQL
{
    public class StoreStagingRecordsDataManager : MediationGenericDataManager, IStoreStagingRecordsDataManager
    {

        readonly string[] columns = { "EventId", "SessionId", "EventTime", "EventStatus", "EventDetails" };
        public void SaveStoreStagingRecordsToDB(List<StoreStagingRecord> storeStagingRecords)
        {
            Object dbApplyStream = InitialiazeStreamForDBApply();
            foreach (StoreStagingRecord storeStagingRecord in storeStagingRecords)
                WriteRecordToStream(storeStagingRecord, dbApplyStream);
            Object preparedStoreStagingRecords = FinishDBApplyStream(dbApplyStream);
            ApplyStoreStagingRecordToDB(preparedStoreStagingRecords);
        }
        public void ApplyStoreStagingRecordToDB(object preparedStoreStagingRecords)
        {
            InsertBulkToTable(preparedStoreStagingRecords as BaseBulkInsertInfo);
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "[dbo].[StoreStagingRecord]",
                Stream = streamForBulkInsert,
                TabLock = true,
                KeepIdentity = false,
                FieldSeparator = '^',
                ColumnNames = columns
            };
        }
        public object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }
        public void WriteRecordToStream(StoreStagingRecord record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}", record.EventId, record.SessionId, record.EventTime, (short)record.EventStatus, Serializer.Serialize(record.EventDetails, true));
        }

        public IEnumerable<StoreStagingRecord> GetStoreStagingRecords()
        {
            return GetItemsSP("[Mediation_Generic].[sp_StoreStagingRecord_GetAll]", StoreStagingRecordMapper);
        }

        #region Mappers

        StoreStagingRecord StoreStagingRecordMapper(IDataReader reader)
        {
            return new StoreStagingRecord()
            {
                EventId = (int)reader["EventId"],
                SessionId = (long)reader["SessionId"],
                EventTime = GetReaderValue<DateTime>(reader, "EventTime"),
                EventStatus = GetReaderValue<EventStatus>(reader, "EventStatus"),
                EventDetails = Serializer.Deserialize<object>(reader["EventDetails"] as string)
            };
        }

        #endregion
    }
}
