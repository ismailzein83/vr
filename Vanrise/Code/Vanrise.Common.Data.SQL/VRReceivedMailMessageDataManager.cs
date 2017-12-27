using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using System.Data;
using Vanrise.Entities;
using Vanrise.Data;


namespace Vanrise.Common.Data.SQL
{
    class VRReceivedMailMessageDataManager : BaseSQLDataManager, IVRReceivedMailMessageDataManager
    {
        readonly string[] columns = { "ConnectionID", "SenderIdentifier", "MessageId", "MessageTime" };

        private Guid _connectionId;
        private string _senderIdentifier;

        public VRReceivedMailMessageDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
        {

        }


       public DateTime GetLastMessageSendTime(Guid connectionId, string senderIdentifier)
        {
            return Convert.ToDateTime(ExecuteScalarSP("common.sp_VRPop3ReadMessageId_GetLastMessageSendTime", connectionId, senderIdentifier) as string);
            //return Convert.ToDateTime(lastMessageSendTimeString);
        }
        public void Insert(Guid connectionId, string senderIdentifier, List<VRReceivedMailMessage> messages)
        {
            _connectionId = connectionId;
            _senderIdentifier = senderIdentifier;
            IBulkApplyDataManager<VRReceivedMailMessage> bulkApplyDataManager = this;
            object streamForDBApply = bulkApplyDataManager.InitialiazeStreamForDBApply();
            foreach (var message in messages)
                bulkApplyDataManager.WriteRecordToStream(message,streamForDBApply);
            object bulkInsertInfo = bulkApplyDataManager.FinishDBApplyStream(streamForDBApply);
            this.InsertBulkToTable(bulkInsertInfo as StreamBulkInsertInfo);
        }


        object Vanrise.Data.IBulkApplyDataManager<VRReceivedMailMessage>.InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        void Vanrise.Data.IBulkApplyDataManager<VRReceivedMailMessage>.WriteRecordToStream(VRReceivedMailMessage record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord
                        (
                            "{0}^{1}^{2}^{3}",
                            _connectionId,
                            _senderIdentifier,
                            record.Header.MessageId,
                            record.Header.MessageSendTime
                        );
        }

        object Vanrise.Data.IBulkApplyDataManager<VRReceivedMailMessage>.FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "common.VRPop3ReadMessageId",
                Stream = streamForBulkInsert,
                TabLock = false,
                KeepIdentity = false,
                FieldSeparator = '^',
                ColumnNames = columns
            };
        }

        public List<string> GetReceivedMailMessagesIdsFromSpecificTime(Guid connectionId, string senderIdentifier, DateTime fromDate)
        {
            return GetItemsSP("common.sp_VRPop3ReadMessageId_GetMessageIdsFromSpecificTime", MessageIdMapper, connectionId, senderIdentifier, fromDate);
        }
/*        public DateTime GetLastMessageSendTime(Guid connectionId, string senderIdentifier)
        {
            return GetItemSP("common.sp_VRPop3ReadMessageId_GetLastMessageSendTime", DateTimeMapper,connectionId, senderIdentifier);
        }
        DateTime DateTimeMapper(IDataReader reader)
        {
            return GetReaderValue<DateTime>(reader,"MessageTime");
        }
 */
        string MessageIdMapper(IDataReader reader)
        {
            return reader["MessageId"] as string;
        }
    }
}