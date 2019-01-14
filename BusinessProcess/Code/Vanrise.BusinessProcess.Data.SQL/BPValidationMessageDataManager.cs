using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Data.SQL;

namespace Vanrise.BusinessProcess.Data.SQL
{
    public class BPValidationMessageDataManager : BaseSQLDataManager, IBPValidationMessageDataManager
    {
        private static Dictionary<string, string> _mapper = new Dictionary<string, string>();
        static BPValidationMessageDataManager()
        {
            _mapper.Add("ValidationMessageId", "ID");
            _mapper.Add("SeverityDescription", "Severity");
        }

        readonly string[] _columns = { "ProcessInstanceID", "ParentProcessID", "TargetKey", "TargetType", "Severity", "Message" };

        public BPValidationMessageDataManager()
            : base(GetConnectionStringName("BusinessProcessTrackingDBConnStringKey", "BusinessProcessTrackingDBConnString"))
        {
        }

        public List<BPValidationMessage> GetBeforeId(BPValidationMessageBeforeIdInput input)
        {
            return GetItemsSP("[bp].[sp_BPValidationMessage_GetBeforeID]", BPValidationMessageMapper, input.LessThanID, input.NbOfRows, input.BPInstanceID);
        }

        public List<BPValidationMessage> GetUpdated(BPValidationMessageUpdateInput input)
        {
            List<BPValidationMessage> bpValidationMessages = new List<BPValidationMessage>();

            ExecuteReaderSP("[bp].[sp_BPValidationMessage_GetUpdated]", (reader) =>
            {
                while (reader.Read())
                    bpValidationMessages.Add(BPValidationMessageMapper(reader));
            },
               input.NbOfRows, ToDBNullIfDefault(input.GreaterThanID), input.BPInstanceID);

            return bpValidationMessages;
        }

        public IEnumerable<BPValidationMessage> GetFilteredBPValidationMessage(BPValidationMessageQuery query)
        {
            string severities = null;
            if (query.Severities != null && query.Severities.Count > 0)
                severities = string.Join(",", query.Severities.Select(n => (int)n));

            return GetItemsSP("bp.sp_BPValidationMessage_GetFiltered", BPValidationMessageMapper, query.ProcessInstanceId, severities);
        }

        public void Insert(IEnumerable<Entities.BPValidationMessage> messages)
        {
            object dbApplyStream = InitialiazeStreamForDBApply();

            foreach (BPValidationMessage msg in messages)
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

        private void WriteRecordToStream(BPValidationMessage record, object dbApplyStream)
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
                TableName = "bp.BPValidationMessage",
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

        private BPValidationMessage BPValidationMessageMapper(IDataReader reader)
        {
            BPValidationMessage bpValidationMessage = new BPValidationMessage()
            {
                ValidationMessageId = (long)reader["ID"],
                ProcessInstanceId = (long)reader["ProcessInstanceID"],
                ParentProcessId = GetReaderValue<long?>(reader, "ParentProcessId"),
                TargetKey = GetReaderValue<object>(reader, "TargetKey"),
                TargetType = reader["TargetType"] as string,
                Severity = (ActionSeverity)((int)reader["Severity"]),
                Message = reader["Message"] as string
            };

            return bpValidationMessage;
        }
    }
}
