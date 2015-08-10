using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Data;

namespace TOne.Analytics.Business
{
   public class BlockedAttemptsManager
    {
        public Vanrise.Entities.IDataRetrievalResult<string> GetBlockedAttempts(Vanrise.Entities.DataRetrievalInput<string> input)
        {
            IBlockedAttemptsDataManager dataManager = AnalyticsDataManagerFactory.GetDataManager<IBlockedAttemptsDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetBlockedAttempts(input));
        }
    }
}
