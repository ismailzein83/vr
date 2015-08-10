using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Data;

namespace TOne.Analytics.Business
{
    public  class BlockedAttemptsManager
    {
        public Vanrise.Entities.IDataRetrievalResult<string> GetCDRData(Vanrise.Entities.DataRetrievalInput<string> input)
        {
            IBlockedAttemptsDataManager datamanager = new IBlockedAttemptsDataManager();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, datamanager.GetBlockedAttempts(input));
        }
    }
}
