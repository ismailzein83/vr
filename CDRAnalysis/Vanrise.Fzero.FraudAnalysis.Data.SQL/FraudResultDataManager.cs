using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data.SQL
{
    public class FraudResultDataManager : BaseSQLDataManager, IFraudResultDataManager
    {
        public FraudResultDataManager()
            : base("CDRDBConnectionString")
        {

        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "[FraudAnalysis].[FraudResult]",
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

        public void WriteRecordToStream(SuspiciousNumber record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("0^{0}^{1}^{2}^{3}^{4}^^0",
                                 record.Number,
                                record.StrategyId,
                                  record.DateDay.Value,
                                 record.SuspicionLevel,
                                 Vanrise.Common.Serializer.Serialize(record.CriteriaValues, true));
        }

        public void ApplyFraudResultToDB(object preparedResult)
        {
            InsertBulkToTable(preparedResult as BaseBulkInsertInfo);
        }
    }
}
