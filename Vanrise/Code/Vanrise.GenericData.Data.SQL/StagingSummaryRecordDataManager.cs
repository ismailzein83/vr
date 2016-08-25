﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.GenericData.Entities;
using Vanrise.Reprocess.Entities;

namespace Vanrise.GenericData.Data.SQL
{
    public class StagingSummaryRecordDataManager : BaseSQLDataManager, IStagingSummaryRecordDataManager
    {
        readonly string[] columns = { "ProcessInstanceId", "StageName", "BatchStart", "Data" };

        public StagingSummaryRecordDataManager()
            : base(GetConnectionStringName("BusinessProcessDBConnStringKey", "BusinessProcessDBConnString"))
        {

        }

        #region Public Methods
        public List<Vanrise.Reprocess.Entities.BatchRecord> GetStageRecordInfo(long processInstanceId, string stageName)
        {
            return GetItemsSP("reprocess.sp_StagingSummaryRecord_GetStageRecordInfo", StageRecordInfoMapper, processInstanceId, stageName);
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

            //return GetItemsSP("reprocess.sp_StagingSummaryRecord_GetAll", StagingSummaryRecordMapper, processInstanceId, stageName);
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
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}", record.ProcessInstanceId, record.StageName, record.BatchStart, Convert.ToBase64String(record.Data));
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
                Data = reader["Data"] != DBNull.Value ? Convert.FromBase64String(reader["Data"] as string) : null,
                StageName = reader["StageName"] as string
            };
        }

        private Vanrise.Reprocess.Entities.BatchRecord StageRecordInfoMapper(IDataReader reader)
        {
            return new StageRecordInfo()
            {
                BatchStart = GetReaderValue<DateTime>(reader, "BatchStart")
            };
        }
        #endregion
    }
}
