using System;
using System.Collections.Generic;
using System.Data;
using Vanrise.Data.SQL;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Data.SQL
{
    public class StagingSummaryRecordDataManager : BaseSQLDataManager, IStagingSummaryRecordDataManager
    {
        readonly string[] columns = { "ProcessInstanceId", "StageName", "BatchStart", "BatchEnd", "Data", "AlreadyFinalised" };

        public StagingSummaryRecordDataManager()
            : base(GetConnectionStringName("BusinessProcessDBConnStringKey", "BusinessProcessDBConnString"))
        {

        }

        #region Public Methods
        public List<StagingSummaryInfo> GetStagingSummaryInfo(long processInstanceId, string stageName)
        {
            return GetItemsSP("reprocess.sp_StagingSummaryRecord_GetStageRecordInfo", StagingSummaryInfoMapper, processInstanceId, stageName);
        }

        public void GetStagingSummaryRecords(long processInstanceId, string stageName, DateTime batchStart, Action<StagingSummaryRecord> onItemLoaded)
        {
            ExecuteReaderSP("reprocess.sp_StagingSummaryRecord_GetRecords",
               (reader) =>
               {
                   while (reader.Read())
                   {
                       StagingSummaryRecord instance = StagingSummaryRecordMapper(reader);
                       onItemLoaded(instance);
                   }
               }, processInstanceId, stageName, batchStart);
        }

        public void DeleteStagingSummaryRecords(long processInstanceId, string stageName, DateTime batchStart)
        {
            ExecuteNonQuerySP("reprocess.sp_StagingSummaryRecord_Delete", processInstanceId, stageName, batchStart);
        }

        public object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }
        public void WriteRecordToStream(StagingSummaryRecord record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}", record.ProcessInstanceId, record.StageName, GetDateTimeForBCP(record.BatchStart), GetDateTimeForBCP(record.BatchEnd), record.Data != null ? Convert.ToBase64String(record.Data) : null, record.AlreadyFinalised ? 1 : 0);
        }


        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "[reprocess].[StagingSummaryRecord]",
                Stream = streamForBulkInsert,
                TabLock = true,
                KeepIdentity = false,
                FieldSeparator = '^',
                ColumnNames = columns
            };
        }

        public void ApplyStreamToDB(object stream)
        {
            base.InsertBulkToTable(stream as StreamBulkInsertInfo);
        }

        #endregion

        #region Mappers
        private StagingSummaryRecord StagingSummaryRecordMapper(IDataReader reader)
        {
            return new StagingSummaryRecord()
            {
                ProcessInstanceId = GetReaderValue<long>(reader, "ProcessInstanceId"),
                BatchStart = GetReaderValue<DateTime>(reader, "BatchStart"),
                BatchEnd = GetReaderValue<DateTime>(reader, "BatchEnd"),
                Data = reader["Data"] != DBNull.Value ? Convert.FromBase64String(reader["Data"] as string) : null,
                AlreadyFinalised = GetReaderValue<bool>(reader, "AlreadyFinalised"),
                StageName = reader["StageName"] as string
            };
        }

        private StagingSummaryInfo StagingSummaryInfoMapper(IDataReader reader)
        {
            return new StagingSummaryInfo()
            {
                BatchStart = GetReaderValue<DateTime>(reader, "BatchStart"),
                BatchEnd = GetReaderValue<DateTime>(reader, "BatchEnd"),
                AlreadyFinalised = GetReaderValue<bool>(reader, "AlreadyFinalised")
            };
        }
        #endregion
    }
}
