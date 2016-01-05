using System;
using System.Collections.Generic;
using System.Data;
using Vanrise.Data.SQL;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;


namespace Vanrise.Fzero.FraudAnalysis.Data.SQL
{
    public class DWCDRDataManager : BaseSQLDataManager, IDWCDRDataManager
    {
        public DWCDRDataManager()
            : base("DWSDBConnString")
        {

        }

        public void ApplyDWCDRsToDB(object preparedDWCDRs)
        {
            InsertBulkToTable(preparedDWCDRs as BaseBulkInsertInfo);
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "[dbo].[Fact_Calls]",
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

        public void WriteRecordToStream(DWCDR record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^{8}^{9}^{10}^{11}^{12}^{13}^{14}^{15}^{16}^{17}",
                                                record.CDRId,
                                                record.IMEI,
                                                record.MSISDN,
                                                record.CaseId,
                                                record.Duration,
                                                record.CallClass,
                                                record.CallType,
                                                record.CaseStatus,
                                                record.NetworkType,
                                                record.Period,
                                                record.Strategy,
                                                record.StrategyKind,
                                                record.SubscriberType,
                                                record.SuspicionLevel,
                                                record.ConnectTime,
                                                record.StrategyUser,
                                                record.CaseUser,
                                                record.BTS
                                    );
        }

    }
}
