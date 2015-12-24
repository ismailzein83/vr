using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Data.SQL;

namespace Vanrise.BusinessProcess.Data.SQL
{
    public class BPValidationMessageDataManager : BaseSQLDataManager, IBPValidationMessageDataManager
    {
        readonly string[] _columns = { "ProcessInstanceID", "ParentProcessID", "TargetKey", "TargetType", "Severity", "Message" };

        public BPValidationMessageDataManager()
            : base(GetConnectionStringName("BusinessProcessTrackingDBConnStringKey", "BusinessProcessTrackingDBConnString"))
        {
        }

        public void Insert(IEnumerable<Entities.ValidationMessage> messages)
        {
            object dbApplyStream = InitialiazeStreamForDBApply();

            foreach (ValidationMessage msg in messages)
            {
                WriteRecordToStream(msg, dbApplyStream);
            }

            object prepareToApplyInfo = FinishDBApplyStream(dbApplyStream);
            ApplyForDB(prepareToApplyInfo);
        }

        private object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        private void WriteRecordToStream(ValidationMessage record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}",
                       record.ProcessInstanceId,
                       record.ParentProcessId,
                       record.TargetKey,
                       record.TargetType,
                       (int)record.Severity,
                       record.Message);
        }

        private object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                ColumnNames = _columns,
                TableName = "bp.ValidationMessage",
                Stream = streamForBulkInsert,
                TabLock = false,
                KeepIdentity = false,
                FieldSeparator = '^',
            };
        }

        private void ApplyForDB(object preparedObject)
        {
            InsertBulkToTable(preparedObject as BaseBulkInsertInfo);
        }
    }
}
