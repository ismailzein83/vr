using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Data;
using TOne.Analytics.Entities;

namespace TOne.Analytics.Business
{
   public class BlockedAttemptsManager
    {
       public Vanrise.Entities.IDataRetrievalResult<BlockedAttempts> GetBlockedAttempts(Vanrise.Entities.DataRetrievalInput<BlockedAttemptsInput> input)
        {
            IBlockedAttemptsDataManager dataManager = AnalyticsDataManagerFactory.GetDataManager<IBlockedAttemptsDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetBlockedAttempts(input));
        }
    }
}
