using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Analytics.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace CP.WhS.Business
{
    public class BlockedAttemptsManager
    {
        public IDataRetrievalResult<BlockedAttemptDetail> GetFilteredBlockedAttempts(DataRetrievalInput<BlockedAttemptQuery> query)
        {
            var connectionSettings = new PortalConnectionManager().GetWhSConnectionSettings();
            var clonedInput = Utilities.CloneObject<DataRetrievalInput<BlockedAttemptQuery>>(query);
            clonedInput.IsAPICall = true;
            if (clonedInput.DataRetrievalResultType == DataRetrievalResultType.Excel)
            {
                return connectionSettings.Post<DataRetrievalInput<BlockedAttemptQuery>, RemoteExcelResult<BlockedAttemptDetail>>("/api/WhS_Analytics/BlockedAttempts/GetBlockedAttemptsData", clonedInput);
            }
            else if (clonedInput.DataRetrievalResultType == DataRetrievalResultType.Normal)
            {
                return connectionSettings.Post<DataRetrievalInput<BlockedAttemptQuery>, BigResult<BlockedAttemptDetail>>("/api/WhS_Analytics/BlockedAttempts/GetBlockedAttemptsData", clonedInput);
            }
            return null;
        }
    }
}
