using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Data;
using TOne.Analytics.Entities;

namespace TOne.Analytics.Business
{
    public class RawCDRLogManager
    {
        public Vanrise.Entities.IDataRetrievalResult<RawCDRLog> GetRawCDRData(Vanrise.Entities.DataRetrievalInput<RawCDRInput> input)
        {
            IRawCDRLogDataManager datamanager = AnalyticsDataManagerFactory.GetDataManager<IRawCDRLogDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, datamanager.GetRawCDRData(input));
        }
    }
}
