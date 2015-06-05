using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Data;
using TOne.Analytics.Entities;

namespace TOne.Analytics.Business
{
    public class CDRManager
    {
        private readonly ICDRDataManager _datamanager;
        public CDRManager()
        {
            _datamanager = AnalyticsDataManagerFactory.GetDataManager<ICDRDataManager>();
        }
        public List<CDR> GetCDRData(DateTime fromDate, DateTime toDate, int nRecords)
        {

            return _datamanager.GetCDRData(fromDate, toDate, nRecords);
        }
    }

}
