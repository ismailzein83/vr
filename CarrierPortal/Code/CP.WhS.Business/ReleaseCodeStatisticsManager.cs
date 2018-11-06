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
    public class ReleaseCodeStatisticsManager
    {
        public IDataRetrievalResult<ReleaseCodeStatDetail> GetFilteredReleaseCodeStatistics(DataRetrievalInput<ReleaseCodeQuery> query)
        {
            var connectionSettings = new PortalConnectionManager().GetWhSConnectionSettings();
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
    }
}
