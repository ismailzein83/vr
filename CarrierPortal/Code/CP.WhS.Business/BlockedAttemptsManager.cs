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
        static Guid WhSConnectionId = new Guid("B8058F6A-6545-465A-9DCA-6A4157ECFECB");
        public IDataRetrievalResult<BlockedAttemptDetail> GetFilteredBlockedAttempts(DataRetrievalInput<BlockedAttemptQuery> query)
        {
            var connectionSettings = GetWhSConnectionSettings();
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

        private VRInterAppRestConnection GetWhSConnectionSettings()
        {
            VRConnectionManager connectionManager = new VRConnectionManager();
            var vrConnection = connectionManager.GetVRConnection<VRInterAppRestConnection>(WhSConnectionId);
            return  vrConnection.Settings as VRInterAppRestConnection;
        }
    }
}
