using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;
using Vanrise.Data.SQL;

namespace Vanrise.AccountBalance.Data.SQL
{
    public class BalanceUsageQueueDataManager : BaseSQLDataManager, IBalanceUsageQueueDataManager
    {
        #region ctor/Local Variables
        public BalanceUsageQueueDataManager()
            : base(GetConnectionStringName("VR_AccountBalance_TransactionDBConnStringKey", "VR_AccountBalance_TransactionDBConnString"))
        {
            new UpdateUsageBalancePayload();
            new CorrectUsageBalancePayload();
        }



        #endregion

        #region Public Methods
        public void LoadUsageBalance<T>(Guid accountTypeId,BalanceUsageQueueType balanceUsageQueueType, Action<BalanceUsageQueue<T>> onUsageBalanceUpdateReady)
        {
            ExecuteReaderSP("[VR_AccountBalance].[sp_BalanceUsageQueue_GetByQueueType]",
                (reader) =>
                {
                    while (reader.Read())
                    {
                        onUsageBalanceUpdateReady(BillingTransactionMapper<T>(reader));
                    }
                }, accountTypeId, balanceUsageQueueType);
        }
        public bool UpdateUsageBalance<T>(Guid accountTypeId,BalanceUsageQueueType balanceUsageQueueType, T balanceUsageDetail)
        {
            byte[] binaryArray = null;
            if (balanceUsageDetail != null)
            {
                binaryArray = Common.ProtoBufSerializer.Serialize(balanceUsageDetail);
                binaryArray = Common.Compressor.Compress(binaryArray);
            }
            return (ExecuteNonQuerySP("[VR_AccountBalance].[sp_BalanceUsageQueue_Update]", accountTypeId,balanceUsageQueueType, binaryArray) > 0);
        }
        public bool DeleteBalanceUsageQueue(long balanceUsageQueueId)
        {
            return (ExecuteNonQuerySP("[VR_AccountBalance].[sp_BalanceUsageQueue_Delete]", balanceUsageQueueId) > 0);
        }
        #endregion

        #region Mappers

        private BalanceUsageQueue<T> BillingTransactionMapper<T>(IDataReader reader)
        {
            return new BalanceUsageQueue<T>
            {
                BalanceUsageQueueId = (long)reader["ID"],
                AccountTypeId = GetReaderValue<Guid>(reader, "AccountTypeID"),
                UsageDetails = Vanrise.Common.ProtoBufSerializer.Deserialize<T>(Common.Compressor.Decompress(GetReaderValue<byte[]>(reader, "UsageDetails")))
            };
        }

        #endregion


       
    }
}
