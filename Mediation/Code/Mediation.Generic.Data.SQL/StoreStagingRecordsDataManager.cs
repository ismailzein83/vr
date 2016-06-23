using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Mediation.Generic.Entities;
using Vanrise.Common;
using Vanrise.Data.SQL;
using Vanrise.GenericData.Business;

namespace Mediation.Generic.Data.SQL
{
    public class StoreStagingRecordsDataManager : MediationGenericDataManager, IStoreStagingRecordsDataManager
    {

        readonly string[] columns = { "EventId", "SessionId", "EventTime", "EventStatus", "EventDetails" };
        DataRecordTypeManager _dataRecordTypeManager = new DataRecordTypeManager();
        int _dataRecordTypeId;
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
        public IEnumerable<StoreStagingRecord> GetStoreStagingRecordsByStatus(EventStatus status)
        {
            return GetItemsSP("[Mediation_Generic].[sp_StoreStagingRecord_GetByStatus]", StoreStagingRecordMapper, (short)status);
        }

        public IEnumerable<StoreStagingRecord> GetStoreStagingRecordsByStatus(EventStatus status, Type type)
        {
            return GetItemsSP("[Mediation_Generic].[sp_StoreStagingRecord_GetByStatus]", StoreStagingRecordMapper, (short)status);
        }
        public IEnumerable<StoreStagingRecord> GetStoreStagingRecordsByIds(IEnumerable<int> eventIds)
        {
            return GetItemsSPCmd("[Mediation_Generic].[sp_StoreStagingRecord_GetByIds]", StoreStagingRecordMapper, (cmd) =>
            {
                var dtPrm = new SqlParameter("@Ids", SqlDbType.Structured);
                dtPrm.TypeName = "dbo.IntIDType";
                dtPrm.Value = BuildEventIdsTable(eventIds);
                cmd.Parameters.Add(dtPrm);
            });
        }

        #region Private Methods
        private DataTable BuildEventIdsTable(IEnumerable<int> eventIds)
        {
            DataTable dtEventIds = new DataTable();
            dtEventIds.Columns.Add("ID", typeof(Int32));
            dtEventIds.BeginLoadData();
            foreach (var z in eventIds)
            {
                DataRow dr = dtEventIds.NewRow();
                dr["ID"] = z;
                dtEventIds.Rows.Add(dr);
            }
            dtEventIds.EndLoadData();
            return dtEventIds;
        }

        #endregion

        #region Mappers

        StoreStagingRecord StoreStagingRecordMapper(IDataReader reader)
        {
            return new StoreStagingRecord()
            {
                EventId = (int)reader["EventId"],
                SessionId = (long)reader["SessionId"],
                EventTime = GetReaderValue<DateTime>(reader, "EventTime"),
                EventStatus = GetReaderValue<EventStatus>(reader, "EventStatus"),
                EventDetails = _dataRecordTypeManager.DeserializeRecord(reader["EventDetails"] as string, _dataRecordTypeId)
            };
        }

        #endregion



        public int DataRecordTypeId
        {
            set { _dataRecordTypeId = value; }
        }
    }
}
