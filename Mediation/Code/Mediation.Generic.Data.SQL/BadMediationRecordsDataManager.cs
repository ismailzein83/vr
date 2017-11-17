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
    public class BadMediationRecordsDataManager : BaseSQLDataManager, IBadMediationRecordsDataManager
    {
        public BadMediationRecordsDataManager()
            : base(GetConnectionStringName("Mediation_GenericRecord_DBConnStringKey", "Mediation_GenericRecord_DBConnString"))
        { 
        
        }

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
                TableName = "[Mediation_Generic].[BadMediationRecord]",
                Stream = streamForBulkInsert,
                TabLock = false,
                KeepIdentity = false,
                FieldSeparator = '^',
                ColumnNames = columns
            };
        }
        public object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }
        public void WriteRecordToStream(BadRecord record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}", record.EventId, record.SessionId, GetDateTimeForBCP(record.EventTime), (short)record.EventStatus, record.MediationDefinitionId, Serializer.Serialize(record.EventDetails, true));
        }

        #endregion

        #region IMediationRecordsDataManager
        public void SaveBadMediationRecordsToDB(List<BadRecord> badRecords)
        {
            Object dbApplyStream = InitialiazeStreamForDBApply();
            foreach (BadRecord mediationRecord in badRecords)
                WriteRecordToStream(mediationRecord, dbApplyStream);
            Object preparedMediationRecords = FinishDBApplyStream(dbApplyStream);
            ApplyMediationRecordToDB(preparedMediationRecords);
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

        #endregion
               
    }
}
