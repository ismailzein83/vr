using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Analytics.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace CP.WhSAnalytics.Business
{
    public class ReleaseCodeStatisticsManager
    {
        static Guid WhSConnectionId = new Guid("B8058F6A-6545-465A-9DCA-6A4157ECFECB");
        public IDataRetrievalResult<ReleaseCodeStatDetail> GetFilteredReleaseCodeStatistics(DataRetrievalInput<ReleaseCodeQuery> query)
        {
            var connectionSettings = GetWhSConnectionSettings();
            var clonedInput = Utilities.CloneObject<DataRetrievalInput<ReleaseCodeQuery>>(query);
            clonedInput.IsAPICall = true;
            if (clonedInput.DataRetrievalResultType == DataRetrievalResultType.Excel)
            {
                return connectionSettings.Post<DataRetrievalInput<ReleaseCodeQuery>, RemoteExcelResult<ReleaseCodeStatDetail>>("/api/WhS_Analytics/ReleaseCode/GetAllFilteredReleaseCodes", clonedInput);
            }
            else if (clonedInput.DataRetrievalResultType == DataRetrievalResultType.Normal)
            {
                return connectionSettings.Post<DataRetrievalInput<ReleaseCodeQuery>, BigResult<ReleaseCodeStatDetail>>("/api/WhS_Analytics/ReleaseCode/GetAllFilteredReleaseCodes", clonedInput);
            }
            return null;
        }

        private VRInterAppRestConnection GetWhSConnectionSettings()
        {
            VRConnectionManager connectionManager = new VRConnectionManager();
            var vrConnection = connectionManager.GetVRConnection<VRInterAppRestConnection>(WhSConnectionId);
            return vrConnection.Settings as VRInterAppRestConnection;
        }
    }
}
