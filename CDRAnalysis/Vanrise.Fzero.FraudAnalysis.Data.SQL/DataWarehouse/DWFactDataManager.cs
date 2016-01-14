using System;
using System.Collections.Generic;
using System.Data;
using Vanrise.Data.SQL;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;


namespace Vanrise.Fzero.FraudAnalysis.Data.SQL
{
    public class DWFactDataManager : BaseSQLDataManager, IDWFactDataManager
    {
        static string[] s_Columns = new string[] {
            "MS_IMEI", "MS_MSISDN" ,"MS_CaseId" ,"MS_Duration" ,"FK_CallClass"     ,"FK_CallType"    ,"FK_CaseStatus" ,"FK_NetworkType" ,"FK_Period"   ,"FK_Strategy"     ,"FK_StrategyKind" , "FK_SubscriberType"    ,"FK_SuspicionLevel"     ,"FK_ConnectTime"     ,"FK_CaseGenerationTime"    ,"FK_StrategyUser","FK_CaseUser"   ,"FK_BTS"    };

        public DWFactDataManager()
            : base("DWSDBConnString")
        {

        }

        public void ApplyDWFactsToDB(object preparedDWFacts)
        {
            InsertBulkToTable(preparedDWFacts as BaseBulkInsertInfo);
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "[dbo].[Facts]",
                ColumnNames = s_Columns,
                Stream = streamForBulkInsert,
                TabLock = false,
                KeepIdentity = false,
                FieldSeparator = '^'
            };
        }

        public object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToStream(DWFact record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^{8}^{9}^{10}^{11}^{12}^{13}^{14}^{15}^{16}^{17}",
                                                record.IMEI,
                                                record.MSISDN,
                                                record.CaseId,
                                                record.DurationInSeconds,
                                                record.CallClassId,
                                                (int)record.CallType,
                                                (int?)record.CaseStatus,
                                                (int?)record.NetworkType,
                                                (int?)record.Period,
                                                record.StrategyId,
                                                (int?)record.StrategyKind,
                                                (int?)record.SubscriberType,
                                                (int?)record.SuspicionLevel,
                                                record.ConnectDateTime,
                                                record.CaseGenerationTime,
                                                record.StrategyUserId,
                                                record.CaseUserId,
                                                record.BTSId
                                    );
        }

    }
}
