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
    public class RepeatedNumbersManager
    {
        public IDataRetrievalResult<RepeatedNumberDetail> GetFilteredBlockedAttempts(DataRetrievalInput<RepeatedNumberQuery> query)
        {
            var connectionSettings = new PortalConnectionManager().GetWhSConnectionSettings();
            var clonedInput = Utilities.CloneObject<DataRetrievalInput<RepeatedNumberQuery>>(query);
            clonedInput.IsAPICall = true;
            if (clonedInput.DataRetrievalResultType == DataRetrievalResultType.Excel)
            {
                return connectionSettings.Post<DataRetrievalInput<RepeatedNumberQuery>, RemoteExcelResult<RepeatedNumberDetail>>("/api/WhS_Analytics/RepeatedNumber/GetAllFilteredRepeatedNumbers", clonedInput);
            }
            else if (clonedInput.DataRetrievalResultType == DataRetrievalResultType.Normal)
            {
                return connectionSettings.Post<DataRetrievalInput<RepeatedNumberQuery>, BigResult<RepeatedNumberDetail>>("/api/WhS_Analytics/RepeatedNumber/GetAllFilteredRepeatedNumbers", clonedInput);
            }
            return null;
        }
    }
}
