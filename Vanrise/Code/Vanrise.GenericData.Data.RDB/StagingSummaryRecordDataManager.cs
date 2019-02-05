using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using Vanrise.Entities;

namespace Vanrise.GenericData.Data.RDB
{
    public class StagingSummaryRecordDataManager : IStagingSummaryRecordDataManager
    {
        #region RDB
        static string TABLE_NAME = "reprocess_StagingSummaryRecord";
        static string TABLE_ALIAS = "record";
        const string COL_ProcessInstanceId = "ProcessInstanceId";
        const string COL_StageName = "StageName";
        const string COL_BatchStart = "BatchStart";
        const string COL_BatchEnd = "BatchEnd";
        const string COL_Data = "Data";
        const string COL_Payload = "Payload";
        const string COL_AlreadyFinalised = "AlreadyFinalised";
        const string COL_CreatedTime = "CreatedTime";


        static StagingSummaryRecordDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ProcessInstanceId, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_StageName, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_BatchStart, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_BatchEnd, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_Data, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar });
            columns.Add(COL_Payload, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar });
            columns.Add(COL_AlreadyFinalised, new RDBTableColumnDefinition { DataType = RDBDataType.Boolean });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "reprocess",
                DBTableName = "StagingSummaryRecord",
                Columns = columns,
                CreatedTimeColumnName = COL_CreatedTime
            });
        }
        #endregion

        #region Private Methods
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("VR_GenericData_Transaction", "BusinessProcessDBConnStringKey", "BusinessProcessDBConnString");
        }
        #endregion

        #region Mappers
        private StagingSummaryRecord StagingSummaryRecordMapper(IRDBDataReader reader)
        {
            var stagingSummaryRecord =  new StagingSummaryRecord()
            {
                ProcessInstanceId = reader.GetLong(COL_ProcessInstanceId),
                BatchStart = reader.GetDateTime(COL_BatchStart),
                BatchEnd = reader.GetDateTime(COL_BatchEnd),
                AlreadyFinalised = reader.GetBoolean(COL_AlreadyFinalised),
                StageName = reader.GetString(COL_StageName)
            };
            var data = reader.GetString(COL_Data);
            if (data != null)
                stagingSummaryRecord.Data = Convert.FromBase64String(data);
            return stagingSummaryRecord;
        }

        private StagingSummaryInfo StagingSummaryInfoMapper(IRDBDataReader reader)
        {
            return new StagingSummaryInfo()
            {
                BatchStart = reader.GetDateTime(COL_BatchStart),
                BatchEnd = reader.GetDateTime(COL_BatchEnd),
                AlreadyFinalised = reader.GetBoolean(COL_AlreadyFinalised),
                Payload = reader.GetString(COL_Payload)
            };
        }
        #endregion

        #region IStagingSummaryRecordDataManager

        readonly string[] columns = { COL_ProcessInstanceId, COL_StageName, COL_BatchStart, COL_BatchEnd, COL_Data, COL_Payload, COL_AlreadyFinalised };

        public void GetStagingSummaryRecords(long processInstanceId, string stageName, DateTime batchStart, DateTime batchEnd, Action<StagingSummaryRecord> onItemLoaded)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            var where = selectQuery.Where();
            where.EqualsCondition(COL_ProcessInstanceId).Value(processInstanceId);
            where.EqualsCondition(COL_StageName).Value(stageName);
            where.EqualsCondition(COL_BatchStart).Value(batchStart);
            where.EqualsCondition(COL_BatchEnd).Value(batchEnd);
            queryContext.ExecuteReader(reader =>
            {
                while (reader.Read())
                {
                    StagingSummaryRecord instance = StagingSummaryRecordMapper(reader);
                    onItemLoaded(instance);
                }
            });
        }

        public void ApplyStreamToDB(object stream)
        {
            stream.CastWithValidate<RDBBulkInsertQueryContext>("stream").Apply();
        }

        public void DeleteStagingSummaryRecords(long processInstanceId, string stageName, DateTime batchStart, DateTime batchEnd)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var deleteQuery = queryContext.AddDeleteQuery();
            deleteQuery.FromTable(TABLE_NAME);
            var where = deleteQuery.Where();
            where.EqualsCondition(COL_ProcessInstanceId).Value(processInstanceId);
            where.EqualsCondition(COL_StageName).Value(stageName);
            where.EqualsCondition(COL_BatchStart).Value(batchStart);
            where.EqualsCondition(COL_BatchEnd).Value(batchEnd);
            queryContext.ExecuteNonQuery();
        }

        public List<StagingSummaryInfo> GetStagingSummaryInfo(long processInstanceId, string stageName)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().Columns(COL_BatchStart, COL_BatchEnd, COL_AlreadyFinalised, COL_Payload);
            var where = selectQuery.Where();
            where.EqualsCondition(COL_ProcessInstanceId).Value(processInstanceId);
            where.EqualsCondition(COL_StageName).Value(stageName);
            selectQuery.Sort().ByColumn(COL_BatchStart, RDBSortDirection.ASC);
            return queryContext.GetItems(StagingSummaryInfoMapper);
        }

        public object InitialiazeStreamForDBApply()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var streamForBulkInsert = queryContext.StartBulkInsert();
            streamForBulkInsert.IntoTable(TABLE_NAME, '^', columns);
            return streamForBulkInsert;
        }

        public void WriteRecordToStream(StagingSummaryRecord record, object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");
            var recordContext = bulkInsertContext.WriteRecord();
            recordContext.Value(record.ProcessInstanceId);
            recordContext.Value(record.StageName);
            recordContext.Value(record.BatchStart);
            recordContext.Value(record.BatchEnd);
            if (record.Data != null)
                recordContext.Value(Convert.ToBase64String(record.Data));
            else
                recordContext.Null();
            recordContext.Value(record.Payload);
            recordContext.Value(record.AlreadyFinalised);
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");
            bulkInsertContext.CloseStream();
            return bulkInsertContext;
        }
        #endregion
    }
}
