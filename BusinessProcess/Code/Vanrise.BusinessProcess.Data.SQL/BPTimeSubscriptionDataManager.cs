using System;
using System.Collections.Generic;
using System.Data;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Data.SQL;

namespace Vanrise.BusinessProcess.Data.SQL
{
    public class BPTimeSubscriptionDataManager : BaseSQLDataManager, IBPTimeSubscriptionDataManager
    {
        public BPTimeSubscriptionDataManager()
            : base(GetConnectionStringName("BusinessProcessDBConnStringKey", "BusinessProcessDBConnString"))
        {

        }

        public IEnumerable<BPTimeSubscription> GetDueBPTimeSubscriptions()
        {
            return GetItemsSP("[bp].[sp_BPTimeSubscription_GetDue]", BPTimeSubscriptionMapper);
        }

        public bool DeleteBPTimeSubscription(long bpTimeSubscriptionId)
        {
            return ExecuteNonQuerySP("bp.sp_BPTimeSubscription_Delete", bpTimeSubscriptionId) > 0;
        }

        public int InsertBPTimeSubscription(long processInstanceId, string bookmarkName, TimeSpan delay)
        {
            return ExecuteNonQuerySP("bp.sp_BPTimeSubscription_Insert", processInstanceId, bookmarkName, delay.TotalSeconds);
        }

        #region Private Methods

        BPTimeSubscription BPTimeSubscriptionMapper(IDataReader reader)
        {
            return new BPTimeSubscription
            {
                BPTimeSubscriptionId = (long)reader["ID"],
                ProcessInstanceId = (long)reader["ProcessInstanceID"],
                Bookmark = reader["Bookmark"] as string,
                DueTime = (DateTime)reader["DueTime"]
            };
        }

        #endregion
    }
}