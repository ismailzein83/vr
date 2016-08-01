﻿using System;
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
    public class BalanceUsageQueueDataManager: BaseSQLDataManager, IBalanceUsageQueueDataManager
    {
        #region ctor/Local Variables
        public BalanceUsageQueueDataManager()
            : base(GetConnectionStringName("VR_AccountBalance_TransactionDBConnStringKey", "VR_AccountBalance_TransactionDBConnString"))
        {
        }

       

        #endregion

        #region Public Methods
        public void LoadUsageBalanceUpdate(Action<BalanceUsageQueue> onUsageBalanceUpdateReady)
        {
            ExecuteReaderSP("[VR_AccountBalance].[sp_BalanceUsageQueue_GetAll]",
                (reader) =>
                {
                    while (reader.Read())
                    {
                        onUsageBalanceUpdateReady(BillingTransactionMapper(reader));
                    }
                });
        }
        public bool UpdateUsageBalance(BalanceUsageDetail balanceUsageDetail)
        {
            byte[] binaryArray = null;
            if (balanceUsageDetail != null)
            {
                binaryArray = Common.ProtoBufSerializer.Serialize(balanceUsageDetail);
            }
            return (ExecuteNonQuerySP("[VR_AccountBalance].[sp_BalanceUsageQueue_Update]", binaryArray) > 0);
        }
        public bool DeleteBalanceUsageQueue(long balanceUsageQueueId)
        {
            return (ExecuteNonQuerySP("[VR_AccountBalance].[sp_BalanceUsageQueue_Delete]", balanceUsageQueueId) > 0);
        }
        #endregion
      
        #region Mappers

        private BalanceUsageQueue BillingTransactionMapper(IDataReader reader)
        {
            return new BalanceUsageQueue
            {
                BalanceUsageQueueId = (long)reader["ID"],
                UsageDetails = Vanrise.Common.ProtoBufSerializer.Deserialize<BalanceUsageDetail>(GetReaderValue<byte[]>(reader,"UsageDetails"))
            };
        }

        #endregion

    }
}
