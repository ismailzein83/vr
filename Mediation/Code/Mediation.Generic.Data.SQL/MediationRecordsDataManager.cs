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
    public class MediationRecordsDataManager : BaseSQLDataManager, IMediationRecordsDataManager
    {
        public MediationRecordsDataManager()
            : base(GetConnectionStringName("Mediation_GenericRecord_DBConnStringKey", "Mediation_GenericRecord_DBConnString"))
        { }

        readonly string[] columns = { "EventId", "SessionId", "EventTime", "EventStatus", "MediationDefinitionId", "EventDetails" };
        DataRecordTypeManager _dataRecordTypeManager = new DataRecordTypeManager();
        Guid _dataRecordTypeId;

        #region IBulkApplyDataManager MediationRecord

        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "[Mediation_Generic].[MediationRecord]",
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
        public void WriteRecordToStream(MediationRecord record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}", record.EventId, record.SessionId, record.EventTime.ToString("yyy-MM-dd HH:mm:ss"), (short)record.EventStatus, record.MediationDefinitionId, Serializer.Serialize(record.EventDetails, true));
        }

        #endregion

        #region IMediationRecordsDataManager
        public void SaveMediationRecordsToDB(List<MediationRecord> mediationRecords)
        {
            Object dbApplyStream = InitialiazeStreamForDBApply();
            foreach (MediationRecord mediationRecord in mediationRecords)
                WriteRecordToStream(mediationRecord, dbApplyStream);
            Object preparedMediationRecords = FinishDBApplyStream(dbApplyStream);
            ApplyMediationRecordToDB(preparedMediationRecords);
        }
        public IEnumerable<MediationRecord> GetMediationRecords()
        {
            return GetItemsSP("[Mediation_Generic].[sp_MediationRecord_GetAll]", MediationRecordMapper);
        }
        public IEnumerable<MediationRecord> GetMediationRecordsByStatus(int mediationDefinitionId, EventStatus status)
        {
            return GetItemsSP("[Mediation_Generic].[sp_MediationRecord_GetByStatus]", MediationRecordMapper, mediationDefinitionId, (short)status);
        }
        public IEnumerable<MediationRecord> GetMediationRecordsByIds(int mediationDefinitionId, IEnumerable<string> sessionIds)
        {
            return GetItemsSPCmd("[Mediation_Generic].[sp_MediationRecord_GetByIds]", MediationRecordMapper, (cmd) =>
            {
                var dtPrm = new SqlParameter("@Ids", SqlDbType.Structured);
                dtPrm.TypeName = "dbo.[StringIDType]";
                dtPrm.Value = BuildEventIdsTable(sessionIds);
                cmd.Parameters.Add(dtPrm);

                dtPrm = new SqlParameter("@MediationDefinitionId", SqlDbType.BigInt);
                dtPrm.Value = mediationDefinitionId;
                cmd.Parameters.Add(dtPrm);
            });
        }
        public bool DeleteMediationRecordsBySessionIds(int mediationDefinitionId, IEnumerable<string> sessionIds)
        {
            return ExecuteNonQuerySPCmd("[Mediation_Generic].[sp_MediationRecord_DeleteBySessionIds]", (cmd) =>
            {
                var dtPrm = new SqlParameter("@Ids", SqlDbType.Structured);
                dtPrm.TypeName = "dbo.[StringIDType]";
                dtPrm.Value = BuildEventIdsTable(sessionIds);
                cmd.Parameters.Add(dtPrm);

                dtPrm = new SqlParameter("@MediationDefinitionId", SqlDbType.BigInt);
                dtPrm.Value = mediationDefinitionId;
                cmd.Parameters.Add(dtPrm);
            }) > 0;
        }

        public Guid DataRecordTypeId
        {
            set { _dataRecordTypeId = value; }
        }

        #endregion

        #region Private Methods
        void ApplyMediationRecordToDB(object preparedMediationRecords)
        {
            InsertBulkToTable(preparedMediationRecords as BaseBulkInsertInfo);
        }
        private DataTable BuildEventIdsTable(IEnumerable<string> eventIds)
        {
            DataTable dtEventIds = new DataTable();
            dtEventIds.Columns.Add("SessionId", typeof(string));
            dtEventIds.BeginLoadData();
            foreach (var id in eventIds)
            {
                DataRow dr = dtEventIds.NewRow();
                dr["SessionId"] = id;
                dtEventIds.Rows.Add(dr);
            }
            dtEventIds.EndLoadData();
            return dtEventIds;
        }

        #endregion

        #region Mappers

        MediationRecord MediationRecordMapper(IDataReader reader)
        {
            return new MediationRecord()
            {
                EventId = (int)reader["EventId"],
                SessionId = reader["SessionId"] as string,
                EventTime = GetReaderValue<DateTime>(reader, "EventTime"),
                EventStatus = (EventStatus)GetReaderValue<byte>(reader, "EventStatus"),
                MediationDefinitionId = (int)reader["MediationDefinitionId"],
                EventDetails = _dataRecordTypeManager.DeserializeRecord(reader["EventDetails"] as string, _dataRecordTypeId)
            };
        }

        #endregion

    }
}
